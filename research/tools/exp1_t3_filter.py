"""Exp 1 — what T3(n, v) actually is as a filter: lag, overshoot, noise suppression, crossing frequency."""
import numpy as np, json, matplotlib; matplotlib.use("Agg"); import matplotlib.pyplot as plt
from t3lab import t3, ema_pine
OUT = "../results/exp1_t3_filter.json"; FIG = "../report/figs/"
N = 600
def impulse(fn):
    x = np.zeros(N); x[50] = 1.0; return fn(x)[50:]
def step(fn):
    x = np.zeros(N); x[50:] = 1.0; return fn(x)[50:]
def props(fn):
    hi = impulse(fn); st = step(fn)
    lag = (np.arange(len(hi)) * hi).sum() / hi.sum()
    w = np.fft.rfftfreq(4096); H = np.abs(np.fft.rfft(hi, 4096))
    per = np.array([2, 3, 4, 5, 8, 10, 20, 50])
    gains = {int(p): float(np.interp(1 / p, w, H)) for p in per}
    return dict(lag=float(lag), overshoot_pct=float((st.max() - 1) * 100), undershoot_pct=float(min(0, hi.min()) / hi.max() * 100),
                noise_var_ratio=float((hi**2).sum()), gain_by_period=gains, peak_gain=float(H.max()),
                peak_gain_period=float(1 / w[H.argmax()]) if w[H.argmax()] > 0 else np.inf)
res = {}
for n in [3, 5, 8, 13, 21]:
    for v in [0.0, 0.5, 0.7, 0.8, 0.9, 1.0]:
        res[f"T3({n},{v})"] = props(lambda x: t3(x, n, v))
    res[f"EMA({n})"] = props(lambda x: ema_pine(x, n))
# crossing frequency on a pure random walk (unit-variance increments) + on random walk with a deterministic trend
rng = np.random.default_rng(7); rw = np.cumsum(rng.standard_normal(200_000))
cross = {}
for n in [3, 5, 8, 13, 21]:
    for v in [0.7, 0.9]:
        d = rw - t3(rw, n, v); cross[f"T3({n},{v})"] = float(np.mean(np.sign(d[1:]) != np.sign(d[:-1])))
    d = rw - ema_pine(rw, n); cross[f"EMA({n})"] = float(np.mean(np.sign(d[1:]) != np.sign(d[:-1])))
res["_crossing_rate_randomwalk"] = cross
json.dump(res, open(OUT, "w"), indent=1)
# ---- figures
fig, ax = plt.subplots(1, 3, figsize=(15, 4))
for v in [0.0, 0.7, 0.9, 1.0]:
    ax[0].plot(step(lambda x: t3(x, 3, v))[:20], marker="o", ms=3, label=f"v={v}")
ax[0].axhline(1, c="k", lw=.6); ax[0].set_title("Step response T3(3, v)  (overshoot = whipsaw source)"); ax[0].legend(); ax[0].set_xlabel("bars after step")
w = np.fft.rfftfreq(4096)
for lbl, fn in [("T3(3,0.9)", lambda x: t3(x, 3, .9)), ("T3(8,0.7)", lambda x: t3(x, 8, .7)), ("EMA(3)", lambda x: ema_pine(x, 3)), ("EMA(8)", lambda x: ema_pine(x, 8))]:
    H = np.abs(np.fft.rfft(impulse(fn), 4096)); ax[1].semilogx(1 / w[1:], H[1:], label=lbl)
ax[1].axhline(1, c="k", lw=.6); ax[1].set_xlim(2, 200); ax[1].invert_xaxis(); ax[1].set_title("Gain vs cycle period (bars) — >1 = amplifies"); ax[1].set_xlabel("period (bars)"); ax[1].legend()
ns = [3, 5, 8, 13, 21]
for v in [0.5, 0.7, 0.9]:
    ax[2].plot(ns, [res[f"T3({n},{v})"]["lag"] for n in ns], marker="o", label=f"T3 v={v}")
ax[2].plot(ns, [res[f"EMA({n})"]["lag"] for n in ns], marker="s", c="k", label="EMA")
ax[2].set_title("Lag (impulse-response centroid, bars)"); ax[2].set_xlabel("length n"); ax[2].legend()
plt.tight_layout(); plt.savefig(FIG + "exp1_t3_filter.png", dpi=110)
for k in ["T3(3,0.9)", "T3(3,0.7)", "T3(5,0.7)", "T3(8,0.7)", "T3(13,0.7)", "EMA(3)", "EMA(8)"]:
    r = res[k]; print(f"{k:11s} lag={r['lag']:.2f} overshoot={r['overshoot_pct']:.1f}% noiseVar={r['noise_var_ratio']:.3f} peakGain={r['peak_gain']:.3f}@{r['peak_gain_period']:.1f}bars gain(P=3)={r['gain_by_period'][3]:.2f} gain(P=5)={r['gain_by_period'][5]:.2f}")
print(cross)
