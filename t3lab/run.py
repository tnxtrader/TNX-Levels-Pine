"""T3 Entry Lab runner.

  python run.py P0            # one phase (P0..P6), in order
  python run.py all           # P0 -> P6, stops at the first phase whose gates fail
  python run.py demo          # code check on SYNTHETIC random-walk data (no edge exists; nothing should survive honestly)
  options: --config config.yaml   --force (P6 only, re-open a test that was already opened: contaminates it)
"""
import argparse, sys, time
from pathlib import Path
import yaml
sys.path.insert(0, str(Path(__file__).parent))
from t3lab.phases import Ctx, PHASES

def demo():
    from t3lab.selftest import synthetic
    base = Path(__file__).parent / "demo"; (base / "data").mkdir(parents=True, exist_ok=True)
    cfg = yaml.safe_load(open(Path(__file__).parent / "config.yaml"))
    ticks = {"CL": 0.01, "GC": 0.1, "NQ": 0.25, "SI": 0.005, "YM": 1.0}
    for k, sym in enumerate(cfg["instruments"]):
        f = base / "data" / cfg["instruments"][sym]["file"]
        if not f.exists():
            synthetic(1300, seed=k, tick=ticks.get(sym, 0.01)).to_csv(f, index=False)
    cfg.update(data_dir="data", out_dir="out", registry="../registry/candidate_registry_5000.csv", registry_limit=150,
               demo_ignore_gates=True)
    cfg["p2_screen"].update(spa_reps=100, shortlist_max=4, min_trades=30)
    cfg["p3_refine"].update(t3_length=[3, 5, 8], t3_vf=[0.7, 0.9], sl_atr=[0.75, 1.0, 1.5], tp_r=[1.0, 2.0], max_bars=[8, 16])
    cfg["p4_evidence"].update(boot_reps=300, null_reps=100)
    cfg["p5_validation"].update(boot_reps=500, null_reps=100); cfg["p6_test"].update(boot_reps=500)
    yaml.safe_dump(cfg, open(base / "config_demo.yaml", "w"), sort_keys=False)
    return base / "config_demo.yaml"

if __name__ == "__main__":
    ap = argparse.ArgumentParser()
    ap.add_argument("phase", choices=list(PHASES) + ["all", "demo"])
    ap.add_argument("--config", default=str(Path(__file__).parent / "config.yaml"))
    ap.add_argument("--force", action="store_true")
    a = ap.parse_args()
    cfgp = demo() if a.phase == "demo" else a.config
    ctx = Ctx(cfgp)
    todo = list(PHASES) if a.phase in ("all", "demo") else [a.phase]
    for p in todo:
        t = time.time()
        st = PHASES[p](ctx, a.force) if p == "P6" else PHASES[p](ctx)
        print(f"{p} finished in {time.time() - t:.0f}s: {st}")
        if st != "PASS" and not ctx.demo:
            print(f"Pipeline stopped at {p}. Read {ctx.out / p / 'report.md'}"); break
