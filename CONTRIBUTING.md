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
