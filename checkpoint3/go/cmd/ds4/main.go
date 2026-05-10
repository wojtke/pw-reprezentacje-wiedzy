// Command ds4 is the primary Fyne GUI entrypoint (O7).
//
// Build (Windows): go build -ldflags="-s -w" -o ds4.exe ./cmd/ds4
// Run   (macOS):   go run ./cmd/ds4
//
// Allowed imports: stdlib + fyne.io/... + ds4/internal/api.
// Forbidden:        ds4/internal/core/... (depguard, see .golangci.yml).
package main

import (
	"fmt"

	"ds4/internal/api"
)

func main() {
	// TODO O7:
	//   1. fyne app.New() + window
	//   2. left:   widget.Entry  (domain editor)
	//      right:  widget.Entry  (query editor) + widget.Button "▶ Oblicz"
	//      below:  widget.Tree   (trace) + widget.Label (answer + summary)
	//   3. menu "Przykłady" populated from api.ListExamples() / api.LoadExample()
	//   4. button handler -> api.Solve(domain, query) -> render SolveResult
	//   5. status bar: |Σ|, |Σ₀|, elapsed, errors with line/column
	res := api.Solve("", "")
	fmt.Println("ds4 (Fyne) — TODO O7. Smoke call:", res.Answer)
}
