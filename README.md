# TNX LEVELS v4

Advanced pivot suite with ETF options levels for TradingView.

![Platform](https://img.shields.io/badge/platform-TradingView-blue)
![Pine](https://img.shields.io/badge/Pine-v6-green)
![License](https://img.shields.io/badge/license-MPL--2.0-orange)

## What It Does

A multi-method pivot indicator that overlays key levels, pivot lines, and ETF options positioning on any chart.

## Features

- **Key levels**: Yearly high/low, last month, 2 months ago, yesterday, 2 days ago, overnight session
- **7 pivot methods**:
  - Classic
  - Woodie
  - Camarilla
  - DeMark
  - Fibonacci
  - Vortex
  - Harmonic Geometric Pivot (custom, based on φ and polar rotation)
- **Axis Projection Band**: Taylor-expansion projection using velocity, acceleration, jerk
- **ETF options levels** (auto-detected by ticker):
  - SPY, SPXL, SPXS, TQQQ, GLD
  - Call Wall, Put Wall, Max Pain, Gamma Flip, Expected Move, 3× General Calls/Puts bars
- **Smart dedup**: Levels within 1 tick merge into a single line + label

## Installation

1. Open TradingView → open any chart
2. Click **Pine Editor** at the bottom
3. Paste `TNX_LEVELS_v4.pine`
4. Click **Add to chart**

## Settings Overview

| Group | Purpose |
|---|---|
| Yearly / Monthly / Daily | Toggle which historical levels to show |
| Overnight | Custom overnight session window |
| Pivot Types | Enable/disable each of the 7 pivot methods |
| Pivot Colors | Colour each method |
| ETF Options Levels | Enter option-derived levels per ticker |

## License

Mozilla Public License 2.0 — see [LICENSE](LICENSE) for details.
