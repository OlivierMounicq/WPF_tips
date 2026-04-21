import { useState, useEffect, useRef, useCallback } from “react”;

const COLORS = {
bg: “#0a0a0f”,
panel: “#11111a”,
border: “#1e1e2e”,
tv: “#ff6b6b”,
qv: “#4ecdc4”,
path: “#a78bfa”,
text: “#e2e8f0”,
muted: “#64748b”,
accent: “#f59e0b”,
};

function generateBrownianPath(steps) {
const path = [0];
for (let i = 1; i <= steps; i++) {
const dt = 1 / steps;
path.push(path[i - 1] + (Math.random() < 0.5 ? 1 : -1) * Math.sqrt(dt));
}
return path;
}

function computeVariations(path) {
let tv = 0;
let qv = 0;
for (let i = 1; i < path.length; i++) {
const diff = path[i] - path[i - 1];
tv += Math.abs(diff);
qv += diff * diff;
}
return { tv, qv };
}

function MiniPath({ path, color }) {
const w = 200, h = 60;
const min = Math.min(…path);
const max = Math.max(…path);
const range = max - min || 1;
const pts = path.map((v, i) => {
const x = (i / (path.length - 1)) * w;
const y = h - ((v - min) / range) * h;
return `${x},${y}`;
}).join(” “);
return (
<svg width={w} height={h} style={{ display: “block” }}>
<polyline points={pts} fill="none" stroke={color} strokeWidth="1.5" strokeLinejoin="round" />
</svg>
);
}

function BarChart({ data, colorFn, label, subtitle }) {
const max = Math.max(…data.map(d => d.value), 0.001);
return (
<div style={{ fontFamily: “‘Courier New’, monospace” }}>
<div style={{ color: COLORS.text, fontSize: 13, marginBottom: 4, fontWeight: “bold” }}>{label}</div>
<div style={{ color: COLORS.muted, fontSize: 11, marginBottom: 12 }}>{subtitle}</div>
{data.map((d, i) => (
<div key={i} style={{ marginBottom: 8 }}>
<div style={{ display: “flex”, justifyContent: “space-between”, marginBottom: 3 }}>
<span style={{ color: COLORS.muted, fontSize: 11 }}>{d.label}</span>
<span style={{ color: colorFn(d.value, max), fontSize: 11, fontWeight: “bold” }}>
{d.value.toFixed(3)}
</span>
</div>
<div style={{ background: COLORS.border, borderRadius: 2, height: 8, overflow: “hidden” }}>
<div style={{
width: `${Math.min((d.value / max) * 100, 100)}%`,
height: “100%”,
background: colorFn(d.value, max),
borderRadius: 2,
transition: “width 0.4s ease”,
}} />
</div>
</div>
))}
</div>
);
}

export default function App() {
const [stepLevels] = useState([10, 20, 50, 100, 200, 500, 1000]);
const [results, setResults] = useState([]);
const [animIdx, setAnimIdx] = useState(0);
const [running, setRunning] = useState(false);
const [paths, setPaths] = useState({});
const timerRef = useRef(null);

const runSimulation = useCallback(() => {
const newResults = [];
const newPaths = {};
for (const steps of stepLevels) {
const path = generateBrownianPath(steps);
const { tv, qv } = computeVariations(path);
newResults.push({ steps, tv, qv });
newPaths[steps] = path;
}
setResults(newResults);
setPaths(newPaths);
setAnimIdx(0);
setRunning(true);
}, [stepLevels]);

useEffect(() => {
runSimulation();
}, []);

useEffect(() => {
if (running && animIdx < stepLevels.length) {
timerRef.current = setTimeout(() => setAnimIdx(i => i + 1), 300);
} else {
setRunning(false);
}
return () => clearTimeout(timerRef.current);
}, [running, animIdx]);

const visible = results.slice(0, animIdx);
const tvData = visible.map(r => ({ label: `N=${r.steps}`, value: r.tv }));
const qvData = visible.map(r => ({ label: `N=${r.steps}`, value: r.qv }));

const tvColor = (v, max) => {
const t = v / max;
const r = Math.round(150 + t * 105);
return `rgb(${r}, 80, 80)`;
};
const qvColor = () => COLORS.qv;

const svgW = 560, svgH = 180;
const padL = 50, padR = 20, padT = 20, padB = 40;
const chartW = svgW - padL - padR;
const chartH = svgH - padT - padB;

const maxTV = Math.max(…(visible.map(r => r.tv)), 1);
const maxQV = Math.max(…(visible.map(r => r.qv)), 1);

function linePoints(data, maxVal) {
return data.map((d, i) => {
const x = padL + (i / (stepLevels.length - 1)) * chartW;
const y = padT + chartH - (d.value / maxVal) * chartH;
return `${x},${y}`;
}).join(” “);
}

const xLabels = stepLevels.map((s, i) => ({
x: padL + (i / (stepLevels.length - 1)) * chartW,
label: s >= 1000 ? “1k” : String(s),
}));

return (
<div style={{
minHeight: “100vh”,
background: COLORS.bg,
color: COLORS.text,
fontFamily: “‘Courier New’, monospace”,
padding: “32px 24px”,
}}>
{/* Header */}
<div style={{ textAlign: “center”, marginBottom: 36 }}>
<div style={{ fontSize: 11, color: COLORS.accent, letterSpacing: 4, marginBottom: 8, textTransform: “uppercase” }}>
Brownian Motion
</div>
<h1 style={{ fontSize: 26, fontWeight: “bold”, color: COLORS.text, margin: 0, lineHeight: 1.2 }}>
Why Total Variation Explodes<br />
<span style={{ color: COLORS.muted, fontSize: 18 }}>but Quadratic Variation Stays Finite</span>
</h1>
</div>

```
  {/* Main chart */}
  <div style={{
    background: COLORS.panel,
    border: `1px solid ${COLORS.border}`,
    borderRadius: 12,
    padding: 24,
    marginBottom: 24,
  }}>
    <div style={{ fontSize: 12, color: COLORS.muted, marginBottom: 16 }}>
      As the number of steps N increases (finer partition), watch what happens to each variation:
    </div>
    <svg width="100%" viewBox={`0 0 ${svgW} ${svgH}`} style={{ overflow: "visible" }}>
      {/* Grid lines */}
      {[0.25, 0.5, 0.75, 1].map(t => (
        <line key={t}
          x1={padL} y1={padT + chartH * (1 - t)}
          x2={padL + chartW} y2={padT + chartH * (1 - t)}
          stroke={COLORS.border} strokeDasharray="4,4" strokeWidth={1}
        />
      ))}
      {/* Axes */}
      <line x1={padL} y1={padT} x2={padL} y2={padT + chartH} stroke={COLORS.border} strokeWidth={1.5} />
      <line x1={padL} y1={padT + chartH} x2={padL + chartW} y2={padT + chartH} stroke={COLORS.border} strokeWidth={1.5} />

      {/* TV line */}
      {visible.length > 1 && (
        <polyline
          points={linePoints(visible.map(r => ({ value: r.tv })), maxTV)}
          fill="none" stroke={COLORS.tv} strokeWidth={2.5} strokeLinejoin="round"
        />
      )}
      {/* QV line (normalized to same scale for clarity) */}
      {visible.length > 1 && (
        <polyline
          points={linePoints(visible.map(r => ({ value: r.qv })), maxTV)}
          fill="none" stroke={COLORS.qv} strokeWidth={2.5} strokeLinejoin="round" strokeDasharray="6,3"
        />
      )}

      {/* Dots */}
      {visible.map((r, i) => {
        const x = padL + (i / (stepLevels.length - 1)) * chartW;
        const yTV = padT + chartH - (r.tv / maxTV) * chartH;
        const yQV = padT + chartH - (r.qv / maxTV) * chartH;
        return (
          <g key={i}>
            <circle cx={x} cy={yTV} r={4} fill={COLORS.tv} />
            <circle cx={x} cy={yQV} r={4} fill={COLORS.qv} />
          </g>
        );
      })}

      {/* X labels */}
      {xLabels.map((l, i) => (
        <text key={i} x={l.x} y={padT + chartH + 18} textAnchor="middle"
          fill={COLORS.muted} fontSize={10}>
          {l.label}
        </text>
      ))}
      <text x={padL + chartW / 2} y={svgH - 2} textAnchor="middle" fill={COLORS.muted} fontSize={10}>
        Number of steps N (partition refinement →)
      </text>

      {/* Legend */}
      <g transform={`translate(${padL + 10}, ${padT + 10})`}>
        <line x1={0} y1={6} x2={20} y2={6} stroke={COLORS.tv} strokeWidth={2.5} />
        <circle cx={10} cy={6} r={3} fill={COLORS.tv} />
        <text x={26} y={10} fill={COLORS.tv} fontSize={11}>Total Variation → ∞</text>
        <line x1={0} y1={26} x2={20} y2={26} stroke={COLORS.qv} strokeWidth={2.5} strokeDasharray="6,3" />
        <circle cx={10} cy={26} r={3} fill={COLORS.qv} />
        <text x={26} y={30} fill={COLORS.qv} fontSize={11}>Quadratic Variation → t (finite!)</text>
      </g>
    </svg>
  </div>

  {/* Bottom panels */}
  <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 16, marginBottom: 24 }}>
    <div style={{ background: COLORS.panel, border: `1px solid ${COLORS.border}`, borderRadius: 12, padding: 20 }}>
      <BarChart
        data={tvData}
        colorFn={tvColor}
        label="📈 Total Variation"
        subtitle="Σ |ΔX| → grows without bound"
      />
      {visible.length > 0 && (
        <div style={{ marginTop: 12, padding: "8px 12px", background: "#ff6b6b11", borderRadius: 6, border: `1px solid ${COLORS.tv}44` }}>
          <span style={{ color: COLORS.tv, fontSize: 11 }}>
            Each |ΔX| ~ √Δt, but N = t/Δt terms → sum ~ t/√Δt → <strong>∞</strong>
          </span>
        </div>
      )}
    </div>
    <div style={{ background: COLORS.panel, border: `1px solid ${COLORS.border}`, borderRadius: 12, padding: 20 }}>
      <BarChart
        data={qvData}
        colorFn={qvColor}
        label="✅ Quadratic Variation"
        subtitle="Σ |ΔX|² → converges to t"
      />
      {visible.length > 0 && (
        <div style={{ marginTop: 12, padding: "8px 12px", background: "#4ecdc411", borderRadius: 6, border: `1px solid ${COLORS.qv}44` }}>
          <span style={{ color: COLORS.qv, fontSize: 11 }}>
            Each |ΔX|² ~ Δt, and N·Δt = t → sum → <strong>t (constant!)</strong>
          </span>
        </div>
      )}
    </div>
  </div>

  {/* Sample paths */}
  <div style={{ background: COLORS.panel, border: `1px solid ${COLORS.border}`, borderRadius: 12, padding: 20, marginBottom: 24 }}>
    <div style={{ fontSize: 12, color: COLORS.muted, marginBottom: 16 }}>
      Sample Brownian paths at different resolutions — same trajectory, more jagged as N grows:
    </div>
    <div style={{ display: "flex", gap: 16, flexWrap: "wrap" }}>
      {[10, 50, 200, 1000].map(n => paths[n] && (
        <div key={n} style={{ flex: 1, minWidth: 160 }}>
          <div style={{ color: COLORS.muted, fontSize: 10, marginBottom: 4 }}>N = {n}</div>
          <MiniPath path={paths[n]} color={COLORS.path} />
        </div>
      ))}
    </div>
  </div>

  {/* Key insight box */}
  <div style={{
    background: `linear-gradient(135deg, #1a1a2e, #16213e)`,
    border: `1px solid ${COLORS.accent}44`,
    borderRadius: 12,
    padding: 20,
  }}>
    <div style={{ fontSize: 12, color: COLORS.accent, marginBottom: 8, letterSpacing: 2, textTransform: "uppercase" }}>Key Insight</div>
    <div style={{ fontSize: 13, color: COLORS.text, lineHeight: 1.7 }}>
      The exponent is everything. Each increment |ΔX| ~ (Δt)<sup>½</sup>. Summing N ~ (Δt)<sup>−1</sup> of them:<br />
      <span style={{ color: COLORS.tv }}>TV: (Δt)<sup>−1</sup> × (Δt)<sup>½</sup> = (Δt)<sup>−½</sup> → <strong>∞</strong></span>
      <span style={{ margin: "0 16px", color: COLORS.muted }}>|</span>
      <span style={{ color: COLORS.qv }}>QV: (Δt)<sup>−1</sup> × (Δt)<sup>1</sup> = (Δt)<sup>0</sup> = <strong>t ✓</strong></span>
    </div>
  </div>

  <div style={{ textAlign: "center", marginTop: 24 }}>
    <button onClick={runSimulation} style={{
      background: "transparent",
      border: `1px solid ${COLORS.accent}`,
      color: COLORS.accent,
      padding: "10px 28px",
      borderRadius: 6,
      cursor: "pointer",
      fontSize: 12,
      letterSpacing: 2,
      textTransform: "uppercase",
    }}>
      ↻ Resimulate
    </button>
  </div>
</div>
```

);
}