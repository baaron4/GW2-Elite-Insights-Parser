$.extend($.fn.dataTable.defaults, { searching: false, ordering: true, paging: false, dom: "t" });

var urls = {
    'Warrior': 'https://wiki.guildwars2.com/images/4/43/Warrior_tango_icon_20px.png',
    'Berserker': 'https://wiki.guildwars2.com/images/d/da/Berserker_tango_icon_20px.png',
    'Spellbreaker': 'https://wiki.guildwars2.com/images/e/ed/Spellbreaker_tango_icon_20px.png',
    'Guardian': 'https://wiki.guildwars2.com/images/8/8c/Guardian_tango_icon_20px.png',
    'Dragonhunter': 'https://wiki.guildwars2.com/images/c/c9/Dragonhunter_tango_icon_20px.png',
    'DragonHunter': 'https://wiki.guildwars2.com/images/c/c9/Dragonhunter_tango_icon_20px.png',
    'Firebrand': 'https://wiki.guildwars2.com/images/0/02/Firebrand_tango_icon_20px.png',
    'Revenant': 'https://wiki.guildwars2.com/images/b/b5/Revenant_tango_icon_20px.png',
    'Herald': 'https://wiki.guildwars2.com/images/6/67/Herald_tango_icon_20px.png',
    'Renegade': 'https://wiki.guildwars2.com/images/7/7c/Renegade_tango_icon_20px.png',
    'Engineer': 'https://wiki.guildwars2.com/images/2/27/Engineer_tango_icon_20px.png',
    'Scrapper': 'https://wiki.guildwars2.com/images/3/3a/Scrapper_tango_icon_200px.png',
    'Holosmith': 'https://wiki.guildwars2.com/images/2/28/Holosmith_tango_icon_20px.png',
    'Ranger': 'https://wiki.guildwars2.com/images/4/43/Ranger_tango_icon_20px.png',
    'Druid': 'https://wiki.guildwars2.com/images/d/d2/Druid_tango_icon_20px.png',
    'Soulbeast': 'https://wiki.guildwars2.com/images/7/7c/Soulbeast_tango_icon_20px.png',
    'Thief': 'https://wiki.guildwars2.com/images/7/7a/Thief_tango_icon_20px.png',
    'Daredevil': 'https://wiki.guildwars2.com/images/e/e1/Daredevil_tango_icon_20px.png',
    'Deadeye': 'https://wiki.guildwars2.com/images/c/c9/Deadeye_tango_icon_20px.png',
    'Elementalist': 'https://wiki.guildwars2.com/images/a/aa/Elementalist_tango_icon_20px.png',
    'Tempest': 'https://wiki.guildwars2.com/images/4/4a/Tempest_tango_icon_20px.png',
    'Weaver': 'https://wiki.guildwars2.com/images/f/fc/Weaver_tango_icon_20px.png',
    'Mesmer': 'https://wiki.guildwars2.com/images/6/60/Mesmer_tango_icon_20px.png',
    'Chronomancer': 'https://wiki.guildwars2.com/images/f/f4/Chronomancer_tango_icon_20px.png',
    'Mirage': 'https://wiki.guildwars2.com/images/d/df/Mirage_tango_icon_20px.png',
    'Necromancer': 'https://wiki.guildwars2.com/images/4/43/Necromancer_tango_icon_20px.png',
    'Reaper': 'https://wiki.guildwars2.com/images/1/11/Reaper_tango_icon_20px.png',
    'Scourge': 'https://wiki.guildwars2.com/images/0/06/Scourge_tango_icon_20px.png',
    
    'Unknown': 'https://wiki.guildwars2.com/images/thumb/d/de/Sword_slot.png/40px-Sword_slot.png',
    'Sword': 'https://wiki.guildwars2.com/images/0/07/Crimson_Antique_Blade.png',
    'Axe': 'https://wiki.guildwars2.com/images/d/d4/Crimson_Antique_Reaver.png',
    'Dagger': 'https://wiki.guildwars2.com/images/6/65/Crimson_Antique_Razor.png',
    'Mace': 'https://wiki.guildwars2.com/images/6/6d/Crimson_Antique_Flanged_Mace.png',
    'Pistol': 'https://wiki.guildwars2.com/images/4/46/Crimson_Antique_Revolver.png',
    'Scepter': 'https://wiki.guildwars2.com/images/e/e2/Crimson_Antique_Wand.png',
    'Focus': 'https://wiki.guildwars2.com/images/8/87/Crimson_Antique_Artifact.png',
    'Shield': 'https://wiki.guildwars2.com/images/b/b0/Crimson_Antique_Bastion.png',
    'Torch': 'https://wiki.guildwars2.com/images/7/76/Crimson_Antique_Brazier.png',
    'Warhorn': 'https://wiki.guildwars2.com/images/1/1c/Crimson_Antique_Herald.png',
    'Greatsword': 'https://wiki.guildwars2.com/images/5/50/Crimson_Antique_Claymore.png',
    'Hammer': 'https://wiki.guildwars2.com/images/3/38/Crimson_Antique_Warhammer.png',
    'Longbow': 'https://wiki.guildwars2.com/images/f/f0/Crimson_Antique_Greatbow.png',
    'Shortbow': 'https://wiki.guildwars2.com/images/1/17/Crimson_Antique_Short_Bow.png',
    'Rifle': 'https://wiki.guildwars2.com/images/1/19/Crimson_Antique_Musket.png',
    'Staff': 'https://wiki.guildwars2.com/images/5/5f/Crimson_Antique_Spire.png'
};

var focusedPlayer = { id: -1 };
var focusedTargets = [];
var focusedPhase = { id: 0 };

Vue.component('encounter-component', {
    props: ['encounter'],
    template: `
    <div>
        <h3 class="card-header text-center">{{ encounter.name }}</h3>
        <div class ="card-body container">
            <div class="d-flex flex-row justify-content-center align-item-center">
                <img class="mr-3 icon-xxl" :src="encounter.icon" :alt="encounter.name">
                <div class="ml-3 d-flex flex-column justify-content-center align-item-center">
                    <div class="mb-2" v-for="target in encounter.targets">
                        <div v-if="encounter.targets.length > 1" class="small" style="text-align:center;">{{ target.name }}</div>
                        <div class="progress" style="width: 100%; height: 20px;" :title="target.left + '% left'">
                            <div class="progress-bar bg-success" :style="{width: target.percent + '%'}" role="progressbar" :aria-valuenow="target.percent" aria-valuemin="0" aria-valuemax="100"></div>
                        </div>
                        <div class="small" style="text-align:center;">{{ target.health }} Health</div>
                    </div>
                    <div class="mb-2 text" :class="getResultClass(encounter.success)">Result: {{ getResultText(encounter.success) }}</div>
                    <div class="mb-2">Duration: {{ encounter.duration }}</div>
                </div>
            </div>
        </div>
    </div>
    `,
    methods: {
        getResultText: function (success) {
            return success ? "Success" : "Failure";
        },
        getResultClass: function (success) {
            return success ? ["text-success"] : ["text-warning"];
        }
    }
});

Vue.component('phase-component', {
    props: ['phases', 'focusedphase'],
    template: `
        <ul class="nav nav-pills">
          <li class="nav-item" v-for="phase in phases" :title="phase.duration / 1000.0 + ' seconds'" >
            <a class="nav-link" @click="focusedphase.id = phase.id" :class="{active: focusedphase.id === phase.id}" >{{phase.name}}</a>
          </li>
        </ul>
    `
});

Vue.component('target-component', {
    props: ['targets','phasetargets', 'focusedphase'],
    template: `
        <div class="d-flex flex-row justify-content-center flex-wrap">
            <img class="icon-lg mr-2 ml-2 target-cell" v-for="target in targets" v-show="phasetargets[focusedphase.id].indexOf(target.id) !== -1" 
                    :src="target.icon" 
                    :alt="target.name" 
                    :title="target.name" 
                    :class="{active: target.active}"
                    @click="select(target)"
            >
        </div>
    `,
    methods: {
        select: function (target) {
            focusedTargets[target.id] = !focusedTargets[target.id];
            target.active = !target.active;
        }
    }
});

Vue.component('player-component', {
    props: ['groups', 'focusedplayer'],
    template: `
        <div>
            <table class="table composition">
                <tbody>
                    <tr v-for="group in groups">
                        <td class="player-cell" v-for="player in group" :class="{active: player.id === focusedplayer.id}" @click="focusedplayer.id = player.id">
                            <div>
                                <img :src="getIcon(player.prof)" :alt="player.prof" class="icon" :title="player.prof">
                                <img v-if="player.condi > 0" src="https://wiki.guildwars2.com/images/5/54/Condition_Damage.png" alt="Condition Damage" class="icon" :title="'Condition Damage - ' + player.condi">
                                <img v-if="player.conc > 0" src="https://wiki.guildwars2.com/images/4/44/Boon_Duration.png" alt="Concentration" class="icon" :title="'Concentration - ' + player.conc">
                                <img v-if="player.heal > 0" src="https://wiki.guildwars2.com/images/8/81/Healing_Power.png" alt="Healing Power" class="icon" :title="'HealingPower - ' + player.heal">
                                <img v-if="player.tough > 0" src="https://wiki.guildwars2.com/images/1/12/Toughness.png" alt="Toughness" class="icon" :title="'Toughness - ' + player.tough">
                            </div>
                            <div>
                                <img v-for="wep in player.firstSet" :src="getIcon(wep)" :title="wep" class="icon">
                                <span v-if="player.firstSet.length > 0 && player.secondSet.length > 0">/</span>
                                <img v-for="wep in player.secondSet" :src="getIcon(wep)" :title="wep" class="icon">
                            </div>
                            <div class="shorten" :title="player.acc">
                                {{ player.name }} 
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    `,
    methods: {
        getIcon: function (path) {
            return urls[path];
        }
    }
});


var createHeaderComponent = function () {
    var targets = [];
    for (var i = 0; i < logData.phases[0].targets.length; i++) {
        var targetData = logData.targets[logData.phases[0].targets[i]];
        targets.push({
            name: targetData.name,
            left: targetData.hpLeft,
            percent: targetData.percent,
            health: targetData.health
        });
    }

    var encounter = {
        name: logData.fightName,
        success: logData.success,
        icon: logData.fightIcon,
        duration: logData.encounterDuration,
        targets: targets
    }

    return new Vue({
        el: "#encounter",
        data: {
            encounter: encounter
        }
    })
}

var createPhaseNavigationComponent = function () {
    var phases = [];

    for (var i = 0; i < logData.phases.length; i++) {
        var phaseData = logData.phases[i];
        phases.push({
            id: i,
            name: phaseData.name,
            duration: phaseData.duration
        })
    }
    return new Vue({
        el: "#phase",
        data: {
            phases: phases,
            focusedphase: focusedPhase
        }
    })
}

var createTargetNavitationComponent = function () {
    var targets = [];
    for (var i = 0; i < logData.targets.length; i++) {
        if (!focusedTargets[i]) {
            focusedTargets[i] = true;
        }
        var targetData = logData.targets[i];
        targets.push({
            id: i,
            name: targetData.name,
            health: targetData.health,
            icon: targetData.icon,
            hitbox: targetData.hbWidth,
            tough: targetData.tough,
            active: true
        });
    }
    var phaseTargets = [];
    for (var i = 0; i < logData.phases.length; i++) {
        var phaseData = logData.phases[i];
        phaseTargets.push(phaseData.targets);
    }
    return new Vue({
        el: "#targets",
        data: {
            targets: targets,
            phasetargets: phaseTargets,
            focusedphase: focusedPhase
        }
    })
}

var createPlayerCompositionComponent = function () {
    var groups = [];

    for (var i = 0; i < logData.players.length; i++) {
        var playerData = logData.players[i];
        if (playerData.isConjure) {
            continue;
        }
        if (!groups[playerData.group]) {
            groups[playerData.group] = [];
        }
        var weps = playerData.weapons;
        var firstSet = [];
        var secondSet = [];
        for (var j = 0; j < weps.length; j++) {
            var wep = weps[j];
            if (wep ) {
                if (wep != "2Hand") {
                    j > 1 ? secondSet.push(wep) : firstSet.push(wep);
                }
            }
            else {
                j > 1 ? secondSet.push("Unknown") : firstSet.push("Unknown");
            }
        }
        if (firstSet[0] === "Unknown" && firstSet[1] === "Unknown") {
            firstSet = [];
        }
        if (secondSet[0] === "Unknown" && secondSet[1] === "Unknown") {
            secondSet = [];
        }
        var player = {
            id: i,
            name: playerData.name,
            acc: playerData.acc,
            prof: playerData.profession,
            tough: playerData.tough,
            conc: playerData.conc,
            heal: playerData.heal,
            condi: playerData.condi,
            firstSet: firstSet,
            secondSet: secondSet
        }
        groups[playerData.group].push(player);
    } 

    var noUndefinedGroups = [];
    for (var i = 0; i < groups.length; i++) {
        if (groups[i]) {
            noUndefinedGroups.push(groups[i]);
        }
    }

    return new Vue({
        el: "#players",
        data: {
            groups: noUndefinedGroups,
            focusedplayer: focusedPlayer
        }
    })
}

window.onload = function () {
    createHeaderComponent();
    createPhaseNavigationComponent();
    createTargetNavitationComponent();
    createPlayerCompositionComponent();
    $(function () { $('[title]').tooltip({ html: true }); });
};
