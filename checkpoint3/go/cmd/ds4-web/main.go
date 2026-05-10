// Command ds4-web is the fallback web frontend (O7b) — HTTP server +
// embedded SPA. Activated only if Fyne is blocked by 5.05.
//
// Build (cross): GOOS=windows GOARCH=amd64 go build -ldflags="-s -w" -o ds4-web.exe ./cmd/ds4-web
// Run:           go run ./cmd/ds4-web
package main

import (
	"embed"
	"encoding/json"
	"fmt"
	"io/fs"
	"log"
	"net"
	"net/http"
	"os/exec"
	"runtime"

	"ds4/internal/api"
)

//go:embed static/*
var staticFS embed.FS

const (
	portMin = 8080
	portMax = 8090
)

func main() {
	mux := http.NewServeMux()

	staticSub, err := fs.Sub(staticFS, "static")
	if err != nil {
		log.Fatalf("ds4-web: embed: %v", err)
	}
	mux.Handle("/", http.FileServer(http.FS(staticSub)))

	mux.HandleFunc("GET /api/examples", handleListExamples)
	mux.HandleFunc("GET /api/examples/{id}", handleLoadExample)
	mux.HandleFunc("POST /api/solve", handleSolve)
	mux.HandleFunc("POST /api/validate", handleValidate)

	ln, err := listenFirstFree(portMin, portMax)
	if err != nil {
		log.Fatalf("ds4-web: no free port in %d-%d: %v", portMin, portMax, err)
	}
	addr := ln.Addr().String()
	url := "http://" + addr

	// ln is already accepting connections — no race, no sleep needed.
	go func() {
		if err := openBrowser(url); err != nil {
			log.Printf("ds4-web: could not open browser: %v (open %s manually)", err, url)
		}
	}()

	fmt.Printf("ds4-web listening on %s — close this console window to stop.\n", url)
	if err := http.Serve(ln, mux); err != nil {
		log.Fatalf("ds4-web: serve: %v", err)
	}
}

func listenFirstFree(lo, hi int) (net.Listener, error) {
	for p := lo; p <= hi; p++ {
		ln, err := net.Listen("tcp", fmt.Sprintf("127.0.0.1:%d", p))
		if err == nil {
			return ln, nil
		}
	}
	return nil, fmt.Errorf("all ports in [%d,%d] busy", lo, hi)
}

func openBrowser(url string) error {
	switch runtime.GOOS {
	case "windows":
		return exec.Command("cmd", "/c", "start", "", url).Start()
	case "darwin":
		return exec.Command("open", url).Start()
	default:
		return exec.Command("xdg-open", url).Start()
	}
}

// --- handlers (thin marshal/unmarshal wrappers around internal/api) ---

func handleListExamples(w http.ResponseWriter, _ *http.Request) {
	writeJSON(w, http.StatusOK, api.ListExamples())
}

func handleLoadExample(w http.ResponseWriter, r *http.Request) {
	id := r.PathValue("id")
	ex, apiErr := api.LoadExample(id)
	if apiErr != nil {
		writeJSON(w, http.StatusNotFound, apiErr)
		return
	}
	writeJSON(w, http.StatusOK, ex)
}

func handleSolve(w http.ResponseWriter, r *http.Request) {
	var body struct {
		Domain string `json:"domain"`
		Query  string `json:"query"`
	}
	if err := json.NewDecoder(r.Body).Decode(&body); err != nil {
		writeJSON(w, http.StatusBadRequest, api.Error{Kind: api.ErrorKindInternal, Message: err.Error()})
		return
	}
	writeJSON(w, http.StatusOK, api.Solve(body.Domain, body.Query))
}

func handleValidate(w http.ResponseWriter, r *http.Request) {
	var body struct {
		Domain string `json:"domain"`
	}
	if err := json.NewDecoder(r.Body).Decode(&body); err != nil {
		writeJSON(w, http.StatusBadRequest, api.Error{Kind: api.ErrorKindInternal, Message: err.Error()})
		return
	}
	writeJSON(w, http.StatusOK, api.ValidateDomain(body.Domain))
}

func writeJSON(w http.ResponseWriter, status int, v any) {
	w.Header().Set("Content-Type", "application/json; charset=utf-8")
	w.WriteHeader(status)
	_ = json.NewEncoder(w).Encode(v)
}
