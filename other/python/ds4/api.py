"""Facade — publiczny kontrakt z frontendem (O5).

Frontend (`gui/`) rozmawia z core'em wyłącznie przez ten moduł.
Wszystkie funkcje przyjmują stringi i zwracają JSON-serializowalne dict-y.
TypedDict-y poniżej to startowy szkielet; konkretny kształt `SolveResult`
(w szczególności struktura trace'a) — decyzja O3+O5+O7.
"""
from __future__ import annotations
from typing import Any, List, Optional, TypedDict


StateDict = dict[str, bool]


class Location(TypedDict):
    line: int
    col: int


class Error(TypedDict):
    """Błąd parsera (O4) lub walidacji semantycznej (O5)."""
    kind: str                       # "syntax" | "semantic"
    message: str
    location: Optional[Location]


class SolveResult(TypedDict, total=False):
    """Wynik solve(). JSON-serializowalny. Pola opcjonalne — kształt
    finalnie ustala O3+O5+O7 (m.in. struktura `trace`)."""
    ok: bool
    error: Optional[Error]
    answer: Optional[bool]
    trace: list[Any]


class ValidationResult(TypedDict):
    ok: bool
    errors: list[Error]


class ExampleSummary(TypedDict):
    id: str
    name: str
    description: str


class Example(TypedDict):
    id: str
    name: str
    description: str
    domain: str
    queries: list[str]


def solve(domain_text: str, query_text: str) -> SolveResult:
    """Pełny przebieg: parsuje dziedzinę i kwerendę, ewaluuje, zwraca wynik."""
    raise NotImplementedError


def validate_domain(domain_text: str) -> ValidationResult:
    """Sprawdza poprawność dziedziny bez ewaluacji kwerendy."""
    raise NotImplementedError


def list_examples() -> List[ExampleSummary]:
    """Lista wbudowanych przykładów do menu Przykłady (czyta z O6)."""
    raise NotImplementedError


def load_example(example_id: str) -> Example:
    """Zwraca pełny przykład (czyta z O6)."""
    raise NotImplementedError
