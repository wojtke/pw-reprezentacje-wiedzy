"""Formuły zdaniowe (O1).

Konkretny zestaw węzłów AST i to, czy używamy postaci normalnej (DNF/CNF/żadnej),
to decyzja O1 wg PK2 §2.2. Poniżej startowy szkielet — O1 dodaje/usuwa węzły
wedle uznania.
"""
from __future__ import annotations
from dataclasses import dataclass
from typing import TYPE_CHECKING

if TYPE_CHECKING:
    from ds4.action_lang.model import State


@dataclass(frozen=True)
class Atom:
    fluent: object  # Fluent


@dataclass(frozen=True)
class Not:
    inner: object


@dataclass(frozen=True)
class And:
    left: object
    right: object


@dataclass(frozen=True)
class Or:
    left: object
    right: object


def evaluate(formula: object, state: "State") -> bool:
    """True iff `state` spełnia `formula`."""
    raise NotImplementedError
