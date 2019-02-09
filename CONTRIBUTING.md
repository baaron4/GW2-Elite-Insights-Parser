## Contributing 

This is an open source project so yes if you can code anything youd like to add feel free to make a pull request!

[How to Contribute](https://akrabat.com/the-beginners-guide-to-contributing-to-a-github-project/)

As for non coders there are still a few things you could consider adding/ maintaining as the game gets updates:


### Adding Boons / Profession Boons / Conditions 


 Go to LuckParser>Models>ParseModels>Boons>Boon.cs 

all boons can be added to private static List<Boon> _allBoons. The private Boon constructor explains how to add new boons.

### Adding Boss Mechanics

 Go to LuckParser>Models>FightLogic>"FightName".cs
 
 You can add a new mechanic inside MechanicList.AddRange.

 See LuckParser>Models>ParseModels>Mechanics>Mechanic.cs to see how a mechanic is built.
