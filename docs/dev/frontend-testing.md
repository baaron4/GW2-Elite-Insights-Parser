# Frontend Testing

Quick reference for regenerating an HTML report to verify UI changes. See [frontend-pipeline.md](frontend-pipeline.md) for architecture details.

---

## 1. Build

From the repo root:

```
dotnet build GW2EIParserCLI/GW2EIParserCLI.csproj -c Release
```

The CLI binary lands at `GW2EI.bin/Release/CLI/GuildWars2EliteInsights-CLI.dll`.

---

## 2. Pick a Log

arcdps writes logs to:

```
%USERPROFILE%\Documents\Guild Wars 2\addons\arcdps\arcdps.cbtlogs\<BossName>\
```

Pick any `.zevtc` file from a boss folder. A mid-sized file (~1–2 MB) parses in a few seconds and is enough to exercise all report sections.

---

## 3. Run

```
dotnet GW2EI.bin/Release/CLI/GuildWars2EliteInsights-CLI.dll <path-to-log.zevtc>
```

The HTML is written next to the source log file. Open it in a browser to inspect the result.

### With a config file

To override default settings (e.g. enable combat replay), create a `.conf` file:

```
# mytest.conf
ParseCombatReplay=True
```

Then pass it with `-c`:

```
dotnet GW2EI.bin/Release/CLI/GuildWars2EliteInsights-CLI.dll -c mytest.conf <path-to-log.zevtc>
```

A `parse.conf` at the repo root is gitignored and can serve as a persistent local override.

---

## 4. Commonly Toggled Settings

| Setting | Default | Notes |
|---------|---------|-------|
| `SaveOutHTML` | `True` | Main artifact |
| `ParseCombatReplay` | `False` | Adds the Combat Replay tab |
| `ParsePhases` | `True` | Phase breakdown in charts |
| `SaveOutJSON` | `False` | Raw JSON alongside the HTML |
| `SaveAtOut` | `True` | Save next to source log; set `False` + `OutLocation=<dir>` to redirect |
| `HtmlCompressJson` | `False` | Smaller file, slower first load |

See `GW2EIParserCommons/Settings/Settings.cs` for the full list with defaults.

---

## 5. Iterating on UI

1. Edit a file under `GW2EIBuilders/Resources/` (JS, CSS, or a template).
2. Rebuild.
3. Re-run the parser on the same log.
4. Hard-refresh the browser (`Ctrl+Shift+R`).

There is no watch mode or hot reload — the full rebuild + reparse cycle is required.
