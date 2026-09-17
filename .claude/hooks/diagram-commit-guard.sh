#!/usr/bin/env bash
# PreToolUse guard: refuse a commit that changes domain/framework C# sources
# without carrying an updated PlantUML class diagram.
#
# Reads a Claude Code hook payload on stdin. Prints a deny verdict as JSON when
# the commit would leave the diagram stale; otherwise prints nothing.
#
# Deliberately not `set -e`: a non-matching grep exits 1 as a normal outcome.
set -uo pipefail

PUML="HRSystem.Domain/docs/diagrams/domain-model.puml"
SVG="HRSystem.Domain/docs/diagrams/domain-model.svg"
RENDER=".claude/skills/plantuml-class-diagrams/scripts/render.sh"

payload=$(cat)
cmd=$(printf '%s' "$payload" | jq -r '.tool_input.command // ""' 2>/dev/null) || exit 0
[ -n "$cmd" ] || exit 0

# Match `git ... commit` anywhere, so `cd x && git commit ...` is caught too.
printf '%s' "$cmd" \
  | grep -qE '(^|[;&|(]|[[:space:]])git([[:space:]]+-[^[:space:]]+)*[[:space:]]+commit([[:space:]]|$)' \
  || exit 0

# Same opt-out convention as git's own hooks.
printf '%s' "$cmd" | grep -qE '(^|[[:space:]])--no-verify([[:space:]]|$)' && exit 0

root=$(git rev-parse --show-toplevel 2>/dev/null) || exit 0
cd "$root" || exit 0

# `git commit -a` sweeps in unstaged tracked changes, so the index alone would
# understate what lands in the commit.
if printf '%s' "$cmd" | grep -qE '(^|[[:space:]])(-[a-zA-Z]*a[a-zA-Z]*|--all)([[:space:]]|$)'; then
    all_mode=1
    files=$(git diff --name-only HEAD)
else
    all_mode=
    files=$(git diff --cached --name-only)
fi

sources=$(printf '%s\n' "$files" \
    | grep -E '^(HRSystem\.Domain|FrameworkX\.Common)/.*\.cs$' \
    | grep -v '/docs/diagrams/')

[ -n "$sources" ] || exit 0

deny() {
    jq -n --arg r "$1" '{
      hookSpecificOutput: {
        hookEventName: "PreToolUse",
        permissionDecision: "deny",
        permissionDecisionReason: $r
      }
    }'
    exit 0
}

if ! printf '%s\n' "$files" | grep -qxF "$PUML"; then
    deny "Domain source changed but the class diagram is not part of this commit:
$(printf '%s\n' "$sources" | sed 's/^/  /')

Run the plantuml-class-diagrams skill to bring $PUML in line with these
changes, then stage it together with the re-rendered $SVG.

If the diagram genuinely needs no update, commit with --no-verify."
fi

# Read the content that will actually be committed, which for -a is the working
# tree rather than the index.
committed_content() {
    if [ -n "$all_mode" ]; then cat "$1"; else git show ":$1"; fi
}

tmp=$(mktemp -d) || exit 0
trap 'rm -rf "$tmp"' EXIT

committed_content "$PUML" > "$tmp/domain-model.puml" 2>/dev/null \
    || deny "Could not read the version of $PUML that this commit would include."
committed_content "$SVG" > "$tmp/staged.svg" 2>/dev/null \
    || deny "$PUML is in this commit but $SVG is not.
Render it and stage the result:
  $RENDER $PUML -t svg"

# Fail open on the toolchain, closed on the discipline check above.
command -v java >/dev/null 2>&1 || {
    printf 'diagram-commit-guard: java not found; skipping SVG verification.\n' >&2
    exit 0
}
"$RENDER" "$tmp/domain-model.puml" -t svg -o "$tmp" >/dev/null 2>&1 || {
    printf 'diagram-commit-guard: PlantUML render unavailable; skipping SVG verification.\n' >&2
    exit 0
}

cmp -s "$tmp/domain-model.svg" "$tmp/staged.svg" || deny "The $SVG in this commit does not match its $PUML.
Re-render it and stage the result:
  $RENDER $PUML -t svg"
