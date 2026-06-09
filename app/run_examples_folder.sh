#!/usr/bin/env bash
# Runs every .domain/.query pair in a folder through the Ds4 CLI and
# compares the TAK/NIE result to the matching .expected file.
# Usage: run_examples_folder.sh <examples_dir>

set -u

dir="${1:-release/examples}"
if [[ ! -d "$dir" ]]; then
  echo "Directory not found: $dir" >&2
  exit 2
fi

cli="src/Ds4.Cli/bin/Release/net8.0/Ds4.Cli.dll"
if [[ ! -f "$cli" ]]; then
  echo "CLI not built: $cli (run 'dotnet build src/Ds4.Cli/Ds4.Cli.csproj -c Release' first)" >&2
  exit 2
fi

normalize() {
  local v
  v="$(printf '%s' "$1" | tr -d '[:space:]' | tr '[:upper:]' '[:lower:]')"
  case "$v" in
    tak|true|yes|1)  echo "TAK" ;;
    nie|false|no|0)  echo "NIE" ;;
    *)               echo "$v" ;;
  esac
}

pass=0
fail=0
missing=0
failed_ids=()

shopt -s nullglob
for dom in "$dir"/*.domain; do
  id="$(basename "$dom" .domain)"
  qry="$dir/$id.query"
  exp="$dir/$id.expected"

  if [[ ! -f "$qry" || ! -f "$exp" ]]; then
    echo "SKIP  $id (missing .query or .expected)"
    ((missing++))
    continue
  fi

  out="$(dotnet "$cli" "$dom" "$qry" 2>/dev/null)" || true
  # Result line is the first line of CLI output (TAK / NIE / BŁĄD ...)
  actual_raw="$(printf '%s\n' "$out" | head -n 1)"
  expected_raw="$(cat "$exp")"

  actual="$(normalize "$actual_raw")"
  expected="$(normalize "$expected_raw")"

  if [[ "$actual" == "$expected" ]]; then
    printf "PASS  %-60s expected=%s\n" "$id" "$expected"
    ((pass++))
  else
    printf "FAIL  %-60s expected=%s got=%s (raw: '%s')\n" "$id" "$expected" "$actual" "$actual_raw"
    ((fail++))
    failed_ids+=("$id")
  fi
done

echo "----------------------------------------------------------------"
echo "Folder:   $dir"
echo "Passed:   $pass"
echo "Failed:   $fail"
echo "Skipped:  $missing"

if ((fail > 0)); then
  echo "Failed examples:"
  for id in "${failed_ids[@]}"; do echo "  - $id"; done
  exit 1
fi
