"""T3 Entry Lab runner.

  python run.py P0 ... P6        run one phase (in order; each needs the previous phase PASS)
  python run.py all              P0 -> P6, stops at the first STOP
  python run.py verify           end-to-end check on synthetic data with the real gates:
                                 a pure random walk must be rejected, a planted edge must be found and pass P6
  options: --config config.yaml, --force (P6 only: re-open a Test window already opened; marks it contaminated)
"""
import argparse, sys, time
from pathlib import Path
import pandas as pd, yaml
sys.path.insert(0, str(Path(__file__).parent))
from t3lab.phases import Ctx, PHASES

HERE = Path(__file__).parent

def run_phases(cfg_path, phases, force=False):
    ctx = Ctx(cfg_path)
    last = None
    for p in phases:
        t = time.time()
        st = PHASES[p](ctx, force) if p == "P6" else PHASES[p](ctx)
        print(f"{p} finished in {time.time() - t:.0f}s: {st}")
        last = (p, st)
        if st != "PASS":
            print(f"Stopped at {p}. Read {ctx.out / p / 'report.md'}")
            break
    return ctx, last

def verify():
    from t3lab.selftest import synthetic
    cfg0 = yaml.safe_load(open(HERE / "config.yaml"))
    reg = pd.read_csv(HERE / cfg0["registry"])
    planted = [dict(candidate_id=f"PLANTED_{s.upper()}", family="STRUCTURE_BREAKOUT", trigger="DONCHIAN_20", filter="NONE",
                    side=s, t3_length=None, t3_vf=None) for s in ("long", "short")]
    sub = pd.concat([reg.sample(60, random_state=1), pd.DataFrame(planted)], ignore_index=True)
    results = {}
    for name, plant in (("noise", 0.0), ("planted", 0.1)):
        base = HERE / "verify" / name
        (base / "data").mkdir(parents=True, exist_ok=True)
        synthetic(1300, seed=11, tick=0.01, plant=plant).to_csv(base / "data" / "CL_30min_continuous_backadjusted.csv", index=False)
        sub.to_csv(base / "candidates.csv", index=False)
        cfg = yaml.safe_load(open(HERE / "config.yaml"))
        cfg["instruments"] = {"CL": cfg["instruments"]["CL"]}
        cfg.update(data_dir="data", out_dir="out", registry="candidates.csv")
        cfg["p2_screen"].update(spa_reps=200)
        cfg["p4_evidence"].update(null_reps=300, boot_reps=1000)
        cfg["p5_validation"].update(null_reps=300, boot_reps=2000)
        cfg["p6_test"].update(boot_reps=2000)
        yaml.safe_dump(cfg, open(base / "config.yaml", "w"), sort_keys=False)
        print(f"\n=== verify: {name} ===")
        ctx, last = run_phases(base / "config.yaml", list(PHASES))
        passed = []
        tf = ctx.out / "P6" / "test.csv"
        if last == ("P6", "PASS") and tf.exists():
            t = pd.read_csv(tf).merge(sub[["candidate_id", "trigger"]], on="candidate_id")
            passed = t[t["pass"]][["candidate_id", "trigger"]].values.tolist()
        results[name] = (last, passed)
    ok_noise = results["noise"][0] != ("P6", "PASS")
    found = results["planted"][1]
    ok_plant = bool(found) and all(trig.startswith(("DONCHIAN", "PREVBAR", "COMPRESSION")) for _, trig in found)
    print("\nVERIFY RESULT")
    print(f"  random walk rejected: {ok_noise} (stopped at {results['noise'][0]})")
    print(f"  planted breakout edge found and passed the sealed test, and nothing else passed: {ok_plant} (P6 passes: {found})")
    return ok_noise and ok_plant

if __name__ == "__main__":
    ap = argparse.ArgumentParser()
    ap.add_argument("phase", choices=list(PHASES) + ["all", "verify"])
    ap.add_argument("--config", default=str(HERE / "config.yaml"))
    ap.add_argument("--force", action="store_true")
    a = ap.parse_args()
    if a.phase == "verify":
        sys.exit(0 if verify() else 1)
    run_phases(a.config, list(PHASES) if a.phase == "all" else [a.phase], a.force)
