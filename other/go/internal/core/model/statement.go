package model

// Statement is one normalized clause of a domain description (PK2 §3.2).
// `impossible` and `initially` are desugared by the parser (O4) and do
// not appear here. `noninertial` survives (DESIGN_go.md §2 rule 3).
//
// go-sumtype:decl Statement
type Statement interface {
	statement()
}

type Causes struct {
	Action  Action
	Effect  Formula
	Precond Formula // ε if absent
}

type Releases struct {
	Action  Action
	Fluent  Fluent
	Precond Formula
}

type Always struct {
	Constraint Formula
}

type Noninertial struct {
	Fluent Fluent
}

type After struct {
	Observed Formula
	Program  []Action // sequence of simple actions
}

type ObservableAfter struct {
	Observed Formula
	Program  []Action
}

func (Causes) statement()          {}
func (Releases) statement()        {}
func (Always) statement()          {}
func (Noninertial) statement()     {}
func (After) statement()           {}
func (ObservableAfter) statement() {}
