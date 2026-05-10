package engine

import "ds4/internal/core/model"

// Query is the AST of a parsed query (Q1 / Q2 × {necessary, possibly}).
// The exact shape is finalized in O3 + O4 (open question §5).
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

// Evaluate runs O3 over a (Domain, Σ, Σ₀, Query) tuple. (TODO O3)
func Evaluate(d *model.Domain, sigma, sigma0 StateSet, q Query) Answer {
	_, _, _, _ = d, sigma, sigma0, q
	panic("engine.Evaluate: TODO O3")
}
