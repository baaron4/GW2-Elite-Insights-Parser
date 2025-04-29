/*jshint esversion: 6 */
"use strict";

let apiRenderServiceOkay = true;
let useDarthmaim = false;

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

const mainComponentWidth =  Math.max(0.9 * window.screen.width, 1450);
const maxStatColumns = Math.floor((mainComponentWidth - 150) / 120);

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

const WeaponIcons = {
    Unknown: "https://wiki.guildwars2.com/images/thumb/d/de/Sword_slot.png/40px-Sword_slot.png",
    Axe: "https://render.guildwars2.com/file/AE4909124900E1A3006CEA394670603D5B0C15EE/631536.png",
    Dagger: "https://render.guildwars2.com/file/2F94A543C87EAEE701BE28B26564C7B3D19C0977/631546.png",
    Mace: "https://render.guildwars2.com/file/6EA5EEBFDC1278F3F997A248362A6F9698CA09FD/631600.png",
    Pistol: "https://render.guildwars2.com/file/51217142E12EB2FE19B1DB1CAE4F1D275CC9EA03/631608.png",
    Scepter: "https://render.guildwars2.com/file/3832066C1A5B45F1C40930C703573C65CB53D73B/631624.png",
    Sword: "https://render.guildwars2.com/file/3C4AA1BD79DAB49201C81D934AC7567B286E711B/631658.png",
    Focus: "https://render.guildwars2.com/file/3F2F9F46E00592FE966F0E976445A87536743513/631554.png",
    Shield: "https://render.guildwars2.com/file/59060CD4B67508090C0F5F436499F07B71080E1B/631632.png",
    Torch: "https://render.guildwars2.com/file/081557906F6FDA4160320E3AFD42D4B11FEDDC0B/631666.png",
    Warhorn: "https://render.guildwars2.com/file/F4407BC09091D6042078B05D4B0757037300A333/631683.png",
    Greatsword: "https://render.guildwars2.com/file/B1A52DB3FCD8A6C744144FD4770BCBE8F95A4CBA/631562.png",
    Hammer: "https://render.guildwars2.com/file/A3455EC1C59AC001E12C65740DE32DA12645EFA5/631576.png",
    Longbow: "https://render.guildwars2.com/file/773EB91B749EB947CBB277D3219090CC1BDCCAC4/631592.png",
    Rifle: "https://render.guildwars2.com/file/9D0F6CE0C16A43FD0E66C55E3E27CCDF260779ED/631616.png",
    Shortbow: "https://render.guildwars2.com/file/3D7A68807006A225D124A4315DDAFB10AA07CE0F/631634.png",
    Staff: "https://render.guildwars2.com/file/F86C3CD9FA20D20EE920590517993211C6F9B99C/631650.png",
    Speargun: "https://render.guildwars2.com/file/5C473933354CB8F1542F9F0FF39A5B445877CC06/631642.png",
    Spear: "https://render.guildwars2.com/file/C427A73B00AB091FE8049AC2FD7EDEB4AF9A093F/631584.png",
    Trident: "https://render.guildwars2.com/file/434F5946A9020500C2EE2E1F0F38E2CF7F0654BC/631675.png",
};

const UIIcons = {
    Facing: "https://i.imgur.com/tZTmTRn.png",
    Barrier: "https://render.guildwars2.com/file/357922487919E8E84B914EAC13D5796DDDC42D14/1770209.png",
    Heal: "https://render.guildwars2.com/file/D4347C52157B040943051D7E09DEAD7AF63D4378/156662.png",
    Crit: "https://render.guildwars2.com/file/C2CEA567E0C43C199C782809544721AA12A6DF0A/2229323.png",
    Flank: "https://render.guildwars2.com/file/44D4631FB427F09BE5B300BE0F537E6F2126BA0B/1012653.png",
    Damage: "https://render.guildwars2.com/file/61AA4919C4A7990903241B680A69530121E994C7/156657.png",
    Breakbar: "https://wiki.guildwars2.com/images/a/ae/Unshakable.png",
    CC: "https://render.guildwars2.com/file/1999B9DB355005D2DD19F66DFFBAA6D466057508/522727.png",
    ConditionDamage: "https://render.guildwars2.com/file/0120CB042BFC2EA6A45BC3DB45155FECDDDE1910/2229318.png",
    ConditionDamageChar: "https://wiki.guildwars2.com/images/5/54/Condition_Damage.png",
    Power: "https://render.guildwars2.com/file/D6CAECEA0FD5FADE04DD6970384ADC5DE309C506/2229322.png",
    HealingPower: "https://render.guildwars2.com/file/9B986DEADC035E58C364A1423975F5F538FC2202/2229321.png",
    HealingPowerChar: "https://wiki.guildwars2.com/images/8/81/Healing_Power.png",
    Vitality: "https://render.guildwars2.com/file/CAE8B4C43FF9D203FA55016700420A0454DFFE02/2229325.png",
    VitalityChar: "https://wiki.guildwars2.com/images/b/be/Vitality.png",
    BoonDuration: "https://render.guildwars2.com/file/6574560606F6BA1B32E9CF0F6C9709D1C1F2D9A6/2207782.png",
    BoonDurationChar: "https://wiki.guildwars2.com/images/4/44/Boon_Duration.png",
    Toughness: "https://render.guildwars2.com/file/432C0F04F740C1377E6D5D56640B57083C031216/2229324.png",
    ToughnessChar: "https://wiki.guildwars2.com/images/1/12/Toughness.png",
    Activation: "https://wiki.guildwars2.com/images/6/6e/Activation.png",
    Duration: "https://render.guildwars2.com/file/7B2193ACCF77E56C13E608191B082D68AA0FAA71/156659.png",
    NumberOfTargets: "https://render.guildwars2.com/file/BBE8191A494B0352259C10EADFDACCE177E6DA5B/1770208.png",
    DownedAlly: "https://wiki.guildwars2.com/images/3/3d/Downed_ally.png",
    DownedEnemy: "https://wiki.guildwars2.com/images/c/c6/Downed_enemy.png",
    Dead: "https://wiki.guildwars2.com/images/4/4a/Ally_death_%28interface%29.png",
    Disconnected: "https://wiki.guildwars2.com/images/f/f5/Talk_end_option_tango.png",
    ConvertHealing: "https://render.guildwars2.com/file/77077EDEB2AF1F3D4062E6428000F44F77616ADE/699527.png",
    TimeWasted: "https://wiki.guildwars2.com/images/b/b3/Out_Of_Health_Potions.png",
    TimeSaved: "https://wiki.guildwars2.com/images/e/eb/Ready.png",
    StunBreak: "https://render.guildwars2.com/file/DCF0719729165FD8910E034CA4E0780F90582D15/156654.png",
    Glance: "https://render.guildwars2.com/file/6CB0E64AF9AA292E332A38C1770CE577E2CDE0E8/102853.png",
    Miss: "https://render.guildwars2.com/file/09770136BB76FD0DBE1CC4267DEED54774CB20F6/102837.png",
    Interrupt: "https://render.guildwars2.com/file/9AE125E930C92FEA0DD99E7EBAEDE4CF5EC556B6/433474.png",
    Evade: "https://wiki.guildwars2.com/images/e/e2/Evade.png",
    Dodge: "https://wiki.guildwars2.com/images/b/b2/Dodge.png",
    Invul: "https://wiki.guildwars2.com/images/e/eb/Determined.png",
    Block: "https://render.guildwars2.com/file/DFB4D1B50AE4D6A275B349E15B179261EE3EB0AF/102854.png",
    Strip: "https://render.guildwars2.com/file/D327055AA824ABDDAD70E2606E1C9AF018FF9902/961449.png",
    Cleanse: "https://render.guildwars2.com/file/F6C2FD7E78EE0D9178AEAEF8B1666477D1E92C99/103544.png",
    CleanseSelf: "https://render.guildwars2.com/file/04EB1106C95B7579EA29FA2F08BAFDA649C60715/103295.png",
    WeaponSwap: "https://wiki.guildwars2.com/images/c/ce/Weapon_Swap_Button.png",
    Commander: "https://wiki.guildwars2.com/images/5/54/Commander_tag_%28blue%29.png",
    HitboxWidth: "https://i.imgur.com/QSI79aT.png",
    HitboxHeight: "https://i.imgur.com/41lsf0q.png",
    ExclamationMark: "https://i.imgur.com/k3tdKEQ.png",
    QuestionMark: "https://i.imgur.com/nSYuby8.png",
    AgainstMoving: "https://i.imgur.com/11uAbd4.png",
    GreenFlag: "https://assets.gw2dat.com/156954.png",
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

const EIUrlParams = new URLSearchParams(window.location.search);
