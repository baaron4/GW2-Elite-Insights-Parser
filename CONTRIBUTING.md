## Contributing 

This is an open source project so yes if you can code anything youd like to add feel free to make a pull request!

[How to Contribute](https://akrabat.com/the-beginners-guide-to-contributing-to-a-github-project/)

As for non coders there are still a few things you could consider adding/ maintaining as the game gets updates:


### Adding Boons / Profession Boons / Conditions 


 Go to LuckParser>Models>ParseModels>Boon.cs 

all boons can be added to private static List<Boon> allBoons

--- 

  Format: ** Boon(NAME,ID,Catagory,type,maxstacks,BoonEnum)**
  
 * name: name of the object
  
 * id: an int thatcan be found with skillID list tab
  
 * source an enum on BoonSource indicating from which class the boon originates (MIXED in doubt)
  
 * type: either BoonType.Duration ex: switness or BoonType.Intensity ex:might
  
 * capacity: both types have a max stacks. If unsure about duration go with 1
 
 * nature: indicates if the object is a CONDITION, BOON, OFFENSIVEBUFFTABLE (offensive buffs you'd like to see on the table, ex: Pinpoint Distribution), DEFENSIVEBUFFTABLE, GRAPHONLY (buffs that should only be visible on graphs), FOOD or UTILITY

 * link: an url to the image of the object

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
