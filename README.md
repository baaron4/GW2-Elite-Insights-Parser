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

NOTE: .evtc files are currently located within "C:\Users\<USERNAME>\Documents\Guild Wars 2\addons\arcdps\arcdps.cbtlogs"
## Usage
![program](https://user-images.githubusercontent.com/30677999/38950127-284f2d10-430a-11e8-937b-67a325a2a296.PNG)

1. Drag and drop 1 or multiple .evtc or .evtc.zip files into program

2. Click parse

3. When done the .html will be located in the same location as the evtc as "samename_boss_result.html"

![htmldisplay](https://user-images.githubusercontent.com/30677999/38950250-816c559e-430a-11e8-8159-1cf073a5fa44.PNG)

## Console Parsing

https://github.com/baaron4/GW2-Elite-Insights-Parser/blob/24df62abfec74446a07816524a98b9d97d87d966/LuckParser/Program.cs#L15-L22

![how to](https://user-images.githubusercontent.com/30677999/40148954-6ec9215a-5936-11e8-94ad-d2520e7c4539.PNG)

If you would like to have your logs parsed without the GUI pass the file location of each evtc file as a string. 

Settings can be configured in the GuildWars2EliteInsights.exe.config file. You can also use a custom config file:

```
GuildWars2EliteInsights.exe -c [config path] [logs]
```

To disable windows-specific commandline magic you can use -p:

```
GuildWars2EliteInsights.exe -p [logs]
```

Note it may take some time for each file to parse and they will not be ready to open the moment they are created.

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

### Other stuffs
- Linus (arena maps/ icons for combat replay)


