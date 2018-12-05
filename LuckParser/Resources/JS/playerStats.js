/*jshint esversion: 6 */

var compilePlayerTab = function () {
    // Base stuff
    Vue.component('dmgdist-player-component', {
        props: ['playerindex', 
            'phaseindex', 'activetargets'
        ],
        template: "#tmplDamageDistPlayer",
        data: function () {
            return {
                distmode: -1,
                targetmode: 1,
                cacheTarget: new Map()
            };
        },
        computed: {
            phase: function() {
                return logData.phases[this.phaseindex];
            },
            player : function() {
                return logData.players[this.playerindex];
            },
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
                cacheID += getTargetCacheID(this.activetargets);
                if (this.cacheTarget.has(cacheID)) {
                    return this.cacheTarget.get(cacheID);
                }
                var dist = {
                    contributedDamage: 0,
                    totalDamage: 0,
                    distribution: [],
                };
                var rows = new Map();
                for (var i = 0; i < this.activetargets.length; i++) {
                    var targetid = this.activetargets[i];
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

    Vue.component("player-graph-tab-component", {
        props: ["playerindex", "phaseindex", "activetargets", "light"],
        data: function () {
            return {
                dpsmode: 0,
                layout: {},
                data: [],
                dpsCache: new Map(),
                dataCache: new Map(),
                playerOffset: 0
            };
        },
        watch: {
            light: {
                handler: function() {
                    var textColor = this.light ? '#495057' : '#cccccc';
                    this.layout.yaxis.gridcolor = textColor;
                    this.layout.yaxis.color = textColor;
                    this.layout.yaxis2.gridcolor = textColor;
                    this.layout.yaxis2.color = textColor;
                    this.layout.yaxis3.gridcolor = textColor;
                    this.layout.yaxis3.color = textColor;
                    this.layout.xaxis.gridcolor = textColor;
                    this.layout.xaxis.color = textColor;
                    this.layout.font.color = textColor;
                    for (var i = 0; i < this.layout.shapes.length; i++) {
                        this.layout.shapes[i].line.color = textColor;
                    }
                    this.layout.datarevision = new Date().getTime();
                }
            }
        },
        created: function () {
            var images = [];
            this.playerOffset += computeRotationData(this.player.details.rotation[this.phaseindex], images, this.data);
            var oldOffset = this.playerOffset;
            this.playerOffset += computeBuffData(this.player.details.boonGraph[this.phaseindex], this.data);
            var dpsY = oldOffset === this.playerOffset ? 'y2' : 'y3';
            this.playerOffset += computeTargetHealthData(this.graph, logData.targets, this.phase, this.data, dpsY);
            this.data.push({
                y: [],
                mode: 'lines',
                line: {
                    shape: 'spline',
                    color: this.player.colTotal,
                },
                yaxis: dpsY,
                hoverinfo: 'name+y',
                name: 'Total DPS'
            });
            this.data.push({
                y: [],
                mode: 'lines',
                line: {
                    shape: 'spline',
                    color: this.player.colTarget,
                },
                yaxis: dpsY,
                hoverinfo: 'name+y',
                name: 'Target DPS'
            });
            this.data.push({
                y: [],
                mode: 'lines',
                line: {
                    shape: 'spline',
                    color: this.player.colCleave,
                },
                yaxis: dpsY,
                hoverinfo: 'name+y',
                name: 'Cleave DPS'
            });
            this.layout = getActorGraphLayout(images, this.light ? '#495057' : '#cccccc');
            computePhaseMarkups(this.layout.shapes, this.layout.annotations, this.phase, this.light ? '#495057' : '#cccccc');
        },
        computed: {
            phase: function() {
                return logData.phases[this.phaseindex];
            },
            player : function() {
                return logData.players[this.playerindex];
            },
            graph: function() {
                return graphData.phases[this.phaseindex];
            },
            graphid: function () {
                return "playergraph-" + this.playerindex + '-' + this.phaseindex;
            },
            graphname: function () {
                var name = "DPS graph";
                name = (this.dpsmode === 0 ? "Full " : (this.dpsmode === 1 ? "10s " : (this.dpsmode === 2 ? "30s " : "Phase "))) + name;
                return name;
            },
            computePhaseBreaks: function () {
                var res = [];
                if (this.phase.subPhases) {
                    for (var i = 0; i < this.phase.subPhases.length; i++) {
                        var subPhase = logData.phases[this.phase.subPhases[i]];
                        res[Math.floor(subPhase.start - this.phase.start)] = true;
                        res[Math.floor(subPhase.end - this.phase.start)] = true;
                    }
                }
                return res;
            },
            computeData: function () {
                this.layout.datarevision = new Date().getTime();
                var res = this.data;
                var data = this.computeDPSRelatedData();
                this.data[this.playerOffset].y = data[0];
                this.data[this.playerOffset + 1].y = data[1];
                this.data[this.playerOffset + 2].y = data[2];
                var offset = 3;
                for (var i = this.playerOffset - this.graph.targets.length; i < this.playerOffset; i++) {
                    this.data[i].y = data[offset++];
                }
                return res;
            }
        },
        methods: {
            computeDPSData: function () {
                var cacheID = this.dpsmode + '-';
                cacheID += getTargetCacheID(this.activetargets);
                if (this.dpsCache.has(cacheID)) {
                    return this.dpsCache.get(cacheID);
                }
                var data;
                var graphData = this.graph.players[this.playerindex];
                if (this.dpsmode < 3) {
                    var lim = (this.dpsmode === 0 ? 0 : (this.dpsmode === 1 ? 10 : 30));
                    data = computePlayerDPS(this.player, graphData, lim, null, this.activetargets, cacheID + '-' + this.phaseindex);
                } else {
                    data = computePlayerDPS(this.player, graphData, 0, this.computePhaseBreaks, this.activetargets, cacheID + '-' + this.phaseindex);
                }
                var res = {
                    maxDPS: data.maxDPS.total,
                    playerDPS: data.dps
                };
                this.dpsCache.set(cacheID, res);
                return res;
            },
            computeDPSRelatedData: function () {
                var cacheID = this.dpsmode + '-';
                cacheID += getTargetCacheID(this.activetargets);
                if (this.dataCache.has(cacheID)) {
                    return this.dataCache.get(cacheID);
                }
                var offset = 0;
                var dpsData = this.computeDPSData();
                var res = [];
                res[offset++] = dpsData.playerDPS.total;
                res[offset++] = dpsData.playerDPS.target;
                res[offset++] = dpsData.playerDPS.cleave;
                for (i = 0; i < this.graph.targets.length; i++) {
                    var health = this.graph.targets[i].health;
                    var hpPoints = [];
                    for (j = 0; j < health.length; j++) {
                        hpPoints[j] = health[j] * dpsData.maxDPS / 100.0;
                    }
                    res[offset++] = hpPoints;
                }
                this.dataCache.set(cacheID, res);
                return res;
            },
        },
        template: "#tmplPlayerTabGraph"
    });

    Vue.component("food-component", {
        props: ["phaseindex", "playerindex"],
        template: "#tmplFood",
        data: function () {
            return {
                cache: new Map()
            };
        },
        mixins: [roundingComponent],
        computed: {
            phase: function() {
                return logData.phases[this.phaseindex];
            },
            food: function() {
                return logData.players[this.playerindex].details.food;
            },
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
        props: ["playerindex", "phaseindex"],
        template: "#tmplSimpleRotation",
        data: function () {
            return {
                autoattack: true,
                small: false
            };
        },
        computed: {
            rotation: function() {
                return logData.players[this.playerindex].details.rotation[this.phaseindex];
            }
        },
        methods: {
            getSkill: function (id) {
                return findSkill(false, id);
            }
        }
    });

    Vue.component("deathrecap-component", {
        props: ["playerindex", "phaseindex"],
        template: "#tmplDeathRecap",
        computed: {
            phase: function() {
                return logData.phases[this.phaseindex];
            },
            recaps: function() {
                return logData.players[this.playerindex].details.deathRecap;
            },
            data: function () {
                if (!this.recaps) {
                    return null;
                }
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
                    var data = {
                        y: [],
                        x: [],
                        type: 'bar',
                        text: [],
                        marker: {
                            color: []
                        }
                    };
                    var j, totalSec, totalDamage;
                    if (recap.toDown) {
                        totalSec = (recap.toDown[0][0] - recap.toDown[recap.toDown.length - 1][0]) / 1000;
                        totalDamage = 0;
                        for (j = recap.toDown.length - 1; j >= 0; j--) {
                            totalDamage += recap.toDown[j][2];
                            data.x.push(recap.toDown[j][0] / 1000);
                            data.y.push(recap.toDown[j][2]);
                            data.text.push(recap.toDown[j][3] + ' - ' + findSkill(recap.toDown[j][4], recap.toDown[j][1]).name);
                            data.marker.color.push('rgb(0,255,0,1)');
                        }
                        res.totalSeconds.down[i] = totalSec;
                        res.totalDamage.down[i] = totalDamage;
                    }
                    if (recap.toKill) {
                        totalSec = (recap.toKill[0][0] - recap.toKill[recap.toKill.length - 1][0]) / 1000;
                        totalDamage = 0;
                        for (j = recap.toKill.length - 1; j >= 0; j--) {
                            totalDamage += recap.toKill[j][2];
                            data.x.push(recap.toKill[j][0] / 1000);
                            data.y.push(recap.toKill[j][2]);
                            data.text.push(recap.toKill[j][3] + ' - ' + findSkill(recap.toKill[j][4], recap.toKill[j][1]).name);
                            data.marker.color.push(recap.toDown ? 'rgb(255,0,0,1)' : 'rgb(0,255,0,1)');
                        }
                        res.totalSeconds.kill[i] = totalSec;
                        res.totalDamage.kill[i] = totalDamage;
                    }
                    res.data.push(data);
                }
                res.layout = {
                    title: 'Damage Taken',
                    font: {
                        color: '#ffffff'
                    },
                    width: 1100,
                    paper_bgcolor: 'rgba(0,0,0,0)',
                    plot_bgcolor: 'rgba(0,0,0,0)',
                    showlegend: false,
                    bargap: 0.05,
                    yaxis: {
                        title: 'Damage'
                    },
                    xaxis: {
                        title: 'Time(seconds)',
                        type: 'category'
                    }
                };
                return res;
            },
            phaseRecaps: function () {
                if (!this.recaps) {
                    return null;
                }
                var res = [];
                for (var i = 0; i < this.recaps.length; i++) {
                    var time = this.recaps[i].time / 1000.0;
                    if (this.phase.start <= time && this.phase.end >= time) {
                        res.push(i);
                    }
                }
                return res;
            }
        }
    });
    // tab
    Vue.component('player-tab-component', {
        props: ['playerindex', 'tabmode',
            'phaseindex', 'activetargets', 'light'
        ],
        computed: {    
            phases: function() {
                return logData.phases;
            },          
            player: function() {
                return logData.players[this.playerindex];
            }
        },
        data: function () {
            return {
                graphdata: graphData
            };
        },
        template: "#tmplPlayerTab",
    });
    // stats
    Vue.component("player-stats-component", {
        props: ["phaseindex", 'activetargets', 'activeplayer', 'light'],
        template: "#tmplPlayerStats",
        data: function () {
            return {
                tabmode: 0
            };
        },
        computed: {
            players: function() {
                return logData.players;
            }
        }
    });
};