---
name: plantuml-class-diagrams
description: Create and render UML class diagrams with PlantUML. Use when the user asks for a class diagram, domain model diagram, UML diagram, or to visualize classes/relationships (e.g. entities, aggregates, value objects in this DDD codebase) as a diagram.
---

# PlantUML class diagrams

Generate class diagrams as PlantUML (`.puml`) source, then render them locally
to SVG/PNG with the bundled script. Java is required (already available on
this machine); the PlantUML jar is downloaded once and cached at
`~/.cache/claude-plantuml/`.

## Workflow

1. **Identify the classes and relationships to diagram.** If the request
   references code in this repo (e.g. `HRSystem.Domain`), read the relevant
   source files first so the diagram reflects the real types, properties, and
   relationships rather than guessing.
2. **Write the `.puml` source.** Use `references/syntax.md` for the syntax
   (visibility, relationship arrows, multiplicities, stereotypes, packages).
   Save the file wherever the user's diagrams belong in the repo (ask if
   unclear; a reasonable default is a `docs/diagrams/` folder next to the
   relevant project).
3. **Render it** with the helper script:

   ```bash
   .claude/skills/plantuml-class-diagrams/scripts/render.sh path/to/diagram.puml -t svg
   ```

   - `-t` selects the output format: `svg` (default, best for viewing/embedding),
     `png`, or `txt` (ASCII art, useful for a quick terminal preview).
   - `-o <dir>` writes the rendered file to a specific output directory
     (defaults to the same directory as the input file).
   - On first run this downloads `plantuml.jar` (~28 MB) from the official
     GitHub releases into `~/.cache/claude-plantuml/`; subsequent renders reuse
     the cached jar and require no network access.
4. **Verify** the render succeeded (script exits 0 and prints the output path)
   before telling the user it's done. Open/describe the SVG or offer it as a
   file if the user wants to inspect it visually.

## Conventions for this codebase

This is a DDD project (see `HRSystem.Domain`). When diagramming domain types,
follow the stereotype/relationship conventions in
`references/syntax.md#ddd-flavored-conventions-used-in-this-repo` — tag
aggregate roots, entities, and value objects distinctly, and prefer
composition for owned entities vs. plain association for cross-aggregate
references.

## Notes

- Don't hand-roll a different renderer or hit a web rendering service — always
  use `scripts/render.sh` so output is reproducible offline after the first
  cache warm-up.
- If `java` isn't on PATH or the download fails, the script reports a clear
  error; don't silently fall back to producing only source with no attempt to
  render.
- Keep `.puml` files as the source of truth; treat rendered SVG/PNG as build
  output (regenerate rather than hand-edit).
