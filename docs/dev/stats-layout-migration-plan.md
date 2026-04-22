# Stats Report — Layout Migration Plan

Reference design: `docs/dev/design/stats-report.html`  
Current entrypoint: `GW2EIBuilders/Resources/template.html` + `htmlTemplates/tmplMainView.html`

---

## What Changes vs. What Stays

### Stays untouched
- All data logic (computed properties, activePhase, activePlayer, activePhaseTargets, tab index routing)
- Every child component: `stat-tables-component`, `buff-tab-component`, `dmgmodifier-stats-container-component`, `mechanics-stats-component`, `graph-stats-component`, `player-rotations-tab-component`, `target-stats-component`, `player-stats-component`
- The `<keep-alive>` wrapper around those children
- The `encounter-component` (boss info — it moves, it doesn't change)
- All data props passed into child components
- `phase-component`, `player-component`, `target-component` — logic stays, only their **location in the DOM** changes (sidebar instead of top bar)

### Changes structurally
- `template.html` — body layout switches from a centered column to a full-viewport flex row (sidebar + main)
- `tmplMainView.html` — the section nav tabs move from a top `<ul>` to a sidebar `<nav>`, the phase/player/target selectors move from a top bar into collapsible sidebar filter blocks
- `ei.css` — full replacement of layout rules; theme variables added; Bootstrap layout classes mostly removed from the shell
- Font — add Noto Sans (Google Fonts); keep `#272B30` as main bg and existing `--text` color (`#aaaaaa`)

---

## Library Compatibility Check

| Need | Status |
|------|--------|
| CSS custom properties (`--var`) | ✅ All modern browsers; no Bootstrap dependency |
| `display: flex` full-viewport layout | ✅ Already used throughout |
| Sticky `<thead>` | ✅ Already used; just needs `overflow: auto` on the right container |
| Collapsible accordion (sidebar filters) | ✅ Pure Vue `v-show` + `data` flag — no Bootstrap collapse needed |
| Bootstrap 4 removal from shell | ✅ Safe — Bootstrap classes are only needed inside child table components (`.table`, `.card`, `.badge` etc.) — those are untouched |
| Vue 2 `<keep-alive>` | ✅ Unchanged |
| Noto Sans font | ✅ Add one `<link>` to `template.html` (already has Open Sans there) |
| No new npm/build step | ✅ All changes are in `.html` / `.css` resource files |

Bootstrap 4 does **not** need to be removed. The shell layout stops using Bootstrap grid classes, but child components keep using `.table`, `.btn`, `.card`, etc. internally.

---

## Color Tokens to Add

Add these as CSS custom properties on `:root` (or on `body.theme-slate` / `body.theme-yeti`) at the top of `ei.css`. Keep the existing `#272B30` background and existing text color.

```css
/* Dark theme (replaces/extends slate) */
body.theme-slate {
  --bg:        #272b30;   /* KEEP — existing slate bg */
  --bg-side:   #1e2226;
  --bg-panel:  #2c3138;
  --bg-hover:  #30363d;
  --bg-input:  #1a1e22;
  --border:    #363c44;
  --border-l:  rgba(54,60,68,0.5);
  --text:      #aaaaaa;   /* KEEP — existing slate text */
  --text-dim:  #7a8490;
  --text-hi:   #d0d4d8;
  --accent:    #4d8fc4;
  --accent-hi: #6aaad8;
  --green:     #6db88a;
  --warn:      #c07850;
  --red:       #c05858;
}

/* Light theme — yeti stays as is; sidebar gets lighter equivalents */
body.theme-yeti {
  --bg:        #f8f9fa;
  --bg-side:   #e9ecef;
  --bg-panel:  #fff;
  --bg-hover:  #e2e6ea;
  --bg-input:  #dee2e6;
  --border:    #ced4da;
  --border-l:  rgba(206,212,218,0.5);
  --text:      #495057;
  --text-dim:  #868e96;
  --text-hi:   #212529;
  --accent:    #007bff;
  --accent-hi: #0069d9;
  --green:     #28a745;
  --warn:      #fd7e14;
  --red:       #dc3545;
}
```

The font change (`body { font-family: 'Noto Sans', ... }`) replaces only the system-font stack in `ei.css:243`. The existing `Open Sans` link in `template.html` can be swapped for Noto Sans there.

---

## File-by-File Work

### 1. `template.html`

**Remove:** the top-level `.d-flex.justify-content-center` header block (lines 73–103 in current file) that contains `encounter-component` and the theme/mode pills. These move into the sidebar.

**Change `<body>` structure** from:
```
loading div
#content (centered column)
  header row (encounter + mode pills)
  <main-view-component>
  footer
```

To:
```
loading div
#content (full-viewport flex row)
  <ei-sidebar-component>        ← new wrapper component
  <div class="ei-main">
    <main-view-component v-if="mode === 0">
    <combat-replay-ui-component v-if="mode === 1">
    <healing-extension-view-component v-if="mode === 2">
  </div>
```

The footer can stay at the bottom of `#content` as a full-width `flex-shrink: 0` bar, or be collapsed into the sidebar bottom. Keep it for now.

**CSS class on `#content`:** replace `:class="{'ei-container-small':…, 'ei-container-big':…}"` with a static `class="ei-shell"` — the new layout is always full-viewport, no max-width centering.

### 2. New file: `htmlTemplates/tmplSidebar.html`

This is the single biggest new component. It consolidates what was previously three separate top-bar strips into one sidebar Vue component.

```
Vue component: "ei-sidebar-component"
Props: light (bool), cr (bool), healingExtShow (bool), mode (Number)

Template structure:
  .ei-sidebar
    .ei-sidebar-boss          ← encounter-component goes here
    .ei-filters               ← collapsible blocks
      Phase filter block      ← phase-component wrapped in collapse
      Players filter block    ← player-component wrapped in collapse
      Targets filter block    ← target-component wrapped in collapse
    .ei-section-label         "Section"
    .ei-nav-list              ← the 8 nav items (was nav-tabs in tmplMainView)
    .ei-sidebar-bottom        ← mode switcher (Statistics / Replay / Healing) + theme toggle
```

**Collapsible filter blocks** — each block has:
- `data: { phaseOpen: false, playersOpen: false, targetsOpen: false }`
- A header row `@click="phaseOpen = !phaseOpen"` with a chevron that rotates with `:style`
- `v-show="phaseOpen"` on the body

The existing `phase-component`, `player-component`, and `target-component` render unchanged inside those bodies — only their container changes.

**Section nav** — move the `tab` data property from `main-view-component` up to this sidebar (or keep it in `main-view-component` and emit events up). The simplest approach that avoids prop-drilling: keep `tab` in `main-view-component` and expose a `currentTab` prop to the sidebar so it can highlight the active item. Clicks in the sidebar emit a `'select-tab'` event that the parent (`template.html` root) passes down as a prop or through `reactiveLogdata`.

Alternatively: lift `tab` into a property on `reactiveLogdata` (the global reactive object in `global.js`) — then both sidebar and main-view read from the same place without prop wiring.

### 3. `tmplMainView.html`

**Remove entirely:**
- The `<div id="phase-nav">` block (lines 3–5)
- The `<div id="actors">` block (lines 6–14)
- The `<ul class="nav nav-tabs">` block (lines 15–40)

**Keep:**
- The `<keep-alive>` block and all child component `v-if` slots (lines 42–65)
- All `computed` properties in the script section
- The `tab` data property (or lift it to global state — see above)

The resulting template is just the `<keep-alive>` block wrapped in a `<div class="ei-main-content">`.

### 4. `ei.css`

**Remove (or scope to legacy):**
- `.ei-container-small`, `.ei-container-big` (replaced by `.ei-shell`)
- `.footer` centering (keep the rule, change positioning)
- `body { font-family: -apple-system … }` (replace with Noto Sans)

**Add (new layout rules):**
```css
/* Shell */
html, body { height: 100%; overflow: hidden; }
.ei-shell {
  display: flex;
  flex-direction: row;
  height: 100vh;
  overflow: hidden;
}

/* Sidebar */
.ei-sidebar {
  width: 230px;
  flex-shrink: 0;
  background: var(--bg-side);
  border-right: 1px solid var(--border);
  display: flex;
  flex-direction: column;
  overflow: hidden;
}
.ei-nav-list { flex: 1; overflow-y: auto; }

/* Main area */
.ei-main {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden;
  background: var(--bg);
}

/* The content area each child component fills */
.ei-main-content {
  flex: 1;
  overflow: auto;
}

/* Section headers sit above the scrollable content */
.ei-section-header {
  flex-shrink: 0;
  background: var(--bg-panel);
  border-bottom: 1px solid var(--border);
}
```

**Leave untouched** (still needed by child components):
- All `.table`, `.btn`, `.icon`, `.buff-*`, `.rot-*`, `.cr-*` rules
- Plotly hover text theme overrides
- Scrollbar styles

---

## Section Headers (per-section header bars)

In the current code, each child component (`stat-tables-component`, `buff-tab-component`, etc.) renders its own controls internally. In the new design, each section has a distinct header bar above the table.

**Recommended approach for this migration:** move each component's top control row into a `<div class="ei-section-header">` at the top of that component's template, so it sits as `flex-shrink: 0` above the scrollable content. This keeps the work inside each individual component file, avoids creating a new central header-switcher, and preserves the `<keep-alive>` boundary.

Each component template structure becomes:
```html
<template>
  <div class="d-flex flex-column" style="height:100%">
    <div class="ei-section-header">
      <!-- controls row(s) for this section -->
    </div>
    <div class="ei-main-content scrollable-y">
      <!-- existing table / graph content -->
    </div>
  </div>
</template>
```

This means **each section component needs a small structural edit** — wrap content in the two-div pattern. The controls that are currently rendered above the table inside the component stay in the component, just move to the header div. Do these one component at a time.

---

## Implementation Order

Do these in sequence so the layout works at each step before moving to the next:

1. **CSS tokens** — add the CSS custom properties block to `ei.css` and swap the font. Nothing breaks yet; tokens just sit unused.

2. **Shell layout** — edit `template.html` and `ei.css` to make `#content` a full-viewport flex row with `.ei-sidebar` + `.ei-main` placeholders. Temporarily put `encounter-component` and the mode pills directly in `.ei-sidebar` with no Vue component wrapper, just to verify the layout works.

3. **Sidebar component** — create `tmplSidebar.html`. Wire up the collapsible phase/player/target filter blocks using `v-show`. Move the section nav (tab switching) here.

4. **Strip `tmplMainView.html`** — remove the top bars, leave only `<keep-alive>`. Confirm all child components still render.

5. **Section headers** — go through each of the 8 section components and apply the `ei-section-header` / `ei-main-content` wrapper pattern. Start with `stat-tables-component` (General Stats) since it's the most used.

6. **Polish** — apply design token classes to sidebar elements (nav-item active states, filter chevrons, boss badge pills, etc.).

---

## What Is Explicitly Out of Scope for This Migration

- Changing any table column layout, data, or sorting logic inside child components
- The Combat Replay view (`tmplCombatReplayUI.html`) — separate design file, separate migration
- The Healing Extension view
- Bar visualizations behind DPS cells (design doc feature, separate follow-up)
- Expandable first-row detail row (design doc feature, separate follow-up)
- The gradient background picker from the design prototype (aesthetic extra, skip for now)
