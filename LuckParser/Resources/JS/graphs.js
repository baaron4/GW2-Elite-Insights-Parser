/*jshint esversion: 6 */

var compileGraphs = function () {
    Vue.component("graph-stats-component", {
        props: ["activetargets", "phaseindex", 'playerindex', 'light'],
        template: `${tmplGraphStats}`,
        data: function () {
            return {
                wvw: !!logData.wvw,
                mode: logData.wvw ? 0 : 1
            };
        },
        computed: {
            phases: function() {
                return logData.phases;
            }
        }
    });
    Vue.component("dps-graph-component", {
        props: ["activetargets", 'mode', 'phaseindex', 'playerindex', 'light'],
        template: `${tmplDPSGraph}`,
        data: function () {
            return {
                dpsmode: 0,
                layout: {},
                data: [],
                dpsCache: new Map(),
                dataCache: new Map()
            };
        },
        created: function () {
            // layout - constant during whole lifetime
            var i, j;
            var textColor = this.light ? '#495057' : '#cccccc';
            this.layout = {
                yaxis: {
                    title: 'DPS',
                    fixedrange: false,
                    rangemode: 'tozero',
                    gridcolor: textColor,
                    color: textColor
                },
                xaxis: {
                    title: 'Time(sec)',
                    color: textColor,
                    gridcolor: textColor,
                    xrangeslider: {}
                },
                hovermode: 'compare',
                legend: {
                    orientation: 'h',
                    font: {
                        size: 15
                    },
                    y: -0.1
                },
                font: {
                    color: textColor
                },
                paper_bgcolor: 'rgba(0,0,0,0)',
                plot_bgcolor: 'rgba(0,0,0,0)',
                displayModeBar: false,
                shapes: [],
                annotations: [],
                autosize: true,
                width: 1100,
                height: 800,
                datarevision: new Date().getTime(),
            };
            computePhaseMarkups(this.layout.shapes, this.layout.annotations, this.phase, textColor);
            // constant part of data
            // dps
            var data = this.data;
            var player;
            for (i = 0; i < logData.players.length; i++) {
                var pText = [];
                player = logData.players[i];
                for (j = 0; j < this.graph.players[i].total.length; j++) {
                    pText.push(player.name);
                }
                data.push({
                    x: this.phase.times,
                    y: [],
                    mode: 'lines',
                    line: {
                        shape: 'spline',
                        color: player.colTarget,
                        width: i === this.playerindex ? 5 : 2
                    },
                    text: pText,
                    hoverinfo: 'y+text+x',
                    name: player.name + ' DPS',
                });
            }
            data.push({
                x: this.phase.times,
                mode: 'lines',
                line: {
                    shape: 'spline'
                },
                hoverinfo: 'name+y+x',
                visible: 'legendonly',
                name: 'All Player Dps'
            });
            // targets health
            computeTargetHealthData(this.graph, logData.targets, this.phase, this.data, null, this.phase.times);
            // mechanics
            for (i = 0; i < graphData.mechanics.length; i++) {
                var mech = graphData.mechanics[i];
                var mechData = mechanicMap[i];
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
                    hoverinfo: 'text+x'
                };
                var time, pts, k;
                if (mechData.enemyMech) {
                    for (j = 0; j < mech.points[this.phaseindex].length; j++) {
                        pts = mech.points[this.phaseindex][j];
                        var tarId = this.phase.targets[j];
                        if (tarId >= 0) {
                            var target = logData.targets[tarId];
                            for (k = 0; k < pts.length; k++) {
                                time = pts[k];
                                chart.x.push(time);
                                chart.text.push(time + 's: ' + target.name);
                            }
                        } else {
                            for (k = 0; k < pts.length; k++) {
                                time = pts[k][0];
                                chart.x.push(time);
                                chart.text.push(time + 's: ' + pts[k][1]);
                            }
                        }
                    }
                } else {
                    for (j = 0; j < mech.points[this.phaseindex].length; j++) {
                        pts = mech.points[this.phaseindex][j];
                        player = logData.players[j];
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
        watch: {
            playerindex: {
                handler: function () {
                    for (var i = 0; i < logData.players.length; i++) {
                        this.data[i].line.width = i === this.playerindex ? 5 : 2;
                    }
                    this.layout.datarevision = new Date().getTime();
                },
                deep: true
            },
            light: {
                handler: function () {
                    var textColor = this.light ? '#495057' : '#cccccc';
                    this.layout.yaxis.gridcolor = textColor;
                    this.layout.yaxis.color = textColor;
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
        computed: {
            graphid: function () {
                return 'dpsgraph-' + this.phaseindex;
            },
            phase: function() {
                return logData.phases[this.phaseindex];
            },
            graph: function() {
                return graphData.phases[this.phaseindex];
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
                        var subPhase = logData.phases[this.phase.subPhases[i]];
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
            computeDPS: function (lim, phasebreaks, cacheID) {
                var maxDPS = {
                    total: 0,
                    cleave: 0,
                    target: 0
                };
                var allDPS = {                   
                    total: [],
                    cleave: [],
                    target: []
                };
                var playerDPS = [];
                for (var i = 0; i < logData.players.length; i++) {
                    var data = computePlayerDPS(logData.players[i], this.graph.players[i], lim, phasebreaks, 
                            this.activetargets, cacheID + '-' + this.phaseindex, this.phase.needsLastPoint ? this.phase.end - this.phase.start : 0);
                    playerDPS.push(data.dps);
                    maxDPS.total = Math.max(maxDPS.total, data.maxDPS.total);
                    maxDPS.cleave = Math.max(maxDPS.cleave, data.maxDPS.cleave);
                    maxDPS.target = Math.max(maxDPS.target, data.maxDPS.target);
                    for (var j = 0; j < data.dps.total.length; j++) {
                        allDPS.total[j] = (allDPS.total[j] || 0) + data.dps.total[j];
                        allDPS.cleave[j] = (allDPS.cleave[j] || 0) + data.dps.cleave[j];
                        allDPS.target[j] = (allDPS.target[j] || 0) + data.dps.target[j];
                    }
                }
                
                return {
                    allDPS: allDPS,
                    playerDPS: playerDPS,
                    maxDPS: maxDPS,
                };
            },
            computeDPSData: function () {
                var cacheID = this.dpsmode + '-';
                cacheID += getTargetCacheID(this.activetargets);
                if (this.dpsCache.has(cacheID)) {
                    return this.dpsCache.get(cacheID);
                }
                var res;
                if (this.dpsmode < 3) {
                    var lim = (this.dpsmode === 0 ? 0 : (this.dpsmode === 1 ? 10 : 30));
                    res = this.computeDPS(lim, null, cacheID);
                } else {
                    res = this.computeDPS(0, this.computePhaseBreaks, cacheID);
                }
                this.dpsCache.set(cacheID, res);
                return res;
            },
            computeDPSRelatedData: function () {
                var cacheID = this.dpsmode + '-' + this.mode + '-';
                var i, j;
                cacheID += getTargetCacheID(this.activetargets);
                if (this.dataCache.has(cacheID)) {
                    return this.dataCache.get(cacheID);
                }
                var res = [];
                var dpsData = this.computeDPSData();
                var offset = 0;
                for (i = 0; i < logData.players.length; i++) {
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
                for (i = 0; i < graphData.mechanics.length; i++) {
                    var mech = graphData.mechanics[i];
                    var mechData = mechanicMap[i];
                    var chart = [];
                    res[offset++] = chart;
                    var time, pts, k, ftime, y, yp1;
                    if (mechData.enemyMech) {
                        for (j = 0; j < mech.points[this.phaseindex].length; j++) {
                            pts = mech.points[this.phaseindex][j];
                            var tarId = this.phase.targets[j];
                            if (tarId >= 0) {
                                for (k = 0; k < pts.length; k++) {
                                    time = pts[k];
                                    ftime = Math.floor(time);
                                    y = hps[j][ftime];
                                    yp1 = hps[j][ftime + 1];
                                    chart.push(this.interpolatePoint(ftime, ftime + 1, y, yp1, time));
                                }
                            } else {
                                for (k = 0; k < pts.length; k++) {
                                    chart.push(maxDPS * 0.5);
                                }
                            }
                        }
                    } else {
                        for (j = 0; j < mech.points[this.phaseindex].length; j++) {
                            pts = mech.points[this.phaseindex][j];
                            for (k = 0; k < pts.length; k++) {
                                time = pts[k];
                                ftime = Math.floor(time);
                                y = res[j][ftime];
                                yp1 = res[j][ftime + 1];
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