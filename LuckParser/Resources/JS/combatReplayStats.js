/*jshint esversion: 6 */

var compileCombatReplay = function () {
    Vue.component("combat-replay-damage-stats-component", {
        props: ["time", "playerindex"],
        template: "#tmplCombatReplayDamageTable",
        data: function() {
            return {
                damageMode : 1
            };
        },
        created() {
            var i, cacheID;
            for (var j = 0; j < this.targets.length; j++) {
                var activeTargets = [j];
                cacheID = 0 + '-';
                cacheID += getTargetCacheID(activeTargets);
                // compute dps for all players
                for (i = 0; i < logData.players.length; i++) {
                    computePlayerDPS(logData.players[i], this.graph[i], 0, null, activeTargets, cacheID + '-' + 0);
                }
            }
            cacheID = 0 + '-';
            cacheID += getTargetCacheID(this.targets);
            // compute dps for all players
            for (i = 0; i < logData.players.length; i++) {
                computePlayerDPS(logData.players[i], this.graph[i], 0, null, this.targets, cacheID + '-' + 0);
            }
        },
        mounted() {
            initTable("#combat-replay-dps-table", 2, "desc");
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
                    var cacheID, data, cur, next;
                    var player = logData.players[i];
                    var graphData = this.graph[i];
                    var dps = [];
                    // targets
                    for (j = 0; j < this.targets.length; j++) {
                        var activeTargets = [j];
                        cacheID = 0 + '-';
                        cacheID += getTargetCacheID(activeTargets);
                        data = computePlayerDPS(player, graphData, 0, null, activeTargets, cacheID + '-' + 0).total.target;
                        cur = data[index];
                        next = data[index+1];
                        if (typeof next !== "undefined") {
                            dps[2 * j] = cur + (this.time / 1000 - index) * Math.max((next - cur), 0);
                        } else {
                            dps[2 * j] = cur;
                        }
                        dps[2 * j + 1] = dps[2 * j] / (Math.max(this.time / 1000, 1));
                    }
                    {
                        cacheID = 0 + '-';
                        cacheID += getTargetCacheID(this.targets);
                        data = computePlayerDPS(player, graphData, 0, null, this.targets, cacheID + '-' + 0).total.total;
                        cur = data[index];
                        next = data[index + 1];
                        if (typeof next !== "undefined") {
                            dps[2 * j] = cur + (this.time / 1000 - index) * Math.max((next - cur), 0);
                        } else {
                            dps[2 * j] = cur;
                        }
                        dps[2 * j + 1] = dps[2 * j] / (Math.max(this.time / 1000, 1));
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
                        if (logData.players[i].combatReplayID == this.animator.selectedPlayerID) {
                            return i;
                        }
                    }
                }
                return -1;
            },
        }
    });
};
