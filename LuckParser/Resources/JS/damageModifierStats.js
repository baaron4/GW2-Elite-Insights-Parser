/*jshint esversion: 6 */

var compileDamageModifiers = function () {
    Vue.component("dmgmodifier-stats-component", {
        props: ['phaseindex', 'playerindex', 'activetargets',
        ],
        template: `${tmplDamageModifierStats}`,
        data: function () {
            return {
                mode: 0,
                displayMode: 0
            };
        },
        computed: {
            phase: function () {
                return logData.phases[this.phaseindex];
            },
            modifiers: function () {
                var buffs = [];
                return buffs;
            }
        }
    };

    Vue.component("dmgmodifier-table-component", {
        props: ['phaseindex', 'id', 'playerindex', 'activetargets', 'modifiers', 'modifiersdata', 'mode'
        ],
        template: `${tmplDamageModifierTable}`,
        data: function () {
            return {
                cache: new Map(),
                cacheTarget: new Map()
            };
        },
        computed: {
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
            initTable("#"+this.id, 1, "asc");
        },
        updated() {
            updateTable("#" + this.id);
        },
    });
};