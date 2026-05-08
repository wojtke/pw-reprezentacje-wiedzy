"""res₀, new, res dla akcji prostej (O2).

Sygnatury i pomocnicze funkcje — decyzja O2 wg PK2 §4.
"""
from __future__ import annotations
from typing import TYPE_CHECKING

if TYPE_CHECKING:
    from ds4.action_lang.model import Action, Domain, State


def res(
    action: "Action",
    state: "State",
    domain: "Domain",
    sigma: frozenset["State"],
) -> frozenset["State"]:
    """Zbiór stanów wynikowych po wykonaniu `action` w `state`.
    ∅ gdy akcja niewykonalna w `state`.
    """
    raise NotImplementedError
