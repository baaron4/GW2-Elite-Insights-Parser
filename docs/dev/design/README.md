# Handoff: GW2 Elite Insights DPS Report — UI Redesign

## Overview

This is a full UI redesign for the Elite Insights GW2 arcdps log parser report. The current report has usability issues: overflowing phase tabs, a visually noisy player selection grid, disconnected boss header, and no clear visual hierarchy. This redesign addresses all of those with a new structural approach.

There are **two distinct views** covered:

1. **Stats Report** — the main data view (General Stats, Buffs, Damage Modifiers, Mechanics, Graph, Rotations, Targets Summary, Player Summary)
2. **Combat Replay** — the animated arena replay with playback controls and player frames

---

## About the Design Files

The files bundled in this package (`stats-report.html`, `combat-replay.html`) are **design reference prototypes built in HTML**. They are NOT production code and should not be shipped directly. They exist to communicate layout, visual style, interactions, and component structure.

Your task is to **recreate these designs within the existing Elite Insights codebase** (C# / Razor / TypeScript frontend, or whatever the current stack is), using its established patterns and component conventions. If no frontend framework is established, these designs lend themselves well to a **React + TypeScript** implementation.

---

## Fidelity

These are **medium-fidelity wireframes**. They convey:
- ✅ Layout structure and component hierarchy
- ✅ Color palette and spacing intentions
- ✅ All interactive states (collapsed/expanded, active tabs, selected phases)
- ✅ Section-specific header patterns
- ✅ Data table structure and column types

They do NOT convey:
- ❌ Final icon assets (class icons, skill icons, boon icons — use existing arcdps assets)
- ❌ Pixel-perfect spacing (treat measurements as approximate ±2–4px)
- ❌ Final copy or real player data

---

## Design Tokens

Use these values throughout. Define them as CSS custom properties or JS tokens.

### Colors

```css
--bg:        #272b30;   /* main content background */
--bg-side:   #1e2226;   /* sidebar, right panel */
--bg-panel:  #2c3138;   /* section headers, cards */
--bg-hover:  #30363d;   /* row hover, nav item hover */
--bg-deep:   #181c20;   /* deepest bg, inputs */
--border:    #363c44;   /* primary borders */
--border-l:  rgba(54,60,68,0.5); /* subtle row dividers */

--text:      #aaaaaa;   /* base text */
--text-dim:  #7a8490;   /* labels, secondary info */
--text-hi:   #d0d4d8;   /* headings, important values */

--accent:    #4d8fc4;   /* primary accent (blue) */
--accent-hi: #6aaad8;   /* hover/active accent */
--green:     #6db88a;   /* success, active players, positive states */
--warn:      #c07850;   /* warnings, HP bars */
--red:       #c05858;   /* enemies, danger states */
```

### Typography

```
Font family: 'Noto Sans', sans-serif  (Google Fonts)
Font weights used: 400, 500, 600, 700

Scale:
  9px  — micro labels, legend text, ticks
  10px — secondary labels, account names, tooltips
  11px — table cells, nav items, small UI text
  12px — section headers, standard UI
  13px — default body, sidebar nav
  18–22px — boss/fight name in General Stats header
```

### Spacing

```
Sidebar width (expanded): 230px
Right panel width: 270px (combat replay), 0 (stats — no right panel)
Section header padding: 12–16px vertical, 16–20px horizontal
Table cell padding: 10px vertical, 12px horizontal (spacious)
Nav item padding: 7px vertical, 14px horizontal
Border radius: 3–4px for buttons/chips; none for table rows
```

---

## View 1: Stats Report

**Reference file:** `stats-report.html`

### Layout (full viewport, no scroll)

```
┌─────────────────────────────────────────────────────────┐
│  Sidebar (230px)  │        Main Content (flex: 1)        │
│                   │  Section Header (varies by section)  │
│  Boss info        │  ─────────────────────────────────── │
│  ─────────────    │  Table / Graph / Rotation content    │
│  Filters          │                                      │
│   ◈ Phase         │                                      │
│   👥 Players      │                                      │
│   🎯 Targets      │                                      │
│  ─────────────    │                                      │
│  Section nav      │                                      │
└─────────────────────────────────────────────────────────┘
```

### Sidebar

**Boss info block** (top, always visible, non-collapsible):
- Boss name: 13px, weight 700, `--text-hi`
- Two badge pills: "Normal Mode" and "✓ Success" (success uses `--green` tint border)
- Subtitle line: "3m 42s · 10 players", 10px, `--text-dim`
- Border-bottom: 1px `--border`

**Filters section** — three collapsible blocks, each with:
- A single-line collapsed state (icon + summary text + chevron `›`)
- An expanded state revealing detailed controls
- Chevron rotates 90° when open
- Clicking the header row toggles open/closed

**Filter: Phase** (collapsed → expanded)
- Collapsed: `◈  Phase: Full Fight  ›`
- Expanded: vertical list of phase items
  - Each item: phase name + right-aligned percentage of fight duration
  - Active item: left border 2px `--accent`, text `--accent-hi`, subtle bg `rgba(77,143,196,0.07)`
  - Sub-phases (Burst 1, Thief, etc.) indented 10px extra, font-size 10.5px
  - Clicking a phase item selects it and closes nothing (stays open)

**Filter: Players** (collapsed → expanded)
- Collapsed: shows "10 players" + two rows of small avatar chips (18px circles), Group 1 and Group 2 separated by a 1px vertical divider
  - Avatar chips: show 2-letter class abbreviation (Nc, Gd, Ms, etc.)
  - Active avatars: border `rgba(109,184,138,0.5)`; inactive (excluded): opacity 0.35
- Expanded: full player list grouped by Group 1 / Group 2
  - Each row: avatar (20px) + player name + green dot if active
  - Clicking a player row toggles them in/out of the current view
  - Group labels: 9px uppercase, `--text-dim`

**Filter: Targets** (collapsed → expanded)
- Collapsed: `🎯  Target: All enemies  ›`
- Expanded: checkbox list — All enemies, Deimos, Shackled Prisoner, Saul D'Alessio, Gambler, etc.
  - Single-select (radio behavior): selecting one deselects others
  - Active: checkbox filled with `--accent`, text `--accent-hi`

**Section nav** (below filters):
- Sections: General Stats, Buffs, Damage Modifiers, Mechanics, Graph, Rotations, Targets Summary, Player Summary
- Each item: 12px, left border 2px transparent → `--accent` when active, bg `rgba(77,143,196,0.08)` when active
- Active item: weight 500, `--text-hi`
- 4px dot indicator beside each label

---

### Section Headers

Each section replaces its header area when navigated to. Headers sit between the sidebar and the table.

#### General Stats header

Full boss portrait + fight details. Only this section shows the portrait.

```
┌─────────────────────────────────────────────────────────────┐
│  [72×72 boss image]  Deimos                                  │
│                      Normal Mode · Duration 3m 42s · ✓ Success  Phase: Full Fight │
│                      ████████████░░░░  31,581,456 / 50,041,000 HP   [Statistics] [Healing] │
│                      ████████████████  Breakbar 5,004,000 max        │
├──────────────────────────────────────────────────────────────┤
│  Damage Stats │ Gameplay Stats │ Offensive Stats │ Defensive Stats │ Support Stats │
└──────────────────────────────────────────────────────────────┘
```

- Background: `--bg-panel`
- Boss name: 20px, weight 700, `--text-hi`
- HP bar: 4px tall, 280px wide, bg `--bg-deep`, fill `--accent` at 63% opacity 0.65
- Breakbar: 4px tall, 200px wide, fill `#8080c0` opacity 0.7
- Statistics/Healing toggle buttons: right-aligned, standard ctrl-btn style
- Sub-tabs row: background `--bg-side`, border-top 1px `--border`
  - Active tab: border-bottom 2px `--accent`, color `--text-hi`, weight 500
  - Inactive: `--text-dim`, no border

#### Buffs header

3 rows of controls. Background `--bg-panel`.

**Row 1** (mode + view + duration):
```
Buffs  |  [Uptimes●] [Volumes]  |  View: [Uptime●] [Gen Self] [Gen Group] [Gen Off-Group] [Gen Squad]  |  [Phase duration●] [Phase active dur.]
```

**Row 2** (buff types — multi-select pills):
```
Type:  [Boons] [Offensive Buffs●] [Support Buffs●] [Defensive Buffs] [Conditions] [Gear Buffs] [Debuffs] [Nourishments] [Enhancements] [Consumables] [Personal Buffs]
```

- Pill buttons: border-radius 10px
- Active pills (Row 1 exclusive groups): `--accent` bg, white text
- Active pills (Row 2 multi-select): `rgba(77,143,196,0.15)` bg, `--accent-hi` text, `rgba(77,143,196,0.4)` border

#### Other section headers

Each follows this pattern: `[Section Title]  |  [contextual filter/toggle buttons]`

- **Damage Modifiers**: All | DPS Modifiers | Offensive | Shared | Gear + Normalise: By hits | By damage
- **Mechanics**: All | Avoid-type | Boss mechanics | Debuffs + Sort: By count | By name
- **Graph**: DPS | Damage | Breakbar + Smooth: 1s | 5s avg | 10s avg | Total
- **Rotations**: Player dropdown + Skills | Boons | Conditions
- **Targets Summary**: All Enemies | [individual target buttons]
- **Player Summary**: All | Group 1 | Group 2 + Stats | Timeline

Button styles:
- Standard (exclusive): `ctrl-btn` — border 1px `--border`, transparent bg, `--text-dim`. Active: `--accent` bg, white text.
- Pill (exclusive or multi): border-radius 10px. Active exclusive: `--accent` bg. Active multi: `rgba(77,143,196,0.15)` bg.
- All headers: padding `7–10px 18px`, `flex-wrap: wrap`, gap 5–6px

---

### Data Table

Common structure for all tabular sections. Sticky `<thead>`.

```
thead th:
  background: --bg-side
  padding: 10px 12px
  font-size: 11px, weight 600, color --text-dim
  border-bottom: 1px --border
  cursor: pointer (sortable)
  hover: color --text
  sorted column: color --accent-hi

tbody td:
  padding: 10px 12px
  font-size: 13px
  border-bottom: 1px rgba(54,60,68,0.4)
  font-variant-numeric: tabular-nums

tr:hover td:
  background: --bg-hover

tr.top-row:
  background: rgba(77,143,196,0.04)
  name cell: weight 600, --text-hi

tr.group-row td:
  color: --text-dim, font-size 11px, italic
  background: rgba(20,24,28,0.3)
  border-top: 1px --border
```

**Bar visualization** (Target DPS column):
- Absolutely positioned div behind the cell value
- Width proportional to player's DPS relative to top player
- Background `--accent`, opacity 0.13
- Only in the primary sort column

**Expandable row** (top player only, default open):
- Appears immediately below the first data row
- Left border 2px `--accent`, indented 36px
- Contains: Avg hit, Crit%, Flanking%, Glancing%, Missed%, Scholar uptime
- Font 10.5px, labels `--text-dim`, values `--text` weight 600

**Damage Stats columns:**
Sub | (class badge) | Name | Account | Target DPS↕ | Target Dmg | Condi | Power | All DPS | All Dmg | CCs | Scholar%

**Buffs columns:**
Sub | (class badge) | Name | [boon columns, varies by selection]
- High values (>20%): `--text-hi`, weight 600
- Dashes for zero: `--text-dim`

---

## View 2: Combat Replay

**Reference file:** `combat-replay.html`

### Layout

```
┌────────────────────────────────────────────────────────────────┐
│  Meta bar: boss name, mode, result, duration                   │
├──────────────┬─────────────────────────────────┬──────────────┤
│ Left sidebar │  Phase bar                      │              │
│  (230px,     ├─────────────────────────────────┤ Right panel  │
│  collapsible)│  Canvas area (flex: 1)          │  (270px)     │
│              │  ┌─────────────────────────┐   │              │
│  Damage      │  │  [Speed overlay TL]     │   │  Targets     │
│  mini-table  │  │                         │   │  ──────────  │
│  ──────────  │  │    Arena circle         │   │  Display     │
│  Display     │  │    (boss+players)       │   │  options     │
│  accordion   │  │                         │   │  ──────────  │
│  ──────────  │  │  [Playback overlay BL]  │   │  Player      │
│  Skills      │  └─────────────────────────┘   │  frames      │
│  accordion   │  Timeline scrubber              │  (G1/G2/NPC) │
│  ──────────  │                                 │              │
│  Range/Cone  │                                 │              │
└──────────────┴─────────────────────────────────┴──────────────┘
```

### Left Sidebar (collapsible)

- Toggle button `‹` / `›` at top-right of sidebar collapses to 38px icon strip
- Collapsed state: vertical strip of icon buttons (📊 ⚙ ⚔ ⭕), each opens a tooltip or expands the sidebar
- Transition: `width 0.18s ease`

**Damage mini-table** (top of sidebar, always shown):
- Columns: Name (truncated), All dmg, Taken, Target
- View mode pills: PS | Cond | Cumulative (exclusive select)
- Row height tight: padding 3px 4px, font-size 10px
- Bar visualization behind "All" column

**Accordion sections** — each has a header row (icon + label + chevron) that toggles body:

1. **Display** (default open): checkboxes for Follow Selected, Highlight Group, Secondary NPCs, Mechanics, Markers, Skills, Hitbox Size, All Minions, Own Minions
2. **Skills** (default closed): checkboxes for Show On Select, Important Buffs, Proj. Mgmt, Heal, Cleanse, Strip, Portal, CC
3. **Range & Cone** (default closed): Circle 1 (radius input), Circle 2 (radius input), Cone (radius + opening angle inputs)

Checkbox style: 11×11px, border-radius 2px, border 1.5px `--border`. Checked: `--accent` bg, white checkmark.

---

### Phase Bar

Slim bar above the canvas. Phases act as **time-seek controls** — clicking jumps the scrubber.

```
[Full Fight●] [Pre Event] [Main Fight] [100%–10%] [Burst 1] [Thief] [Burst 2] [Drunkard] [Burst 3] [Gambler] [10%–0%]
```

- Background: `--bg-panel`, border-bottom 1px `--border`
- Active chip: `--accent` bg, white text, weight 500
- Sub-phase chips (Burst 1 etc.): slightly smaller (10px, padding 2px 8px)
- Clicking a chip → seeks scrubber to that phase's start timestamp

---

### Canvas Area

Background `--bg-side` (#1e2226). The arena is a circle centered within the canvas.

**Arena circle:**
- Responsive: `width: min(calc(100% - 20px), calc(100vh - 280px))`, `aspect-ratio: 1`
- Background: `radial-gradient(ellipse, #2a2f35 0%, #1a1e22 100%)`
- Border: 1.5px solid `#3a4248`
- Inner circle (boss platform): inset 22% on all sides, 50% opacity, subtle border

**Dots:**
- Player dots: 10×10px circles, border 1.5px `rgba(109,184,138,0.7)`, fill `rgba(109,184,138,0.25)`
- Enemy/boss dot: 14–18px, border 1.5px `rgba(192,88,88,0.7)`, fill `rgba(192,88,88,0.2)`, border-width 2px for main boss
- AoE mechanics: larger circles, `rgba(192,88,88,0.12)` fill, `rgba(192,88,88,0.25)` border, 1px dashed
- Range rings: dashed circles `rgba(100,130,160,0.3)`, controlled by Range & Cone settings

**Top-left overlay (speed + view):**
- Position: `absolute; top: 10px; left: 10px`
- Speed pills: Full | 4× | 10× | 20×
  - Style: `rgba(30,34,40,0.85)` bg, `rgba(54,60,68,0.8)` border, `backdrop-filter: blur(4px)`
  - Active: `rgba(77,143,196,0.3)` bg, `rgba(77,143,196,0.5)` border, `--accent-hi` text
- View pills (row below): PS | Condi | Cumulative — same style, slightly smaller

**Bottom overlay (playback controls + scrubber):**
- Full width of canvas, `position: absolute; bottom: 0`
- Gradient: `linear-gradient(to top, rgba(15,18,22,0.92) 0%, transparent 100%)`, padding 28px 14px 10px
- Buttons: Play/Pause, ⟳ Restart, ← Back, ⊡ Reset View
  - Style: `rgba(40,46,54,0.9)` bg, `rgba(70,80,92,0.8)` border, backdrop-filter
  - Play button: primary style with `rgba(77,143,196,0.35)` bg
- Timestamp: right-aligned, `--text-dim`, tabular-nums
- Scrubber: 3px track, `--accent` fill, 11px thumb circle
  - Phase markers: 2px vertical ticks at phase start positions
  - Clicking track seeks to that position
  - Time labels below: 0s, 30s, 1m, 1m 30s, 2m, 2m 30s

---

### Right Panel — Targets + Players (combined, no tabs)

**Target health cards** (top, always visible):
- One card per enemy target, background `--bg-panel`, border 1px `--border`, border-radius 4px
- Target name: 12px weight 600 `--text-hi` + colored dot (red for active boss, gray for inactive)
- HP bar: 5px tall, fill `--warn` opacity 0.75 for boss
- HP label: 10px `--text-dim` showing exact numbers
- Breakbar: 3px tall below HP bar, fill `#8080c0` opacity 0.7
- Inactive targets: opacity 0.6

**Display options strip** (below targets):
- Multi-select pill row: Boons | Conditions | Buffs | Rotation | HP bar
- Controls what appears on player frames below
- Active: `rgba(77,143,196,0.1)` bg, `--accent-hi` text, `rgba(77,143,196,0.4)` border

**Player frames** (scrollable list, grouped):
- Group headers: 9px uppercase, `--text-dim`, sticky within scroll

Each frame:
```
┌──────────────────────────────────────────────┐
│ [28px avatar]  Name            [Q][F][M][A][S][P] │
│                [████████░░] HP bar + % label      │
└──────────────────────────────────────────────┘
```
- Avatar: 28×28px, border-radius 3px, class abbreviation, `--bg-deep` bg
  - Active player: border `--green`
  - Inactive/dead: opacity reduced
- Player name: 11px weight 500 `--text`
- HP bar: 3px, color: green >80%, `--warn` 50–80%, `--red` <50%
- HP% label: 9px `--text-dim`
- Boon icons: 16×16px squares, border-radius 2px
  - Active boon: `rgba(109,184,138,0.1)` bg, `rgba(109,184,138,0.4)` border, `--green` text
  - Inactive: `--bg-panel` bg, `--border` border, `--text-dim` text
  - Show/hide controlled by display pills above
- Selected frame (clicked player): `rgba(77,143,196,0.08)` bg

---

## Interactions Summary

| Action | Result |
|--------|--------|
| Click phase (sidebar or phase bar) | Marks phase active; in stats view, filters data; in replay, seeks scrubber |
| Click player avatar (sidebar filter) | Toggles player in/out of current view |
| Click target (sidebar filter) | Single-selects that target |
| Click nav section | Loads section-specific header + table |
| Click sub-tab (General Stats) | Swaps table columns |
| Click ctrl-btn (exclusive) | Deactivates siblings in same row, activates self |
| Click ctrl-pill (multi) | Toggles individual pill |
| Click ‹ (left sidebar, combat) | Collapses sidebar to icon strip (width 38px) |
| Click accordion header | Toggles body open/closed, chevron rotates |
| Click scrubber track | Seeks to clicked position |
| Click Play | Toggles Play/Pause |
| Click phase chip (combat) | Seeks to phase start timestamp |
| Hover table row | `--bg-hover` background |
| Hover nav item | `--bg-hover`, `--text` color |

---

## Assets Needed

- **Class icons**: Use existing arcdps class icons (Necromancer, Guardian, Mesmer, Ranger, Warrior, Elementalist, Thief, Engineer, Revenant). Currently shown as 2-letter abbreviations (Nc, Gd, Ms…) in the prototypes.
- **Boon icons**: Quickness, Fury, Might, Alacrity, Swiftness, Protection — use existing boon icon assets
- **Skill icons**: Use existing skill icon assets
- **Boss portrait**: Boss thumbnail image shown in General Stats header
- **Buff/condition icons**: For Buffs section column headers

---

## Files in this Package

| File | Description |
|------|-------------|
| `stats-report.html` | Stats report view — full layout with all sections and interactive states |
| `combat-replay.html` | Combat replay view — canvas, overlays, sidebar, right panel |
| `README.md` | This document |

---

## Implementation Notes

- Both views share the same color tokens — define them once globally
- The sidebar filter system (Phase / Players / Targets) should be a reusable component — it appears in both views conceptually
- Phase selection is a **global filter context** that affects all sections; treat it as top-level app state
- The table component should support: sortable columns, bar visualization layer, expandable rows, group summary rows — make it generic enough to serve all 10+ sections
- The Combat Replay canvas uses the existing replay rendering engine; the layout just wraps it differently — the canvas itself is a black box
- Boon/class icon assets already exist in the arcdps ecosystem; do not recreate them
- Use `font-variant-numeric: tabular-nums` on all numeric cells so columns don't shift width
