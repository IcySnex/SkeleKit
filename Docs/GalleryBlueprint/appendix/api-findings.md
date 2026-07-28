# API audit findings

This pass records issues only; no framework or Gallery source is changed.

1. **Compiler-generated type is documented/export-shaped.** The XML baseline contains `SkeleKit.GridExtensions.<G>$8682DAC7B51EDBAE0D7FA41B91C3E7AB`1.<M>$9B047F02C64B08A172A5A5DD2C88066F`. It originates from generic extension syntax in `GridExtensions`; it is assigned to the implementation-surface appendix, not the normal gallery.
2. **XML is a documentation baseline, not a complete accessibility proof.** The tables retain exact XML IDs, while `verification.md` separately compares source and compiled metadata. Public/protected declarations without XML documentation must be treated as findings rather than silently omitted.
3. **Rider compatibility text conflicts.** `Docs/Rider-Plugin.md` says the plugin is pinned to Rider `261.*`/2026.1, while `plugin.xml` and Gradle configuration currently declare `262`/`262.*`. The tooling blueprint reports the live configuration and flags the older prose.
4. **Hot reload is conditional, not a framework default.** It requires `EnableHotReload=true`, Debug, and an `iossimulator*` runtime identifier. Release and physical-device builds do not get the interpreter/runtime switches.
5. **`Bindable<T>` cannot use an interface `T`.** Collection sources therefore use `BindableList<T>` concrete conversions; examples must not imply a user-defined conversion to an interface-typed bindable.
6. **Shadow and clipping compete.** A view cannot both implicitly clip its rounded layer and cast a shadow; use an outer shadow host and inner rounded/clipped content when both are required.
7. **NaN is semantic in layout/text APIs.** Do not normalize NaN defaults to zero: it commonly means automatic, unspecified, or fallback sizing.

Questions that require product intent rather than source facts should remain findings. This inventory does not guess a new default or compatibility promise.