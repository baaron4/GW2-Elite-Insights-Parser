# GW2 Elite Insights Parser

[![Github](https://img.shields.io/static/v1?style=for-the-badge&message=GitHub%20Page&color=%23131519&logo=GitHub&logoColor=FFFFFF&label=)](https://github.com/baaron4/GW2-Elite-Insights-Parser)
[![JSON DOCS](https://img.shields.io/static/v1?style=for-the-badge&message=JSON%20DOCS&color=%23131519&logo=JSON&logoColor=FFFFFF&label=)](https://baaron4.github.io/GW2-Elite-Insights-Parser/Json/index.html)

[![Release](https://img.shields.io/github/v/release/baaron4/GW2-Elite-Insights-Parser?style=for-the-badge&labelColor=%23131519&color=%23ffce26)](https://github.com/baaron4/GW2-Elite-Insights-Parser/releases/latest)
[![GitHub issues](https://img.shields.io/github/issues/baaron4/GW2-Elite-Insights-Parser?style=for-the-badge&labelColor=%23131519&color=ffce26)](https://github.com/baaron4/GW2-Elite-Insights-Parser/issues)
[![GitHub Downloads (all assets, all releases)](https://img.shields.io/github/downloads/baaron4/GW2-Elite-Insights-Parser/total?style=for-the-badge&labelColor=%23131519&color=%23ffce26)](https://github.com/baaron4/GW2-Elite-Insights-Parser/releases/latest)

[![GitHub contributors](https://img.shields.io/github/contributors/baaron4/GW2-Elite-Insights-Parser?style=for-the-badge&labelColor=%23131519&color=%23ffce26)](https://github.com/baaron4/GW2-Elite-Insights-Parser/graphs/contributors)
[![GitHub forks](https://img.shields.io/github/forks/baaron4/GW2-Elite-Insights-Parser?style=for-the-badge&labelColor=%23131519&color=ffce26)](https://github.com/baaron4/GW2-Elite-Insights-Parser/forks)
[![GitHub watchers](https://img.shields.io/github/watchers/baaron4/GW2-Elite-Insights-Parser?style=for-the-badge&labelColor=%23131519&color=ffce26)](https://github.com/baaron4/GW2-Elite-Insights-Parser/watchers)
[![GitHub Repo stars](https://img.shields.io/github/stars/baaron4/GW2-Elite-Insights-Parser?style=for-the-badge&labelColor=%23131519&color=ffce26)](https://github.com/baaron4/GW2-Elite-Insights-Parser/stargazers)

## Contact
For general ArcDPS-related discussions and troubleshooting, including ArcDPS, ArcDPS addons, or EVTC log parsing, please join our Discord server.

[![](https://discordapp.com/api/guilds/456611641526845473/widget.png?style=banner2)](https://discord.gg/T4kSbKJ5Sf)

The key for verifying the release's signature can be found on [openpgp](https://keys.openpgp.org/search?q=CC2A0529D3469F39A6155C73FA6E8DECE596BCE9).

## Logging

We suggest following [this guide](https://snowcrows.com/guides/arcdps/arcdps) written by Snow Crows on how to setup your ArcDPS installation and generate encounter logs.

## Set Up

1. Download the GW2EI.zip file from the [latest release](https://github.com/baaron4/GW2-Elite-Insights-Parser/releases/latest). Don't forget to check regularly to stay updated (we have a channel that notifies new releases on our discord).

2. Extract all files anywhere you like.

3. Launch GuildWars2EliteInsights.exe (ui, windows only) or GuildWars2EliteInsights-CLI.exe (console).

NOTE: ArcDPS EVTC log files are located by default at "C:\Users\\\<USERNAME>\\Documents\Guild Wars 2\addons\arcdps\arcdps.cbtlogs".

## UI Usage
![program](./docs/Images/EILook.PNG)

1. Drag and drop one or multiple .evtc, .evtc.zip, or .zevtc files onto the program.

2. Click parse.

3. Click open when parsing done.

You can change the settings at any time using the Settings window.

## Console Usage

![how to](https://user-images.githubusercontent.com/30677999/40148954-6ec9215a-5936-11e8-94ad-d2520e7c4539.PNG)

Settings can be configured using .conf files (see Settings/sample.conf for an example). You can then use it with -c.
For console:

```
GuildWars2EliteInsights-CLI.exe -c [config path] [logs]
```
For UI:

```
GuildWars2EliteInsight.exe -c [config path] [logs]
```

Note it may take some time for each file to parse and they will not be ready to open the moment they are created.

## Settings
![settings](./docs/Images/EISettings.PNG)
### Output Settings

-__SaveAtOut__: if true, the generated files will be in the same location as the source file.

-__OutLocation__: secondary output path, will be used if SaveAtOut is false.

-__SaveOutTrace__: if true, log files will be generated.

-__Anonymous__: if true, player character and account names will be obfuscated.

-__AddPoVProf__: if true, the profession of the pov will be added to the generated files' name.

-__AddDuration__: if true, the duration (truncated to seconds) will be added to the generated files' name.

### Parser Settings

-__SingleThreaded__: if true, only a single thread will be used for parsing a single log.

-__ParseMultipleLogs__: if true, multiple logs will be parsed in parallel.

-__SkipFailedTries__: if true, failed encounters will not be parsed.

-__CustomTooShort__: Customize encounter duration in ms below which logs will not be parsed.

### GUI only Parser Settings

-__AutoAdd__: if true, EI will automatically add logs that appear in AutoAddPath.

-__AutoAddPath__: the path to listen to for automatic additions.

-__AutoParse__: if true, every added log file will be automatically processed.

### Encounter Settings

-__ParsePhases__: if true, phases will be parsed.

-__ParseCombatReplay__: if true, combat replay will be computed.

-__ComputeDamageModifiers__: if true, damage modifiers will be computed.

-__DetailledWvW__: if true, enemy players will not be merged into one in WvW logs and they'll appear as standard targets. Warning: the generated files and the generation time will grow exponentially, use it only on organized sorties (Guild zergs, GvG, ...).

### HTML settings

-__SaveOutHTML__: if true, html logs will be generated.

-__HtmlExternalScripts__: if true, css and js files will be separated from the html.

-__HtmlExternalScriptsPath__: Available if HtmlExternalScripts is enabled. Fill in an absolute path to place the external script files at a different location than the report file. If you use this tool on a server and wish to populate reports directly remember to set valid path in __HtmlExternalScriptsCdn__. Otherwise users might not be able to open the external scripts within your report.

-__HtmlExternalScriptsCdn__: Available if HtmlExternalScripts is enabled. Will use an url for the external script files. Generate once, use multiple times. Will reduce needed webspace a bit. If this option is set the settings of __HtmlExternalScriptsPath__ will not be used to include external sources. Think about CORS if you use a separate server for static source files.

-__LightTheme__: if true, the html will use a light theme by default. Please note that the theme can be dynamically changed on the html post generation.

-__HtmlCompressJson__: if true, the input json of the html will be compressed (roughly %60 gain in size).

### CSV Settings

-__SaveOutCSV__: if true, csv logs will be generated.

### Raw Format Settings

-__SaveOutJSON__: if true, json logs will be generated.

-__IndentJSON__: if true, generated json logs will be indented instead of being on a single line.

-__SaveOutXML__: if true, xml logs will be generated.

-__IndentXML__: if true, generated xml logs will be indented instead of being on a single line.

-__CompressRaw__: if true, xml and json logs will be compressed.

-__RawTimelineArrays__: if true, xml and json logs will contain graph related data.

### Upload Settings

-__UploadToDPSReports__: if true, the log will be uploaded to dps.reports using EI as generator.

-__DPSReportUserToken__: dps.report user token.

-__UploadToWingman__: if true, the log will be uploaded to Wingman via the "uploadProcessed" endpoint.

-__WebhookURL__: Webhook URL to send an embed or simple message to.

-__SendEmbedToWebhook__: if true, the Webhook URL will receive a small embed containing meta data + dps.reports link.

-__SendSimpleMessageToWebhook__: if true, only the dps.reports link will be sent to the webhook.

### General Settings

-__MemoryLimit__: In MB. If the application uses more RAM than provided number, the application will exit with code 2. 0 to disable the feature. When enabled, the maximum between given number and 100 MB will be used.

## HTML Overview

For a more detailed look, please check [this guide](https://snowcrows.com/guides/arcdps/reading-logs) written by Snow Crows.

### Header

<img src="./docs/Images/header.PNG" width="60%" height="60%">

The header shows you the status of the fight and lets you swap themes and modules. There are three modules available: Statistics, Combat Replay and Healing Statistics.

### Footer

<img src="./docs/Images/footer.PNG" width="60%" height="60%">

On the footer you'll find meta data regarding the encounter and the parser.

### Statistics
#### Navigation

<img src="./docs/Images/selection.PNG" width="60%" height="60%" >

This panel is where the main navigation of the Statistics module will happen, you can select targets, players, phases and components. 

The target selection will impact what you'll observe on every panel that has a "Target" section.

On players, you can observe gear related scores (between 0 and 10, please check "question mark" for a detailed explanation on how this value is computed), used weapons and the commander tag (if applicable).

#### General Stats
<img src="./docs/Images/general.PNG" width="60%" height="60%">

On general stats you can see macro statistics regarding incoming/outgoing damage and player behavior:
- "Damage Stats" contains outgoing damage related information.
- "Gameplay Stats" contains generic information like time spent casting or not casting skills, average distance to the commander player, etc...
- "Offensive Stats" contains secondary player information like critical hit rates, flaking rates, number of time one's attack was blocked/absorbed, etc...
- "Defensive Stats" contains incoming damage related information.
- "Support Stats" contains boon strips, conditions removal, resurrection and related information.

#### Buffs

<img src="./docs/Images/buff.PNG" width="60%" height="60%">

This component will show you buff uptimes, ordered by categories, and generation information for each player.

On generation tables, please check the "question mark" above for a detailed explanation of the meaning of the tooltips.

#### Damage Modifiers

<img src="./docs/Images/damageMods.PNG" width="60%" height="60%">

This component contains damage modifiers, ordered by categories.

The modifiers are categorised by outgoing and incoming, then further split into gear based, shared and class based modifiers.

Damage modifiers with a positive value indicate that the player dealt or taken increased damage, negative value means the opposite.

Please note that it is not possible to check traits or gear which means that Elite Insights will assume that every gear and trait based damage modifiers are present. Buff based damage modifiers are only shown if present.

#### Mechanics

<img src="./docs/Images/mechanics.PNG" width="60%" height="60%">

A very straightforward component that contains a summary of important fight specific mechanics.

Depending on the nature of the mechanic, the column can be considered just as informative, a success or a failure.

#### Graph

<img src="./docs/Images/graph.PNG" width="60%" height="60%">

Damage graph that also contains enemy health, enemy breakbar and fight mechanics information. The graph is fully interactive and can be exported. Shown damage can also be customized:
- The time interval in between the information is computed
- The nature of the information:
   - DPS in [x - interval, x].
   - DPS in [x - interval / 2, x + interval / 2].
   - Cumulative damage in [x - interval, x].

#### Targets Summary

<img src="./docs/Images/targets.PNG" width="60%" height="60%">

This component focuses on the selected target:
- Outgoing damage distribution per skill for the target and its minions.
- Incoming damage distribution per skill.
- Graph that contains outgoing damage, health, breakbar, rotation and buff presences. The graph is fully interactive and can be exported. Damage related customizations on the main graph are also applicable here.
- Buff status contains condition and boon uptimes on the boss. For conditions, it is also possible to see generation done by each player.

#### Player Summary

<img src="./docs/Images/players.PNG" width="60%" height="60%">

This component focuses on the selected target:
- Outgoing damage distribution per skill for the player and their minions.
- Incoming damage distribution per skill.
- Customizable simple rotation component for a tidier look on skill ordering.
- Graph that contains outgoing damage, health, rotation and buff presences. Information related to targets' health and breakbar can also be displayed. The graph is fully interactive and can be exported. Damage related customizations on the main graph are also applicable here.
- Information on the consumables used by the player.
- A succession of small graphs that details incoming damage before each death.

### Combat Replay

#### Main Display

<img src="./docs/Images/mainCR.PNG" width="60%" height="60%">

The main display is where the animation happens.

It is possible to control the animation speed and jump to specific fight phases.

The display supports two manipulations: Pan and Zoom.

#### Damage Table

<img src="./docs/Images/damageCR.PNG" width="30%" height="30%">

Displays damage/DPS in real time. The picture says it all.

#### Selection

<img src="./docs/Images/selectionCR.PNG" width="45%" height="45%">

Allows to display or remove specific information from the Combat Replay such as:
- Group highlights
- Secondary NPCs
- Encounter Mechanics
- Player Skills
- Use in-game hitbox sizes
- Show all minions
- Show selected's minions

Player skills displayed are categorised in the following groups:
- Show on select: displays only when the player is selected
- Important buffs: skill that applies an important buff such as Stability
- Projectile Management: skill that can reflect or destroy projectiles
- Heal: healing skill
- Cleanse: condition removal skill
- Strip: boon removal skill
- Portal: a portal skill such as Mesmer's Open Portal, Scourge's Sand Swell, Thief's Shadow Portal, etc...

#### Indicators

<img src="./docs/Images/indicatorCR.PNG" width="45%" height="45%">

With this panel you can customize the display further by adding range indicators and cone indicators on the selected player.

#### Players

<img src="./docs/Images/playersCR.PNG" width="30%" height="30%">

<img src="./docs/Images/playersBuffCR.PNG" width="30%" height="30%">

This component lets you select a specific player. Once a player is selected, they will appear with a green square around on the main display.

If "Highlight Selected Group" is checked, players on the same group as the selected player will have a blue square around them.

You can observe the status of the currently active players:
- Health and Barrier
- Present buff/debuffs
- Skill casts

#### Targets

<img src="./docs/Images/targetsCR.PNG" width="30%" height="30%">

This component lets you select a specific target. Once a target is selected, they will appear with a green square around on the main display.

On this component you can observe the status of the currently active targets:
- Health and Barrier
- Breakbar state and select a breakbar phase
- Present buff/debuffs
- Skill casts

#### Mechanics

<img src="./docs/Images/mechanicsCR.PNG" width="30%" height="30%">

With this table, you can directly jump on the timestamp of a specific mechanic.

It is possible to filter the table by:
- The type of the mechanic
- The actor involved

### Healing Statistics

<img src="./docs/Images/healingStatistics.png" width="60%" height="60%">

Elite Insights fully supports healing statistics through the [ArcDPS Healing Stats Extension](https://github.com/Krappa322/arcdps_healing_stats).

Download and install this extenstion to gain access to healing statistics. Read their README for further information.

## JSON Overview 

The JSON documentation can be found [here](https://baaron4.github.io/GW2-Elite-Insights-Parser/Json/index.html).

## Contributors

Thank you to all our [contributors](https://github.com/baaron4/GW2-Elite-Insights-Parser/graphs/contributors).

Special thanks to Linus and TBTerra for creating images for the Combat Replay arenas.
