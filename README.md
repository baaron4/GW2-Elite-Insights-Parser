# GW2-Elite-Insights-Parser
## Set Up:

1. Go to Code tab

2. Go to Release section

3. Download the GW2EI.zip file

4. Extract all files anywhere you like

5. Open GW2EI.exe(feel free to make a shortcut and move to desktop)

6. gg

NOTE: .evtc files are currently located within "C:\Users\<USERNAME>\Documents\Guild Wars 2\addons\arcdps\arcdps.cbtlogs"
## Usage
![program](https://user-images.githubusercontent.com/30677999/38950127-284f2d10-430a-11e8-937b-67a325a2a296.PNG)

1. Drag and drop 1 or multiple .evtc files into program

2. Click parse

3. Cancel will only cancel the parseing of the next log, not current

4. When done the .html will be located in the same location as the evtc as "samename_boss_result.html"

![htmldisplay](https://user-images.githubusercontent.com/30677999/38950250-816c559e-430a-11e8-8159-1cf073a5fa44.PNG)

## Auto Parseing

https://github.com/baaron4/GW2-Elite-Insights-Parser/blob/24df62abfec74446a07816524a98b9d97d87d966/LuckParser/Program.cs#L15-L22

![how to](https://user-images.githubusercontent.com/30677999/40148954-6ec9215a-5936-11e8-94ad-d2520e7c4539.PNG)

If you would like to have your logs parsed automatically pass the file location of each evtc file as a string. 

Settings can be configured in the GuildWars2EliteInsights.exe.config file. 

Note it may take some time for each file to parse and they will not be ready to open the moment they are created.

## Contributing 

This is an open source project so yes if you can code anything youd like to add feel free to make a pull request!

[How to Contribute](https://akrabat.com/the-beginners-guide-to-contributing-to-a-github-project/)

As for non coders there are still a few things you could consider adding/ maintaining as the game gets updates:


### Adding Boons / Profession Boons / Conditions 


 Go to LuckParser>Models>ParseModels>Boon.cs 

all boons can be added to private static List<Boon> allBoons

--- 

  Format: ** Boon(NAME,ID,Catagory,type,maxstacks,BoonEnum)**
  
  * NAME is a string
  
 * ID is an int can be found with skillID list tab
  
 * Catagory an enum on BoonSource indicating what class may use it
  
 * type: either "duration" ex: switness or "intensity" ex:might
  
 * maxstacks: both types have a max stacks. If unsure about duration go with 1
 
 * BoonEnum: Indicates weather or not the boon is universal,conidtion, offensive, defensive, or none sharable 

### Adding Boss Mechanics

 Go to LuckParser>Models>ParseModels>MechanicData.cs
 
 GetMechList() containts a list of all boss mechanics
 
 --- 
 
 Find the right place (based on boss) and add to the list with a new Mechanic() with format:
 
 Format: Mechanic(ID,Name,type,bossid,PlotlyShape,altName)
 
* ID is an int the skill id or boon id that is in reference to the mechanic
 
* Name is a string of the actual name of the mechanic
 
* type is an int 
        //0 boon on player
        //1 boon on boss
        //2 skill by player
        //3 skill on player
        //4 enemy boon stripped
        //5 spawn check
        //6 boss cast (check finished)
        
 * bossid is the id of th eboss it comes from. If not from any make 0
  
  * PloltyShape is a string copy from others then modify shape and color based on [Plotly Shapes](https://codepen.io/etpinard/pen/LLZGZV/)

 * altName is a string a name that is friendly to raiders and recognizable
