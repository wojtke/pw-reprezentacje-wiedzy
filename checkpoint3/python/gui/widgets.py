"""Renderery wyniku i błędów (O7) — kształt argumentów wg facade O5."""
from __future__ import annotations


def render_solve_result(parent, result) -> None:
    """Renderuje SolveResult: odpowiedź + podsumowanie + trace."""
    raise NotImplementedError


def render_error(parent, error) -> None:
    """Renderuje błąd parsera/walidacji."""
    raise NotImplementedError
