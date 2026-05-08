"""Akcje złożone (O3) — wg PK2 §5."""
from __future__ import annotations
from typing import TYPE_CHECKING

if TYPE_CHECKING:
    from ds4.action_lang.model import Action, Domain, State


def res_compound(
    compound: frozenset["Action"],
    state: "State",
    domain: "Domain",
    sigma: frozenset["State"],
) -> frozenset["State"]:
    """Stany wynikowe po wykonaniu akcji złożonej w `state`.
    ∅ gdy w `state` nie istnieje żadna dopuszczalna realizacja.
    """
    raise NotImplementedError
