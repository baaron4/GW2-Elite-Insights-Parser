/*jshint esversion: 6 */

function computeTargetDPS(targetid, graph, lim, phasebreaks) {
    var totalDamage = 0;
    var maxDPS = 0;
    var totalDPS = [0];
    var dpsData = graph.targets[targetid].total;

    for (var j = 1; j < dpsData.length; j++) {
        var limID = 0;
        if (lim > 0) {
            limID = Math.max(j - lim, 0);
        }
        totalDamage += dpsData[j] - dpsData[limID];
        if (phasebreaks && phasebreaks[j - 1]) {
            limID = j - 1;
            totalDamage = 0;
        }
        totalDPS[j] = Math.round(totalDamage / (j - limID));
        maxDPS = Math.max(maxDPS, totalDPS[j]);
    }
    return {
        dps: totalDPS,
        maxDPS: maxDPS
    };
}

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
        props: ["target", "phaseindex", "players", "phase", "graphdata", "boons", "conditions", 'targetindex', 'phases', 'mode'],
        template: "#tmplTargetTab",
    });
    // stats
    Vue.component("target-stats-component", {
        props: ["players", "targets", "phase", "phaseindex", "graphdata", "presentboons", "presentconditions", 'phases'],
        template: "#tmplTargetStats",
        data: function () {
            return {
                mode: 0
            };
        },
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

    Vue.component("target-graph-tab-component", {
        props: ["targetindex", "target", "phase", "phases", "phaseindex", "graph"],
        data: function () {
            return {
                dpsmode: 0,
                layout: {},
                data: [],
                dpsCache: new Map(),
                dataCache: new Map(),
                targetOffset: 0
            };
        },
        created: function () {
            var images = [];
            this.targetOffset += computeRotationData(this.target.details.rotation[this.phaseindex], images, this.data);
            var offsets = computeBuffData(this.target.details.boonGraph[this.phaseindex], this.data);
            this.targetOffset += offsets.actorOffset;
            var dpsY = 'y' + (2 + offsets.y);
            {
                var health = this.graph.targets[this.phaseTargetIndex].health;
                var hpTexts = [];
                for (j = 0; j < health.length; j++) {
                    hpTexts[j] = health[j] + "%";
                }
                var res = {
                    text: hpTexts,
                    mode: 'lines',
                    line: {
                        shape: 'spline',
                        dash: 'dashdot'
                    },
                    hoverinfo: 'text+x+name',
                    name: this.target.name + ' health',
                    yaxis: dpsY
                };
                this.data.push(res);
            }
            this.targetOffset++;
            this.data.push({
                y: [],
                mode: 'lines',
                line: {
                    shape: 'spline'
                },
                yaxis: dpsY,
                name: 'Total DPS'
            });
            this.layout = getActorGraphLayout(images, offsets.y);
            computePhaseMarkups(this.layout.shapes, this.layout.annotations, this.phase);
        },
        computed: {
            phaseTargetIndex: function () {
                return this.phase.targets.indexOf(this.targetindex);
            },
            graphid: function () {
                return "targetgraph-" + this.phaseTargetIndex + '-' + this.phaseindex;
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
                        var subPhase = this.phases[this.phase.subPhases[i]];
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
                this.data[this.targetOffset].y = data[0];
                this.data[this.targetOffset - 1].y = data[1];
                return res;
            }
        },
        methods: {
            computeDPSData: function () {
                var cacheID = this.dpsmode;
                if (this.dpsCache.has(cacheID)) {
                    return this.dpsCache.get(cacheID);
                }
                //var before = performance.now();
                var res;
                if (this.dpsmode < 3) {
                    var lim = (this.dpsmode === 0 ? 0 : (this.dpsmode === 1 ? 10 : 30));
                    res = computeTargetDPS(this.phaseTargetIndex, this.graph, lim, null);
                } else {
                    res = computeTargetDPS(this.phaseTargetIndex, this.graph, 0, this.computePhaseBreaks);
                }
                this.dpsCache.set(cacheID, res);
                return res;
            },
            computeDPSRelatedData: function () {
                var cacheID = this.dpsmode;
                if (this.dataCache.has(cacheID)) {
                    return this.dataCache.get(cacheID);
                }
                var dpsData = this.computeDPSData();
                var res = [];
                res[0] = dpsData.dps;
                {
                    var health = this.graph.targets[this.phaseTargetIndex].health;
                    var hpPoints = [];
                    for (j = 0; j < health.length; j++) {
                        hpPoints[j] = health[j] * dpsData.maxDPS / 100.0;
                    }
                    res[1] = hpPoints;
                }
                this.dataCache.set(cacheID, res);
                return res;
            },
        },
        template: "#tmplTargetTabGraph"
    });
};
