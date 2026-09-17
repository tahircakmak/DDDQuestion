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

## Commit expectations: keep the class diagram in sync

A commit that changes `.cs` files under `HRSystem.Domain/` or
`FrameworkX.Common/` must also carry an updated
`HRSystem.Domain/docs/diagrams/domain-model.puml`, plus the `.svg` re-rendered
from it via the `plantuml-class-diagrams` skill. A `PreToolUse` hook
(`.claude/hooks/diagram-commit-guard.sh`) enforces this by refusing Claude's
`git commit` when the diagram is missing or the committed `.svg` doesn't match
its `.puml`.

- Escape hatch: commit with `--no-verify` when the diagram genuinely needs no
  update.
- Stage in a **separate** call from the commit. The guard inspects the index
  before the command runs, so `git add … && git commit …` as one call is judged
  against the pre-`add` index and gets refused. (`git commit -a` is fine — the
  guard accounts for unstaged tracked changes in that case.)
- The check only sees commits made **through Claude**. A commit from a bare
  terminal or the VS Code Source Control panel is not inspected.
- SVG verification is skipped with a warning when `java` or PlantUML is
  unavailable; the diagram-presence rule still applies.
