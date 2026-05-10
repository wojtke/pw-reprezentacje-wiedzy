package api

// Solve parses domain + query, runs the engine, and packages the result.
// Never returns a Go error: failures show up as SolveResult.Error so the
// JSON contract for the web variant stays uniform.
//
// (TODO O5: orchestrate parser → model → engine.)
func Solve(domain, query string) SolveResult {
	_, _ = domain, query
	return SolveResult{
		Answer: AnswerError,
		Error: &Error{
			Kind:    ErrorKindInternal,
			Message: "api.Solve: TODO O5",
		},
	}
}

// ValidateDomain parses the domain and checks |Σ| > 0 ∧ |Σ₀| > 0
// (DESIGN_go.md §O5). (TODO O5.)
func ValidateDomain(domain string) ValidationResult {
	_ = domain
	return ValidationResult{
		OK: false,
		Error: &Error{
			Kind:    ErrorKindInternal,
			Message: "api.ValidateDomain: TODO O5",
		},
	}
}

// ListExamples returns the menu entries for O7/O7b. Backed by O6. (TODO O5.)
func ListExamples() []ExampleSummary {
	return nil
}

// LoadExample returns the full payload for a single example. (TODO O5.)
func LoadExample(id string) (Example, *Error) {
	_ = id
	return Example{}, &Error{
		Kind:    ErrorKindInternal,
		Message: "api.LoadExample: TODO O5",
	}
}
