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

var Layout = function (desc) {
    this.desc = desc;
    this.tabs = null;
}

Layout.prototype.addTab = function (tab) {
    if (this.tabs === null) {
        this.tabs = [];
    }
    this.tabs.push(tab);
}

var Tab = function (name, options) {
    this.name = name;
    options = options ? options : {};
    this.layout = null;
    this.desc = options.desc ? options.desc : null;
    this.active = options.active ? options.active : false;
}

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
                        <div class="progress" style="width: 100%; height: 20px;" :title="target.hpLeft + '% left'">
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
    props: ['phases'],
    template: `
        <ul class="nav nav-pills">
          <li class="nav-item" v-for="phase in phases" :title="phase.duration / 1000.0 + ' seconds'" >
            <a class="nav-link" @click="select(phase,phases)" :class="{active: phase.active}" >{{phase.name}}</a>
          </li>
        </ul>
    `,
    methods: {
        select: function (phase, phases) {
            var oldStatus = phase.active;
            for (var i = 0; i < phases.length; i++) {
                phases[i].active = false;
            }
            phase.active = !oldStatus;
        }
    }
});

Vue.component('target-component', {
    props: ['targets','phases'],
    template: `
        <div class="d-flex flex-row justify-content-center flex-wrap">
            <img class="icon-lg mr-2 ml-2 target-cell" v-for="target in targets" v-show="show(target, targets, phases)" 
                    :src="target.icon" 
                    :alt="target.name" 
                    :title="target.name" 
                    :class="{active: target.active}"
                    @click="target.active = !target.active"
            >
        </div>
    `,
    methods: {
        show: function (target, targets, phases) {
            var index = targets.indexOf(target);
            var activePhase = null;
            for (var i = 0; i < phases.length; i++) {
                if (phases[i].active) {
                    activePhase = phases[i];
                    break;
                }
            }
            return activePhase.targets.indexOf(index) !== -1;
        },
    }
});

Vue.component('player-component', {
    props: ['groups'],
    template: `
        <div>
            <table class="table composition">
                <tbody>
                    <tr v-for="group in groups">
                        <td class="player-cell" v-for="player in group" :class="{active: player.active}" @click="select(player,groups)">
                            <div>
                                <img :src="getIcon(player.profession)" :alt="player.profession" class="icon" :title="player.prof">
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
        },
        select: function (player, groups) {
            var oldStatus = player.active;
            for (var i = 0; i < groups.length; i++) {
                var group = groups[i];
                for (var j = 0; j < group.length; j++) {
                    group[j].active = false;
                }
            }
            player.active = !oldStatus;
        }
    }
});

Vue.component('general-layout-component', {
    name: "general-layout-component",
    props: ['layout', "phases"],
    template: `
        <div>
            <h2 v-if="layout.desc" :class="{'text-center': !!phases}">{{ layoutName }}</h2>
            <ul class="nav nav-tabs">
                <li v-for="tab in layout.tabs">
                    <a class="nav-link" :class="{active: tab.active}" @click="select(tab, layout.tabs)"> {{ tab.name }} </a>
                </li>
            </ul>
            <div v-for="tab in layout.tabs" v-show="tab.active">
                <div v-if="tab.desc">{{ tab.desc }}</div>
                <div v-if="tab.layout">
                    <general-layout-component :layout="tab.layout"></general-layout-component>
                </div>
            </div>
        </div>
    `,
    methods: {
        select: function (tab, tabs) {
            for (var i = 0; i < tabs.length; i++) {
                tabs[i].active = false;
            }
            tab.active = true;
        }
    },
    computed: {
        layoutName: function () {
            if (!this.phases) {
                return this.layout.desc;
            }
            var phaseName = "";
            for (var i = 0; i < this.phases.length; i++) {
                if (this.phases[i].active) {
                    phaseName = this.phases[i].name;
                    break;
                }
            }
            return this.layout.desc ? phaseName + " " + this.layout.desc : phaseName;
        }
    }
});


var processData = function () {
    for (var i = 0; i < logData.phases.length; i++) {
        logData.phases[i].active = i === 0;
    }
    for (var i = 0; i < logData.targets.length; i++) {
        var targetData = logData.targets[i];
        targetData.active = true;
    }
    for (var i = 0; i < logData.players.length; i++) {
        var playerData = logData.players[i];
        playerData.active = false;
    } 
}

var createHeaderComponent = function () {
    var targets = [];
    for (var i = 0; i < logData.phases[0].targets.length; i++) {
        var targetData = logData.targets[logData.phases[0].targets[i]];
        targets.push(targetData);
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
    return new Vue({
        el: "#phase",
        data: {
            phases: logData.phases,
        }
    })
}

var createTargetNavitationComponent = function () {
    
    return new Vue({
        el: "#targets",
        data: {
            targets: logData.targets,
            phases: logData.phases
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
        groups[playerData.group].push(playerData);
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
            groups: noUndefinedGroups
        }
    })
}

var createGeneralStatsComponent = function () {
    var layout = new Layout("Summary");
    // general stats
    var stats = new Tab("General Stats", { active: true })
    var statsLayout = new Layout(null);
    statsLayout.addTab(new Tab("Damage Stats", { active: true }));
    statsLayout.addTab(new Tab("Gameplay Stats"));
    statsLayout.addTab(new Tab("Defensive Stats"));
    statsLayout.addTab(new Tab("Support Stats"));
    stats.layout = statsLayout;
    layout.addTab(stats);
    // buffs
    var buffs = new Tab("Buffs");
    var buffLayout = new Layout(null);
    buffLayout.addTab(new Tab("Boons", { active: true }));
    buffLayout.addTab(new Tab("Offensive Buffs"));
    buffLayout.addTab(new Tab("Defensive Buffs"));
    buffLayout.addTab(new Tab("Personal Buffs"));
    buffs.layout = buffLayout;
    layout.addTab(buffs);
    // mechanics
    var mechanics = new Tab("Mechanics");
    layout.addTab(mechanics);
    // graphs
    var graphs = new Tab("Graph");
    layout.addTab(graphs);
    // targets
    var targets = new Tab("Targets");
    layout.addTab(targets);
    // player
    var player = new Tab("Selected Player");
    layout.addTab(player);

    new Vue({
        el: "#content",
        data: {
            layout: layout,
            phases: logData.phases
        }
    })
    return layout;
}

window.onload = function () {
    processData();
    createHeaderComponent();
    createPhaseNavigationComponent();
    createTargetNavitationComponent();
    createPlayerCompositionComponent();
    createGeneralStatsComponent();
    var element = document.getElementById("loading");
    element.parentNode.removeChild(element);
    $(function () { $('[title]').tooltip({ html: true }); });
};
