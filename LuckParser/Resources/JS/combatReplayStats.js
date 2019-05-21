/*jshint esversion: 6 */

var compileCombatReplay = function () {
    var timeRefreshComponent = {
        props: ["time"],
        data: function() {
            return {
                refreshTime: 0
            };
        },
        computed: {
            timeToUse: function() {
                if (animator) {
                    var animated = animator.animation !== null;
                    if (animated) {
                        var speed = animator.speed;
                        if (Math.abs(this.time - this.refreshTime) > speed * 48) {
                            this.refreshTime = this.time;
                            return this.time;
                        }
                        return this.refreshTime;
                    } else {
                        this.refreshTime = this.time;
                        return this.time;
                    }
                }
                return this.time;
            },
        },
    };

    Vue.component("combat-replay-damage-stats-component", {
        mixins: [timeRefreshComponent],
        props: ["playerindex"],
        template: `${tmplCombatReplayDamageTable}`,
        data: function () {
            return {
                damageMode: 1
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
            phase: function () {
                return logData.phases[0];
            },
            targets: function () {
                return this.phase.targets;
            },
            graph: function () {
                return graphData.phases[0].players;
            },
            tableData: function () {
                var rows = [];
                var cols = [];
                var sums = [];
                var total = [];
                var tS = this.timeToUse / 1000.0;
                var curTime = Math.floor(tS);
                var nextTime = curTime + 1;
                var dur = Math.floor(this.phase.end - this.phase.start);
                if (nextTime == dur + 1 && this.phase.needsLastPoint) {
                    nextTime = this.phase.end - this.phase.start;
                }
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
                        cur = data[curTime];
                        next = data[curTime + 1];
                        if (typeof next !== "undefined") {
                            dps[2 * j] = cur + (tS - curTime) * (next - cur)/(nextTime - curTime);
                        } else {
                            dps[2 * j] = cur;
                        }
                        dps[2 * j + 1] = dps[2 * j] / (Math.max(tS, 1));
                    }
                    cacheID = 0 + '-';
                    cacheID += getTargetCacheID(this.targets);
                    data = computePlayerDPS(player, graphData, 0, null, this.targets, cacheID + '-' + 0).total.total;
                    cur = data[curTime];
                    next = data[curTime + 1];
                    if (typeof next !== "undefined") {
                        dps[2 * j] = cur + (tS - curTime) * (next - cur)/(nextTime - curTime);
                    } else {
                        dps[2 * j] = cur;
                    }
                    dps[2 * j + 1] = dps[2 * j] / (Math.max(tS, 1));
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

    Vue.component("combat-replay-actor-buffs-stats-component", {
        mixins: [timeRefreshComponent],
        props: ["actorindex", "enemy"],
        template: `${tmplCombatReplayActorBuffStats}`,
        methods: {
            findBuffState: function (states, timeS, start, end) {
                // when the array exists, it covers from 0 to fightEnd by construction
                var id = Math.floor((end + start) / 2);
                if (id === start || id === end) {
                    return states[id][1];
                }
                var item = states[id];
                var itemN = states[id + 1];
                var x = item[0];
                var xN = itemN[0];
                if (timeS < x) {
                    return this.findBuffState(states, timeS, start, id);
                } else if (timeS > xN) {
                    return this.findBuffState(states, timeS, id, end);
                } else {
                    return item[1];
                }
            }
        },
        computed: {
            boons: function () {
                var hash = new Set();
                for (var i = 0; i < logData.boons.length; i++) {
                    hash.add(logData.boons[i]);
                }
                return hash;
            },
            offs: function () {
                var hash = new Set();
                for (var i = 0; i < logData.offBuffs.length; i++) {
                    hash.add(logData.offBuffs[i]);
                }
                return hash;
            },
            defs: function () {
                var hash = new Set();
                for (var i = 0; i < logData.defBuffs.length; i++) {
                    hash.add(logData.defBuffs[i]);
                }
                return hash;
            },
            conditions: function () {
                var hash = new Set();
                for (var i = 0; i < logData.conditions.length; i++) {
                    hash.add(logData.conditions[i]);
                }
                return hash;
            },
            actor: function () {
                return this.enemy ? logData.targets[this.actorindex] : logData.players[this.actorindex];
            },
            buffData: function () {
                return this.actor.details.boonGraph[0];
            },
            data: function () {
                var res = {
                    offs: [],
                    defs: [],
                    boons: [],
                    conditions: [],
                    enemies: [],
                    others: [],
                    consumables: []
                };
                for (var i = 0; i < this.buffData.length; i++) {
                    var data = this.buffData[i];
                    var id = data.id;
                    var arrayToFill = [];
                    var buff = findSkill(true, id);
                    if (buff.consumable) {
                        arrayToFill = res.consumables;
                    } else if (buff.enemy) {
                        arrayToFill = res.enemies;
                    } else if (this.boons.has(id)) {
                        arrayToFill = res.boons;
                    } else if (this.offs.has(id)) {
                        arrayToFill = res.offs;
                    } else if (this.defs.has(id)) {
                        arrayToFill = res.defs;
                    } else if (this.conditions.has(id)) {
                        arrayToFill = res.conditions;
                    } else {
                        arrayToFill = res.others;
                    }
                    var t = this.timeToUse / 1000;
                    var val = this.findBuffState(data.states, t, 0, data.states.length - 1);
                    if (val > 0) {
                        arrayToFill.push({
                            state: val,
                            buff: buff
                        });
                    }
                }
                return res;
            }
        }
    });

    Vue.component("combat-replay-player-status-component", {
        props: ["playerindex", "time"],
        template: `${tmplCombatReplayPlayerStatus}`,
        computed: {
            player: function () {
                return logData.players[this.playerindex];
            },
            status: function () {
                var crPData = animator.playerData.get(this.player.combatReplayID);
                var icon = crPData.getIcon(this.time);
                return icon === deadIcon ? 0 : icon === downIcon ? 1 : icon === dcIcon ? 2 : 3;
            },
        }
    });

    Vue.component("combat-replay-target-status-component", {
        props: ["targetindex", "time"],
        template: `${tmplCombatReplayTargetStatus}`,
        methods: {
            getPercent: function (time) {
                var curTime = Math.floor(time / 1000);
                var nextTime = curTime + 1;
                var dur = Math.floor(this.phase.end - this.phase.start);
                if (nextTime == dur + 1 && this.phase.needsLastPoint) {
                    nextTime = this.phase.end - this.phase.start;
                }
                var data = this.healths;
                var cur = data[curTime];
                var next = data[curTime + 1];
                if (typeof next !== "undefined") {
                    res = cur + (time / 1000 - curTime) * (next - cur) / (nextTime - curTime);
                } else {
                    res = cur;
                }
                return res;
            },
            getGradient: function (time) {
                var template = 'linear-gradient(to right, $green$, $middle$, $black$)';
                var res = this.getPercent(time);
                var greenPercent = "green " + res + "%";
                var blackPercent = "black " + (100 - res) + "%";
                var middle = res + "%";
                template = template.replace('$green$', greenPercent);
                template = template.replace('$black$', blackPercent);
                template = template.replace('$middle$', middle);
                return template;
            }
        },
        computed: {
            phase: function () {
                return logData.phases[0];
            },
            healths: function () {
                return graphData.phases[0].targetsHealthForCR[this.targetindex];
            },
            target: function () {
                return logData.targets[this.targetindex];
            }
        }
    });

    Vue.component("combat-replay-actor-rotation-component", {
        mixins: [timeRefreshComponent],
        props: ["actorindex", "enemy"],
        template: `${tmplCombatReplayActorRotation}`,
        methods: {
            findRotationIndex: function (rotation, timeS, start, end) {
                if (end === 0) {
                    return 0;
                }
                if (timeS < rotation[start][0]) {
                    return start;
                } else if (timeS > rotation[end][0] + rotation[end][2] / 1000.0) {
                    return end;
                }
                var id = Math.floor((end + start) / 2);
                var item, x, duration;
                if (id === start || id === end) {               
                    item = rotation[start];
                    x = item[0];
                    duration = item[2] / 1000.0;
                    if (timeS >= x && x + duration >= timeS) {
                        return start;
                    }
                    return end;
                }
                item = rotation[id];
                x = item[0];
                duration = item[2] / 1000.0;
                if (timeS < x) {
                    return this.findRotationIndex(rotation, timeS, start, id);
                } else if (timeS > x + duration) {
                    return this.findRotationIndex(rotation, timeS, id, end);
                } else {
                    return id;
                }
            }
        },
        computed: {
            actor: function () {
                return this.enemy ? logData.targets[this.actorindex] : logData.players[this.actorindex];
            },
            actorRotation: function () {
                return this.actor.details.rotation[0].filter(x => x[2] > 1e-2);
            },
            rotation: function () {
                var res = {
                    current: null,
                    nexts: []
                };
                var time = this.timeToUse / 1000.0;
                var id = this.findRotationIndex(this.actorRotation, time, 0, this.actorRotation.length - 1);
                var j, next;
                var item = this.actorRotation[id];
                var x = item[0];
                var skillId = item[1];
                var endType = item[3];
                var duration = item[2] / 1000.0;
                var skill = findSkill(false, skillId);
                if (x <= time && time <= x + duration) {
                    res.current = {
                        skill: skill,
                        end: endType
                    };
                    for (j = id + 1; j < this.actorRotation.length; j++) {
                        next = this.actorRotation[j];
                        res.nexts.push({
                            skill: findSkill(false, next[1]),
                            end: next[3]
                        });
                        if (res.nexts.length == 3) {
                            break;
                        }
                    }
                } else {
                    for (j = id; j < this.actorRotation.length; j++) {
                        next = this.actorRotation[j];
                        res.nexts.push({
                            skill: findSkill(false, next[1]),
                            end: next[3]
                        });
                        if (res.nexts.length == 3) {
                            break;
                        }
                    }
                }
                return res;
            },
        }
    });

    Vue.component("combat-replay-player-stats-component", {
        props: ["playerindex", "time"],
        template: `${tmplCombatReplayPlayerStats}`
    });

    Vue.component("combat-replay-target-stats-component", {
        props: ["targetindex", "time"],
        template: `${tmplCombatReplayTargetStats}`
    });

    Vue.component("combat-replay-damage-data-component", {
        template: `${tmplCombatReplayDamageData}`,
        props: ["time", "selectedplayer", "selectedplayerid"],
        computed: {
            playerindex: function () {
                if (this.selectedplayer) {
                    for (var i = 0; i < logData.players.length; i++) {
                        if (logData.players[i].combatReplayID == this.selectedplayerid) {
                            return i;
                        }
                    }
                }
                return -1;
            }
        }
    });

    Vue.component("combat-replay-targets-stats-component", {
        props: ["time"],
        template: `${tmplCombatReplayTargetsStats}`,
        methods: {
            alive: function (status) {
                return status.start <= this.time && status.end >= this.time;
            }
        },
        computed: {
            targets: function () {
                var res = [];
                for (var i = 0; i < logData.targets.length; i++) {
                    var target = logData.targets[i];
                    var crTarget = animator.targetData.get(target.combatReplayID);
                    res.push({
                        start: crTarget.start,
                        end: crTarget.end
                    });
                }
                return res;
            },
        }
    });

    Vue.component("combat-replay-players-stats-component", {
        props: ["time", "selectedplayer", "selectedplayerid"],
        template: `${tmplCombatReplayPlayersStats}`,
        computed: {
            selectedplayerindex: function () {
                if (this.selectedplayer) {
                    for (var i = 0; i < logData.players.length; i++) {
                        if (logData.players[i].combatReplayID == this.selectedplayerid) {
                            return i;
                        }
                    }
                }
                return -1;
            },
            groups: function () {
                var res = [];
                var i = 0;
                for (i = 0; i < logData.players.length; i++) {
                    var playerData = logData.players[i];
                    if (playerData.isConjure) {
                        continue;
                    }
                    if (!res[playerData.group]) {
                        res[playerData.group] = [];
                    }
                    res[playerData.group].push(playerData);
                }
                return res;
            }
        }
    });

    Vue.component("combat-replay-status-data-component", {
        template: `${tmplCombatReplayStatusData}`,
        props: ["time", "selectedplayer", "selectedplayerid"],
        data: function() {
            return {
                details: false,
                mode: 0
            };
        }
    });
};
