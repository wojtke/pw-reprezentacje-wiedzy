package engine

import (
	"context"

	"ds4/internal/core/model"
)

// StateSet is the set representation engine uses internally.
type StateSet map[model.State]struct{}

// Sigma builds Σ from the domain's `always` constraints (PK2 §4.1).
// Long-running for large |F|; callers pass ctx so api.Solve can be
// cancelled. (TODO O2)
func Sigma(ctx context.Context, d *model.Domain) (StateSet, error) {
	_, _ = ctx, d
	panic("engine.Sigma: TODO O2")
}

// Sigma0 builds Σ₀ by forward-applying Res from each σ ∈ Σ to filter
// against `initially` / `after` / `observable after` (PK2 §4.2, §8.3).
// Lives in O2 because the filter uses the same simple-action Res.
// (TODO O2)
func Sigma0(ctx context.Context, d *model.Domain, sigma StateSet) (StateSet, error) {
	_, _, _ = ctx, d, sigma
	panic("engine.Sigma0: TODO O2")
}

// Res returns the (possibly empty) set of successor states after executing
// a simple action in s (PK2 §4.3). Empty result == action not executable
// (falls out of the desugared `impossible A if π ≡ A causes ⊥ if π`).
//
// No ctx parameter on purpose: Res is on the hottest path of the engine
// (called per (action, state) combination in process trees). Callers
// hold the ctx and check it between calls — see engine/query.go.
//
// (TODO O2)
func Res(d *model.Domain, sigma StateSet, a model.Action, s model.State) StateSet {
	_, _, _, _ = d, sigma, a, s
	panic("engine.Res: TODO O2")
}
