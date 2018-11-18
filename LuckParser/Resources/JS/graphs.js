/*jshint esversion: 6 */

var compileGraphs = function () {
    Vue.component("graph-stats-component", {
        props: ["phases", "activetargets", "targets", "players", "phaseid", 'selectedplayerindex', 'light'],
        template: "#tmplGraphStats",
        data: function () {
            return {
                mode: 1,
                graphdata: graphData
            };
        }
    });
    Vue.component("dps-graph-component", {
        props: ["phases", "activetargets", "targets", "players", 'mechanics', 'graph', 'mode', 'phase', 'phaseid', 'selectedplayerindex', 'light'],
        template: "#tmplDPSGraph",
        data: function () {
            return {
                dpsmode: 0,
                layout: {},
                data: [],
                dpsCache: new Map(),
                dataCache: new Map(),
                mechanicsData: mechanicMap,
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
            for (i = 0; i < this.players.length; i++) {
                player = this.players[i];
                data.push({
                    y: [],
                    mode: 'lines',
                    line: {
                        shape: 'spline',
                        color: player.colTarget,
                        width: i === this.selectedplayerindex ? 5 : 2
                    },
                    name: player.name + ' DPS',
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
            computeTargetHealthData(this.graph, this.targets, this.phase, this.data);
            // mechanics
            for (i = 0; i < this.mechanics.length; i++) {
                var mech = this.mechanics[i];
                var mechData = this.mechanicsData[i];
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
        watch: {
            selectedplayerindex: {
                handler: function () {
                    for (var i = 0; i < this.players.length; i++) {
                        this.data[i].line.width = i === this.selectedplayerindex ? 5 : 2;
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
                var playerDPS = [];
                for (var i = 0; i < this.players.length; i++) {
                    computePlayerDPS(i, this.graph, playerDPS, maxDPS, allDPS, lim, phasebreaks, this.activetargets);
                }
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
                    targetsID = targetsID << (this.activetargets[i] + 1);
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
                for (i = 0; i < this.activetargets.length; i++) {
                    targetsID = targetsID << (this.activetargets[i] + 1);
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
                for (i = 0; i < this.mechanics.length; i++) {
                    var mech = this.mechanics[i];
                    var mechData = this.mechanicsData[i];
                    chart = [];
                    res[offset++] = chart;
                    var time, pts, k, ftime, y, yp1;
                    if (mechData.enemyMech) {
                        for (j = 0; j < mech.points[this.phaseid].length; j++) {
                            pts = mech.points[this.phaseid][j];
                            var tarId = this.phase.targets[j];
                            if (tarId >= 0) {
                                target = this.targets[tarId];
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
                        for (j = 0; j < mech.points[this.phaseid].length; j++) {
                            pts = mech.points[this.phaseid][j];
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