/*jshint esversion: 6 */


function computeTargetDPS(target, dpsData,lim, phasebreaks, cacheID, lastTime) {
    if (!target.dpsGraphCache) {
        target.dpsGraphCache = new Map();
    }
    if (target.dpsGraphCache.has(cacheID)) {
        return target.dpsGraphCache.get(cacheID);
    }
    var totalDamage = 0;
    var totalDPS = [0];
    var maxDPS = 0;
    var start = 0;
    var limID, j;
    var end = dpsData.length;
    if (lastTime > 0) {
        end--;
    }
    for (j = 1; j < end; j++) {
        limID = 0;
        if (lim > 0) {
            limID = Math.max(j - lim, 0);
            start = limID;
        }
        totalDamage += dpsData[j] - dpsData[limID];
        if (phasebreaks && phasebreaks[j - 1]) {
            start = j - 1;
            totalDamage = 0;
        }
        totalDPS[j] = Math.round(totalDamage / (j - start));
        maxDPS = Math.max(maxDPS, totalDPS[j]);
    }   
    // last point management
    if (lastTime > 0) {
        limID = 0;
        if (lim > 0) {
            limID = Math.round(Math.max(lastTime - lim, 0));
            start = limID;
        }
        totalDamage += (lastTime - j + 1) * dpsData[j] - dpsData[limID];
        if (phasebreaks && phasebreaks[j - 1]) {
            start = j - 1;
            totalDamage = 0;
        }
        totalDPS[j] = Math.round(totalDamage / (lastTime - start));
        maxDPS = Math.max(maxDPS, totalDPS[j]);
    }
    if (maxDPS < 1e-6) {
        maxDPS = 10;
    }
    var res = {
        dps: totalDPS,
        maxDPS: maxDPS
    };
    target.dpsGraphCache.set(cacheID, res);
    return res;
}

var compileTargetTab = function () {
    // base
    Vue.component("buff-stats-target-component", {
        props: ['phaseindex', 'playerindex', 'targetindex'],
        template: `${tmplBuffStatsTarget}`,
        data: function () {
            return {
                cacheCondi: new Map(),
                cacheCondiSums: new Map(),
                cacheBoon: new Map()
            };
        },
        computed: {
            boons: function() {
                var data = [];
                for (var i = 0; i < logData.boons.length; i++) {
                    data[i] = findSkill(true, logData.boons[i]);
                }
                return data;
            },
            conditions: function() {
                var data = [];
                for (var i = 0; i < logData.conditions.length; i++) {
                    data[i] = findSkill(true, logData.conditions[i]);
                }
                return data;
            },
            phase: function() {
                return logData.phases[this.phaseindex];
            },
            target: function() {
                return logData.targets[this.targetindex];
            },
            targetPhaseIndex: function () {
                return this.phase.targets.indexOf(this.targetindex);
            },
            hasBoons: function () {
                return this.phase.targetsBoonTotals[this.targetPhaseIndex];
            },
            condiData: function () {
                if (this.cacheCondi.has(this.phaseindex)) {
                    return this.cacheCondi.get(this.phaseindex);
                }
                var res = [];
                var i;
                if (this.targetPhaseIndex === -1) {
                    for (i = 0; i < logData.players.length; i++) {
                        res.push({
                            player: logData.players[i],
                            data: {
                                avg: 0.0,
                                data: []
                            }
                        });
                    }
                } else {
                    for (i = 0; i < logData.players.length; i++) {
                        res.push({
                            player: logData.players[i],
                            data: this.phase.targetsCondiStats[this.targetPhaseIndex][i]
                        });
                    }
                }
                this.cacheCondi.set(this.phaseindex, res);
                return res;
            },
            condiSums: function () {
                if (this.cacheCondiSums.has(this.phaseindex)) {
                    return this.cacheCondiSums.get(this.phaseindex);
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
                this.cacheCondiSums.set(this.phaseindex, res);
                return res;
            },
            boonData: function () {
                if (this.cacheBoon.has(this.phaseindex)) {
                    return this.cacheBoon.get(this.phaseindex);
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
                this.cacheBoon.set(this.phaseindex, res);
                return res;
            }
        }
    });

    Vue.component('dmgdist-target-component', {
        props: ['targetindex',
            'phaseindex'
        ],
        template: `${tmplDamageDistTarget}`,
        data: function () {
            return {
                distmode: -1
            };
        },
        computed: {
            target: function() {
                return logData.targets[this.targetindex];
            },
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
        props: ["phaseindex", "playerindex", 'targetindex', 'mode', 'light'],
        template: `${tmplTargetTab}`,
        computed: {
            phases: function() {
                return logData.phases;
            },
            target: function() {
                return logData.targets[this.targetindex];
            }
        }
    });
    // stats
    Vue.component("target-stats-component", {
        props: ["playerindex", "phaseindex", 'light', "simplephase"],
        template: `${tmplTargetStats}`,
        data: function () {
            return {
                mode: 0
            };
        },
        computed: {
            phase: function() {
                return logData.phases[this.phaseindex];
            },
            targets: function() {
                return logData.targets;
            },
            phaseTargets: function () {
                var res = [];
                for (var i = 0; i < this.phase.targets.length; i++) {
                    var tar = logData.targets[this.phase.targets[i]];
                    res.push(tar);
                }
                if (this.simplephase.focus === null) {
                    this.simplephase.focus = res[0] ? res[0].id : null;
                }
                return res;
            }
        }
    });

    Vue.component("target-graph-tab-component", {
        props: ["targetindex", "phaseindex", 'light'],
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
        watch: {
            light: {
                handler: function () {
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
            this.targetOffset += computeRotationData(this.target.details.rotation[this.phaseindex], images, this.data);
            var oldOffset = this.targetOffset;
            this.targetOffset += computeBuffData(this.target.details.boonGraph[this.phaseindex], this.data);
            var dpsY = oldOffset === this.targetOffset ? 'y2' : 'y3';
            {
                var health = this.graph.targets[this.phaseTargetIndex].health;
                var hpTexts = [];
                for (j = 0; j < health.length; j++) {
                    hpTexts[j] = health[j] + "% hp";
                }
                var res = {
                    x: this.phase.times,
                    text: hpTexts,
                    mode: 'lines',
                    line: {
                        shape: 'spline',
                        dash: 'dashdot'
                    },
                    hoverinfo: 'text',
                    name: this.target.name + ' health',
                    yaxis: dpsY
                };
                this.data.push(res);
            }
            this.targetOffset++;
            this.data.push({
                x: this.phase.times,
                y: [],
                mode: 'lines',
                line: {
                    shape: 'spline'
                },
                yaxis: dpsY,
                hoverinfo: 'name+y',
                name: 'Total DPS'
            });
            this.layout = getActorGraphLayout(images, this.light ? '#495057' : '#cccccc');
            computePhaseMarkups(this.layout.shapes, this.layout.annotations, this.phase, this.light ? '#495057' : '#cccccc');
        },
        computed: {
            target: function() {
                return logData.targets[this.targetindex];
            },
            phase: function() {
                return logData.phases[this.phaseindex];
            },
            graph: function() {
                return graphData.phases[this.phaseindex];
            },
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
                var dpsData = this.graph.targets[this.phaseTargetIndex].dps;
                if (this.dpsmode < 3) {
                    var lim = (this.dpsmode === 0 ? 0 : (this.dpsmode === 1 ? 10 : 30));
                    res = computeTargetDPS(this.target, dpsData, lim, null, cacheID + '-' + this.phaseindex, this.phase.needsLastPoint ? this.phase.end - this.phase.start : 0);
                } else {
                    res = computeTargetDPS(this.target, dpsData, 0, this.computePhaseBreaks, cacheID + '-' + this.phaseindex, this.phase.needsLastPoint ? this.phase.end - this.phase.start : 0);
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
        template: `${tmplTargetTabGraph}`
    });
};
