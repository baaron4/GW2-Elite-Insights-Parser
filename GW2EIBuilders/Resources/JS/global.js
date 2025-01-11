/*jshint esversion: 6 */
"use strict";

let apiRenderServiceOkay = true;

const quickColor = {
    r: 220,
    g: 20,
    b: 220
};
const slowColor = {
    r: 220,
    g: 125,
    b: 30
};
const normalColor = {
    r: 125,
    g: 125,
    b: 125
};

const DamageType = {
    All: 0,
    Power: 1,
    Condition: 2,
    Breakbar: 3
};

const GraphType = {
    DPS: 0,
    Damage: 1,
    CenteredDPS: 2
};

const simpleLogData = {
    phases: [],
    players: [],
    targets: []
};
//
// polyfill for string include
// https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/String/includes
if (!String.prototype.includes) {
    Object.defineProperty(String.prototype, "includes", {
        value: function (search, start) {
            if (typeof start !== 'number') {
                start = 0;
            }
            if (start + search.length > this.length) {
                return false;
            } else {
                return this.indexOf(search, start) !== -1;
            }
        }
    });
}

const themes = {
    "yeti": "https://cdnjs.cloudflare.com/ajax/libs/bootswatch/4.1.1/yeti/bootstrap.min.css",
    "slate": "https://cdnjs.cloudflare.com/ajax/libs/bootswatch/4.1.1/slate/bootstrap.min.css"
};

const urls = {
    Unknown: "https://wiki.guildwars2.com/images/thumb/d/de/Sword_slot.png/40px-Sword_slot.png",
    Axe: "https://assets.gw2dat.com/631536.png",
    Dagger: "https://assets.gw2dat.com//631546.png",
    Mace: "https://assets.gw2dat.com//631600.png",
    Pistol: "https://assets.gw2dat.com/631608.png",
    Scepter: "https://assets.gw2dat.com/631624.png",
    Sword: "https://assets.gw2dat.com/631658.png",
    Focus: "https://assets.gw2dat.com/631554.png",
    Shield: "https://assets.gw2dat.com/631632.png",
    Torch: "https://assets.gw2dat.com/631666.png",
    Warhorn: "https://assets.gw2dat.com/631683.png",
    Greatsword: "https://assets.gw2dat.com/631562.png",
    Hammer: "https://assets.gw2dat.com/631576.png",
    Longbow: "https://assets.gw2dat.com/631592.png",
    Rifle: "https://assets.gw2dat.com/631616.png",
    Shortbow: "https://assets.gw2dat.com/631634.png",
    Staff: "https://assets.gw2dat.com/631650.png",
    Speargun: "https://assets.gw2dat.com/631642.png",
    Spear: "https://assets.gw2dat.com/631584.png",
    Trident: "https://assets.gw2dat.com/631675.png",
};

const specs = [
    "Warrior",
    "Berserker",
    "Spellbreaker",
    "Bladesworn",
    //
    "Revenant",
    "Herald",
    "Renegade",
    "Vindicator",
    //
    "Guardian",
    "Dragonhunter",
    "Firebrand",
    "Willbender",
    //
    "Ranger",
    "Druid",
    "Soulbeast",
    "Untamed",
    //
    "Engineer",
    "Scrapper",
    "Holosmith",
    "Mechanist",
    //
    "Thief",
    "Daredevil",
    "Deadeye",
    "Specter",
    //
    "Mesmer",
    "Chronomancer",
    "Mirage",
    "Virtuoso",
    //
    "Necromancer",
    "Reaper",
    "Scourge",
    "Harbinger",
    //
    "Elementalist",
    "Tempest",
    "Weaver",
    "Catalyst"
];

const SpecToBase = {
    Warrior: 'Warrior',
    Berserker: 'Warrior',
    Spellbreaker: 'Warrior',
    Bladesworn: 'Warrior',
    //
    Revenant: "Revenant",
    Herald: "Revenant",
    Renegade: "Revenant",
    Vindicator: "Revenant",
    //
    Guardian: "Guardian",
    Dragonhunter: "Guardian",
    Firebrand: "Guardian",
    Willbender: "Guardian",
    //
    Ranger: "Ranger",
    Druid: "Ranger",
    Soulbeast: "Ranger",
    Untamed: "Ranger",
    //
    Engineer: "Engineer",
    Scrapper: "Engineer",
    Holosmith: "Engineer",
    Mechanist: "Engineer",
    //
    Thief: "Thief",
    Daredevil: "Thief",
    Deadeye: "Thief",
    Specter: "Thief",
    //
    Mesmer: "Mesmer",
    Chronomancer: "Mesmer",
    Mirage: "Mesmer",
    Virtuoso: "Mesmer",
    //
    Necromancer: "Necromancer",
    Reaper: "Necromancer",
    Scourge: "Necromancer",
    Harbinger: "Necromancer",
    //
    Elementalist: "Elementalist",
    Tempest: "Elementalist",
    Weaver: "Elementalist",
    Catalyst: "Elementalist"
};

const SkillDecorationCategory = {
    "Show On Select" : 1 << 0,
    "Important Buffs": 1 << 1,
    "Projectile Management": 1 << 2,
    "Heal": 1 << 3,
    "Cleanse": 1 << 4,
    "Strip": 1 << 5,
    "Portal": 1 << 6,
    "CC": 1 << 7,
};

let AllSkillDecorations = 0;
for (let key in SkillDecorationCategory) {
    AllSkillDecorations |= SkillDecorationCategory[key];
}

let DefaultSkillDecorations = AllSkillDecorations & ~SkillDecorationCategory.CC;

const RotationStatus = {
    UNKNOWN: 0,
    REDUCED: 1,
    CANCEL: 2,
    FULL: 3,
    INSTANT: 4
};

const UIIcons = {
    Barrier: "https://assets.gw2dat.com/1770209.png",
    Heal: "https://assets.gw2dat.com/156662.png",
    Crit: "https://assets.gw2dat.com/2229323.png",
    Flank: "https://assets.gw2dat.com/1012653.png",
    Damage: "https://assets.gw2dat.com/156657.png",
    Breakbar: "https://wiki.guildwars2.com/images/a/ae/Unshakable.png",
    CC: "https://assets.gw2dat.com/522727.png",
    ConditionDamage: "https://assets.gw2dat.com/2229318.png",
    Power: "https://assets.gw2dat.com/2229322.png",
    HealingPower: "https://assets.gw2dat.com/2229321.png",
    Activation: "https://wiki.guildwars2.com/images/6/6e/Activation.png",
    Duration: "https://assets.gw2dat.com/156659.png",
    NumberOfTargets: "https://assets.gw2dat.com/1770208.png",
    DownedAlly: "https://wiki.guildwars2.com/images/3/3d/Downed_ally.png",
    DownedEnemy: "https://wiki.guildwars2.com/images/c/c6/Downed_enemy.png",
    Dead: "https://wiki.guildwars2.com/images/4/4a/Ally_death_%28interface%29.png",
    ConvertHealing: "https://wiki.guildwars2.com/images/4/4a/Litany_of_Wrath.png",
    TimeWasted: "https://wiki.guildwars2.com/images/b/b3/Out_Of_Health_Potions.png",
    TimeSaved: "https://wiki.guildwars2.com/images/e/eb/Ready.png",
    StunBreak: "https://assets.gw2dat.com/156654.png",
    Glance: "https://assets.gw2dat.com/102853.png",
    Miss: "https://assets.gw2dat.com/102837.png",
    Interrupt: "https://assets.gw2dat.com/433474.png",
    Evade: "https://wiki.guildwars2.com/images/e/e2/Evade.png",
    Dodge: "https://wiki.guildwars2.com/images/b/b2/Dodge.png",
    Invul: "https://wiki.guildwars2.com/images/e/eb/Determined.png",
    Block: "https://assets.gw2dat.com/102854.png",
    Strip: "https://assets.gw2dat.com/961449.png",
    Cleanse: "https://assets.gw2dat.com/103544.png"
};

const EIUrlParams = new URLSearchParams(window.location.search);
