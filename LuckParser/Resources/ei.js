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

    'Question': 'https://wiki.guildwars2.com/images/thumb/d/de/Sword_slot.png/40px-Sword_slot.png',
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
            <div class="d-flex flex justify-content-center">
                <img class="mr-3" :src="encounter.icon" :alt="encounter.name" class="icon-xxl">
                <div class="ml-3">
                    <div class="mb-2" v-for="target in encounter.targets">
                        <div class="small" style="text-align:center;">{{ target.name }}</div>
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
    props: ['phases', 'focusedPhase'],
    template: `
        
    `,
    methods: {
        select: function (id) {
            if (focusedPhase.id !== id) {
                focusedPhase.id = id;
            }
        },
        isActive: function (id) {
            return focusedPhase.id === id;
        }        
    }
});

Vue.component('target-component', {
    props: ['targets', 'focusedtargets'],
    template: `
        
    `,
    methods: {
        select: function (id) {
            focusedTargets[id] = !focusedTargets[id];
        },
        isActive: function (id) {
            return focusedTargets[id];
        }
    }
});

Vue.component('player-component', {
    props: ['groups', 'focusedplayer'],
    template: `
        
    `,
    methods: {
        select: function (id) {
            if (focusedPlayer.id !== id) {
                focusedPlayer.id = id;
            }
        },
        isActive: function (id) {
            return focusedPlayer.id === id;
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

window.onload = function () {
    var header = createHeaderComponent();
};
