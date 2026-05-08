"""Q1 i Q2 × {necessary, possibly} (O3) — semantyka wg PK2 §7.3."""
from __future__ import annotations
from typing import TYPE_CHECKING

if TYPE_CHECKING:
    from ds4.action_lang.model import Domain, State
    from ds4.query_lang.process import Process


def q1_necessary_executable(
    process: "Process",
    sigma_0: frozenset["State"],
    domain: "Domain",
    sigma: frozenset["State"],
) -> bool:
    raise NotImplementedError


def q1_possibly_executable(
    process: "Process",
    sigma_0: frozenset["State"],
    domain: "Domain",
    sigma: frozenset["State"],
) -> bool:
    raise NotImplementedError


def q2_necessary_after(
    process: "Process",
    gamma: object,  # Formula
    sigma_0: frozenset["State"],
    domain: "Domain",
    sigma: frozenset["State"],
) -> bool:
    raise NotImplementedError


def q2_possibly_after(
    process: "Process",
    gamma: object,  # Formula
    sigma_0: frozenset["State"],
    domain: "Domain",
    sigma: frozenset["State"],
) -> bool:
    raise NotImplementedError
