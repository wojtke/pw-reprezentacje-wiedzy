"""Generacja Σ i Σ₀ (O2) — wg PK2 §4.1, §4.2."""
from __future__ import annotations
from typing import TYPE_CHECKING

if TYPE_CHECKING:
    from ds4.action_lang.model import Domain, State


def generate_sigma(domain: "Domain") -> frozenset["State"]:
    """Stany dopuszczalne: spełniają wszystkie zdania `always`."""
    raise NotImplementedError


def generate_sigma0(domain: "Domain", sigma: frozenset["State"]) -> frozenset["State"]:
    """Stany początkowe: podzbiór Σ zgodny ze zdaniami `initially`, `after`
    i `observable after` (PK2 §4.2, §8.3). Należy do O2 — używa wyłącznie
    `res` dla akcji prostej (które tu jest), nie wymaga akcji złożonych z O3.
    """
    raise NotImplementedError
