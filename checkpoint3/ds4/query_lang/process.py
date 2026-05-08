"""Procesy: iteracja Ψ + trace (O3)."""
from __future__ import annotations
from typing import Any, TYPE_CHECKING

if TYPE_CHECKING:
    from ds4.action_lang.model import Action, Domain, State


Process = tuple[frozenset["Action"], ...]

# Kształt trace'u — decyzja O3+O5+O7 (cross-cutting: musi unieść
# wielość σ₀ i wielość trajektorii Ψ z PK2 §6.2).
Trace = list[Any]


def execute(
    process: Process,
    sigma_0: frozenset["State"],
    domain: "Domain",
    sigma: frozenset["State"],
) -> Trace:
    raise NotImplementedError
