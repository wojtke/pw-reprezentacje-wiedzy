package api

import "context"

// Solve parses domain + query, runs the engine, and packages the result.
// Never returns a Go error: failures show up as SolveResult.Error so the
// JSON contract for the web variant stays uniform.
//
// ctx is honored at coarse-grained checkpoints (between parser/engine
// phases and at process-tree branch boundaries inside O3). On cancel,
// Solve returns SolveResult{Answer: AnswerError, Error: {Kind: "internal",
// Message: "cancelled"}}. O7 wires this to a "Cancel" button; O7b wires
// it to r.Context() (so closing the tab kills in-flight Solve).
//
// (TODO O5: orchestrate parser → model → engine.)
func Solve(ctx context.Context, domain, query string) SolveResult {
	_, _, _ = ctx, domain, query
	return SolveResult{
		Answer: AnswerError,
		Error: &Error{
			Kind:    ErrorKindInternal,
			Message: "api.Solve: TODO O5",
		},
	}
}

// ValidateDomain parses the domain and checks |Σ| > 0 ∧ |Σ₀| > 0
// (DESIGN_go.md §O5). ctx semantics same as Solve. (TODO O5.)
func ValidateDomain(ctx context.Context, domain string) ValidationResult {
	_, _ = ctx, domain
	return ValidationResult{
		OK: false,
		Error: &Error{
			Kind:    ErrorKindInternal,
			Message: "api.ValidateDomain: TODO O5",
		},
	}
}

// ListExamples returns the menu entries for O7/O7b. Backed by O6.
// ctx is accepted for symmetry with the rest of the facade; the current
// implementation reads from an embed.FS so cancellation is a no-op, but
// that may change if examples ever come from disk/network. (TODO O5.)
func ListExamples(ctx context.Context) []ExampleSummary {
	_ = ctx
	return nil
}

// LoadExample returns the full payload for a single example.
// ctx semantics: same as ListExamples. (TODO O5.)
func LoadExample(ctx context.Context, id string) (Example, *Error) {
	_, _ = ctx, id
	return Example{}, &Error{
		Kind:    ErrorKindInternal,
		Message: "api.LoadExample: TODO O5",
	}
}
