package model

// State is a total valuation of all fluents in the domain. It MUST be
// comparable / hashable so that map[State]struct{} works as a set
// (DESIGN_go.md §O2). The concrete representation (sorted-string,
// bitmap, ...) is decided in O1 implementation.
type State struct {
	// TODO(O1): pick a representation. Placeholder keeps the type comparable.
	bits string
}

// TODO(O1): Holds, With, Without, NewSet helpers and friends.
