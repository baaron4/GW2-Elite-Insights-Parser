# EI Report Frontend Pipeline

This document describes the complete pipeline that produces the HTML reports emitted by Elite Insights. It is intended for agents (or developers) who need to make UI changes without breaking the build or the data contract.

---

## 1. High-Level Architecture

The report is a **single self-contained HTML file**. All JavaScript, CSS, and data are either inlined or written to an adjacent asset folder. There is **no Node.js / npm build step** — the "bundling" is done entirely inside C# at report-generation time.

```
C# parser
  └─► JSON serialisation (System.Text.Json, source-generated)
        └─► HTMLAssets.cs  (template compilation, JS concatenation)
              └─► HTMLBuilder.cs  (placeholder substitution → final .html)
```

---

## 2. Key Files and Roles

| File | Role |
|------|------|
| `GW2EIBuilders/HTMLBuilder.cs` | Orchestrates generation; replaces all `${…}` placeholders in the root template |
| `GW2EIBuilders/HTMLAssets.cs` | Loads, minifies, and concatenates all JS templates into one blob; called once per build |
| `GW2EIBuilders/Resources/template.html` | Root HTML page skeleton (7.6 kB) |
| `GW2EIBuilders/Resources/ei.js` | Vue root-instance bootstrap (~363 lines); mounts `#content` |
| `GW2EIBuilders/Resources/JS/global.js` | Global reactive state shared across all components |
| `GW2EIBuilders/Resources/JS/mixins.js` | Vue mixins: number formatting, graph helpers, table headers |
| `GW2EIBuilders/Resources/JS/functions.js` | Utility functions: icon URLs, skill lookup, graph data prep |
| `GW2EIBuilders/Resources/ei.css` | Custom styles (layout, theming, scrollbars) |
| `GW2EIBuilders/Resources/htmlTemplates/**` | ~95 Vue component templates (statistics views) |
| `GW2EIBuilders/Resources/combatReplayTemplates/**` | ~15 Vue component templates (combat replay UI) |
| `GW2EIBuilders/Resources/htmlHealingExtensionTemplates/**` | ~17 Vue component templates (healing extension) |
| `GW2EIBuilders/HtmlModels/` | C# DTOs that are serialised to JSON and injected into the report |
| `GW2EIBuilders/GW2EIBuilders.csproj` | Marks every `*.js / *.css / *.html` under `Resources/` as an embedded resource |

---

## 3. Frontend Technology Stack

| Technology | Version | Purpose |
|------------|---------|---------|
| **Vue.js** | 2.7.14 | Component framework (no build step — loaded from embedded CDN copy) |
| **Bootstrap** | 4.1.1 | CSS layout grid and components (Bootswatch themes: *slate* dark / *yeti* light) |
| **Plotly.js** | 3.0.0 | DPS, healing, and damage charts |
| **jQuery** | 3.7.0 | DOM helpers, event binding |
| **Popper.js** | 1.16.1 | Tooltip positioning |
| **Pako** | 1.0.10 | Optional client-side gzip decompression of JSON payload |

All libraries are inlined into the output HTML (or served from the adjacent asset folder); there is no reliance on external CDNs at viewing time.

---

## 4. Template System

### 4.1 Template File Format

Every file under `htmlTemplates/`, `combatReplayTemplates/`, and `htmlHealingExtensionTemplates/` follows this two-section format:

```html
<template>
  <!-- HTML markup with Vue directives (v-for, v-if, :bind, @event, …) -->
</template>

<script>
  // Vue component options object (no `export default` — it is inlined directly)
  {
    props: [...],
    computed: { ... },
    methods: { ... }
  }
</script>
```

`HTMLAssets.cs` extracts both sections, strips all tab/newline whitespace from the HTML, and builds a JavaScript object:

```js
{ template: "<minified HTML>", /* rest of script content */ }
```

All compiled components are then concatenated into a single JS blob and injected at the `TEMPLATE_COMPILE` / `TEMPLATE_CR_COMPILE` / `TEMPLATE_HEALING_EXT_COMPILE` placeholder positions inside `ei.js`.

### 4.2 Script Load Order

`HTMLAssets.cs` concatenates scripts in a fixed order before the compiled templates:

1. `JS/global.js` — reactive state
2. `JS/mixins.js` — shared mixins
3. `JS/functions.js` — utility helpers
4. *(compiled template block)*
5. `ei.js` — Vue root instance mount

### 4.3 Adding a New Component

1. Create `GW2EIBuilders/Resources/htmlTemplates/tmplMyComponent.html` using the `<template>` / `<script>` format.
2. Register it inside `ei.js` (or the parent component) as a local or global Vue component.
3. The `.csproj` glob `Resources\**\*.html` picks it up automatically — no manual resource registration needed.

---

## 5. JSON Data Injection

The C# parser produces several data blobs that are injected as JavaScript variables in `template.html`:

| JS variable | Placeholder | C# source |
|-------------|-------------|-----------|
| `_logData` | `'${logDataJson}'` | `LogDataDto` → `System.Text.Json` |
| `_graphData` | `'${graphDataJson}'` | `ChartDataDto` |
| `_crData` | `'${CRDataJson}'` | `CombatReplayDto` |
| `_healingStatsExtension` | `'${healingDataJson}'` | `HealingStatsExtension` |
| `_barrierStatsExtension` | `'${barrierDataJson}'` | `BarrierStatsExtension` |

At page load, `ei.js` checks for `window.pako`; if present it decompresses the Base64+gzip payload, otherwise it uses the raw JSON string. The decompressed object is stored in `window.reactiveLogdata` and made reactive via `Vue.observable`.

**Data contract rule:** The shape of the DTO classes in `GW2EIBuilders/HtmlModels/` is the contract between C# and JavaScript. Renaming or removing a DTO field will silently break any template that references it.

---

## 6. Theming

Two Bootstrap/Bootswatch themes are supported: **slate** (dark, default) and **yeti** (light). The user's choice is persisted in `localStorage`. The active theme name is also baked into the HTML at generation time via the `${bootstrapTheme}` placeholder so the initial render matches the user's last preference without a flash.

To change global visual style:
- **Layout / component styles** → edit `GW2EIBuilders/Resources/ei.css`
- **Bootstrap overrides** → add CSS custom properties or additional rules in `ei.css`
- **Charts** → edit Plotly layout options in `JS/functions.js` (look for `getDefaultGraph*` helpers)

---

## 7. Combat Replay

Combat replay is an HTML5 Canvas animation system separate from the statistics view. Key files:

| File | Role |
|------|------|
| `Resources/JS/CR-JS/animator.js` | Main animation loop, canvas rendering |
| `Resources/JS/CR-JS/actors.js` | Actor (player/enemy) sprite management |
| `Resources/JS/CR-JS/decorations.js` | Visual effects: AoE circles, lines, icons |
| `combatReplayTemplates/tmplCombatReplayUI.html` | Root UI panel component |
| `combatReplayTemplates/tmplCombatReplayAnimationControl.html` | Playback controls |

Combat replay data is injected as `_crData` and is only included in the HTML when the parser option is enabled.

---

## 8. Build / Embedding Pipeline (MSBuild)

1. `GW2EIBuilders.csproj` includes all files under `Resources/` with `<EmbeddedResource>`.
2. MSBuild auto-generates `Properties/Resources.resx` and `Properties/Resources.Designer.cs`.
3. At report generation time, `HTMLAssets` reads resources via `Properties.Resources.*` typed accessors, performs in-memory template compilation, and hands the assembled JS string to `HTMLBuilder`.
4. `HTMLBuilder` does string-replacement on the root template and writes the final `.html` file.

There is **no watch mode, hot reload, or dev server**. To iterate on UI changes:
1. Edit the relevant `.html` / `.js` / `.css` file under `Resources/`.
2. Rebuild the C# project.
3. Re-run the parser on an `.evtc` or `.zevtc` log to regenerate the report.
4. Open the report in a browser.

---

## 9. Vue Component Patterns in Use

- **Global components** registered in `ei.js` via `Vue.component(name, def)`.
- **Mixins** (`numberComponent`, `graphComponent`, `damageGraphComponent`, `playerHeaderComponent`, `encounterPhaseComponent`) provide shared computed properties and methods — import via `mixins: [...]` in the component script section.
- **Props** are the primary data input; avoid reading `window.reactiveLogdata` directly inside templates — receive it through the prop chain from the root instance.
- **No Vuex / Vue Router** — state is managed through the global reactive object and prop drilling.

---

## 10. What to Avoid

- **Do not add a Node.js build step** without first updating `HTMLAssets.cs` to call it; the C# project has no npm lifecycle hooks.
- **Do not upgrade to Vue 3** without rewriting every component — the template syntax and options API have breaking changes.
- **Do not rename DTO properties** without updating every JavaScript reference to that property across all template files.
- **Do not remove the `TEMPLATE_COMPILE` placeholder comment** from `ei.js` — `HTMLAssets.cs` searches for it by string to inject the compiled component block.
- **Avoid large inline images** in CSS/JS — all assets are inlined into the HTML, so binary blobs inflate the file size significantly.
