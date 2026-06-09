# Checkpoint 3 — DS4 Reasoner

Patrz [`DESIGN.md`](DESIGN.md) dla architektury, podziału obszarów i decyzji technicznych.

## Uruchomienie (dev)

```bash
pip install -e ".[dev]"
python main.py            # GUI (O7)
pytest                    # testy (na razie wszystko xfail)
```

## Layout

- `ds4/` — core (pure-Python, zero zależności UI)
  - `action_lang/` — O1 (model + formuły) + O2 (Σ, Σ₀, res)
  - `query_lang/` — O3 (akcje złożone, procesy, Q1/Q2)
  - `parser/` — O4 (lexer + parsery)
  - `examples/` — O6 (przykłady + oczekiwane odpowiedzi)
  - `api.py` — O5 (facade)
- `gui/` — O7 (Tkinter)
- `packaging/` — O8 (PyInstaller)
- `tests/` — testy jednostkowe + e2e
