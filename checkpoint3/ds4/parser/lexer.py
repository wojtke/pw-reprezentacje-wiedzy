"""Lexer (O4) — z lokalizacją line/col w każdym tokenie."""
from dataclasses import dataclass


@dataclass(frozen=True)
class Token:
    kind: str
    text: str
    line: int
    col: int


def tokenize(text: str) -> list[Token]:
    raise NotImplementedError
