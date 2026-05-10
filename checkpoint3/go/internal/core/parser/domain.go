package parser

import (
	"ds4/internal/core/engine"
	"ds4/internal/core/model"
)

// ParseDomain parses a domain description and returns the desugared model.
// (TODO O4: define participle struct types for the 8 PK2 §3.2 statements,
// build parser, normalize → desugar `impossible`, `initially`.)
func ParseDomain(text string) (*model.Domain, error) {
	_ = text
	panic("parser.ParseDomain: TODO O4")
}

// ParseQuery parses a single query (Q1/Q2 × {necessary, possibly}).
// (TODO O4.)
func ParseQuery(text string) (engine.Query, error) {
	_ = text
	panic("parser.ParseQuery: TODO O4")
}
