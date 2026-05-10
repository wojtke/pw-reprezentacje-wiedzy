package engine

import "ds4/internal/core/model"

// TraceNode is the per-σ₀ execution tree produced by O3 (DESIGN_go.md §O3).
// Children encode nondeterminism of Res; BlockedReason is non-empty when
// the action was not executable in this branch.
type TraceNode struct {
	Step          int
	Action        model.Action // empty at the root
	Decomposition []model.Action
	State         model.State
	Children      []*TraceNode
	BlockedReason string
}
