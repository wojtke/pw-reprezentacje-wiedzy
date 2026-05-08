"""Testy end-to-end (O5) — przez ds4.api z przykładami O6.

Każdy przykład × każda kwerenda = jeden test, oracle z ds4.examples.answers.EXPECTED.
"""
import pytest


@pytest.mark.xfail(reason="TODO: po stabilizacji facade (O5) i przykładów (O6)")
def test_placeholder() -> None:
    raise NotImplementedError
