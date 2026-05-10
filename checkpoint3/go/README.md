# DS4 Reasoner (Go)

Implementation of Project 4 — action language + query language reasoner — in Go.
Design: see [`../DESIGN_go.md`](../DESIGN_go.md).

## Layout

```
go/
├── go.mod
├── cmd/
│   ├── ds4/                # Fyne GUI entrypoint (primary, O7)
│   └── ds4-web/            # HTTP+SPA entrypoint (fallback, O7b)
│       └── static/         # //go:embed-ed SPA assets
├── internal/
│   ├── api/                # facade (O5) — only contract for frontends
│   ├── core/
│   │   ├── model/          # O1 — Fluent, Action, State, Domain, Statement, Formula
│   │   ├── engine/         # O2, O3 — Σ, Σ₀, Res, query evaluation
│   │   └── parser/         # O4 — participle-based DomainParser, QueryParser
│   └── examples/           # O6 — //go:embed *.txt + manifest.json
└── tests/                  # e2e via api facade (goldie)
```

## Dev workflow

| Target              | Command                                                                          |
|---------------------|----------------------------------------------------------------------------------|
| Run Fyne app (mac)  | `go run ./cmd/ds4`                                                               |
| Run web app         | `go run ./cmd/ds4-web`                                                           |
| Test                | `go test ./...`                                                                  |
| Update goldens      | `go test ./tests/... -update`                                                    |
| Lint                | `golangci-lint run`                                                              |
| Build Fyne (Win)    | `go build -ldflags="-s -w" -o ds4.exe ./cmd/ds4`        *(run on Windows)*       |
| Build web (cross)   | `GOOS=windows GOARCH=amd64 go build -ldflags="-s -w" -o ds4-web.exe ./cmd/ds4-web` |

See `Makefile` for shortcuts.
