// Package tests holds e2e tests that go through the api facade only
// (DESIGN_go.md §O5). Each example × each query = one golden file.
//
// Run:                go test ./tests/...
// Update goldens:     go test ./tests/... -update
package tests

import (
	"context"
	"fmt"
	"testing"

	"ds4/internal/api"
	"ds4/internal/examples"
)

// TestExamplesOracle iterates every embedded example and checks that
// api.Solve agrees with the manifest's expected answer (C3, C5).
//
// (TODO O5: replace the t.Skip with goldie.AssertJson once O1–O4 land.)
func TestExamplesOracle(t *testing.T) {
	man, err := examples.Load()
	if err != nil {
		t.Fatalf("load manifest: %v", err)
	}
	if len(man.Examples) == 0 {
		t.Skip("no examples in manifest yet (O6)")
	}

	for _, ex := range man.Examples {
		ex := ex
		t.Run(ex.ID, func(t *testing.T) {
			domain, err := examples.ReadDomain(ex)
			if err != nil {
				t.Fatalf("read domain %s: %v", ex.File, err)
			}
			for i, q := range ex.Queries {
				q := q
				t.Run(formatQueryName(i, q.Text), func(t *testing.T) {
					t.Skip("TODO O5: hook up goldie + assert q.Expected once core is wired")
					_ = api.Solve(context.Background(), domain, q.Text)
				})
			}
		})
	}
}

func formatQueryName(i int, text string) string {
	if len(text) > 40 {
		text = text[:40] + "…"
	}
	return fmt.Sprintf("%s#%d", text, i)
}
