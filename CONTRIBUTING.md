# Contributing

This is an open source project so yes if you can code anything youd like to add feel free to make a pull request!

[How to Contribute](https://akrabat.com/the-beginners-guide-to-contributing-to-a-github-project/)

As for non coders there are still a few things you could consider adding/ maintaining as the game gets updates:

## Adding Boons / Profession Boons / Conditions

Go to LuckParser>Models>ParseModels>Boons>Boon.cs

Boons can be added to private static list _allBoons using the following parameters:
- string name: name of the boon
- long id: id of the boon
- BoonSource source: where does the boon come from (spec, item, ...)
- BoonType type: intensity or duration
- int capacity: max number of stack
- BoonNature nature: category of the boon (boon, condition, shareable offensive buff, etc)
- Logic logic: stacking logic of tne boon. In doubt use OVerride
- string link: url to the icon of the buff

## Adding Boss Mechanics

Go to LuckParser>Models>FightLogic>"FightName".cs with FightName being the fight you are interested in.
You can add a new mechanic inside MechanicList.AddRange.

EI supports several mechanic types, each using the same signature:
- EnemyBoonApply: a buff application on an enemy entity
- EnemyBoonRemove: a buff removal on an enemy entity
- EnemyCastEnd: the ending of an enemy cast animation
- EnemyCastStart: the start of an enemy cast animation
- HitOnEnemey: physical hit on an enemy entity
- PlayerBoonApply: a buff application on a player
- PlayerBoonRemove: a buff removal on a player
- PlayerCastStart: the start of a player cast animation
- PlayerCastEnd: the ending of a player cast animation
- PlayerOnPlayer: a mechanic that affects two players at the same time
- SkillOnPlayer: a player getting hit by a skill (can be physical or indirect)
- SpawnMechanic: spawn of an enemy entity

Each type is built using the following parameters:
- long skillId: id of the mechanic
- string inGameName: the "official" name of the mechanic
- MechanicPlotlySetting plotlySetting: an object that contains color, symbol and size data for the html plots
- string shortName: the name that'll appear on the table, you can regroup two mechanics together by giving them the same shortName
- string description: description of the mechanics, that'll appear when hovering on the shortName
- string fullName: the name that'll be used as legend on the html plots
- int internalCooldown: used to filter multiple hits. A grace period during which getting hit by the mechanic does not count. In ms.

## Adding support for a new fight

If you are feeling a little bit more courageous, you can add support for a new fight. To do so you need:
- the trigger id of the fight
- an icon for the fight
- an icon for the target(s) you wish to track

and that's it. 
Only "simple" support will be explained. Adding combat replay requires the creation of custom assets and proper coordinates positioning. Advanced usage, like phases, also requires good server_events/combat_events understanding.

Now create a file name "MyFightName".cs in LuckParser>Models>FightLogic that inherits from:
- RaidLogic if a raid fight
- FractalLogic if a fractal fight
- FightLogic otherwise

Implement the constructor MyFightName(ushort triggerID). Put inside:
- MechanicList.AddRange(new List<Mechanic>{}) : this will be used to add mechanics to the fight
- Extension = "MyFightExtension" : the suffix of the fight
- IconUrl = url to the icon of the fight

Before we continue working on that file, do the following steps:
- go to LuckParser>Parser>ParseEnum.cs and add all the required target/mob ids to TargetIDS and TrashIDS respectively
- go to LuckParser>GeneralHelper.cs and add all the required target/mob icons to GetNPCIcon
- go to LuckParser>Models>FightData and add the trigger id to the constructor

Implement GetUniqueTargetIDs() that'll return an HashSet of ids that are supposed to be unique during the fight. This is used in case the target despawn during the fight and comes back. For arcdps it'll appear as two different entities. By putting the id in the method, we'll merge those entities into one.

Implement GetFightTargetsIds() that'll return a list of ids to track.

That's it.
