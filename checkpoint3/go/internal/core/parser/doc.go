// Package parser implements O4 from DESIGN_go.md: text → AST using
// alecthomas/participle/v2 (grammar lives in struct tags).
//
//   - DomainParser: text → model.Domain (after desugaring `impossible`,
//     `initially`).
//   - QueryParser:  text → engine.Query.
//
// Errors carry lexer.Position so O5 can map them to api.Error{Kind:"syntax"}.
package parser
