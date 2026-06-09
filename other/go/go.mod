module ds4

go 1.22

// Runtime deps (added when needed):
//   github.com/alecthomas/participle/v2  -- O4 parser
//   fyne.io/fyne/v2                      -- O7 GUI (cmd/ds4 only)
//
// Test/lint deps (added when needed):
//   github.com/stretchr/testify          -- unit assertions
//   github.com/sebdah/goldie/v2          -- e2e golden tests
//   github.com/BurntSushi/go-sumtype     -- exhaustiveness linter (via golangci-lint)
