package model

// Domain is the parsed, desugared, validated description of an action
// domain. Built by O4, consumed by O2/O3.
type Domain struct {
	Fluents    []Fluent
	Actions    []Action
	Statements []Statement

	// Inertial set F_I — derived from `noninertial` statements (PK2 §4.4).
	// Used by O2 in `New` (DESIGN_go.md §O1, §O2).
	Inertial map[Fluent]bool
}
