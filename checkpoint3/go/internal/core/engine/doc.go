// Package engine implements O2 + O3 from DESIGN_go.md:
//
//   - O2 (action_dynamics.go): Σ, Σ₀, Res for simple actions (PK2 §4),
//     including the F_I-aware New / Res₀ minimization.
//
//   - O3 (query.go, trace.go): composite actions, processes, four
//     query forms Q1/Q2 × {necessary, possibly} (PK2 §5, §7, §8.7).
//     Builds a TraceNode tree, not a reachable-state set.
//
// No imports of fyne.io/... or net/http (depguard rule).
package engine
