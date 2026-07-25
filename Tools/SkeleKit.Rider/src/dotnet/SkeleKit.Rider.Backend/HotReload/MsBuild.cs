using System.Diagnostics;
using System.Text;

namespace SkeleKit.Rider.Backend.HotReload;

internal static class MsBuild
{
	const string DumpTargets = """
		<Project>
			<Target Name="SkeleDumpCscArgs" AfterTargets="CoreCompile" Condition=" '$(SkeleArgsOut)' != '' ">
				<WriteLinesToFile File="$(SkeleArgsOut)" Lines="@(CscCommandLineArgs)" Overwrite="true" WriteOnlyWhenDifferent="false" />
			</Target>
		</Project>
		""";


	static readonly TimeSpan Timeout = TimeSpan.FromMinutes(3);

	static string? dotnetPath;


	static (int ExitCode, string Diagnostics) Run(
		string executable,
		string arguments,
		string workingDirectory)
	{
		ProcessStartInfo start = new(executable, arguments)
		{
			WorkingDirectory = workingDirectory,
			UseShellExecute = false,
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			CreateNoWindow = true,
			EnvironmentVariables =
			{
				// a build server left over from another SDK can answer with stale evaluation
				["MSBUILDDISABLENODEREUSE"] = "1",
				["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1"
			}
		};

		using Process process = new();
		process.StartInfo = start;

		StringBuilder diagnostics = new();

		process.OutputDataReceived += (_, line) => Append(diagnostics, line.Data);
		process.ErrorDataReceived += (_, line) => Append(diagnostics, line.Data);

		process.Start();
		process.BeginOutputReadLine();
		process.BeginErrorReadLine();

		if (!process.WaitForExit((int)Timeout.TotalMilliseconds))
		{
			try
			{
				process.Kill();
			}
			catch
			{
				// ignored
			}

			return (-1, "timed out");
		}

		return (process.ExitCode, diagnostics.ToString());
	}

	static void Append(
		StringBuilder diagnostics,
		string? line)
	{
		if (line is null)
			return;

		lock (diagnostics)
		{
			if (diagnostics.Length < 8192)
				diagnostics.Append(line).Append('\n');
		}
	}

	static string? ResolveDotnet()
	{
		if (dotnetPath is not null)
			return dotnetPath.Length > 0 ? dotnetPath : null;

		dotnetPath = Candidates().FirstOrDefault(File.Exists) ?? "";

		return dotnetPath.Length > 0 ? dotnetPath : null;
	}

	static IEnumerable<string> Candidates()
	{
		if (Environment.GetEnvironmentVariable("SKELEKIT_DOTNET") is string configured && configured.Length > 0)
			yield return configured;

		// the host that started this backend, when it is a .NET one
		if (Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") is string host && host.Length > 0)
			yield return host;

		foreach (string directory in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(Path.PathSeparator))
		{
			if (directory.Length > 0)
				yield return Path.Combine(directory, "dotnet");
		}

		yield return "/usr/local/share/dotnet/dotnet";
		yield return "/opt/homebrew/bin/dotnet";
		yield return "/usr/bin/dotnet";
	}


	public static List<string>? CscCommandLineArgs(
		AppProject project,
		Action<string> log)
	{
		string? dotnet = ResolveDotnet();
		if (dotnet is null)
		{
			log("cannot find the dotnet CLI; set the SKELEKIT_DOTNET environment variable to its full path");
			return null;
		}

		string targets = Path.Combine(Path.GetTempPath(), "skelekit-dumpcscargs.targets");
		string output = Path.Combine(Path.GetTempPath(), $"skelekit-cscargs-{Guid.NewGuid():N}.args");

		try
		{
			File.WriteAllText(targets, DumpTargets);

			StringBuilder arguments = new();
			arguments.Append($"msbuild \"{project.ProjectFile}\" -nologo -v:q -nodeReuse:false -t:Compile");
			arguments.Append(" -p:ProvideCommandLineArgs=true -p:SkipCompilerExecution=true");
			arguments.Append($" -p:Configuration={project.Configuration}");
			if (project.TargetFramework.Length > 0)
				arguments.Append($" -p:TargetFramework={project.TargetFramework}");
			if (project.RuntimeIdentifier.Length > 0)
				arguments.Append($" -p:RuntimeIdentifier={project.RuntimeIdentifier}");
			arguments.Append($" -p:CustomAfterMicrosoftCommonTargets=\"{targets}\" -p:SkeleArgsOut=\"{output}\"");

			(int exitCode, string diagnostics) = Run(dotnet, arguments.ToString(), project.ProjectDir);

			if (!File.Exists(output))
			{
				log($"msbuild could not describe {project.AssemblyName} (exit {exitCode})");
				foreach (string line in diagnostics.Split('\n').Where(line => line.Trim().Length > 0).Take(10))
					log($"  {line.TrimEnd()}");

				return null;
			}

			return [.. File.ReadAllLines(output)];
		}
		catch (Exception exception)
		{
			log($"msbuild probe failed: {exception.Message}");
			return null;
		}
		finally
		{
			try
			{
				File.Delete(output);
			}
			catch
			{
				// ignore grr :3
			}
		}
	}
}
