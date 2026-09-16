#!/usr/bin/env bash
# Renders a PlantUML (.puml) file to an image using a local plantuml.jar,
# downloading the jar to a cache directory on first use.
#
# Usage: render.sh <input.puml> [-t svg|png|txt] [-o output_dir]

set -euo pipefail

PLANTUML_VERSION="${PLANTUML_VERSION:-1.2026.8}"
CACHE_DIR="${PLANTUML_CACHE_DIR:-$HOME/.cache/claude-plantuml}"
JAR_PATH="${PLANTUML_JAR:-$CACHE_DIR/plantuml-${PLANTUML_VERSION}.jar}"
JAR_URL="https://github.com/plantuml/plantuml/releases/download/v${PLANTUML_VERSION}/plantuml-${PLANTUML_VERSION}.jar"

FORMAT="svg"
OUT_DIR=""
INPUT=""

while [[ $# -gt 0 ]]; do
  case "$1" in
    -t) FORMAT="$2"; shift 2 ;;
    -o) OUT_DIR="$2"; shift 2 ;;
    -h|--help)
      echo "Usage: $0 <input.puml> [-t svg|png|txt] [-o output_dir]"
      exit 0
      ;;
    *) INPUT="$1"; shift ;;
  esac
done

if [[ -z "$INPUT" ]]; then
  echo "Error: no input .puml file given." >&2
  echo "Usage: $0 <input.puml> [-t svg|png|txt] [-o output_dir]" >&2
  exit 1
fi

if [[ ! -f "$INPUT" ]]; then
  echo "Error: input file not found: $INPUT" >&2
  exit 1
fi

if ! command -v java >/dev/null 2>&1; then
  echo "Error: java is required to run PlantUML but was not found on PATH." >&2
  exit 1
fi

if [[ ! -f "$JAR_PATH" ]]; then
  echo "PlantUML jar not found at $JAR_PATH — downloading v${PLANTUML_VERSION}..." >&2
  mkdir -p "$CACHE_DIR"
  TMP_JAR="$(mktemp "${CACHE_DIR}/plantuml.XXXXXX.jar")"
  if ! curl -fSL --max-time 60 -o "$TMP_JAR" "$JAR_URL"; then
    rm -f "$TMP_JAR"
    echo "Error: failed to download PlantUML jar from $JAR_URL" >&2
    echo "You can set PLANTUML_JAR to point at an existing local jar instead." >&2
    exit 1
  fi
  mv "$TMP_JAR" "$JAR_PATH"
fi

case "$FORMAT" in
  svg) FLAG="-tsvg" ;;
  png) FLAG="-tpng" ;;
  txt) FLAG="-ttxt" ;;
  *) echo "Error: unsupported format '$FORMAT' (use svg, png, or txt)." >&2; exit 1 ;;
esac

ARGS=(-jar "$JAR_PATH" "$FLAG" -charset UTF-8)
if [[ -n "$OUT_DIR" ]]; then
  mkdir -p "$OUT_DIR"
  ARGS+=(-o "$(cd "$OUT_DIR" && pwd)")
fi
ARGS+=("$INPUT")

java "${ARGS[@]}"
echo "Rendered $INPUT -> ${FORMAT}" >&2
