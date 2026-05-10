package engine

import (
	"context"

	"ds4/internal/core/model"
)

// Query is the AST of a parsed query (Q1 / Q2 × {necessary, possibly}).
// Shape committed below; concrete Process iteration support is finalized
// during O3 + O4 implementation.
//
// go-sumtype:decl Query
type Query interface {
	query()
}

// Q1: <quant> executable P
type Q1Executable struct {
	Quant   Quantifier
	Process Process
}

// Q2: <quant> γ after P
type Q2GoalAfter struct {
	Quant   Quantifier
	Goal    model.Formula
	Process Process
}

func (Q1Executable) query() {}
func (Q2GoalAfter) query()  {}

// Quantifier — necessary | possibly (PK2 §7.3).
type Quantifier int

const (
	QuantNecessary Quantifier = iota
	QuantPossibly
)

// Process is a sequence of (composite or simple) actions Ψ — exact shape
// finalized in O3 + O4.
type Process struct {
	Steps []CompositeAction
}

// CompositeAction is `A ⊆ actions` from PK2 §5.
type CompositeAction struct {
	Members []model.Action
}

// Answer is the engine-level result of a single Solve(): yes/no plus the
// per-σ₀ trace forest used to render the derivation tree.
type Answer struct {
	Holds   bool
	Forest  []*TraceNode  // one tree per σ₀ ∈ Σ₀
	Witness []model.State // states satisfying γ for Q2 (∃-witness or ∀-min set)
}

// Evaluate runs O3 over a (Domain, Σ, Σ₀, Query) tuple. ctx is checked
// between σ₀ branches and at process-tree decomposition points so a
// cancelled api.Solve drops out promptly. On ctx.Err(), returns
// Answer{} and the error. (TODO O3)
func Evaluate(ctx context.Context, d *model.Domain, sigma, sigma0 StateSet, q Query) (Answer, error) {
	_, _, _, _, _ = ctx, d, sigma, sigma0, q
	panic("engine.Evaluate: TODO O3")
}
