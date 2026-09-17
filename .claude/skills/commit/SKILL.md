---
name: commit
description: Create a git commit in this repo, keeping the domain class diagram in sync. Use when the user asks to commit, stage and commit, or wrap up changes — especially when anything under HRSystem.Domain or FrameworkX.Common was touched.
---

# Committing in this repo

The domain class diagram at `HRSystem.Domain/docs/diagrams/domain-model.puml`
is checked in, so it rots the moment the model changes without it. Nothing
enforces this mechanically — that is what this procedure is for.

## Procedure

1. **See what changed.** `git status --short` plus `git diff` (and
   `git diff --cached` if anything is already staged). Read the actual diff;
   don't infer from filenames.

2. **Decide whether the diagram is affected.** It needs updating only when the
   *shape* the diagram shows has moved:

   - **Update it** for: a new/removed/renamed public property or method on a
     domain type, a changed type or signature, a new or deleted entity/enum/
     interface, a changed relationship (navigation property, FK, multiplicity,
     base class or implemented interface).
   - **Leave it alone** for: method *body* edits, private members, local
     variables, comments, formatting, test-only changes, or anything outside
     `HRSystem.Domain/` and `FrameworkX.Common/`.

   This judgement is the whole reason this is a skill and not a hook — a
   pre-commit check can only see *that* a `.cs` file changed, never *whether*
   the public surface actually moved.

3. **If affected, refresh the diagram** with the `plantuml-class-diagrams`
   skill: edit the `.puml` to match the new surface, then render it:

   ```bash
   .claude/skills/plantuml-class-diagrams/scripts/render.sh \
     HRSystem.Domain/docs/diagrams/domain-model.puml -t svg
   ```

   Both the `.puml` and the re-rendered `.svg` belong in the same commit as the
   code change. Committing one without the other is the failure mode to avoid.

4. **Stage deliberately.** Name the paths (`git add <paths>`); avoid `git add -A`
   / `git add .`, which pick up stray local files. Re-check with
   `git status --short` that exactly what you intended is staged.

5. **Write the message in this repo's style** (see `git log`): an imperative
   subject line, then a body explaining *why* — the constraint, the reason for
   the approach, what was deliberately not done. Skip the body only for genuinely
   trivial changes. End with the Co-Authored-By trailer for the current model.

6. **Split unrelated concerns** into separate commits rather than one bundle. A
   domain change and a tooling change are two commits, even when asked to
   "commit everything".

7. **Commit, then verify** with `git status` and `git log --oneline -1`.

## Notes

- Only commit when the user asked for it.
- Don't push unless the user asked for that too; it's a separate decision.
- `dotnet build` / `dotnet test` before committing is worth it when the change
  is more than cosmetic — the test project uses `EnsureCreated()`, so a domain
  property change needs no migration but can still break compilation.
