/*jshint esversion: 6 */
$.extend($.fn.dataTable.defaults, {
    searching: false,
    ordering: true,
    paging: false,
    dom: "t"
});

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

var DataTypes = {
    damageTable: 0,
    defTable: 1,
    supTable: 2,
    gameplayTable: 3,
    mechanicTable: 4
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
    props: ["targets", "phases"],
    methods: {
        show: function (target) {
            var index = this.targets.indexOf(target);
            var activePhase = null;
            for (var i = 0; i < this.phases.length; i++) {
                if (this.phases[i].active) {
                    activePhase = this.phases[i];
                    break;
                }
            }
            return activePhase.targets.indexOf(index) !== -1;
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
    props: ["layout", "phase", "targets", "players"],
    data: function () {
        return {
            cache: new Map()
        };
    },
    mounted() {
        $(function () {
            $("#dps-table").DataTable({
                order: [
                    [4, "desc"]
                ]
            });
        });
    },
    updated() {
        var table = $("#dps-table");
        var order = table.DataTable().order();
        table.DataTable().destroy();
        table.DataTable().order(order);
        table.DataTable().draw();
    },
    computed: {
        tableData: function () {
            var phase = this.phase;
            //
            /*var activeTargets = [];
                        var id = 0;
                        for (var j = 0; j < phase.targets.length; j++) {
                            if (this.targets[phase.targets[j]].active) {
                                activeTargets.push(phase.targets[j]);
                            }
                        }
                        for (var i = 0; i < activeTargets.length; i++) {
                            id += Math.pow(2, activeTargets[i]);
                        }
                        if (this.cache.has(phase)) {
                            var res = this.cache.get(phase);
                            if (res.has(id)) {
                                return res.get(id);
                            }
                        }*/
            //
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
                if (!groups[player.group])
                    groups[player.group] = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
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
                if (groups[i])
                    sums.push({
                        name: "Group " + i,
                        dps: groups[i]
                    });
            }
            sums.push({
                name: "Total",
                dps: total
            });
            var res = {
                rows: rows,
                sums: sums
            };
            //
            /*if (!this.cache.has(phase)) {
                            this.cache.set(phase, new Map());
                        }
                        this.cache.get(phase).set(id, res);*/
            //
            return res;
        }
    }
});

Vue.component("defense-stats-component", {
    props: ["layout", "phase", "players"],
    data: function () {
        return {
            cache: new Map()
        };
    },
    mounted() {
        $(function () {
            $("#def-table").DataTable({
                order: [
                    [4, "desc"]
                ]
            });
        });
    },
    updated() {
        var table = $("#def-table");
        var order = table.DataTable().order();
        table.DataTable().destroy();
        table.DataTable().order(order);
        table.DataTable().draw();
    },
    computed: {
        tableData: function () {
            //
            /*if (this.cache.has(this.phase)) {
                            return this.cache.get(this.phase);
                        }*/
            //
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
                if (!groups[player.group]) groups[player.group] = [0, 0, 0, 0, 0, 0, 0];
                for (var j = 0; j < total.length; j++) {
                    total[j] += def[j];
                    groups[player.group][j] += def[j];
                }
            }
            for (i = 0; i < groups.length; i++) {
                if (groups[i])
                    sums.push({
                        name: "Group " + i,
                        def: groups[i]
                    });
            }
            sums.push({
                name: "Total",
                def: total
            });
            var res = {
                rows: rows,
                sums: sums
            };
            //
            //this.cache.set(this.phase, res);
            //
            return res;
        }
    }
});

Vue.component("support-stats-component", {
    props: ["layout", "phase", "players"],
    data: function () {
        return {
            cache: new Map()
        };
    },
    mounted() {
        $(function () {
            $("#sup-table").DataTable({
                order: [
                    [4, "desc"]
                ]
            });
        });
    },
    updated() {
        var table = $("#sup-table");
        var order = table.DataTable().order();
        table.DataTable().destroy();
        table.DataTable().order(order);
        table.DataTable().draw();
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
            //
            /*if (this.cache.has(this.phase)) {
                            return this.cache.get(this.phase);
                        }*/
            //
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
                if (!groups[player.group]) groups[player.group] = [0, 0, 0, 0];
                for (var j = 0; j < sup.length; j++) {
                    total[j] += sup[j];
                    groups[player.group][j] += sup[j];
                }
            }
            for (i = 0; i < groups.length; i++) {
                if (groups[i])
                    sums.push({
                        name: "Group " + i,
                        sup: groups[i]
                    });
            }
            sums.push({
                name: "Total",
                sup: total
            });
            var res = {
                rows: rows,
                sums: sums
            };
            //
            //this.cache.set(this.phase, res);
            //
            return res;
        }
    }
});

Vue.component("gameplay-stats-component", {
    props: ["layout", "phase", "targets", "players"],
    data: function () {
        return {
            mode: 0,
            cache: new Map()
        };
    },
    mounted() {
        $(function () {
            $("#dmg-table").DataTable({
                order: [
                    [4, "desc"]
                ]
            });
        });
    },
    updated() {
        var table = $("#dmg-table");
        var order = table.DataTable().order();
        table.DataTable().destroy();
        table.DataTable().order(order);
        table.DataTable().draw();
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
            //
            /*var activeTargets = null;
                        var id = 0;
                        if (this.mode) {
                            activeTargets = [];
                            for (var j = 0; j < phase.targets.length; j++) {
                                if (this.targets[phase.targets[j]].active) {
                                    activeTargets.push(phase.targets[j]);
                                }
                            }
                        }
                        if (activeTargets) {
                            for (var i = 0; i < activeTargets.length; i++) {
                                id += Math.pow(2, activeTargets[i]);
                            }
                        }
                        if (this.cache.has(phase)) {
                            var res = this.cache.get(phase);
                            if (res.has(this.mode)) {
                                res = res.get(this.mode);
                                if (res.has(id)) {
                                    return res.get(id);
                                }
                            }
                        }*/
            //
            var rows = [];
            for (var i = 0; i < phase.dmgStats.length; i++) {
                var commons = [];
                var data = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
                var player = this.players[i];
                if (player.isConjure) continue;
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
            //
            /*var cache = this.cache;
                        if (!cache.has(phase)) {
                            cache.set(phase, new Map());
                        }
                        cache = cache.get(phase);
                        if (!cache.has(this.mode)) {
                            cache.set(this.mode, new Map());
                        }
                        cache = cache.get(this.mode);
                        cache.set(id, res);*/
            //
            return res;
        }
    }
});
Vue.component("mechanics-stats-component", {
    props: ["phase", "players", "enemies", "mechanics"],
    mounted() {
        $("#playermechs").DataTable({
            order: [
                [3, "desc"]
            ]
        });
        var enemyTable = $("#enemymechs");
        if (enemyTable.length > 0) {
            enemyTable.DataTable({
                order: [
                    [0, "desc"]
                ]
            });
        }
    },
    updated() {
        var table = $("#playermechs");
        var order = table.DataTable().order();
        table.DataTable().destroy();
        table.DataTable().order(order);
        table.DataTable().draw();
        table = $("#enemymechs");
        if (table.length > 0) {
            order = table.DataTable().order();
            table.DataTable().destroy();
            table.DataTable().order(order);
            table.DataTable().draw();
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
                    player: enemy.name,
                    mechs: phase.enemyMechanicStats[i]
                });
            }
            return rows;
        }
    }
});

Vue.component("buff-table-component", {
    props: ["buffs", "playerdata" , "players", "generation", "condition", "target"],
    template: `
        <div>
            <table>
                <thead>
                    <tr>
                        <th>Sub</th>
                        <th></th>
                        <th>Name</th>
                        <th v-for="buff in buffs" :data-original-title="buff.name">
                            <img :src="buff.icon" :alt="buff.name" class="icon icon-hover">
                        </th>
                    </tr>
                </thead>
                <tbody>                
                    <tr v-for="row in rows">
                        <td>{{ row.player.group }}</td>
                        <td :data-original-title="row.player.profession"><img :src="row.player.icon" :alt="row.player.profession" class="icon">
                            <span style="display:none">
                                {{ row.player.profession }}
                            </span>
                        </td>
                        <td class="text-left" :data-original-title="getAvgTooltip(row.data.avg)">
                            {{ row.player.name }}
                        </td>           
                        <td v-for=" {buff, index} in buffs" :data-original-title="getCellTooltip(buff, row.data.val[index])">
                            {{ getCellValue(row.data.val[index], buff) }}
                        </td>
                    </tr>
                </tbody>
                <tfoot v-if="players.length > 0">
                    <tr v-for="sum in sums">                                   
                        <td></td>
                        <td></td>
                        <td :data-original-title="getAvgTooltip(sum.data.avg)">{{sum.data.name}}</td>                       
                        <td v-for=" {buff, index} in buffs" :data-original-title="getCellTooltip(buff, row.data.val[index])">
                            {{ getCellValue(row.data.val[index], buff) }}
                        </td>
                    </tr>
                </tfoot>
            </table>
        </div>
    `,
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
            if (this.generation && val[0] > 0) {
                return val[1] + (buff.stacking ? "%" : " ") + " with overstack";
            } else if (buff.stacking && val[1] > 0){
                return "Uptime: " + val[1] + "%";
            }
            return false;
        },
        getCellValue: function (buff, val) {
            if (val[0] > 0) {
                var res = val[0];
                return buff.stacking ? res : res + "%";
            }
            return "-";
        }
    }
});

Vue.component("personal-buff-table-component", {});

Vue.component("buff-stats-component", {});

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
            active: true
        })
    );
    buffLayout.addTab(new Tab("Offensive Buffs"));
    buffLayout.addTab(new Tab("Defensive Buffs"));
    buffLayout.addTab(new Tab("Personal Buffs"));
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
    var targets = new Tab("Targets");
    layout.addTab(targets);
    // player
    var player = new Tab("Selected Player");
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
            phase: function () {
                var phases = this.logdata.phases;
                for (var i = 0; i < phases.length; i++) {
                    if (phases[i].active) return phases[i];
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
            }
        }
    });
    var element = document.getElementById("loading");
    element.parentNode.removeChild(element);
    $("body").tooltip({
        selector: "[data-original-title]",
        html: true
    });
};
