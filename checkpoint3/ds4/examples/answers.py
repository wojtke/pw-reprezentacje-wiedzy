"""Oczekiwane odpowiedzi dla przykładów (O6).

Każdy przykład to lista pozycji: tekst kwerendy + oczekiwana odpowiedź TAK/NIE
+ krótka notatka „skąd" (jakie σ₀, którędy idzie trace) używana przy ręcznym
debugowaniu rozbieżności w testach e2e.

Format finalny ustalany razem z O5 (testy e2e to konsumują).
"""
from typing import TypedDict


class ExpectedQuery(TypedDict):
    query: str
    answer: bool
    note: str


# TODO (O6): wyprowadzić ręcznie z semantyki dla każdej kwerendy.
EXPECTED: dict[str, list[ExpectedQuery]] = {
    "switches": [],
    "ysp": [],
    "russian_turkey": [],
}
