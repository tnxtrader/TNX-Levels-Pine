import sys, pathlib
sys.path.insert(0, str(pathlib.Path(__file__).resolve().parents[1]))
import pytest
from t3lab import selftest

@pytest.mark.parametrize("name,passed,detail", selftest.run_all())
def test_selftest(name, passed, detail):
    assert passed, f"{name}: {detail}"
