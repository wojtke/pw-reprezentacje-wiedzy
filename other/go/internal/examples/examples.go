// Package examples is O6 from DESIGN_go.md: built-in domains + queries
// shipped inside the .exe via //go:embed. Acts as both the "Examples"
// menu source for O7/O7b and the oracle for e2e tests (C3, C5).
//
// This package does NOT import internal/api on purpose — the dependency
// runs the other way (api orchestrates examples). The Expected type is
// duplicated as a local string alias; api.LoadExample() converts it to
// api.Answer.
package examples

import (
	"embed"
	"encoding/json"
	"fmt"
	"io/fs"
)

//go:embed manifest.json data/*.txt
var fsys embed.FS

// Expected is the oracle answer for a query in the manifest. Mirrors
// api.Answer values ("yes" | "no") but kept independent to preserve
// the api → examples dependency direction.
type Expected string

const (
	ExpectedYes Expected = "yes"
	ExpectedNo  Expected = "no"
)

// Valid reports whether e is a recognized expected answer.
func (e Expected) Valid() bool { return e == ExpectedYes || e == ExpectedNo }

// Manifest mirrors manifest.json (DESIGN_go.md §O6).
type Manifest struct {
	Examples []Entry `json:"examples"`
}

type Entry struct {
	ID      string   `json:"id"`
	Title   string   `json:"title"`
	File    string   `json:"file"` // relative to data/
	Tags    []string `json:"tags,omitempty"`
	Queries []Query  `json:"queries"`
}

type Query struct {
	Text     string   `json:"text"`
	Expected Expected `json:"expected"`
	Note     string   `json:"note,omitempty"`
}

// Load parses the embedded manifest and validates Expected values.
// (TODO O6: populate manifest entries with real domains/queries.)
func Load() (*Manifest, error) {
	b, err := fs.ReadFile(fsys, "manifest.json")
	if err != nil {
		return nil, fmt.Errorf("examples: read manifest: %w", err)
	}
	var m Manifest
	if err := json.Unmarshal(b, &m); err != nil {
		return nil, fmt.Errorf("examples: parse manifest: %w", err)
	}
	for _, e := range m.Examples {
		for i, q := range e.Queries {
			if !q.Expected.Valid() {
				return nil, fmt.Errorf("examples: %s query #%d: invalid expected %q", e.ID, i, q.Expected)
			}
		}
	}
	return &m, nil
}

// ReadDomain returns the raw text of the domain file for a given entry.
func ReadDomain(e Entry) (string, error) {
	b, err := fs.ReadFile(fsys, "data/"+e.File)
	if err != nil {
		return "", fmt.Errorf("examples: read %s: %w", e.File, err)
	}
	return string(b), nil
}
