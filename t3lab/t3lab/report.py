"""Every phase writes out/<PHASE>/report.md + gates.json + PNG plots + CSV tables.
Report layout is fixed: What it did / Tried / Changed / Found / Gates."""
from __future__ import annotations
import json, datetime
from pathlib import Path
import matplotlib
matplotlib.use("Agg")
import matplotlib.pyplot as plt
import pandas as pd

class Report:
    def __init__(self, out: Path, phase: str, title: str):
        self.dir = Path(out) / phase; self.dir.mkdir(parents=True, exist_ok=True)
        self.phase, self.title = phase, title
        self.sec = {"did": [], "tried": [], "changed": [], "found": []}
        self.gates, self.figs = [], []

    def add(self, section, text):
        self.sec[section].append(text)

    def table(self, section, df: pd.DataFrame, name: str, max_rows=30):
        df.to_csv(self.dir / f"{name}.csv", index=False)
        show = df.head(max_rows)
        self.sec[section].append(f"`{name}.csv` ({len(df)} rows{', first ' + str(max_rows) if len(df) > max_rows else ''}):\n\n"
                                 + show.to_markdown(index=False, floatfmt=".4g"))

    def fig(self, fig, name, caption):
        fig.tight_layout(); fig.savefig(self.dir / f"{name}.png", dpi=110); plt.close(fig)
        self.figs.append((name, caption))

    def gate(self, name, value, rule, passed, hard=True):
        self.gates.append(dict(gate=name, value=value, rule=rule, passed=bool(passed), hard=hard))

    @property
    def status(self):
        return "PASS" if all(g["passed"] for g in self.gates if g["hard"]) else "STOP"

    def save(self, extra=None):
        st = self.status
        md = [f"# {self.phase} — {self.title}", f"_generated {datetime.datetime.now():%Y-%m-%d %H:%M}_  ·  **status: {st}**", ""]
        for key, head in (("did", "What this phase did"), ("tried", "What was tried"), ("changed", "What was changed"),
                          ("found", "What was found")):
            md += [f"## {head}", ""] + (self.sec[key] or ["(nothing)"]) + [""]
        md += ["## Gates", "", "| gate | value | rule | hard | result |", "|---|---|---|---|---|"]
        md += [f"| {g['gate']} | {g['value']} | {g['rule']} | {'yes' if g['hard'] else 'report'} | "
               f"{'PASS' if g['passed'] else 'FAIL'} |" for g in self.gates]
        md += ["", "## Plots", ""] + [f"**{c}**\n\n![{n}]({n}.png)\n" for n, c in self.figs]
        (self.dir / "report.md").write_text("\n".join(md))
        json.dump(dict(phase=self.phase, status=st, gates=self.gates, **(extra or {})),
                  open(self.dir / "gates.json", "w"), indent=1, default=str)
        print(f"[{self.phase}] {st} -> {self.dir / 'report.md'}")
        return st

def require(out: Path, phase: str):
    g = Path(out) / phase / "gates.json"
    if not g.exists():
        raise SystemExit(f"{phase} has not been run. Run it first.")
    if json.load(open(g))["status"] != "PASS":
        raise SystemExit(f"{phase} status is STOP. Read {g.parent / 'report.md'} — the pipeline stops there by design.")
