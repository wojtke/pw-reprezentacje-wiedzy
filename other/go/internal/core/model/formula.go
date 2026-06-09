package model

// Formula is a propositional formula over fluents (PK2 §3.2 / §5.2).
//
// go-sumtype:decl Formula
type Formula interface {
	formula()
}

// Concrete variants — the exact set is finalized in O1.

type FTrue struct{}
type FFalse struct{}
type FAtom struct{ Name Fluent }
type FNot struct{ X Formula }
type FAnd struct{ L, R Formula }
type FOr struct{ L, R Formula }
type FImpl struct{ L, R Formula }
type FIff struct{ L, R Formula }

func (FTrue) formula()  {}
func (FFalse) formula() {}
func (FAtom) formula()  {}
func (FNot) formula()   {}
func (FAnd) formula()   {}
func (FOr) formula()    {}
func (FImpl) formula()  {}
func (FIff) formula()   {}

// Eval returns whether s |= f. (TODO O1)
func Eval(f Formula, s State) bool {
	_ = f
	_ = s
	panic("model.Eval: TODO O1")
}

// DNF converts f to disjunctive normal form (PK2 §5.2 / §8.0).
// Required by O3 for action-conflict detection. (TODO O1)
func DNF(f Formula) Formula {
	_ = f
	panic("model.DNF: TODO O1")
}
