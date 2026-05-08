"""Klasy danych dziedziny (O1).

Konkretny zestaw węzłów `Statement` (po desugaringu `impossible` i `initially`
w O4 zostaje 6 typów: `causes`, `releases`, `always`, `noninertial`, `after`,
`observable after`) — patrz DESIGN §O1 i PK2 §3.2. Tu trzymamy tylko warstwę
fundamentalną; resztę dokłada O1.
"""
from __future__ import annotations
from dataclasses import dataclass


@dataclass(frozen=True)
class Fluent:
    name: str


@dataclass(frozen=True)
class Action:
    name: str


@dataclass(frozen=True)
class State:
    """Wartościowanie fluentów. Reprezentacja: zbiór fluentów prawdziwych."""
    true_fluents: frozenset[Fluent]


@dataclass(frozen=True)
class Domain:
    fluents: frozenset[Fluent]
    actions: frozenset[Action]
    statements: tuple  # tuple[Statement, ...] — warianty Statement decyzja O1
    # `Domain` udostępnia też pochodne wymagane przez O2:
    # - inertial_fluents() : frozenset[Fluent]   (fluenty NIE objęte `noninertial`)
    # Sygnatura/forma — decyzja O1.
