/*jshint esversion: 6 */
$.extend($.fn.dataTable.defaults, {
    searching: false,
    ordering: true,
    paging: false,
    retrieve: true,
    dom: "t"
});

// polyfill for shallow copies
// https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Object/assign
if (typeof Object.assign != 'function') {
    // Must be writable: true, enumerable: false, configurable: true
    Object.defineProperty(Object, "assign", {
        value: function assign(target, varArgs) { // .length of function is 2
            'use strict';
            if (target == null) { // TypeError if undefined or null
                throw new TypeError('Cannot convert undefined or null to object');
            }

            var to = Object(target);

            for (var index = 1; index < arguments.length; index++) {
                var nextSource = arguments[index];

                if (nextSource != null) { // Skip over if undefined or null
                    for (var nextKey in nextSource) {
                        // Avoid bugs when hasOwnProperty is shadowed
                        if (Object.prototype.hasOwnProperty.call(nextSource, nextKey)) {
                            to[nextKey] = nextSource[nextKey];
                        }
                    }
                }
            }
            return to;
        },
        writable: true,
        configurable: true
    });
}

var specs = [
    "Warrior", "Berserker", "Spellbreaker", "Revenant", "Herald", "Renegade", "Guardian", "Dragonhunter", "Firebrand",
    "Ranger", "Druid", "Soulbeast", "Engineer", "Scrapper", "Holosmith", "Thief", "Daredevil", "Deadeye",
    "Mesmer", "Chronomancer", "Mirage", "Necromancer", "Reaper", "Scourge", "Elementalist", "Tempest", "Weaver"
];

/*var lazyTableUpdater = null;
if ('IntersectionObserver' in window) {
    lazyTableUpdater = new IntersectionObserver(function (entries, observer) {
        entries.forEach(function (entry) {
            if (entry.isIntersecting) {
                var id = entry.target.id;
                var table = $("#" + id);
                if ($.fn.dataTable.isDataTable(table)) {
                    table.DataTable().rows().invalidate('dom').draw();
                }
                observer.unobserve(entry.target);
            }
        });
    });
}*/

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

function findSkill(isBuff, id) {
    var skill;
    if (isBuff) {
        skill = logData.buffMap['b' + id] || {};
    } else {
        skill = logData.skillMap["s" + id] || {};
    }
    skill.condi = isBuff;
    return skill;
}

function getMechanics() {
    return logData.mechanics;
}

var initTable = function (id, cell, order, orderCallBack) {
    var table = $(id);
    if (!table.length) {
        return;
    }
    /*if (lazyTableUpdater) {
        var lazyTable = document.querySelector(id);
        var lazyTableObserver = new IntersectionObserver(function (entries, observer) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    table.DataTable({
                        order: [
                            [cell, order]
                        ]
                    });
                    if (orderCallBack) {
                        table.DataTable().on('order.dt', orderCallBack);
                    }
                    observer.unobserve(entry.target);
                }
            });
        });
        lazyTableObserver.observe(lazyTable);
    } else {*/
    table.DataTable({
        order: [
            [cell, order]
        ]
    });
    if (orderCallBack) {
        table.DataTable().on('order.dt', orderCallBack);
    }
    //}
};

var updateTable = function (id) {
    /*if (lazyTableUpdater) {
        var lazyTable = document.querySelector(id);
        lazyTableUpdater.unobserve(lazyTable);
        lazyTableUpdater.observe(lazyTable);
    } else {*/
    var table = $(id);
    if ($.fn.dataTable.isDataTable(id)) {
        table.DataTable().rows().invalidate('dom');
        table.DataTable().draw();
    }
    //}
};

var roundingComponent = {
    methods: {
        round: function (value) {
            if (isNaN(value)) {
                return 0;
            }
            return Math.round(value);
        },
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
    }
};

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
    dpsGraph: 12
};

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

var compileCommons = function () {
    Vue.component("graph-component", {
        props: ['id', 'layout', 'data'],
        template: '<div :id="id"></div>',
        mounted: function () {
            var div = document.querySelector(this.queryID);
            Plotly.react(div, this.data, this.layout);
        },
        computed: {
            queryID: function () {
                return "#" + this.id;
            }
        },
        watch: {
            layout: {
                handler: function () {
                    var div = document.querySelector(this.queryID);
                    var duration = 500;
                    Plotly.animate(div, { data: this.data }, {
                        transition: {
                            duration: duration,
                            easing: 'cubic-in-out'
                        },
                        frame: {
                            duration: duration
                        }
                    });
                    var _this = this;
                    setTimeout(function () { Plotly.relayout(div, _this.layout); }, 1.5 * duration);
                },
                deep: true
            }
        }
    });
    Vue.component("buff-table-component", {
        props: ["buffs", "playerdata", "generation", "condition", "sums", "id"],
        template: "#tmplBuffTable",
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
            getCellTooltip: function (buff, val, uptime) {
                if (val instanceof Array) {
                    if (!uptime && this.generation && val[0] > 0) {
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
            initTable("#" + this.id, 0, "asc");
        },
        updated() {
            updateTable("#" + this.id);
        }
    });

    Vue.component("damagedist-table-component", {
        props: ["dmgdist", "tableid", "actor", "isminion", "istarget"],
        template: "#tmplDamageDistTable",
        data: function () {
            return {
                sortdata: {
                    order: "desc",
                    index: 2
                }
            };
        },
        mixins: [roundingComponent],
        mounted() {
            var _this = this;
            initTable(
                "#" + this.tableid,
                this.sortdata.index,
                this.sortdata.order,
                function () {
                    var order = $("#" + _this.tableid)
                        .DataTable()
                        .order();
                    _this.sortdata.order = order[0][1];
                    _this.sortdata.index = order[0][0];
                }
            );
        },
        beforeUpdate() {
            $("#" + this.tableid)
                .DataTable()
                .destroy();
        },
        updated() {
            var _this = this;
            initTable(
                "#" + this.tableid,
                this.sortdata.index,
                this.sortdata.order,
                function () {
                    var order = $("#" + _this.tableid)
                        .DataTable()
                        .order();
                    _this.sortdata.order = order[0][1];
                    _this.sortdata.index = order[0][0];
                }
            );
        },
        methods: {
            getSkill: function (isBoon, id) {
                return findSkill(isBoon, id);
            }
        }
    });
};

var compileHeader = function () {
    Vue.component("encounter-component", {
        props: ["logdata"],
        template: "#tmplEncounter",
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
        template: "#tmplPhase",
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
        template: "#tmplTargets",
        methods: {
            show: function (target) {
                var index = this.targets.indexOf(target);
                return this.phase.targets.indexOf(index) !== -1;
            }
        }
    });

    Vue.component("player-component", {
        props: ["players"],
        template: "#tmplPlayers",
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
};

var compileGeneralStats = function () {
    Vue.component("damage-stats-component", {
        props: ["phase", "activetargets", "players", "phaseindex"],
        template: "#tmplDamageTable",
        data: function () {
            return {
                cacheTarget: new Map()
            };
        },
        mounted() {
            initTable("#dps-table", 4, "desc");
        },
        updated() {
            updateTable("#dps-table");
        },
        computed: {
            tableData: function () {
                var cacheID = this.phaseindex + '-';
                var targetsID = 1;
                var i;
                for (i = 0; i < this.activetargets.length; i++) {
                    var target = this.activetargets[i];
                    targetsID = targetsID << (target.id + 1);
                }
                cacheID += targetsID;
                if (this.cacheTarget.has(cacheID)) {
                    return this.cacheTarget.get(cacheID);
                }
                var phase = this.phase;
                var rows = [];
                var sums = [];
                var total = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
                var groups = [];
                var j;
                for (i = 0; i < phase.dpsStats.length; i++) {
                    var dpsStat = phase.dpsStats[i];
                    var dpsTargetStat = [0, 0, 0, 0, 0, 0];
                    for (j = 0; j < this.activetargets.length; j++) {
                        var tar = phase.dpsStatsTargets[i][this.activetargets[j].id];
                        for (var k = 0; k < dpsTargetStat.length; k++) {
                            dpsTargetStat[k] += tar[k];
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
                this.cacheTarget.set(cacheID, res);
                return res;
            }
        }
    });

    Vue.component("defense-stats-component", {
        props: ["phase", "players"],
        template: "#tmplDefenseTable",
        data: function () {
            return {
                cache: new Map()
            };
        },
        mounted() {
            initTable("#def-table", 4, "desc");
        },
        updated() {
            updateTable("#def-table");
        },
        computed: {
            tableData: function () {
                if (this.cache.has(this.phase)) {
                    return this.cache.get(this.phase);
                }
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
                this.cache.set(this.phase, res);
                return res;
            }
        }
    });

    Vue.component("support-stats-component", {
        props: ["phase", "players"],
        template: "#tmplSupportTable",
        data: function () {
            return {
                cache: new Map()
            };
        },
        mixins: [roundingComponent],
        mounted() {
            initTable("#sup-table", 4, "desc");
        },
        updated() {
            updateTable("#sup-table");
        },
        computed: {
            tableData: function () {
                if (this.cache.has(this.phase)) {
                    return this.cache.get(this.phase);
                }
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
                this.cache.set(this.phase, res);
                return res;
            }
        }
    });

    Vue.component("gameplay-stats-component", {
        props: ["phase", "activetargets", "players", "phaseindex"],
        template: "#tmplGameplayTable",
        mixins: [roundingComponent],
        data: function () {
            return {
                mode: 0,
                cache: new Map(),
                cacheTarget: new Map()
            };
        },
        mounted() {
            initTable("#dmg-table", 4, "desc");
        },
        updated() {
            updateTable("#dmg-table");
        },
        computed: {
            tableData: function () {
                if (this.cache.has(this.phase)) {
                    return this.cache.get(this.phase);
                }
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
                            data[j] = stats[j];
                        }
                    }
                    rows.push({
                        player: player,
                        commons: commons,
                        data: data
                    });
                }
                this.cache.set(this.phase, rows);
                return rows;
            },
            tableDataTarget: function () {
                var cacheID = this.phaseindex + '-';
                var targetsID = 1;
                var i;
                for (i = 0; i < this.activetargets.length; i++) {
                    var target = this.activetargets[i];
                    targetsID = targetsID << (target.id + 1);
                }
                cacheID += targetsID;
                if (this.cacheTarget.has(cacheID)) {
                    return this.cacheTarget.get(cacheID);
                }
                var phase = this.phase;
                var rows = [];
                for (i = 0; i < phase.dmgStats.length; i++) {
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
                            for (var k = 0; k < this.activetargets.length; k++) {
                                var tar = phase.dmgStatsTargets[i][this.activetargets[k].id];
                                data[j] += tar[j];
                            }
                        }
                    }
                    rows.push({
                        player: player,
                        commons: commons,
                        data: data
                    });
                }
                this.cacheTarget.set(cacheID, rows);
                return rows;
            }
        }
    });
    Vue.component("dmgmodifier-stats-component", {
        props: ['phases', 'phaseindex',
            'phase', 'players', 'activetargets'
        ],
        template: "#tmplDamageModifierTable",
        data: function () {
            return {
                mode: 0,
                cache: new Map(),
                cacheTarget: new Map()
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
                    buffs.push(findSkill(true, modifier[0]));
                }
                return buffs;
            },
            rows: function () {
                if (this.cache.has(this.phase)) {
                    return this.cache.get(this.phase);
                }
                var rows = [];
                var j;
                for (var i = 0; i < this.players.length; i++) {
                    var player = this.players[i];
                    if (player.isConjure) {
                        continue;
                    }
                    var dmgModifier = this.phase.dmgModifiersCommon[i];
                    var data = [];
                    for (j = 0; j < this.modifiers.length; j++) {
                        data.push([0, 0, 0, 0]);
                    }
                    for (j = 0; j < dmgModifier.length; j++) {
                        data[j] = dmgModifier[j].slice(1);
                    }
                    rows.push({
                        player: player,
                        data: data
                    });
                }
                this.cache.set(this.phase, rows);
                return rows;
            },
            rowsTarget: function () {
                var cacheID = this.phaseindex + '-';
                var targetsID = 1;
                var i;
                for (i = 0; i < this.activetargets.length; i++) {
                    var target = this.activetargets[i];
                    targetsID = targetsID << (target.id + 1);
                }
                cacheID += targetsID;
                if (this.cacheTarget.has(cacheID)) {
                    return this.cacheTarget.get(cacheID);
                }
                var rows = [];
                var j;
                for (i = 0; i < this.players.length; i++) {
                    var player = this.players[i];
                    if (player.isConjure) {
                        continue;
                    }
                    var dmgModifier = this.phase.dmgModifiersTargetsCommon[i];
                    var data = [];
                    for (j = 0; j < this.modifiers.length; j++) {
                        data.push([0, 0, 0, 0]);
                    }
                    for (j = 0; j < this.activetargets.length; j++) {
                        var modifier = dmgModifier[this.activetargets[j].id];
                        for (var k = 0; k < modifier.length; k++) {
                            var targetData = modifier[k].slice(1);
                            var curData = data[k];
                            for (var l = 0; l < targetData.length; l++) {
                                curData[l] += targetData[l];
                            }
                            data[k] = curData;
                        }
                    }
                    rows.push({
                        player: player,
                        data: data
                    });
                }
                this.cacheTarget.set(cacheID, rows);
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
            initTable("#dmgmodifier-table", 0, "asc");
        },
        updated() {
            updateTable('#dmgmodifier-table');
        },
    });
};

var compileBuffStats = function () {
    Vue.component("personal-buff-table-component", {
        props: ['phase', 'persbuffs', 'players'],
        template: "#tmplPersonalBuffTable",
        data: function () {
            return {
                specs: specs,
                bases: [],
                specToBase: specToBase,
                mode: "Warrior",
                cache: new Map()
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
                if (this.cache.has(this.phase)) {
                    return this.cache.get(this.phase);
                }
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
                this.cache.set(this.phase, res);
                return res;
            },
            buffs: function () {
                var res = [];
                for (var i = 0; i < this.orderedSpecs.length; i++) {
                    var spec = this.orderedSpecs[i];
                    var data = [];
                    for (var j = 0; j < this.persbuffs[spec.name].length; j++) {
                        data.push(findSkill(true, this.persbuffs[spec.name][j]));
                    }
                    res.push(data);
                }
                return res;
            }
        }
    });

    Vue.component("buff-stats-component", {
        props: ['datatypes', 'datatype', 'phase', 'players', 'presentboons', 'presentoffs', 'presentdefs'],
        template: "#tmplBuffStats",
        data: function () {
            return {
                mode: 0,
                cache: new Map()
            };
        },
        computed: {
            boons: function () {
                var data = [];
                for (var i = 0; i < this.presentboons.length; i++) {
                    data[i] = findSkill(true, this.presentboons[i]);
                }
                return data;
            },
            offs: function () {
                var data = [];
                for (var i = 0; i < this.presentoffs.length; i++) {
                    data[i] = findSkill(true, this.presentoffs[i]);
                }
                return data;
            },
            defs: function () {
                var data = [];
                for (var i = 0; i < this.presentdefs.length; i++) {
                    data[i] = findSkill(true, this.presentdefs[i]);
                }
                return data;
            },
            buffData: function () {
                if (this.cache.has(this.phase)) {
                    return this.cache.get(this.phase);
                }
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
                    var i, k;
                    for (i = 0; i < _this.players.length; i++) {
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
                    for (i = 0; i < gravg.length; i++) {
                        if (gravg[i]) {
                            for (k = 0; k < gravg[i].length; k++) {
                                gravg[i][k] = Math.round(100 * gravg[i][k] / grcount[i]) / 100;
                            }
                            avg.push({
                                name: "Group " + i,
                                data: gravg[i],
                            });
                        }
                    }
                    for (k = 0; k < totalavg.length; k++) {
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
                this.cache.set(this.phase, res);
                return res;
            }
        },
    });
};

var compilePlayerTab = function () {

    // Base stuff
    Vue.component('dmgdist-player-component', {
        props: ['player', 'playerindex', 'phase',
            'phaseindex', 'activetargets', 'datatype'
        ],
        template: "#tmplDamageDistPlayer",
        data: function () {
            return {
                distmode: -1,
                targetmode: 0,
                cacheTarget: new Map()
            };
        },
        computed: {
            actor: function () {
                if (this.distmode === -1) {
                    return this.player;
                }
                return this.player.minions[this.distmode];
            },
            dmgdist: function () {
                if (this.distmode === -1) {
                    return this.player.details.dmgDistributions[this.phaseindex];
                }
                return this.player.details.minions[this.distmode].dmgDistributions[this.phaseindex];
            },
            dmgdisttarget: function () {
                var cacheID = this.phaseindex + '-' + this.distmode + '-';
                var targetsID = 1;
                var i;
                for (i = 0; i < this.activetargets.length; i++) {
                    var target = this.activetargets[i];
                    targetsID = targetsID << (target.id + 1);
                }
                cacheID += targetsID;
                if (this.cacheTarget.has(cacheID)) {
                    return this.cacheTarget.get(cacheID);
                }
                var dist = {
                    contributedDamage: 0,
                    totalDamage: 0,
                    distribution: [],
                };
                var rows = new Map();
                for (i = 0; i < this.activetargets.length; i++) {
                    var targetid = this.activetargets[i].id;
                    var targetDist = this.distmode === -1 ?
                        this.player.details.dmgDistributionsTargets[this.phaseindex][targetid] :
                        this.player.details.minions[this.distmode].dmgDistributionsTargets[this.phaseindex][targetid];
                    dist.contributedDamage += targetDist.contributedDamage;
                    dist.totalDamage += targetDist.totalDamage;
                    var distribution = targetDist.distribution;
                    for (var k = 0; k < distribution.length; k++) {
                        var targetDistribution = distribution[k];
                        if (rows.has(targetDistribution[1])) {
                            var row = rows.get(targetDistribution[1]);
                            row[2] += targetDistribution[2];
                            if (row[3] < 0) {
                                row[3] = targetDistribution[3];
                            } else if (targetDistribution[3] >= 0) {
                                row[3] = Math.min(targetDistribution[3], row[3]);
                            }
                            row[4] = Math.max(targetDistribution[4], row[4]);
                            row[6] += targetDistribution[6];
                            row[7] += targetDistribution[7];
                            row[8] += targetDistribution[8];
                            row[9] += targetDistribution[9];
                        } else {
                            rows.set(targetDistribution[1], targetDistribution.slice(0));
                        }

                    }
                }
                rows.forEach(function (value, key, map) {
                    dist.distribution.push(value);
                });
                dist.contributedDamage = Math.max(dist.contributedDamage, 0);
                dist.totalDamage = Math.max(dist.totalDamage, 0);
                this.cacheTarget.set(cacheID, dist);
                return dist;
            }
        },
    });

    Vue.component('dmgtaken-player-component', {
        props: ['player', 'playerindex', 'datatype',
            'phaseindex'
        ],
        template: "#tmplDamageTakenPlayer",
        computed: {
            dmgtaken: function () {
                return this.player.details.dmgDistributionsTaken[this.phaseindex];
            }
        },
    });

    Vue.component("food-component", {
        props: ["food", "phase"],
        template: "#tmplFood",
        data: function () {
            return {
                cache: new Map()
            };
        },
        computed: {
            data: function () {
                if (this.cache.has(this.phase)) {
                    return this.cache.get(this.phase);
                }
                var res = {
                    start: [],
                    refreshed: []
                };
                for (var k = 0; k < this.food.length; k++) {
                    var foodData = this.food[k];
                    if (!foodData.name) {
                        var skill = findSkill(true, foodData.id);
                        foodData.name = skill.name;
                        foodData.icon = skill.icon;
                    }
                    if (foodData.time >= this.phase.start && foodData.time <= this.phase.end) {
                        if (foodData.time === 0) {
                            res.start.push(foodData);
                        } else {
                            res.refreshed.push(foodData);
                        }
                    }
                }
                this.cache.set(this.phase, res);
                return res;
            }
        }
    });

    Vue.component("simplerotation-component", {
        props: ["rotation"],
        template: "#tmplSimpleRotation",
        data: function () {
            return {
                autoattack: true,
                small: false
            };
        },
        methods: {
            getSkill: function (id) {
                return findSkill(false, id);
            }
        }
    });

    Vue.component("deathrecap-component", {
        props: ["recaps", "playerindex", "phase"],
        template: "#tmplDeathRecap",
        computed: {
            data: function () {
                var res = {
                    totalSeconds: {
                        down: [],
                        kill: []
                    },
                    totalDamage: {
                        down: [],
                        kill: []
                    },
                    data: [],
                    layout: {}
                };
                for (var i = 0; i < this.recaps.length; i++) {
                    var recap = this.recaps[i];
                    var j, totalSec, totalDamage;
                    if (recap.toDown !== null) {
                        totalSec = (recap.toDown[0][0] - recap.toDown[recap.toDown.length - 1][0]) / 1000;
                        totalDamage = 0;
                        for (j = 0; j < recap.toDown.length; j++) {
                            totalDamage += recap.toDown[j][2];
                        }
                        res.totalSeconds.down[i] = totalSec;
                        res.totalDamage.down[i] = totalDamage;
                    }
                    if (recap.toKill !== null) {
                        totalSec = (recap.toKill[0][0] - recap.toKill[recap.toKill.length - 1][0]) / 1000;
                        totalDamage = 0;
                        for (j = 0; j < recap.toKill.length; j++) {
                            totalDamage += recap.toKill[j][2];
                        }
                        res.totalSeconds.kill[i] = totalSec;
                        res.totalDamage.kill[i] = totalDamage;
                    }
                }
                return res;
            }
        }
    });
    // tab
    Vue.component('player-tab-component', {
        props: ['player', 'playerindex', 'phase',
            'phaseindex', 'activetargets', 'datatype'
        ],
        template: "#tmplPlayerTab",
        data: function () {
            return {
                mode: 0
            };
        },
    });
    // stats
    Vue.component("player-stats-component", {
        props: ["players", "phaseindex", "phase", 'activetargets', 'datatype', 'activeplayer'],
        template: "#tmplPlayerStats",
    });
};

var compileTargetTab = function () {
    // base
    Vue.component("buff-stats-target-component", {
        props: ['target', 'phase', 'players', 'boons', 'conditions', 'targetindex'],
        template: "#tmplBuffStatsTarget",
        data: function () {
            return {
                cacheCondi: new Map(),
                cacheCondiSums: new Map(),
                cacheBoon: new Map()
            };
        },
        computed: {
            targetPhaseIndex: function () {
                return this.phase.targets.indexOf(this.targetindex);
            },
            hasBoons: function () {
                return this.phase.targetsBoonTotals[this.targetPhaseIndex];
            },
            condiData: function () {
                if (this.cacheCondi.has(this.phase)) {
                    return this.cacheCondi.get(this.phase);
                }
                var res = [];
                var i;
                if (this.targetPhaseIndex === -1) {
                    for (i = 0; i < this.players.length; i++) {
                        res.push({
                            player: this.players[i],
                            data: {
                                avg: 0.0,
                                data: []
                            }
                        });
                    }
                } else {
                    for (i = 0; i < this.players.length; i++) {
                        res.push({
                            player: this.players[i],
                            data: this.phase.targetsCondiStats[this.targetPhaseIndex][i]
                        });
                    }
                }
                this.cacheCondi.set(this.phase, res);
                return res;
            },
            condiSums: function () {
                if (this.cacheCondiSums.has(this.phase)) {
                    return this.cacheCondiSums.get(this.phase);
                }
                var res = [];
                if (this.targetPhaseIndex === -1) {
                    res.push({
                        icon: this.target.icon,
                        name: this.target.name,
                        avg: 0,
                        data: []
                    });
                } else {
                    var targetData = this.phase.targetsCondiTotals[this.targetPhaseIndex];
                    res.push({
                        icon: this.target.icon,
                        name: this.target.name,
                        avg: targetData.avg,
                        data: targetData.data
                    });
                }
                this.cacheCondiSums.set(this.phase, res);
                return res;
            },
            boonData: function () {
                if (this.cacheBoon.has(this.phase)) {
                    return this.cacheBoon.get(this.phase);
                }
                var res = [];
                if (this.targetPhaseIndex === -1 || !this.hasBoons) {
                    res.push({
                        player: this.target,
                        data: {
                            avg: 0.0,
                            data: []
                        }
                    });
                } else {
                    var targetData = this.phase.targetsBoonTotals[this.targetPhaseIndex];
                    res.push({
                        player: this.target,
                        data: targetData
                    });
                }
                this.cacheBoon.set(this.phase, res);
                return res;
            }
        }
    });

    Vue.component('dmgdist-target-component', {
        props: ['target', 'targetindex',
            'phaseindex'
        ],
        template: "#tmplDamageDistTarget",
        data: function () {
            return {
                distmode: -1
            };
        },
        computed: {
            actor: function () {
                if (this.distmode === -1) {
                    return this.target;
                }
                return this.target.minions[this.distmode];
            },
            dmgdist: function () {
                if (this.distmode === -1) {
                    return this.target.details.dmgDistributions[this.phaseindex];
                }
                return this.target.details.minions[this.distmode].dmgDistributions[this.phaseindex];
            }
        },
    });
    // tab
    Vue.component("target-tab-component", {
        props: ["target", "phaseindex", "players", "phase", "boons", "conditions", 'targetindex'],
        template: "#tmplTargetTab",
        data: function () {
            return {
                mode: 0
            };
        }
    });
    // stats
    Vue.component("target-stats-component", {
        props: ["players", "targets", "phase", "phaseindex", "presentboons", "presentconditions"],
        template: "#tmplTargetStats",
        computed: {
            phaseTargets: function () {
                var res = [];
                for (var i = 0; i < this.phase.targets.length; i++) {
                    var tar = this.targets[this.phase.targets[i]];
                    res.push(tar);
                }
                if (!this.phase.focus) {
                    this.phase.focus = res[0] || null;
                }
                return res;
            },
            boons: function () {
                var data = [];
                for (var i = 0; i < this.presentboons.length; i++) {
                    data[i] = findSkill(true, this.presentboons[i]);
                }
                return data;
            },
            conditions: function () {
                var data = [];
                for (var i = 0; i < this.presentconditions.length; i++) {
                    data[i] = findSkill(true, this.presentconditions[i]);
                }
                return data;
            }
        }
    });
};

var compileMechanics = function () {
    Vue.component("mechanics-stats-component", {
        props: ["phase", "players", "enemies"],
        template: "#tmplMechanicsTable",
        data: function () {
            return {
                cacheP: new Map(),
                cacheE: new Map()
            };
        },
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
                var mechanics = getMechanics();
                var playerMechanics = [];
                for (var i = 0; i < mechanics.length; i++) {
                    if (mechanics[i].playerMech) {
                        playerMechanics.push(mechanics[i]);
                    }
                }
                return playerMechanics;
            },
            playerMechRows: function () {
                if (this.cacheP.has(this.phase)) {
                    return this.cacheP.get(this.phase);
                }
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
                this.cacheP.set(this.phase, rows);
                return rows;
            },
            enemyMechHeader: function () {
                var mechanics = getMechanics();
                var enemyMechanics = [];
                for (var i = 0; i < mechanics.length; i++) {
                    if (mechanics[i].enemyMech) {
                        enemyMechanics.push(mechanics[i]);
                    }
                }
                return enemyMechanics;
            },
            enemyMechRows: function () {
                if (this.cacheE.has(this.phase)) {
                    return this.cacheE.get(this.phase);
                }
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
                this.cacheE.set(this.phase, rows);
                return rows;
            }
        }
    });
};

var compileGraphs = function () {
    Vue.component("graph-stats-component", {
        props: ["phases", "activetargets", "targets", "players", 'graphdata', "phaseid"],
        template: "#tmplGraphStats",
        data: function () {
            return {
                mode: 0
            };
        }
    });
    Vue.component("dps-graph-component", {
        props: ["phases", "activetargets", "targets", "players", 'mechanics', 'graph', 'mode', 'phase', 'phaseid'],
        template: "#tmplDPSGraph",
        data: function () {
            return {
                dpsmode: 0,
                layout: {},
                data: [],
                dpsCache: new Map(),
                dataCache: new Map(),
            };
        },
        created: function () {
            // layout - constant during whole lifetime
            var i, j;
            this.layout = {
                yaxis: {
                    title: 'DPS',
                    fixedrange: false,
                    rangemode: 'tozero',
                    color: '#cccccc'
                },
                xaxis: {
                    title: 'Time(sec)',
                    color: '#cccccc',
                    xrangeslider: {}
                },
                hovermode: 'compare',
                legend: {
                    orientation: 'h',
                    font: {
                        size: 15
                    }
                },
                font: {
                    color: '#cccccc'
                },
                paper_bgcolor: 'rgba(0,0,0,0)',
                plot_bgcolor: 'rgba(0,0,0,0)',
                displayModeBar: false,
                shapes: [],
                annotations: [],
                autosize: false,
                width: 1100,
                height: 1000,
                datarevision: new Date().getTime(),
            };
            if (this.phase.markupAreas) {
                for (i = 0; i < this.phase.markupAreas.length; i++) {
                    var area = this.phase.markupAreas[i];
                    if (area.label) {
                        this.layout.annotations.push({
                            x: (area.end + area.start) / 2,
                            y: 1,
                            xref: 'x',
                            yref: 'paper',
                            xanchor: 'center',
                            yanchor: 'bottom',
                            text: area.label + '<br>' + '(' + Math.round(1000 * (area.end - area.start)) / 1000 + ' s)',
                            showarrow: false
                        });
                    }
                    if (area.highlight) {
                        this.layout.shapes.push({
                            type: 'rect',
                            xref: 'x',
                            yref: 'paper',
                            x0: area.start,
                            y0: 0,
                            x1: area.end,
                            y1: 1,
                            fillcolor: '#808080',
                            opacity: 0.125,
                            line: {
                                width: 0
                            }
                        });
                    }
                }
            }
            if (this.phase.markupLines) {
                for (i = 0; i < this.phase.markupLines.length; i++) {
                    var x = this.phase.markupLines[i];
                    this.layout.shapes.push({
                        type: 'line',
                        xref: 'x',
                        yref: 'paper',
                        x0: x,
                        y0: 0,
                        x1: x,
                        y1: 1,
                        opacity: 0.35,
                        line: {
                            color: '#00c0ff',
                            width: 2,
                            dash: 'dash'
                        }
                    });
                }
            }
            // constant part of data
            // dps
            var data = this.data;
            var player;
            for (i = 0; i < this.players.length; i++) {
                player = this.players[i];
                data.push({
                    y: [],
                    mode: 'lines',
                    line: {
                        shape: 'spline',
                        color: player.colTarget
                    },
                    name: player.name + ' DPS'
                });
            }
            data.push({
                mode: 'lines',
                line: {
                    shape: 'spline'
                },
                visible: 'legendonly',
                name: 'All Player Dps'
            });
            // targets health
            var target;
            for (i = 0; i < this.graph.targets.length; i++) {
                var health = this.graph.targets[i].health;
                var hpTexts = [];
                for (j = 0; j < health.length; j++) {
                    hpTexts[j] = health[j] + "%";
                }
                target = this.targets[this.phase.targets[i]];
                data.push({
                    text: hpTexts,
                    mode: 'lines',
                    line: {
                        shape: 'spline',
                        dash: 'dashdot'
                    },
                    hoverinfo: 'text+x+name',
                    name: target.name + ' health',
                    _yaxis: 'y2'
                });
            }
            // mechanics
            var mechArray = getMechanics();
            for (i = 0; i < this.mechanics.length; i++) {
                var mech = this.mechanics[i];
                var mechData = mechArray[i];
                var chart = {
                    x: [],
                    mode: 'markers',
                    visible: mech.visible ? null : 'legendonly',
                    type: 'scatter',
                    marker: {
                        symbol: mech.symbol,
                        color: mech.color,
                        size: mech.size ? mech.size : 15
                    },
                    text: [],
                    name: mechData.name,
                    hoverinfo: 'text'
                };
                var time, pts, k;
                if (mechData.enemyMech) {
                    for (j = 0; j < mech.points[this.phaseid].length; j++) {
                        pts = mech.points[this.phaseid][j];
                        var tarId = this.phase.targets[j];
                        if (tarId >= 0) {
                            target = this.targets[tarId];
                            for (k = 0; k < pts.length; k++) {
                                time = pts[k];
                                chart.x.push(time);
                                chart.text.push(time + 's: ' + target.name);
                            }
                        } else {
                            for (k = 0; k < pts.length; k++) {
                                time = pts[k];
                                chart.x.push(time);
                                chart.text.push(time + 's');
                            }
                        }
                    }
                } else {
                    for (j = 0; j < mech.points[this.phaseid].length; j++) {
                        pts = mech.points[this.phaseid][j];
                        player = this.players[j];
                        for (k = 0; k < pts.length; k++) {
                            time = pts[k];
                            chart.x.push(time);
                            chart.text.push(time + 's: ' + player.name);
                        }
                    }
                }
                data.push(chart);
            }
        },
        computed: {
            graphid: function () {
                return 'dpsgraph-' + this.phaseid;
            },
            graphname: function () {
                var name = "DPS graph";
                name = (this.dpsmode === 0 ? "Full " : (this.dpsmode === 1 ? "10s " : (this.dpsmode === 2 ? "30s " : "Phase "))) + name;
                name = (this.mode === 0 ? "Total " : (this.mode === 1 ? "Target " : "Cleave ")) + name;
                return name;
            },
            computePhaseBreaks: function () {
                var res = [];
                if (this.phase.subPhases) {
                    for (var i = 0; i < this.phase.subPhases.length; i++) {
                        var subPhase = this.phases[this.phase.subPhases[i]];
                        res[Math.floor(subPhase.start - this.phase.start)] = true;
                        res[Math.floor(subPhase.end - this.phase.start)] = true;
                    }
                }
                return res;
            },
            computeData: function () {
                this.layout.datarevision = new Date().getTime();
                var points = this.computeDPSRelatedData();
                var res = this.data;
                for (var i = 0; i < points.length; i++) {
                    res[i].y = points[i];
                }
                return res;
            }
        },
        methods: {
            computeDPS: function (lim, phasebreaks) {
                var maxDPS = {
                    total: 0,
                    target: 0,
                    cleave: 0
                };
                var allDPS = {
                    total: [0],
                    target: [0],
                    cleave: [0]
                };
                //var before = performance.now();
                var playerDPS = [];
                for (var i = 0; i < this.players.length; i++) {
                    var totalDamage = 0;
                    var targetDamage = 0;
                    var totalDPS = [0];
                    var cleaveDPS = [0];
                    var targetDPS = [0];
                    var dpsData = this.graph.players[i];
                    for (var j = 1; j < dpsData.total.length; j++) {
                        var limID = 0;
                        if (lim > 0) {
                            limID = Math.max(j - lim, 0);
                        }
                        totalDamage += dpsData.total[j] - dpsData.total[limID];
                        for (var k = 0; k < this.activetargets.length; k++) {
                            var targetid = this.activetargets[k].id;
                            targetDamage += dpsData.targets[targetid][j] - dpsData.targets[targetid][limID];
                        }
                        if (phasebreaks && phasebreaks[j - 1]) {
                            limID = j - 1;
                            totalDamage = 0;
                            targetDamage = 0;
                        }
                        totalDPS[j] = Math.round(totalDamage / (j - limID));
                        targetDPS[j] = Math.round(targetDamage / (j - limID));
                        cleaveDPS[j] = Math.round((totalDamage - targetDamage) / (j - limID));
                        allDPS.total[j] = totalDPS[j] + (allDPS.total[j] || 0);
                        allDPS.target[j] = targetDPS[j] + (allDPS.target[j] || 0);
                        allDPS.cleave[j] = cleaveDPS[j] + (allDPS.cleave[j] || 0);
                        maxDPS.total = Math.max(maxDPS.total, totalDPS[j]);
                        maxDPS.target = Math.max(maxDPS.target, targetDPS[j]);
                        maxDPS.cleave = Math.max(maxDPS.cleave, cleaveDPS[j]);
                    }
                    playerDPS.push({
                        total: totalDPS,
                        target: targetDPS,
                        cleave: cleaveDPS
                    });
                }
                //var after = performance.now();
                //console.log("DPS Data " + (after - before));
                return {
                    allDPS: allDPS,
                    playerDPS: playerDPS,
                    maxDPS: maxDPS,
                };
            },
            computeDPSData: function () {
                var cacheID = this.dpsmode + '-';
                var targetsID = 1;
                for (var i = 0; i < this.activetargets.length; i++) {
                    var target = this.activetargets[i];
                    targetsID = targetsID << (target.id + 1);
                }
                cacheID += targetsID;
                if (this.dpsCache.has(cacheID)) {
                    return this.dpsCache.get(cacheID);
                }
                var res;
                if (this.dpsmode < 3) {
                    var lim = (this.dpsmode === 0 ? 0 : (this.dpsmode === 1 ? 10 : 30));
                    res = this.computeDPS(lim, null);
                } else {
                    res = this.computeDPS(0, this.computePhaseBreaks);
                }
                this.dpsCache.set(cacheID, res);
                return res;
            },
            computeDPSRelatedData: function () {
                var cacheID = this.dpsmode + '-' + this.mode + '-';
                var targetsID = 1;
                var i, j;
                var target;
                for (i = 0; i < this.activetargets.length; i++) {
                    target = this.activetargets[i];
                    targetsID = targetsID << (target.id + 1);
                }
                cacheID += targetsID;
                if (this.dataCache.has(cacheID)) {
                    return this.dataCache.get(cacheID);
                }
                var res = [];
                var dpsData = this.computeDPSData();
                var offset = 0;
                for (i = 0; i < this.players.length; i++) {
                    var pDPS = dpsData.playerDPS[i];
                    res[offset++] = (this.mode === 0 ? pDPS.total : (this.mode === 1 ? pDPS.target : pDPS.cleave));
                }
                res[offset++] = (this.mode === 0 ? dpsData.allDPS.total : (this.mode === 1 ? dpsData.allDPS.target : dpsData.allDPS.cleave));
                var maxDPS = (this.mode === 0 ? dpsData.maxDPS.total : (this.mode === 1 ? dpsData.maxDPS.target : dpsData.maxDPS.cleave));
                var hps = [];
                for (i = 0; i < this.graph.targets.length; i++) {
                    var health = this.graph.targets[i].health;
                    var hpPoints = [];
                    for (j = 0; j < health.length; j++) {
                        hpPoints[j] = health[j] * maxDPS / 100.0;
                    }
                    hps[i] = hpPoints;
                    res[offset++] = hpPoints;
                }
                var mechArray = getMechanics();
                for (i = 0; i < this.mechanics.length; i++) {
                    var mech = this.mechanics[i];
                    var mechData = mechArray[i];
                    chart = [];
                    res[offset++] = chart;
                    var time, pts, k;
                    if (mechData.enemyMech) {
                        for (j = 0; j < mech.points[this.phaseid].length; j++) {
                            pts = mech.points[this.phaseid][j];
                            var tarId = this.phase.targets[j];
                            if (tarId >= 0) {
                                target = this.targets[tarId];
                                for (k = 0; k < pts.length; k++) {
                                    time = pts[k];
                                    var ftime = Math.floor(time);
                                    var y = hps[j][ftime];
                                    var yp1 = hps[j][ftime + 1];
                                    chart.push(this.interpolatePoint(ftime, ftime + 1, y, yp1, time));
                                }
                            } else {
                                for (k = 0; k < pts.length; k++) {
                                    chart.push(maxDPS * 0.5);
                                }
                            }
                        }
                    } else {
                        for (j = 0; j < mech.points[this.phaseid].length; j++) {
                            pts = mech.points[this.phaseid][j];
                            for (k = 0; k < pts.length; k++) {
                                time = pts[k];
                                var ftime = Math.floor(time);
                                var y = res[j][ftime];
                                var yp1 = res[j][ftime + 1];
                                chart.push(this.interpolatePoint(ftime, ftime + 1, y, yp1, time));
                            }
                        }
                    }
                }
                this.dataCache.set(cacheID, res);
                return res;
            },
            interpolatePoint: function (x1, x2, y1, y2, x) {
                if (typeof y2 !== "undefined") {
                    return y1 + (y2 - y1) / (x2 - x1) * (x - x1);
                } else {
                    return y1;
                }
            }
        }
    });
};

var createLayout = function () {
    // Compile
    Vue.component("general-layout-component", {
        name: "general-layout-component",
        template: "#tmplGeneralLayout",
        props: ["layout", "phase"],
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
    //
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
    var graphs = new Tab("Graph", {
        dataType: DataTypes.dpsGraph
    });
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
    Vue.config.performance = true;
    var i;
    for (i = 0; i < logData.phases.length; i++) {
        logData.phases[i].active = i === 0;
        logData.phases[i].focus = null;
    }
    for (i = 0; i < logData.targets.length; i++) {
        var targetData = logData.targets[i];
        targetData.active = true;
    }
    for (i = 0; i < logData.players.length; i++) {
        var playerData = logData.players[i];
        playerData.active = false;
        playerData.icon = urls[playerData.profession];
    }

    var layout = createLayout();
    compileCommons();
    compileHeader();
    compileGeneralStats();
    compileBuffStats();
    compileMechanics();
    compileGraphs();
    compilePlayerTab();
    compileTargetTab();
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
                        return {
                            phase: phases[i],
                            index: i
                        };
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
            activePlayer: function () {
                var players = this.logdata.players;
                for (var i = 0; i < players.length; i++) {
                    if (players[i].active) {
                        return true;
                    }
                }
                return false;
            },
            activePhaseTargets: function () {
                var res = [];
                var targets = this.logdata.targets;
                var activePhase = this.phaseData.phase;
                for (var i = 0; i < activePhase.targets.length; i++) {
                    var target = targets[activePhase.targets[i]];
                    if (target.active) {
                        res.push({
                            target: target,
                            id: i
                        });
                    }
                }
                return res;
            }
        },
        mounted() {
            var element = document.getElementById("loading");
            element.parentNode.removeChild(element);
        }
    });
    $("body").tooltip({
        selector: "[data-original-title]",
        html: true
    });
};
