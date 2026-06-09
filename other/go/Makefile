.PHONY: dev dev-web test test-update lint sumtype build-fyne build-web tidy

dev:
	go run ./cmd/ds4

dev-web:
	go run ./cmd/ds4-web

test:
	go test ./...

test-update:
	go test ./tests/... -update

lint:
	golangci-lint run

# Sum-type exhaustiveness check — separate tool (not a golangci-lint linter).
# Install once: `go install github.com/BurntSushi/go-sumtype@latest`
sumtype:
	go-sumtype ./...

# Run on Windows (cgo + OpenGL toolchain required for Fyne).
build-fyne:
	go build -ldflags="-s -w" -o ds4.exe ./cmd/ds4

# Cross-compiles from any host (no cgo).
build-web:
	GOOS=windows GOARCH=amd64 go build -ldflags="-s -w" -o ds4-web.exe ./cmd/ds4-web

tidy:
	go mod tidy
