# Verification report

## Reconciled inputs

- `Source/Framework/SkeleKit.iOS/**/*.cs`, excluding build output.
- `Source/Framework/SkeleKit.iOS/bin/Debug/net10.0-ios/SkeleKit.iOS.dll`.
- `Source/Framework/SkeleKit.iOS/bin/Debug/net10.0-ios/SkeleKit.iOS.xml`.
- `Source/Framework/SkeleKit.Generators/PageGenerator.cs` and diagnostics `SKEL001`–`SKEL003`.
- `Source/Framework/SkeleKit.iOS/build/SkeleKit.iOS.targets`.
- Rider `plugin.xml`, Gradle compatibility configuration, and user-facing `Docs/Rider-Plugin.md`.

## Counts

| Surface | Types | Declared members |
| --- | ---: | ---: |
| XML documented baseline | 155 | 943 |
| Compiled accessible metadata (includes implementation-shaped/compiler-generated exports) | 156 | 1186 |
| Reconciled canonical tables | 156 | 1202 |
| Source type-name index (public and nonpublic; partial declarations coalesced by name) | 209 | n/a |

Metadata member counting includes visible fields, properties, events, constructors, methods, operators, and conversions and excludes property/event accessors. The XML count is the canonical documented consumer inventory; discrepancies are routed to `api-findings.md` and `implementation-surface.md`.

## Generated consumer surface

`[Page]` drives two generated extension methods:

- `GeneratedPages.UsePages(this SkeleApplicationBuilder)` registers discovered pages as defaults; manual registrations take precedence.
- `GeneratedPages.Build(this SkeleApplicationBuilder)` applies generated registrations and calls `BuildCore()`.

`SKEL001` rejects a marked class that does not inherit `ContentView`; `SKEL002` rejects an abstract page; `SKEL003` rejects a missing accessible parameterless constructor or, for `ContentView<TViewModel>`, a constructor whose first parameter is the view-model type and whose remaining parameters are optional.

## Mechanical checks

- Every XML type maps to one row in `coverage.md`.
- Every mapped canonical path exists.
- Every visual type has a showcase matrix and state/environment pass.
- Exact XML IDs preserve overload identity.
- Relative Markdown links and final diff scope are checked by the repository verification commands.
- Inline snippets are compile-shape specimens. They intentionally accept an already-created specimen so constructors, DI, permissions, networking, and device services are not faked.

## Executed verification

- Extracted **90** `csharp` fences into isolated wrappers in a temporary `net10.0-ios` project, referenced the framework and page generator, and built for `iossimulator-arm64`: **0 errors, 0 warnings**.
- Resolved every relative Markdown link in all **32** blueprint files: **0 broken links**.
- Parsed `coverage.md`: **156 rows, 156 unique reconciled types**, each with one canonical path.
- Counted canonical API rows: **1,202**, matching the reconciled XML/metadata union used by these pages.
- Ran `dotnet test Source/Tests/SkeleKit.Tests/SkeleKit.Tests.csproj -c Debug`: **191 passed**.
