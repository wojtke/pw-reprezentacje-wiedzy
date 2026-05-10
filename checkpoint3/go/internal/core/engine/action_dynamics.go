package engine

import "ds4/internal/core/model"

// StateSet is the set representation engine uses internally.
type StateSet map[model.State]struct{}

// Sigma builds Σ from the domain's `always` constraints (PK2 §4.1). (TODO O2)
func Sigma(d *model.Domain) StateSet {
	_ = d
	panic("engine.Sigma: TODO O2")
}

// Sigma0 builds Σ₀ by filtering Σ through `initially` / `after` /
// `observable after` (PK2 §4.2, §8.3). Lives in O2 because the filtering
// requires backward application of Res. (TODO O2)
func Sigma0(d *model.Domain, sigma StateSet) StateSet {
	_, _ = d, sigma
	panic("engine.Sigma0: TODO O2")
}

// Res returns the (possibly empty) set of successor states after executing
// a simple action in s (PK2 §4.3). Empty result == action not executable.
// (TODO O2)
func Res(d *model.Domain, sigma StateSet, a model.Action, s model.State) StateSet {
	_, _, _, _ = d, sigma, a, s
	panic("engine.Res: TODO O2")
}
