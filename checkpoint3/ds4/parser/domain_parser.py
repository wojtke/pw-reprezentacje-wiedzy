"""Parser dziedziny (O4) — produkuje `Domain` z O1."""
from __future__ import annotations
from typing import TYPE_CHECKING

if TYPE_CHECKING:
    from ds4.action_lang.model import Domain


def parse_domain(text: str) -> "Domain":
    raise NotImplementedError
