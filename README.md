## Contact
For quick questions, discussions or other conversation that isn't an issue feel free to join an open discord channel: 
https://discord.gg/dCDEPXx

Our GitHub Page: https://baaron4.github.io/GW2-Elite-Insights-Parser/ 

# GW2-Elite-Insights-Parser
## Set Up

1. Go to Code tab

2. Go to Release section

3. Download the GW2EI.zip file

4. Extract all files anywhere you like

5. Open GW2EI.exe (feel free to make a shortcut and move to desktop)

6. gg

NOTE: ArcDPS log files are currently located within "C:\Users\<USERNAME>\Documents\Guild Wars 2\addons\arcdps\arcdps.cbtlogs"
## Usage
![program](https://user-images.githubusercontent.com/30677999/38950127-284f2d10-430a-11e8-937b-67a325a2a296.PNG)

1. Drag and drop 1 or multiple .evtc, .evtc.zip or .zevtc files into program

2. Click parse

3. When done the .html will be located in the same location as the evtc, or at the desired output location, as "samename_boss_result.html"

![htmldisplay](https://user-images.githubusercontent.com/30677999/38950250-816c559e-430a-11e8-8159-1cf073a5fa44.PNG)

## Console Parsing

https://github.com/baaron4/GW2-Elite-Insights-Parser/blob/24df62abfec74446a07816524a98b9d97d87d966/LuckParser/Program.cs#L15-L22

![how to](https://user-images.githubusercontent.com/30677999/40148954-6ec9215a-5936-11e8-94ad-d2520e7c4539.PNG)

If you would like to have your logs parsed without the GUI pass the file location of each evtc file as a string. 

Settings can be configured using .conf files (see Settings/sample.conf for an example). You can then use it with -c:

```
GuildWars2EliteInsights.exe -c [config path] [logs]
```

To disable windows-specific commandline magic you can use -p:

```
GuildWars2EliteInsights.exe -p [logs]
```

You can start the application in GUI mode using -ui option:
```
GuildWars2EliteInsights.exe -c [config path] -ui [logs]
```

Note it may take some time for each file to parse and they will not be ready to open the moment they are created.

## Settings

### Output Settings

-__SaveAtOut__: if true, the generated files will be in the same location as the source file.

-__OutLocation__: secondary output path, will be used if SaveAtOut is false.

-__Anonymous__: if true, player character and account names will be obfuscated.

-__AddPoVProf__: if true, the profession of the pov will be added to the generated files' name.

-__AddDuration__: if true, the duration (truncated to seconds) will be added to the generated files' name.

### Parser Settings

-__MultiThreaded__: if true, multiple threads will be used for parsing a single log.

-__ParseMultipleLogs__: if true, multiple logs will be parsed in parallel.

-__SaveOutTrace__: if true, log files will be generated.

-__SkipFailedTries__: if true, failed encounters will not be parsed.

-__CustomTooShort__: Customize encounter duration in ms below which logs will not be parsed.

### GUI only Parser Settings

-__AutoAdd__: if true, EI will automatically add logs that appear in AutoAddPath.

-__AutoAddPath__: the path to listen to for automatic additions.

-__AutoParse__: if true, every added log file will be automatically processed.

### Encounter Settings

-__ParsePhases__: if true, phases will be parsed.

-__ParseCombatReplay__: if true, combat replay will be computed.

-__ComputeDamageModifiers__: if true, damage modifiers will be computed ( forcefully disabled on WvW logs for now).

### HTML settings

-__SaveOutHTML__: if true, html logs will be generated.

-__HtmlExternalScripts__: if true, css and js files will be separated from the html.

-__LightTheme__: if true, the html will use a light theme by default. Please note that the theme can be dynamically changed on the html post generation.

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

-__UploadToDPSReportsRH__: if true, the log will be uploaded to dps.reports using RH as generator.

-__UploadToRaidar__: if true, the log will be uploaded to raidar (not used).

-__WebhookURL__: Webhook URL to send an embed or simple message to.

-__SendEmbedToWebhook__: if true, the Webhook URL will receive a small embed containing meta data + dps.reports link. (Does not work with ParseMultipleLogs for now)

-__SendSimpleMessageToWebhook__: if true, only the dps.reports link will be sent to the webhook.

## JSON Documentation

The Json documentation can be found [here](https://baaron4.github.io/GW2-Elite-Insights-Parser/Json/index.html)

## Contributors
### Developers
- baaron4
- EliphasNUIT
- cordbleibaum
- QuiCM
- amgine
- Linus
- Sejsel
- Flomix
- Stonos
- Hobinjk

### Other stuffs
- Linus (arena maps/ icons for combat replay)


