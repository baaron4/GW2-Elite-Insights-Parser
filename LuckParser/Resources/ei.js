/*jshint esversion: 6 */
$.extend($.fn.dataTable.defaults, {
    searching: false,
    ordering: true,
    paging: false,
    retrieve: true,
    dom: "t"
});

var specs = [
    "Warrior", "Berserker", "Spellbreaker", "Revenant", "Herald", "Renegade", "Guardian", "Dragonhunter", "Firebrand",
    "Ranger", "Druid", "Soulbeast", "Engineer", "Scrapper", "Holosmith", "Thief", "Daredevil", "Deadeye",
    "Mesmer", "Chronomancer", "Mirage", "Necromancer", "Reaper", "Scourge", "Elementalist", "Tempest", "Weaver"
];

var lazyTableUpdater = null;

if ('IntersectionObserver' in window) {
    lazyTableUpdater = new IntersectionObserver(function (entries, observer) {
        entries.forEach(function (entry) {
            if (entry.isIntersecting) {
                var id = entry.target.id;
                var table = $("#" + id);
                if ($.fn.dataTable.isDataTable(table)) {
                    var order = table.DataTable().order();
                    table.DataTable().destroy();
                    table.DataTable().order(order);
                    table.DataTable().draw();
                }
                observer.unobserve(entry.target);
            }
        });
    });
}

var specToBase = {
    Warrior: 'Warrior',
    Berserker: 'Warrior',
    Spellbreaker: 'Warrior',
    Revenant: "Revenant",
    Herald: "Revenant",
    Renegade: "Revenant",
    Guardian: "Guardian",
    Dragonhunter: "Guardian",
    Firebrand: "Guardian",
    Ranger: "Ranger",
    Druid: "Ranger",
    Soulbeast: "Ranger",
    Engineer: "Engineer",
    Scrapper: "Engineer",
    Holosmith: "Engineer",
    Thief: "Thief",
    Daredevil: "Thief",
    Deadeye: "Thief",
    Mesmer: "Mesmer",
    Chronomancer: "Mesmer",
    Mirage: "Mesmer",
    Necromancer: "Necromancer",
    Reaper: "Necromancer",
    Scourge: "Necromancer",
    Elementalist: "Elementalist",
    Tempest: "Elementalist",
    Weaver: "Elementalist"
};

var urls = {
    Warrior: "https://wiki.guildwars2.com/images/4/43/Warrior_tango_icon_20px.png",
    Berserker: "https://wiki.guildwars2.com/images/d/da/Berserker_tango_icon_20px.png",
    Spellbreaker: "https://wiki.guildwars2.com/images/e/ed/Spellbreaker_tango_icon_20px.png",
    Guardian: "https://wiki.guildwars2.com/images/8/8c/Guardian_tango_icon_20px.png",
    Dragonhunter: "https://wiki.guildwars2.com/images/c/c9/Dragonhunter_tango_icon_20px.png",
    DragonHunter: "https://wiki.guildwars2.com/images/c/c9/Dragonhunter_tango_icon_20px.png",
    Firebrand: "https://wiki.guildwars2.com/images/0/02/Firebrand_tango_icon_20px.png",
    Revenant: "https://wiki.guildwars2.com/images/b/b5/Revenant_tango_icon_20px.png",
    Herald: "https://wiki.guildwars2.com/images/6/67/Herald_tango_icon_20px.png",
    Renegade: "https://wiki.guildwars2.com/images/7/7c/Renegade_tango_icon_20px.png",
    Engineer: "https://wiki.guildwars2.com/images/2/27/Engineer_tango_icon_20px.png",
    Scrapper: "https://wiki.guildwars2.com/images/3/3a/Scrapper_tango_icon_200px.png",
    Holosmith: "https://wiki.guildwars2.com/images/2/28/Holosmith_tango_icon_20px.png",
    Ranger: "https://wiki.guildwars2.com/images/4/43/Ranger_tango_icon_20px.png",
    Druid: "https://wiki.guildwars2.com/images/d/d2/Druid_tango_icon_20px.png",
    Soulbeast: "https://wiki.guildwars2.com/images/7/7c/Soulbeast_tango_icon_20px.png",
    Thief: "https://wiki.guildwars2.com/images/7/7a/Thief_tango_icon_20px.png",
    Daredevil: "https://wiki.guildwars2.com/images/e/e1/Daredevil_tango_icon_20px.png",
    Deadeye: "https://wiki.guildwars2.com/images/c/c9/Deadeye_tango_icon_20px.png",
    Elementalist: "https://wiki.guildwars2.com/images/a/aa/Elementalist_tango_icon_20px.png",
    Tempest: "https://wiki.guildwars2.com/images/4/4a/Tempest_tango_icon_20px.png",
    Weaver: "https://wiki.guildwars2.com/images/f/fc/Weaver_tango_icon_20px.png",
    Mesmer: "https://wiki.guildwars2.com/images/6/60/Mesmer_tango_icon_20px.png",
    Chronomancer: "https://wiki.guildwars2.com/images/f/f4/Chronomancer_tango_icon_20px.png",
    Mirage: "https://wiki.guildwars2.com/images/d/df/Mirage_tango_icon_20px.png",
    Necromancer: "https://wiki.guildwars2.com/images/4/43/Necromancer_tango_icon_20px.png",
    Reaper: "https://wiki.guildwars2.com/images/1/11/Reaper_tango_icon_20px.png",
    Scourge: "https://wiki.guildwars2.com/images/0/06/Scourge_tango_icon_20px.png",

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
    Staff: "https://wiki.guildwars2.com/images/5/5f/Crimson_Antique_Spire.png"
};

var initTable = function (id, cell, order) {
    var table = $(id);
    if (lazyTableUpdater) {
        var lazyTable = document.querySelector(id);
        var lazyTableObserver = new IntersectionObserver(function (entries, observer) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    table.DataTable({ order: [[cell, order]] });
                    observer.unobserve(entry.target);
                }
            });
        });
        lazyTableObserver.observe(lazyTable);
    } else {
        table.DataTable({ order: [[cell, order]] });
    }
};

var updateTable = function(id) {   
    if (lazyTableUpdater) {
        var lazyTable = document.querySelector(id);
        lazyTableUpdater.unobserve(lazyTable);
        lazyTableUpdater.observe(lazyTable);
    } else {
        var table = $(id);
        if ($.fn.dataTable.isDataTable(id)) {
            var order = table.DataTable().order();
            table.DataTable().destroy();
            table.DataTable().order(order);
            table.DataTable().draw();
        }
    }
}

var DataTypes = {
    damageTable: 0,
    defTable: 1,
    supTable: 2,
    gameplayTable: 3,
    mechanicTable: 4,
    boonTable: 5,
    offensiveBuffTable: 6,
    defensiveBuffTable: 7,
    personalBuffTable: 8,
    dmgModifiersTable: 9,
    playerTab: 10,
    targetTab: 11,
};

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
    playerData.icon = urls[playerData.profession];
}

var Layout = function (desc) {
    this.desc = desc;
    this.tabs = null;
};

Layout.prototype.addTab = function (tab) {
    if (this.tabs === null) {
        this.tabs = [];
    }
    this.tabs.push(tab);
};

var Tab = function (name, options) {
    this.name = name;
    options = options ? options : {};
    this.layout = null;
    this.desc = options.desc ? options.desc : null;
    this.active = options.active ? options.active : false;
    this.dataType =
        typeof options.dataType !== "undefined" ? options.dataType : -1;
};

Vue.component("encounter-component", {
    props: ["logdata"],
    methods: {
        getResultText: function (success) {
            return success ? "Success" : "Failure";
        },
        getResultClass: function (success) {
            return success ? ["text-success"] : ["text-warning"];
        }
    },
    computed: {
        encounter: function () {
            var logData = this.logdata;
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
            };
            return encounter;
        }
    }
});

Vue.component("phase-component", {
    props: ["phases"],
    methods: {
        select: function (phase) {
            for (var i = 0; i < this.phases.length; i++) {
                this.phases[i].active = false;
            }
            phase.active = true;
        }
    }
});

Vue.component("target-component", {
    props: ["targets", "phase"],
    methods: {
        show: function (target) {
            var index = this.targets.indexOf(target);
            return this.phase.targets.indexOf(index) !== -1;
        }
    }
});

Vue.component("player-component", {
    props: ["players"],
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
    },
    computed: {
        groups: function () {
            var aux = [];
            var i = 0;
            for (i = 0; i < this.players.length; i++) {
                var playerData = this.players[i];
                if (playerData.isConjure) {
                    continue;
                }
                if (!aux[playerData.group]) {
                    aux[playerData.group] = [];
                }
                aux[playerData.group].push(playerData);
            }

            var noUndefinedGroups = [];
            for (i = 0; i < aux.length; i++) {
                if (aux[i]) {
                    noUndefinedGroups.push(aux[i]);
                }
            }
            return noUndefinedGroups;
        }
    }
});

Vue.component("general-layout-component", {
    name: "general-layout-component",
    props: ["layout", "phase"],
    template: `
        <div>
            <h2 v-if="layout.desc" :class="{'text-center': !!phase}">{{ layoutName }}</h2>
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
            if (!this.phase) {
                return this.layout.desc;
            }
            return this.layout.desc ?
                this.phase.name + " " + this.layout.desc :
                this.phase.name;
        }
    }
});

Vue.component("damage-stats-component", {
    props: ["phase", "targets", "players"],
    mounted() {
        initTable("#dps-table", 4, "desc");
    },
    updated() {
        updateTable("#dps-table");
    },
    computed: {
        tableData: function () {
            var phase = this.phase;
            var rows = [];
            var sums = [];
            var total = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
            var groups = [];
            var i, j;
            for (i = 0; i < phase.dpsStats.length; i++) {
                var dpsStat = phase.dpsStats[i];
                var dpsTargetStat = [0, 0, 0, 0, 0, 0];
                for (j = 0; j < phase.targets.length; j++) {
                    if (this.targets[phase.targets[j]].active) {
                        var tar = phase.dpsStatsTargets[i][j];
                        for (var k = 0; k < dpsTargetStat.length; k++) {
                            dpsTargetStat[k] += tar[k];
                        }
                    }
                }
                var player = this.players[i];
                if (!groups[player.group]) {
                    groups[player.group] = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
                }
                var dps = dpsTargetStat.concat(dpsStat);
                for (j = 0; j < dps.length; j++) {
                    total[j] += dps[j];
                    groups[player.group][j] += dps[j];
                }
                rows.push({
                    player: player,
                    dps: dps
                });
            }
            for (i = 0; i < groups.length; i++) {
                if (groups[i]) {
                    sums.push({
                        name: "Group " + i,
                        dps: groups[i]
                    });
                }
            }
            sums.push({
                name: "Total",
                dps: total
            });
            var res = {
                rows: rows,
                sums: sums
            };
            return res;
        }
    }
});

Vue.component("defense-stats-component", {
    props: ["phase", "players"],
    mounted() {
        initTable("#def-table", 4, "desc");
    },
    updated() {
        updateTable("#def-table");
    },
    computed: {
        tableData: function () {
            var rows = [];
            var sums = [];
            var total = [0, 0, 0, 0, 0, 0, 0];
            var groups = [];
            var i;
            for (i = 0; i < this.phase.defStats.length; i++) {
                var def = this.phase.defStats[i];
                var player = this.players[i];
                if (player.isConjure) {
                    continue;
                }
                rows.push({
                    player: player,
                    def: def
                });
                if (!groups[player.group]) {
                    groups[player.group] = [0, 0, 0, 0, 0, 0, 0];
                }
                for (var j = 0; j < total.length; j++) {
                    total[j] += def[j];
                    groups[player.group][j] += def[j];
                }
            }
            for (i = 0; i < groups.length; i++) {
                if (groups[i]) {
                    sums.push({
                        name: "Group " + i,
                        def: groups[i]
                    });
                }
            }
            sums.push({
                name: "Total",
                def: total
            });
            var res = {
                rows: rows,
                sums: sums
            };
            return res;
        }
    }
});

Vue.component("support-stats-component", {
    props: ["phase", "players"],
    mounted() {
        initTable("#sup-table", 4, "desc");
    },
    updated() {
        updateTable("#sup-table");
    },
    computed: {
        tableData: function () {
            var rows = [];
            var sums = [];
            var total = [0, 0, 0, 0];
            var groups = [];
            var i;
            for (i = 0; i < this.phase.healStats.length; i++) {
                var sup = this.phase.healStats[i];
                var player = this.players[i];
                if (player.isConjure) {
                    continue;
                }
                rows.push({
                    player: player,
                    sup: sup
                });
                if (!groups[player.group]) {
                    groups[player.group] = [0, 0, 0, 0];
                }
                for (var j = 0; j < sup.length; j++) {
                    total[j] += sup[j];
                    groups[player.group][j] += sup[j];
                }
            }
            for (i = 0; i < groups.length; i++) {
                if (groups[i]) {
                    sums.push({
                        name: "Group " + i,
                        sup: groups[i]
                    });
                }

            }
            sums.push({
                name: "Total",
                sup: total
            });
            var res = {
                rows: rows,
                sums: sums
            };
            return res;
        }
    }
});

Vue.component("gameplay-stats-component", {
    props: ["phase", "targets", "players"],
    data: function () {
        return {
            mode: 0,
        };
    },
    mounted() {
        initTable("#dmg-table", 4, "desc");
    },
    updated() {
        updateTable("#dmg-table");
    },
    methods: {
        round2: function (value) {
            if (isNaN(value)) {
                return 0;
            }
            var mul = 100;
            return Math.round(mul * value) / mul;
        },
        round3: function (value) {
            if (isNaN(value)) {
                return 0;
            }
            var mul = 1000;
            return Math.round(mul * value) / mul;
        }
    },
    computed: {
        tableData: function () {
            var phase = this.phase;
            var rows = [];
            for (var i = 0; i < phase.dmgStats.length; i++) {
                var commons = [];
                var data = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
                var player = this.players[i];
                if (player.isConjure) {
                    continue;
                }
                var stats = phase.dmgStats[i];
                for (var j = 0; j < stats.length; j++) {
                    if (j >= 14) {
                        commons[j - 14] = stats[j];
                    } else {
                        if (!this.mode) {
                            data[j] = stats[j];
                        } else {
                            for (var k = 0; k < phase.targets.length; k++) {
                                if (this.targets[phase.targets[k]].active) {
                                    var tar = phase.dmgStatsTargets[i][k];
                                    data[j] += tar[j];
                                }
                            }
                        }
                    }
                }
                rows.push({
                    player: player,
                    commons: commons,
                    data: data
                });
            }
            var res = {
                rows: rows
            };
            return res;
        }
    }
});
Vue.component("mechanics-stats-component", {
    props: ["phase", "players", "enemies", "mechanics"],
    mounted() {
        initTable("#playermechs", 0, "asc");
        //
        if (this.enemyMechHeader.length) {           
            initTable("#enemymechs", 0, "asc");
        }
    },
    updated() {        
        updateTable("#playermechs");
        //
        if (this.enemyMechHeader.length) {           
            updateTable("#enemymechs");
        }
    },
    computed: {
        playerMechHeader: function () {
            var mechanics = this.mechanics;
            var playerMechanics = [];
            for (var i = 0; i < mechanics.length; i++) {
                if (mechanics[i].playerMech) {
                    playerMechanics.push(mechanics[i]);
                }
            }
            return playerMechanics;
        },
        playerMechRows: function () {
            var phase = this.phase;
            var players = this.players;
            var rows = [];
            for (var i = 0; i < players.length; i++) {
                var player = players[i];
                if (player.isConjure) {
                    continue;
                }
                rows.push({
                    player: player,
                    mechs: phase.mechanicStats[i]
                });
            }
            return rows;
        },
        enemyMechHeader: function () {
            var mechanics = this.mechanics;
            var enemyMechanics = [];
            for (var i = 0; i < mechanics.length; i++) {
                if (mechanics[i].enemyMech) {
                    enemyMechanics.push(mechanics[i]);
                }
            }
            return enemyMechanics;
        },
        enemyMechRows: function () {
            var phase = this.phase;
            var enemies = this.enemies;
            var rows = [];
            for (var i = 0; i < enemies.length; i++) {
                var enemy = enemies[i];
                rows.push({
                    enemy: enemy.name,
                    mechs: phase.enemyMechanicStats[i]
                });
            }
            return rows;
        }
    }
});

Vue.component("buff-table-component", {
    props: ["buffs", "playerdata", "generation", "condition", "sums", "id"],
    template: "#buff-table-template",
    methods: {
        getAvgTooltip: function (avg) {
            if (avg) {
                return (
                    "Average number of " +
                    (this.condition ? "conditions: " : "boons: ") +
                    avg
                );
            }
            return false;
        },
        getCellTooltip: function (buff, val) {
            if (val instanceof Array) {
                if (this.generation && val[0] > 0) {
                    return val[1] + (buff.stacking ? "" : "%") + " with overstack";
                } else if (buff.stacking && val[1] > 0) {
                    return "Uptime: " + val[1] + "%";
                }
            }
            return false;
        },
        getCellValue: function (buff, val) {
            var value = val;
            if (val instanceof Array) {
                value = val[0];
            }
            if (value > 0) {
                return buff.stacking ? value : value + "%";
            }
            return "-";
        }
    },
    mounted() {
        initTable('#'+this.id, 0, "asc");
    },
    updated() {       
        updateTable('#'+this.id);
    },
});

Vue.component("personal-buff-table-component", {
    props: ['phase', 'persbuffs', 'players', 'buffmap'],
    data: function () {
        return {
            specs: specs,
            bases: [],
            specToBase: specToBase,
            mode: "Warrior",
        };
    },
    computed: {
        orderedSpecs: function () {
            var res = [];
            var aux = new Set();
            for (var i = 0; i < this.specs.length; i++) {
                var spec = this.specs[i];
                var pBySpec = [];
                for (var j = 0; j < this.players.length; j++) {
                    if (this.players[j].profession === spec) {
                        pBySpec.push(j);
                    }
                }
                if (pBySpec.length) {
                    aux.add(this.specToBase[spec]);
                    res.push({
                        ids: pBySpec,
                        name: spec
                    });
                }
            }
            this.bases = [];
            var _this = this;
            aux.forEach(function (value, value2, set) {
                _this.bases.push(value);
            });
            this.mode = this.bases[0];
            return res;
        },
        data: function () {
            var res = [];
            for (var i = 0; i < this.orderedSpecs.length; i++) {
                var spec = this.orderedSpecs[i];
                var dataBySpec = [];
                for (var j = 0; j < spec.ids.length; j++) {
                    dataBySpec.push({
                        player: this.players[spec.ids[j]],
                        data: this.phase.persBuffStats[spec.ids[j]]
                    });
                }
                res.push(dataBySpec);
            }
            return res;
        },
        buffs: function () {
            var res = [];
            for (var i = 0; i < this.orderedSpecs.length; i++) {
                var spec = this.orderedSpecs[i];
                var data = [];
                for (var j = 0; j < this.persbuffs[spec.name].length; j++) {
                    var boonid = 'b' + this.persbuffs[spec.name][j];
                    data.push(this.buffmap[boonid]);
                }
                res.push(data);
            }
            return res;
        }
    }
});

Vue.component("buff-stats-component", {
    props: ['datatypes', 'datatype', 'phase', 'players', 'presentboons', 'presentoffs', 'presentdefs', 'buffmap'],
    data: function () {
        return {
            mode: 0,
        };
    },
    computed: {
        boons: function () {
            var data = [];
            for (var i = 0; i < this.presentboons.length; i++) {
                var boonid = "b" + this.presentboons[i];
                data[i] = this.buffmap[boonid];
            }
            return data;
        },
        offs: function () {
            var data = [];
            for (var i = 0; i < this.presentoffs.length; i++) {
                var boonid = "b" + this.presentoffs[i];
                data[i] = this.buffmap[boonid];
            }
            return data;
        },
        defs: function () {
            var data = [];
            for (var i = 0; i < this.presentdefs.length; i++) {
                var boonid = "b" + this.presentdefs[i];
                data[i] = this.buffmap[boonid];
            }
            return data;
        },
        buffData: function () {
            var _this = this;
            var getData = function (stats, genself, gengroup, genoffgr, gensquad) {
                var uptimes = [],
                    gens = [],
                    gengr = [],
                    genoff = [],
                    gensq = [];
                var avg = [],
                    gravg = [],
                    totalavg = [];
                var grcount = [],
                    totalcount = 0;
                for (var i = 0; i < _this.players.length; i++) {
                    var player = _this.players[i];
                    if (player.isConjure) {
                        continue;
                    }
                    uptimes.push({
                        player: player,
                        data: stats[i]
                    });
                    gens.push({
                        player: player,
                        data: genself[i]
                    });
                    gengr.push({
                        player: player,
                        data: gengroup[i]
                    });
                    genoff.push({
                        player: player,
                        data: genoffgr[i]
                    });
                    gensq.push({
                        player: player,
                        data: gensquad[i]
                    });
                    if (!gravg[player.group]) {
                        gravg[player.group] = [];
                        grcount[player.group] = 0;
                    }
                    totalcount++;
                    grcount[player.group]++;
                    for (var j = 0; j < stats[i].data.length; j++) {
                        totalavg[j] = (totalavg[j] || 0) + stats[i].data[j][0];
                        gravg[player.group][j] = (gravg[player.group][j] || 0) + stats[i].data[j][0];
                    }
                }
                for (var i = 0; i < gravg.length; i++) {
                    if (gravg[i]) {
                        for (var k = 0; k < gravg[i].length; k++) {
                            gravg[i][k] = Math.round(100 * gravg[i][k] / grcount[i]) / 100;
                        }
                        avg.push({
                            name: "Group " + i,
                            data: gravg[i],
                        });
                    }
                }
                for (var k = 0; k < totalavg.length; k++) {
                    totalavg[k] = Math.round(100 * totalavg[k] / totalcount) / 100;
                }
                avg.push({
                    name: "Total",
                    data: totalavg
                });
                return [uptimes, gens, gengr, genoff, gensq, avg];
            };
            var res = {
                boonsData: getData(this.phase.boonStats, this.phase.boonGenSelfStats,
                    this.phase.boonGenGroupStats, this.phase.boonGenOGroupStats, this.phase.boonGenSquadStats),
                offsData: getData(this.phase.offBuffStats, this.phase.offBuffGenSelfStats,
                    this.phase.offBuffGenGroupStats, this.phase.offBuffGenOGroupStats, this.phase.offBuffGenSquadStats),
                defsData: getData(this.phase.defBuffStats, this.phase.defBuffGenSelfStats,
                    this.phase.defBuffGenGroupStats, this.phase.defBuffGenOGroupStats, this.phase.defBuffGenSquadStats)
            };
            return res;
        }
    },
});

Vue.component("dmgmodifier-stats-component", {
    props: ['phases',
        'phase', 'players', 'targets', 'buffmap'
    ],
    data: function () {
        return {
            mode: 0
        };
    },
    computed: {
        modifiers: function () {
            var dmgModifiersCommon = this.phases[0].dmgModifiersCommon;
            if (!dmgModifiersCommon.length) {
                return [];
            }
            var dmgModifier = dmgModifiersCommon[0];
            var buffs = [];
            for (var i = 0; i < dmgModifier.length; i++) {
                var modifier = dmgModifier[i];
                var boonid = 'b' + modifier[0];
                if (this.buffmap[boonid]) {
                    buffs.push(this.buffmap[boonid]);
                }
            }
            return buffs;
        },
        rows: function () {
            var rows = [];
            for (var i = 0; i < this.players.length; i++) {
                var player = this.players[i];
                if (player.isConjure) {
                    continue;
                }
                var dmgModifier = this.mode === 0 ? this.phase.dmgModifiersCommon[i] : this.phase.dmgModifiersTargetsCommon[i];
                var data = [];
                for (var j = 0; j < this.modifiers.length; j++) {
                    data.push([0, 0, 0, 0]);
                }
                if (this.mode === 0) {
                    for (var j = 0; j < dmgModifier.length; j++) {
                        data[j] = dmgModifier[j].slice(1);
                    }
                } else {
                    for (var j = 0; j < this.phase.targets.length; j++) {
                        if (this.targets[this.phase.targets[j]].active) {
                            var modifier = dmgModifier[j];
                            for (var k = 0; k < modifier.length; k++) {
                                var target = modifier[k].slice(1);
                                var curData = data[k];
                                for (var l = 0; l < target.length; l++) {
                                    curData[l] += target[l];
                                }
                                data[k] = curData;
                            }
                        }
                    }
                }
                rows.push({
                    player: player,
                    data: data
                });
            }
            return rows;
        }
    },
    methods: {
        getTooltip: function (item) {
            var hits = item[0] + " out of " + item[1] + " hits";
            var gain = "Pure Damage: " + item[2];
            var damageIncrease = Math.round(100 * 100 * (item[3] / (item[3] - item[2]) - 1.0)) / 100;
            var increase = "Damage Gain: " + (isNaN(damageIncrease) ? "0" : damageIncrease) + "%";
            return hits + "<br>" + gain + "<br>" + increase;
        },
        getCellValue: function (item) {
            var res = Math.round(100 * 100 * item[0] / Math.max(item[1], 1)) / 100;
            return isNaN(res) ? 0 : res;
        }
    },
    mounted() {
        initTable("#dmgmodifier-table", 1, "asc");
    },
    updated() {
        updateTable('#dmgmodifier-table');
    },
});

Vue.component("damagedist-table-component",{
    props: ['dmgdist', 'buffmap', 'skillmap', 'tableid' ],
    template: "#damagedist-table-template",
    mounted() {
        initTable('#'+this.tableid, 2, "desc");
    },
    updated() {       
        updateTable('#'+this.tableid);
    },
    methods: {
        round3: function (value) {
            if (isNaN(value)) {
                return 0;
            }
            var mul = 1000;
            return Math.round(mul * value) / mul;
        }
    },
    computed: {
        rows: function() {
            var res = [];
            var distrib = this.dmgdist.distribution;
            for (var i = 0; i< distrib.length; i++) {
                var data = distrib[i];
                var skill;
                var id = data[1]
                if (data[0] === 1) {
                    id = 'b'+id;
                    skill = this.buffmap[id];
                } else {
                    id = 's'+id;
                    skill = this.skillmap[id];
                }
                res.push({data: data, skill: skill});
            }
            return res;
        }
    }
});

var createLayout = function () {
    var layout = new Layout("Summary");
    // general stats
    var stats = new Tab("General Stats", {
        active: true
    });
    var statsLayout = new Layout(null);
    statsLayout.addTab(
        new Tab("Damage Stats", {
            active: true,
            dataType: DataTypes.damageTable
        })
    );
    statsLayout.addTab(
        new Tab("Gameplay Stats", {
            dataType: DataTypes.gameplayTable
        })
    );
    statsLayout.addTab(
        new Tab("Damage Modifiers Stats", {
            dataType: DataTypes.dmgModifiersTable
        })
    );
    statsLayout.addTab(
        new Tab("Defensive Stats", {
            dataType: DataTypes.defTable
        })
    );
    statsLayout.addTab(
        new Tab("Support Stats", {
            dataType: DataTypes.supTable
        })
    );
    stats.layout = statsLayout;
    layout.addTab(stats);
    // buffs
    var buffs = new Tab("Buffs");
    var buffLayout = new Layout(null);
    buffLayout.addTab(
        new Tab("Boons", {
            active: true,
            dataType: DataTypes.boonTable
        })
    );
    buffLayout.addTab(new Tab("Offensive Buffs", {
        dataType: DataTypes.offensiveBuffTable
    }));
    buffLayout.addTab(new Tab("Defensive Buffs", {
        dataType: DataTypes.defensiveBuffTable
    }));
    buffLayout.addTab(new Tab("Personal Buffs", {
        dataType: DataTypes.personalBuffTable
    }));
    buffs.layout = buffLayout;
    layout.addTab(buffs);
    // mechanics
    var mechanics = new Tab("Mechanics", {
        dataType: DataTypes.mechanicTable
    });
    layout.addTab(mechanics);
    // graphs
    var graphs = new Tab("Graph");
    layout.addTab(graphs);
    // targets
    var targets = new Tab("Targets Summary", {
        dataType: DataTypes.targetTab
    });
    layout.addTab(targets);
    // player
    var player = new Tab("Player Summary", {
        dataType: DataTypes.playerTab
    });
    layout.addTab(player);
    return layout;
};

window.onload = function () {
    var layout = createLayout();
    new Vue({
        el: "#content",
        data: {
            logdata: logData,
            layout: layout,
            datatypes: DataTypes
        },
        computed: {
            phaseData: function () {
                var phases = this.logdata.phases;
                for (var i = 0; i < phases.length; i++) {
                    if (phases[i].active) {
                        return {phase:phases[i], index: i};
                    }
                }
            },
            dataType: function () {
                var cur = this.layout.tabs;
                while (cur !== null) {
                    for (var i = 0; i < cur.length; i++) {
                        var tab = cur[i];
                        if (tab.active) {
                            if (tab.layout === null) {
                                return tab.dataType;
                            } else {
                                cur = tab.layout.tabs;
                                break;
                            }
                        }
                    }
                }
                return -1;
            },
            activePlayer: function() {
                var players = this.logdata.players;
                for (var i = 0; i < players.length; i++) {
                    if (players[i].active) {
                        return players[i];
                    }
                }
                return null;
            }
        },
        beforeMount() {
            var element = document.getElementById("loading");
            element.parentNode.removeChild(element);
        }
    });
    $("body").tooltip({
        selector: "[data-original-title]",
        html: true
    });
};
