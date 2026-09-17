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

## Commits: use the `commit` skill

Commit through the `commit` skill rather than ad-hoc `git` calls. It carries
this repo's one non-obvious rule: `HRSystem.Domain/docs/diagrams/domain-model.puml`
(and its rendered `.svg`) is checked in, so a change to the public surface of a
type under `HRSystem.Domain/` or `FrameworkX.Common/` belongs in the same commit
as the diagram refresh.

Whether the diagram is affected is a judgement call — a new or renamed public
member or a changed relationship means yes; a method body, a private member, or
formatting means no. The skill spells out that distinction along with this
repo's commit-message style.
