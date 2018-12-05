/*jshint esversion: 6 */

var compileCombatReplay = function () {
    Vue.component("combat-replay-damage-stats-component", {
        props: ["time", "playerindex"],
        template: "#tmplCombatReplayDamageTable",
        data: function () {
            return {
                cache: []
            };
        },
        mounted() {
            initTable("#combat-replay-dps-table", 0, "desc");
        },
        updated() {
            updateTable("#combat-replay-dps-table");
        },
        computed: {
            targets: function () {
                return logData.phases[0].targets;
            },
            graph: function () {
                return graphData.phases[0].players;
            },
            tableData: function () {
                var cacheID = this.time / 200;
                if (this.cache[cacheID]) {
                    return this.cache[cacheID];
                }
                var prevStatus = this.cache[cacheID - 1];
                var rows = [];
                var cols = [];
                var sums = [];
                var total = [];
                var index = Math.floor(this.time / 1000);
                var i, j;
                for (j = 0; j < this.targets.length; j++) {
                    var target = logData.targets[this.targets[j]];
                    cols.push(target);
                }
                for (i = 0; i < this.graph.length; i++) {
                    var dpsStat = this.graph[i];
                    var player = logData.players[i];
                    var dps = [];
                    for (j = 0; j < this.targets.length; j++) {
                        var tar = dpsStat.targets[this.targets[j]];
                        var damage = tar[index + 1];
                        if (typeof damage !== "undefined") {
                            dps[2 * j] = prevStatus ? prevStatus.rows[i].dps[2 * j] + 0.2 * damage : 0.2 * damage;
                        } else {
                            dps[2 * j] = prevStatus ? prevStatus.rows[i].dps[2 * j] : 0;
                        }
                        dps[2 * j + 1] = dps[2 * j] / this.time; 
                    }
                    {                 
                        var totalDamage = dpsStat.total[index + 1];
                        if (typeof totalDamage !== "undefined") {
                            dps[2 * j] = prevStatus ? prevStatus.rows[i].dps[2 * j] + 0.2 * totalDamage : 0.2 * totalDamage;
                        } else {
                            dps[2 * j] = prevStatus ? prevStatus.rows[i].dps[2 * j] : 0;
                        }
                        dps[2 * j + 1] = dps[2 * j] / this.time; 
                    }
                    for (j = 0; j < dps.length; j++) {
                        total[j] = (total[j] || 0) + dps[j];
                    }
                    rows.push({
                        player: player,
                        dps: dps
                    });
                }
                sums.push({
                    name: "Total",
                    dps: total
                });
                var res = {
                    cols: cols,
                    rows: rows,
                    sums: sums
                };
                this.cache[cacheID] = res;
                return res;
            }
        }
    });

    Vue.component("combat-replay-data-component", {
        template: "#tmplCombatReplayData",
        props: ["animator"],
        computed: {
            playerindex: function() {
                if (this.animator.selectedPlayer) {
                    for (var i = 0; i < logData.players.length; i++) {
                        if (logData.players[i].combatReplayID == this.animator.selectedPlayer) {
                            return i;
                        }
                    }
                }
                return -1;
            },
            time: function() {
                var time = Math.floor(this.animator.time / 200) * 200;
                return time;
            }
        }
    });
};
