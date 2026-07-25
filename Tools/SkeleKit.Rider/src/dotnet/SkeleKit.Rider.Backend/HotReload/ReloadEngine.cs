using System.Text;
using System.Reflection.Metadata.Ecma335;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.Text;

namespace SkeleKit.Rider.Backend.HotReload;

internal sealed class ReloadEngine
{
	public enum Outcome
	{
		Unchanged,
		Applied,
		Skipped,
		Failed
	}



	static HashSet<string> ReferencedTypes(
		Compilation compilation,
		IEnumerable<IMethodSymbol> methods)
	{
		HashSet<string> found = new(StringComparer.Ordinal);

		foreach (IMethodSymbol method in methods)
		{
			foreach (SyntaxReference syntaxReference in method.DeclaringSyntaxReferences)
			{
				SyntaxNode declaration = syntaxReference.GetSyntax();
				IOperation? root = compilation.GetSemanticModel(declaration.SyntaxTree).GetOperation(declaration);
				if (root is null)
					continue;

				foreach (IOperation operation in Operations(root))
					AddOperation(operation);
			}
		}

		return found;

		void AddOperation(
			IOperation operation)
		{
			AddType(operation.Type);

			switch (operation)
			{
				case IInvocationOperation value:
					AddSymbol(value.TargetMethod);
					break;
				case IObjectCreationOperation value:
					AddSymbol(value.Constructor);
					break;
				case IFieldReferenceOperation value:
					AddSymbol(value.Field);
					break;
				case IPropertyReferenceOperation value:
					AddSymbol(value.Property);
					break;
				case IEventReferenceOperation value:
					AddSymbol(value.Event);
					break;
				case IMethodReferenceOperation value:
					AddSymbol(value.Method);
					break;
				case IConversionOperation value:
					AddSymbol(value.OperatorMethod);
					break;
				case IUnaryOperation value:
					AddSymbol(value.OperatorMethod);
					break;
				case IBinaryOperation value:
					AddSymbol(value.OperatorMethod);
					break;
				case ICompoundAssignmentOperation value:
					AddSymbol(value.OperatorMethod);
					break;
				case IIncrementOrDecrementOperation value:
					AddSymbol(value.OperatorMethod);
					break;
				case ITypeOfOperation value:
					AddType(value.TypeOperand);
					break;
				case ISizeOfOperation value:
					AddType(value.TypeOperand);
					break;
				case IIsTypeOperation value:
					AddType(value.TypeOperand);
					break;
				case IDeclarationPatternOperation value:
					AddType(value.MatchedType);
					break;
				case ITypePatternOperation value:
					AddType(value.MatchedType);
					break;
				case IRecursivePatternOperation value:
					AddType(value.MatchedType);
					break;
			}
		}

		void AddSymbol(
			ISymbol? symbol)
		{
			switch (symbol)
			{
				case ITypeSymbol type:
					AddType(type);
					break;
				case IMethodSymbol value:
					AddType(value.ContainingType);
					AddType(value.ReturnType);
					foreach (IParameterSymbol parameter in value.Parameters)
						AddType(parameter.Type);
					foreach (ITypeSymbol argument in value.TypeArguments)
						AddType(argument);
					break;
				case IFieldSymbol value:
					AddType(value.ContainingType);
					AddType(value.Type);
					break;
				case IPropertySymbol value:
					AddType(value.ContainingType);
					AddType(value.Type);
					foreach (IParameterSymbol parameter in value.Parameters)
						AddType(parameter.Type);
					break;
				case IEventSymbol value:
					AddType(value.ContainingType);
					AddType(value.Type);
					break;
			}
		}

		void AddType(ITypeSymbol? type)
		{
			while (true)
			{
				if (type is null || type.SpecialType != SpecialType.None) return;

				switch (type)
				{
					case IArrayTypeSymbol array:
						type = array.ElementType;
						continue;
					case IPointerTypeSymbol pointer:
						type = pointer.PointedAtType;
						continue;
					case IFunctionPointerTypeSymbol function:
						AddSymbol(function.Signature);
						break;
					case INamedTypeSymbol named:
						foreach (ITypeSymbol argument in named.TypeArguments) AddType(argument);

						INamedTypeSymbol definition = named.OriginalDefinition;
						if (definition.TypeKind != TypeKind.Error && !definition.IsAnonymousType && !SymbolEqualityComparer.Default.Equals(definition.ContainingAssembly, compilation.Assembly) && definition.ContainingAssembly is not null) found.Add($"{definition.ContainingAssembly.Identity.Name}:{FullMetadataName(definition)}");
						break;
				}

				break;
			}
		}
	}

	static string FullMetadataName(
		INamedTypeSymbol type)
	{
		if (type.ContainingType is INamedTypeSymbol containing)
			return FullMetadataName(containing) + "+" + type.MetadataName;

		string @namespace = type.ContainingNamespace?.ToDisplayString() ?? "";
		return @namespace.Length == 0 ? type.MetadataName : @namespace + "." + type.MetadataName;
	}

	static IEnumerable<IOperation> Operations(
		IOperation root)
	{
		Stack<IOperation> pending = new();
		pending.Push(root);

		while (pending.Count > 0)
		{
			IOperation current = pending.Pop();
			yield return current;

			foreach (IOperation child in current.ChildOperations)
				pending.Push(child);
		}
	}

	static string ReadStable(
		string path)
	{
		for (int attempt = 0; ; attempt++)
		{
			try
			{
				using FileStream stream = new(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				using StreamReader reader = new(stream);

				return reader.ReadToEnd();
			}
			catch (IOException) when (attempt < 10)
			{
				Thread.Sleep(30);
			}
		}
	}

	static LineMapping[] LineMappings(
		string path,
		SourceText oldText,
		SourceText newText)
	{
		IReadOnlyList<TextChangeRange> ranges = newText.GetChangeRanges(oldText);
		if (ranges.Count == 0)
			return [];

		List<(int OldLine, int NewLine)> changes = [];
		int characterDelta = 0;

		foreach (TextChangeRange range in ranges)
		{
			int oldPosition = Math.Min(range.Span.End, oldText.Length);
			int newPosition = Math.Min(range.Span.Start + characterDelta + range.NewLength, newText.Length);
			int oldLine = oldText.Lines.GetLinePosition(oldPosition).Line + 1;
			int newLine = newText.Lines.GetLinePosition(newPosition).Line + 1;

			if (oldLine != newLine)
			{
				int last = changes.Count - 1;
				if (last >= 0 && changes[last].OldLine == oldLine)
					changes[last] = (oldLine, newLine);
				else
					changes.Add((oldLine, newLine));
			}

			characterDelta += range.NewLength - range.Span.Length;
		}

		return changes.Count == 0 ? [] : [new(path, changes)];
	}


	public static ReloadEngine? Create(
		AppProject project,
		Action<string> log)
	{
		List<string>? commandLine = MsBuild.CscCommandLineArgs(project, log);
		if (commandLine is null)
			return null;

		CscInvocation csc = CscInvocation.Parse(commandLine, project.ProjectDir);
		csc.AlignReferencesWithDeployment(project.DeployedDll, log);

		MetadataShape? deployed = MetadataShape.OfAssembly(project.DeployedDll);
		if (deployed is null)
		{
			log($"{project.AssemblyName}: cannot read {Path.GetFileName(project.DeployedDll)}");
			return null;
		}

		foreach (bool runGenerators in new[] { true, false })
		{
			Compilation compilation = Project.Build(csc, runGenerators, log).Compilation;

			MetadataShape? rebuilt = MetadataShape.OfCompilation(compilation);
			if (rebuilt is null)
			{
				log($"{project.AssemblyName}: the rebuilt compilation does not compile{(runGenerators ? ", retrying without source generators" : "")}");
				continue;
			}

			if (!rebuilt.Matches(deployed))
			{
				log($"{project.AssemblyName}: rebuilt {rebuilt}, deployed has {deployed}{(runGenerators ? ", retrying without source generators" : "")}");
				foreach (string missing in deployed.Missing(rebuilt).Take(5))
					log($"    missing from the rebuild: {missing}");

				continue;
			}

			return new(project, compilation, new(project.DeployedDll, compilation));
		}

		log($"{project.AssemblyName}: cannot reproduce the deployed assembly, so hot reload is off for it");

		return null;
	}


	readonly AppProject project;
	readonly Baseline baseline;
	readonly Dictionary<string, SyntaxTree> trees;

	Compilation snapshot;
	int domain;
	int module;

	ReloadEngine(
		AppProject project,
		Compilation compilation,
		Baseline baseline)
	{
		this.project = project;
		this.baseline = baseline;
		snapshot = compilation;

		trees = compilation.SyntaxTrees
			.Where(tree => !string.IsNullOrEmpty(tree.FilePath) && File.Exists(tree.FilePath))
			.GroupBy(tree => Path.GetFullPath(tree.FilePath), StringComparer.OrdinalIgnoreCase)
			.ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
	}


	public Guid Mvid => baseline.Mvid;


	void Accept(
		string path,
		SyntaxTree tree,
		Compilation compilation)
	{
		trees[path] = tree;
		snapshot = compilation;
	}

	bool Resolve(
		SdbConnection connection)
	{
		if (module != 0)
			return true;

		domain = connection.RootDomain();
		module = connection.FindModule(domain, project.AssemblyName);

		return module != 0;
	}

	bool UsesUnreferencedType(
		Compilation oldCompilation,
		Compilation newCompilation,
		IEnumerable<SemanticEdit> edits,
		out string unsafeReference)
	{
		IEnumerable<SemanticEdit> semanticEdits = edits as SemanticEdit[] ?? edits.ToArray();
		HashSet<string> existingMethodTypes = ReferencedTypes(
			oldCompilation,
			semanticEdits.Select(edit => edit.OldSymbol).OfType<IMethodSymbol>());
		HashSet<string> checkedTypes = new(StringComparer.Ordinal);
		string foundReference = "";

		foreach (IMethodSymbol method in semanticEdits
			.Select(edit => edit.NewSymbol)
			.OfType<IMethodSymbol>())
		{
			foreach (SyntaxReference syntaxReference in method.DeclaringSyntaxReferences)
			{
				SyntaxNode declaration = syntaxReference.GetSyntax();
				SemanticModel model = newCompilation.GetSemanticModel(declaration.SyntaxTree);
				IOperation? root = model.GetOperation(declaration);
				if (root is null)
					continue;

				foreach (IOperation operation in Operations(root))
				{
					if (UnsafeType(operation.Type)
						|| operation switch
						{
							IInvocationOperation value => UnsafeSymbol(value.TargetMethod),
							IObjectCreationOperation value => UnsafeSymbol(value.Constructor),
							IFieldReferenceOperation value => UnsafeSymbol(value.Field),
							IPropertyReferenceOperation value => UnsafeSymbol(value.Property),
							IEventReferenceOperation value => UnsafeSymbol(value.Event),
							IMethodReferenceOperation value => UnsafeSymbol(value.Method),
							IConversionOperation value => UnsafeSymbol(value.OperatorMethod),
							IUnaryOperation value => UnsafeSymbol(value.OperatorMethod),
							IBinaryOperation value => UnsafeSymbol(value.OperatorMethod),
							ICompoundAssignmentOperation value => UnsafeSymbol(value.OperatorMethod),
							IIncrementOrDecrementOperation value => UnsafeSymbol(value.OperatorMethod),
							ITypeOfOperation value => UnsafeType(value.TypeOperand),
							ISizeOfOperation value => UnsafeType(value.TypeOperand),
							IIsTypeOperation value => UnsafeType(value.TypeOperand),
							IDeclarationPatternOperation value => UnsafeType(value.MatchedType),
							ITypePatternOperation value => UnsafeType(value.MatchedType),
							IRecursivePatternOperation value => UnsafeType(value.MatchedType),
							_ => false
						})
					{
						unsafeReference = foundReference;
						return true;
					}
				}
			}
		}

		unsafeReference = "";
		return false;

		bool UnsafeSymbol(
			ISymbol? symbol) =>
			symbol switch
			{
				ITypeSymbol type => UnsafeType(type),
				IMethodSymbol value => UnsafeType(value.ContainingType)
					|| UnsafeType(value.ReturnType)
					|| value.Parameters.Any(parameter => UnsafeType(parameter.Type))
					|| value.TypeArguments.Any(UnsafeType),
				IFieldSymbol value => UnsafeType(value.ContainingType) || UnsafeType(value.Type),
				IPropertySymbol value => UnsafeType(value.ContainingType)
					|| UnsafeType(value.Type)
					|| value.Parameters.Any(parameter => UnsafeType(parameter.Type)),
				IEventSymbol value => UnsafeType(value.ContainingType) || UnsafeType(value.Type),
				_ => false
			};

		bool UnsafeType(ITypeSymbol? type)
		{
			while (true)
			{
				if (type is null || type.SpecialType != SpecialType.None) return false;

				switch (type)
				{
					case IArrayTypeSymbol array:
						type = array.ElementType;
						continue;
					case IPointerTypeSymbol pointer:
						type = pointer.PointedAtType;
						continue;
					case IFunctionPointerTypeSymbol function:
						return UnsafeSymbol(function.Signature);
					case ITypeParameterSymbol:
					case IDynamicTypeSymbol:
						return false;
					case INamedTypeSymbol named:
						{
							foreach (ITypeSymbol argument in named.TypeArguments)
							{
								if (UnsafeType(argument))
									return true;
							}

							INamedTypeSymbol definition = named.OriginalDefinition;
							if (definition.TypeKind == TypeKind.Error || definition.IsAnonymousType || SymbolEqualityComparer.Default.Equals(definition.ContainingAssembly, newCompilation.Assembly) || definition.ContainingAssembly is null) return false;

							string name = FullMetadataName(definition);
							string key = $"{definition.ContainingAssembly.Identity.Name}:{name}";
							if (!checkedTypes.Add(key) || existingMethodTypes.Contains(key) || baseline.ContainsType(name)) return false;

							foundReference = $"{name} from {definition.ContainingAssembly.Identity.Name}";
							return true;
						}
					default:
						return false;
				}
			}
		}
	}


	public bool Matches(
		SdbConnection connection,
		out string reason)
	{
		if (!Resolve(connection))
		{
			reason = $"{project.AssemblyName} is not loaded in the app";
			return false;
		}

		Guid running = connection.ModuleMvid(module);
		if (running == baseline.Mvid)
		{
			reason = "";
			return true;
		}

		if (running == Guid.Empty)
		{
			reason = $"could not verify the running build of {project.AssemblyName}; restart the debug session";
			module = 0;
			return false;
		}

		reason = $"the app is running a different build of {project.AssemblyName} ({running:D}) than the one on disk ({baseline.Mvid:D}); rebuild and redeploy";
		module = 0;

		return false;
	}

	public Outcome Apply(
		string path,
		SdbConnection connection,
		DebuggerWorkerSync debuggerWorker,
		Action<string> log,
		Action<string> notice)
	{
		if (!trees.TryGetValue(path, out SyntaxTree? oldTree))
			return Outcome.Unchanged;

		string text = ReadStable(path);
		if (text == oldTree.ToString())
			return Outcome.Unchanged;

		string name = Path.GetFileName(path);

		SyntaxTree newTree = CSharpSyntaxTree.ParseText(
			SourceText.From(text, Encoding.UTF8),
			(CSharpParseOptions)oldTree.Options,
			path);

		Compilation updated = snapshot.ReplaceSyntaxTree(oldTree, newTree);
		List<SemanticEdit> edits = Differ.Edits(snapshot, updated, oldTree, newTree, out List<string> rude);

		if (rude.Count > 0)
		{
			foreach (string note in rude)
				log($"  ! {note}");

			log($"  {name}: structural change, restart to apply");
			notice($"Skipped {name}: added or removed a member, restart to apply.");

			return Outcome.Skipped;
		}

		if (edits.Count == 0)
		{
			Accept(path, newTree, updated);
			return Outcome.Unchanged;
		}

		if (UsesUnreferencedType(snapshot, updated, edits, out string unsafeReference))
		{
			log($"  {name}: uses an unreferenced runtime type ({unsafeReference}), restart to apply");
			notice($"Skipped {name}: uses a type the running build never referenced, restart to apply.");

			return Outcome.Skipped;
		}

		using MemoryStream metadata = new();
		using MemoryStream il = new();
		using MemoryStream pdb = new();

		EmitDifferenceResult result = updated.EmitDifference(baseline.Emit, edits, _ => false, metadata, il, pdb, CancellationToken.None);
		if (!result.Success)
		{
			log($"  {name}: could not build the delta");
			foreach (Diagnostic diagnostic in result.Diagnostics.Where(entry => entry.Severity == DiagnosticSeverity.Error).Take(3))
				log($"    {diagnostic}");

			notice($"Hot reload failed for {name}: could not build the delta.");

			return Outcome.Failed;
		}

		byte[] metadataBytes = metadata.ToArray();
		if (!Resolve(connection))
		{
			log($"  {name}: {project.AssemblyName} is not loaded in the app yet");
			return Outcome.Failed;
		}

		byte[] ilBytes = il.ToArray();
		byte[] pdbBytes = pdb.ToArray();
		int[] updatedMethods = result.UpdatedMethods
			.Select(handle => MetadataTokens.GetToken(handle))
			.ToArray();
		int[] updatedTypes = result.ChangedTypes
			.Select(handle => MetadataTokens.GetToken(handle))
			.ToArray();
		LineMapping[] lineMappings = LineMappings(path, oldTree.GetText(), newTree.GetText());

		bool symbolsStaged = debuggerWorker.Stage(
			project.AssemblyName,
			baseline.Mvid,
			ilBytes,
			metadataBytes,
			pdbBytes,
			updatedMethods,
			updatedTypes,
			lineMappings,
			out string syncReason);
		if (!symbolsStaged)
			log($"  {name}: Rider symbol sync unavailable ({syncReason})");

		int error = connection.ApplyChanges(domain, module, metadataBytes, ilBytes, pdbBytes);
		if (error != 0)
		{
			log($"  {name}: APPLY_CHANGES error {error}");
			notice(error == SdbConnection.InvalidObjectError
				? $"Hot reload did not apply to {name}. If Rider is paused, resume and save again."
				: $"Hot reload failed for {name}: apply error {error}.");
			module = 0;

			return Outcome.Failed;
		}

		if (symbolsStaged)
			connection.NotifyEnCUpdate(module);

		if (result.Baseline is EmitBaseline next)
			baseline.Emit = next;

		Accept(path, newTree, updated);

		log($"  {name}: {edits.Count} edit(s), reloaded");
		notice(symbolsStaged
			? $"Hot reloaded {name}; symbols updated."
			: $"Hot reloaded {name}, but symbols did not update (breakpoints may drift).");

		return Outcome.Applied;
	}
}
