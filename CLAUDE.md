# CLAUDE.md

## Code navigation: Grep vs. LSP

This is a multi-project .NET solution (`FrameworkX.Common`, `HRSystem.Domain`,
`HRSystem.Web`, `HRSystem.Tests`) with an LSP server configured for C#. Use
each tool for what it's good at rather than defaulting to one:

- **Grep/Glob/Explore** for open-ended discovery: finding where a string or
  pattern appears, locating a file, searching across non-code files. Use this
  first when you don't yet know where something lives.
- **LSP** (`goToDefinition`, `findReferences`, `documentSymbol`,
  `goToImplementation`, `hover`, call hierarchy) when correctness matters more
  than speed: refactors, "who actually calls this," resolving base
  classes/interfaces across project boundaries (e.g. a `HRSystem.Domain`
  repository extending a generic base in `FrameworkX.Common`), or confirming
  a real reference vs. a coincidental text match. LSP needs a starting
  location, so it typically follows a grep, not replaces it.

Reach for LSP specifically when a grep-based answer could be wrong in a way
that matters — e.g. missing an inherited member, or treating a same-named
symbol in two files as the same thing.
