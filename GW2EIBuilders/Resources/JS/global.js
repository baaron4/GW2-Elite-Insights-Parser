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
    Sword: "https://wiki.guildwars2.com/images/0/07/Crimson_Antique_Blade.png",
    Axe: "https://wiki.guildwars2.com/images/d/d4/Crimson_Antique_Reaver.png",
    Dagger: "https://wiki.guildwars2.com/images/6/65/Crimson_Antique_Razor.png",
    Mace: "https://wiki.guildwars2.com/images/6/6d/Crimson_Antique_Flanged_Mace.png",
    Pistol: "https://wiki.guildwars2.com/images/4/46/Crimson_Antique_Revolver.png",
    Scepter: "https://wiki.guildwars2.com/images/e/e2/Crimson_Antique_Wand.png",
    Focus: "https://wiki.guildwars2.com/images/8/87/Crimson_Antique_Artifact.png",
    Shield: "https://wiki.guildwars2.com/images/b/b0/Crimson_Antique_Bastion.png",
    Torch: "https://wiki.guildwars2.com/images/7/76/Crimson_Antique_Brazier.png",
    Warhorn: "https://wiki.guildwars2.com/images/1/1c/Crimson_Antique_Herald.png",
    Greatsword: "https://wiki.guildwars2.com/images/5/50/Crimson_Antique_Claymore.png",
    Hammer: "https://wiki.guildwars2.com/images/3/38/Crimson_Antique_Warhammer.png",
    Longbow: "https://wiki.guildwars2.com/images/f/f0/Crimson_Antique_Greatbow.png",
    Shortbow: "https://wiki.guildwars2.com/images/1/17/Crimson_Antique_Short_Bow.png",
    Rifle: "https://wiki.guildwars2.com/images/1/19/Crimson_Antique_Musket.png",
    Staff: "https://wiki.guildwars2.com/images/5/5f/Crimson_Antique_Spire.png",
    Trident: "https://wiki.guildwars2.com/images/9/98/Crimson_Antique_Trident.png",
    Speargun: "https://wiki.guildwars2.com/images/3/3b/Crimson_Antique_Harpoon_Gun.png",
    Spear: "https://wiki.guildwars2.com/images/c/cb/Crimson_Antique_Impaler.png"
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
};

let AllSkillDecorations = 0;
for (let key in SkillDecorationCategory) {
    AllSkillDecorations |= SkillDecorationCategory[key];
}
let DefaultSkillDecorations = AllSkillDecorations;

const RotationStatus = {
    UNKNOWN: 0,
    REDUCED: 1,
    CANCEL: 2,
    FULL: 3,
    INSTANT: 4
};
