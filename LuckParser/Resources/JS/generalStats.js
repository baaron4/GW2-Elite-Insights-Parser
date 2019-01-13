/*jshint esversion: 6 */

var compileGeneralStats = function () {
    Vue.component("damage-stats-component", {
        props: ["activetargets", "playerindex", "phaseindex"],
        template: `${tmplDamageTable}`,
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
            phase: function () {
                return logData.phases[this.phaseindex];
            },
            tableData: function () {
                var cacheID = this.phaseindex + '-';
                cacheID += getTargetCacheID(this.activetargets);
                if (this.cacheTarget.has(cacheID)) {
                    return this.cacheTarget.get(cacheID);
                }
                var rows = [];
                var sums = [];
                var total = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
                var groups = [];
                var i, j;
                for (i = 0; i < this.phase.dpsStats.length; i++) {
                    var dpsStat = this.phase.dpsStats[i];
                    var dpsTargetStat = [0, 0, 0, 0, 0, 0];
                    for (j = 0; j < this.activetargets.length; j++) {
                        var tar = this.phase.dpsStatsTargets[i][this.activetargets[j]];
                        for (var k = 0; k < dpsTargetStat.length; k++) {
                            dpsTargetStat[k] += tar[k];
                        }
                    }
                    var player = logData.players[i];
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
        props: ["phaseindex", "playerindex"],
        template: `${tmplDefenseTable}`,
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
            phase: function () {
                return logData.phases[this.phaseindex];
            },
            tableData: function () {
                if (this.cache.has(this.phaseindex)) {
                    return this.cache.get(this.phaseindex);
                }
                var rows = [];
                var sums = [];
                var total = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
                var groups = [];
                var i;
                for (i = 0; i < this.phase.defStats.length; i++) {
                    var def = this.phase.defStats[i];
                    var player = logData.players[i];
                    if (player.isConjure) {
                        continue;
                    }
                    rows.push({
                        player: player,
                        def: def
                    });
                    if (!groups[player.group]) {
                        groups[player.group] = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
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
                this.cache.set(this.phaseindex, res);
                return res;
            }
        }
    });

    Vue.component("support-stats-component", {
        props: ["phaseindex", "playerindex"],
        template: `${tmplSupportTable}`,
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
            phase: function () {
                return logData.phases[this.phaseindex];
            },
            tableData: function () {
                if (this.cache.has(this.phaseindex)) {
                    return this.cache.get(this.phaseindex);
                }
                var rows = [];
                var sums = [];
                var total = [0, 0, 0, 0];
                var groups = [];
                var i;
                for (i = 0; i < this.phase.healStats.length; i++) {
                    var sup = this.phase.healStats[i];
                    var player = logData.players[i];
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
                this.cache.set(this.phaseindex, res);
                return res;
            }
        }
    });

    Vue.component("gameplay-stats-component", {
        props: ["activetargets", "playerindex", "phaseindex"],
        template: `${tmplGameplayTable}`,
        mixins: [roundingComponent],
        data: function () {
            return {
                mode: 0,
                cache: new Map(),
                cacheTarget: new Map()
            };
        },
        mounted() {
            initTable("#dmg-table", 1, "desc");
        },
        updated() {
            updateTable("#dmg-table");
        },
        computed: {
            phase: function () {
                return logData.phases[this.phaseindex];
            },
            tableData: function () {
                if (this.cache.has(this.phaseindex)) {
                    return this.cache.get(this.phaseindex);
                }
                var rows = [];
                var sums = [];
                var groups = [];
                var total = {
                    name: "Total",
                    data: [],
                    commons: [],
                    count: 0
                };
                for (var i = 0; i < this.phase.dmgStats.length; i++) {
                    var commons = [];
                    var data = [];
                    var player = logData.players[i];
                    if (player.isConjure) {
                        continue;
                    }
                    if (!groups[player.group]) {
                        groups[player.group] = {
                            name: "Group " + player.group,
                            data: [],
                            commons: [],
                            count: 0
                        };
                    }
                    groups[player.group].count++;
                    total.count++;
                    var stats = this.phase.dmgStats[i];
                    for (var j = 0; j < stats.length; j++) {
                        if (j >= 17) {
                            commons[j - 17] = stats[j];
                            groups[player.group].commons[j - 17] = (groups[player.group].commons[j - 17] || 0) + commons[j - 17];
                            total.commons[j - 17] = (total.commons[j - 17] || 0) + commons[j - 17];
                        } else {
                            data[j] = stats[j];
                            groups[player.group].data[j] = (groups[player.group].data[j] || 0) + data[j];
                            total.data[j] = (total.data[j] || 0) + data[j];
                        }
                    }
                    rows.push({
                        player: player,
                        commons: commons,
                        data: data
                    });
                }
                for (var i = 0; i < groups.length; i++) {
                    if (groups[i]) {
                        sums.push(groups[i]);
                    }
                }
                sums.push(total);
                var res = {
                    rows: rows,
                    sums: sums
                };
                this.cache.set(this.phaseindex, res);
                return res;
            },
            tableDataTarget: function () {
                var cacheID = this.phaseindex + '-';
                cacheID += getTargetCacheID(this.activetargets);
                if (this.cacheTarget.has(cacheID)) {
                    return this.cacheTarget.get(cacheID);
                }
                var rows = [];
                var sums = [];
                var groups = [];
                var total = {
                    name: "Total",
                    data: [],
                    commons: [],
                    count: 0
                };
                for (var i = 0; i < this.phase.dmgStats.length; i++) {
                    var commons = [];
                    var data = [];
                    var player = logData.players[i];
                    if (player.isConjure) {
                        continue;
                    }
                    if (!groups[player.group]) {
                        groups[player.group] = {
                            name: "Group " + player.group,
                            data: [],
                            commons: [],
                            count: 0
                        };
                    }
                    groups[player.group].count++;
                    total.count++;
                    var stats = this.phase.dmgStats[i];
                    for (var j = 0; j < stats.length; j++) {
                        if (j >= 17) {
                            commons[j - 17] = stats[j];
                            groups[player.group].commons[j - 17] = (groups[player.group].commons[j - 17] || 0) + commons[j - 17];
                            total.commons[j - 17] = (total.commons[j - 17] || 0) + commons[j - 17];
                        } else {
                            for (var k = 0; k < this.activetargets.length; k++) {
                                var tar = this.phase.dmgStatsTargets[i][this.activetargets[k]];
                                data[j] = (data[j] || 0) + tar[j];
                            }
                            groups[player.group].data[j] = (groups[player.group].data[j] || 0) + data[j];
                            total.data[j] = (total.data[j] || 0) + data[j];
                        }
                    }
                    rows.push({
                        player: player,
                        commons: commons,
                        data: data
                    });
                }
                for (var i = 0; i < groups.length; i++) {
                    if (groups[i]) {
                        sums.push(groups[i]);
                    }
                }
                sums.push(total);
                var res = {
                    rows: rows,
                    sums: sums
                };
                this.cacheTarget.set(cacheID, res);
                return res;
            }
        }
    });

    Vue.component("dmgmodifier-stats-component", {
        props: ['phaseindex', 'playerindex', 'activetargets'
        ],
        template: `${tmplDamageModifierTable}`,
        data: function () {
            return {
                mode: 0,
                cache: new Map(),
                cacheTarget: new Map()
            };
        },
        computed: {
            phase: function () {
                return logData.phases[this.phaseindex];
            },
            modifiers: function () {
                var buffs = [];
                for (var i = 0; i < logData.dmgCommonModifiersBuffs.length; i++) {
                    buffs.push(findSkill(true, logData.dmgCommonModifiersBuffs[i]));
                }
                return buffs;
            },
            tableData: function () {
                if (this.cache.has(this.phaseindex)) {
                    return this.cache.get(this.phaseindex);
                }
                var rows = [];
                var sums = [];
                var groups = [];
                var total = {
                    name: "Total",
                    data: []
                };
                var j;
                for (var i = 0; i < logData.players.length; i++) {
                    var player = logData.players[i];
                    if (player.isConjure) {
                        continue;
                    }
                    if (!groups[player.group]) {
                        groups[player.group] = {
                            name: "Group" + player.group,
                            data: []
                        };
                    }
                    var dmgModifier = this.phase.dmgModifiersCommon[i];
                    var data = [];
                    for (j = 0; j < this.modifiers.length; j++) {
                        data[j] = dmgModifier[j].slice(0);
                        if (!groups[player.group].data[j]) {
                            groups[player.group].data[j] = [0, 0, 0, 0];
                        }
                        if (!total.data[j]) {
                            total.data[j] = [0, 0, 0, 0];
                        }
                        for (var k = 0; k < data[j].length; k++) {
                            groups[player.group].data[j][k] += data[j][k];
                            total.data[j][k] += data[j][k];
                        }
                    }
                    rows.push({
                        player: player,
                        data: data
                    });
                }
                for (var i = 0; i < groups.length; i++) {
                    if (groups[i]) {
                        sums.push(groups[i]);
                    }
                }
                sums.push(total);
                var res = {
                    rows: rows,
                    sums: sums
                };
                this.cache.set(this.phaseindex, res);
                return res;
            },
            tableDataTarget: function () {
                var cacheID = this.phaseindex + '-';
                cacheID += getTargetCacheID(this.activetargets);
                if (this.cacheTarget.has(cacheID)) {
                    return this.cacheTarget.get(cacheID);
                }
                var rows = [];
                var sums = [];
                var groups = [];
                var total = {
                    name: "Total",
                    data: []
                };
                var j;
                for (var i = 0; i < logData.players.length; i++) {
                    var player = logData.players[i];
                    if (player.isConjure) {
                        continue;
                    }
                    if (!groups[player.group]) {
                        groups[player.group] = {
                            name: "Group" + player.group,
                            data: []
                        };
                    }
                    var data = [];
                    for (j = 0; j < this.modifiers.length; j++) {
                        data[j] = [0, 0, 0, 0];
                        if (!groups[player.group].data[j]) {
                            groups[player.group].data[j] = [0, 0, 0, 0];
                        }
                        if (!total.data[j]) {
                            total.data[j] = [0, 0, 0, 0];
                        }
                    }
                    var dmgModifier = this.phase.dmgModifiersTargetsCommon[i];
                    for (j = 0; j < this.activetargets.length; j++) {
                        var modifier = dmgModifier[this.activetargets[j]];
                        for (var k = 0; k < this.modifiers.length; k++) {
                            var targetData = modifier[k].slice(0);
                            var curData = data[k];
                            for (var l = 0; l < targetData.length; l++) {
                                curData[l] += targetData[l];
                            }
                        }
                    }
                    for (j = 0; j < this.modifiers.length; j++) {
                        for (var k = 0; k < data[j].length; k++) {
                            groups[player.group].data[j][k] += data[j][k];
                            total.data[j][k] += data[j][k];
                        }
                    }
                    rows.push({
                        player: player,
                        data: data
                    });
                }
                for (var i = 0; i < groups.length; i++) {
                    if (groups[i]) {
                        sums.push(groups[i]);
                    }
                }
                sums.push(total);
                var res = {
                    rows: rows,
                    sums: sums
                };
                this.cacheTarget.set(cacheID, res);
                return res;
            }
        },
        methods: {
            getTooltip: function (item) {
                var hits = item[0] + " out of " + item[1] + " hits";
                if (item[3] > 0) {
                    var gain = "Pure Damage: " + item[2];
                    var damageIncrease = Math.round(100 * 100 * (item[3] / (item[3] - item[2]) - 1.0)) / 100;
                    var increase = "Damage Gain: " + (isNaN(damageIncrease) ? "0" : damageIncrease) + "%";
                    return hits + "<br>" + gain + "<br>" + increase;
                } else {
                    var done = "Damage Done: " + item[2];
                    return hits + "<br>" + done;
                }
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
};