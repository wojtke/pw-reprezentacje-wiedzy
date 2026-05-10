// Package api is the O5 facade — the only contract between frontends
// (cmd/ds4, cmd/ds4-web) and the core. Types here carry json tags so the
// web variant can marshal them with encoding/json directly; Fyne reads
// the same fields by name. See DESIGN_go.md §O5.
//
// Translation rule: core types (model.Domain, model.State, engine.TraceNode,
// engine.Answer) MUST NOT escape this package. Solve() builds api.SolveResult
// by walking engine.TraceNode → api.Branch / api.Step and converting
// model.State (opaque, hashable) → api.State (sorted []string).
package api

// SolveResult is what Solve() returns to the UI layer.
//
// Open question (DESIGN_go.md §5): final field set. The shape below is
// the agreed-on starting point and MUST stay JSON-serializable.
type SolveResult struct {
	Answer  Answer   `json:"answer"`
	Trace   []Branch `json:"trace"`             // one Branch per σ₀
	Witness []State  `json:"witness,omitempty"` // for Q2: states satisfying γ
	Summary Summary  `json:"summary"`
	Error   *Error   `json:"error,omitempty"`
}

// Answer is the YES / NO / ERROR axis.
type Answer string

const (
	AnswerYes   Answer = "yes"
	AnswerNo    Answer = "no"
	AnswerError Answer = "error"
)

// Branch is a per-σ₀ execution tree (mirrors engine.TraceNode but
// strictly JSON-friendly so frontends never see core types).
type Branch struct {
	InitialState  State  `json:"initial_state"`
	Steps         []Step `json:"steps"`
	BlockedReason string `json:"blocked_reason,omitempty"`
}

// Step is one node of the trace tree (children encode nondeterminism).
type Step struct {
	Action        string   `json:"action,omitempty"`
	Decomposition []string `json:"decomposition,omitempty"`
	State         State    `json:"state"`
	Children      []Step   `json:"children,omitempty"`
	BlockedReason string   `json:"blocked_reason,omitempty"`
}

// State is a sorted list of fluents true in this state — JSON-friendly.
type State struct {
	True []string `json:"true"`
}

// Summary feeds the status bar (|Σ|, |Σ₀|, elapsed).
type Summary struct {
	SigmaCount  int   `json:"sigma_count"`
	Sigma0Count int   `json:"sigma0_count"`
	ElapsedMs   int64 `json:"elapsed_ms"`
}

// ErrorKind is the discriminator for Error.
type ErrorKind string

const (
	ErrorKindSyntax   ErrorKind = "syntax"
	ErrorKindSemantic ErrorKind = "semantic"
	ErrorKindInternal ErrorKind = "internal"
)

// Error is the structured error shape returned by Solve / ValidateDomain.
type Error struct {
	Kind     ErrorKind `json:"kind"`
	Message  string    `json:"message"`
	Location *Location `json:"location,omitempty"` // for syntax errors (from O4)
}

// Location is a 1-based source position (line, column) — supplied by O4.
type Location struct {
	Line   int `json:"line"`
	Column int `json:"column"`
}

// ValidationResult is what ValidateDomain returns: model is parseable
// AND |Σ| > 0 AND |Σ₀| > 0 (DESIGN_go.md §O5).
type ValidationResult struct {
	OK    bool   `json:"ok"`
	Error *Error `json:"error,omitempty"`
}

// ExampleSummary is one entry in the Examples menu (O6 → O7/O7b).
type ExampleSummary struct {
	ID    string   `json:"id"`
	Title string   `json:"title"`
	Tags  []string `json:"tags,omitempty"` // e.g. "Z1", "Z6", "Q2-necessary"
}

// Example is the full payload the UI loads when the user picks one.
type Example struct {
	ID      string         `json:"id"`
	Title   string         `json:"title"`
	Domain  string         `json:"domain"`
	Queries []ExampleQuery `json:"queries"`
}

type ExampleQuery struct {
	Text     string `json:"text"`
	Expected Answer `json:"expected"` // oracle for e2e tests (C3, C5)
	Note     string `json:"note,omitempty"`
}
