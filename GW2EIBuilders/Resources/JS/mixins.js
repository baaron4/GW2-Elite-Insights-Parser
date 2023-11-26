"use strict";

var numberComponent = {
    methods: {
        // https://stackoverflow.com/questions/16637051/adding-space-between-numbers
        integerWithSpaces: function(x) {
            return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, " ");
        },
        round: function (value) {
            if (isNaN(value) || !isFinite(value)) {
                return 0;
            }
            return Math.round(value);
        },
        round1: function (value) {
            if (isNaN(value) || !isFinite(value)) {
                return 0;
            }
            var mul = 10;
            return Math.round(mul * value) / mul;
        },
        round2: function (value) {
            if (isNaN(value) || !isFinite(value)) {
                return 0;
            }
            var mul = 100;
            return Math.round(mul * value) / mul;
        },
        round3: function (value) {
            if (isNaN(value) || !isFinite(value)) {
                return 0;
            }
            var mul = 1000;
            return Math.round(mul * value) / mul;
        }
    }
};

var damageGraphComponent = {
    data: function() {
        return {
            graphdata: {
                dpsmode: 0,
                graphmode: logData.wvw ? GraphType.Damage : GraphType.DPS,
                damagemode: DamageType.All,
            }
        };
    },
};

var graphComponent = {
    data: function () {
        return {
            layout: {},
            dpsCache: new Map(),
            dataCache: new Map(),
        };
    },
    computed: {       
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
        phase: function () {
            return logData.phases[this.phaseindex];
        },
        graph: function () {
            return graphData.phases[this.phaseindex];
        },
    },
    methods: {
            updateVisibily: function (images, x0, x1) {
                var redraw = false;
                for (var i = 0; i < images.length; i++) {
                    var image = images[i];
                    var old = image.visible;
                    image.visible = typeof x0 === "undefined" || ((image.x <= x1+10 && image.x >= x0 - 10) && (x1 - x0) < 75);
                    redraw = redraw || image.visible !== old;
                }
                return redraw;
            },
            updateVisibilyInQuad: function (images, x0, x1, y0, y1) {
                var redraw = false;
                for (var i = 0; i < images.length; i++) {
                    var image = images[i];
                    var old = image.visible;
                    image.visible = typeof x0 === "undefined" || (((image.x <= x1+10 && image.x >= x0 - 10) && (x1 - x0) < 75) && ((image.y <= y1+10 && image.y >= y0 - 10) && (y1 - y0) < 75)) ;
                    redraw = redraw || image.visible !== old;
                }
                return redraw;
            },
    }
};

var timeRefreshComponent = {
    props: ["time"],
    data: function () {
        return {
            refreshTime: 0
        };
    },
    computed: {
        timeToUse: function () {
            if (animator) {
                var animated = animator.animation !== null;
                if (animated) {
                    var speed = animator.speed;
                    if (Math.abs(this.time - this.refreshTime) > speed * 64) {
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

var sortedTableComponent = {
    methods:  {       
        sortByBase: function(sortdata, key, index) {
            index = index >= 0 ? index : -1;
            if (sortdata.key !== key || index !== sortdata.index) {
                sortdata.order = "asc";
            } else {
                sortdata.order = sortdata.order === "asc" ? "desc" : "asc";
            }
            sortdata.key = key;
            sortdata.index = index;
        },
        getHeaderClassBase: function(sortdata, key, index) {
            index = index >= 0 ? index : -1;
            if (sortdata.key === key && sortdata.index === index) {
                if (sortdata.order === "asc") {
                    return {"sorted_asc" : true};
                } else {
                    return {"sorted_desc": true};
                }
            };
            return {'sorted': true};
        },
        getBodyClassBase: function(sortdata, key, index) {
            index = index >= 0 ? index : -1;
            return {'sorted': sortdata.key === key && sortdata.index === index};
        },
    }
};

var sortedDistributionComponent = {
    methods: {    
        sortBy: function(key, index, func) {
            this.sortByBase(this.sortdata, key, index);
            this.sortdata.sortFunc = func ? func : null;
        },
        getHeaderClass: function(key, index) {
            return this.getHeaderClassBase(this.sortdata, key, index);
        },
        getBodyClass: function(key, index) {
            var classes = this.getBodyClassBase(this.sortdata, key, index);
            return classes;
        },     
        getCastBodyClass: function(key, index, data) {
            var res = this.getBodyClass(key, index);
            var innacurate = {higherOrEqual: (!this.getSkill(data).condi && this.getCast(data)) && this.showInequality(data)};
            Object.assign(res, innacurate);
            return res;
        },
        getHitsPerCastBodyClass: function(key, index, data) {
            var res = this.getBodyClass(key, index);
            var innacurate = {lowerOrEqual: (!this.getSkill(data).condi && this.getConnectedHits(data) && this.getCast(data)) && this.showInequality(data)};
            Object.assign(res, innacurate);
            return res;
        },
        sortData: function(rows) {
            var order = this.sortdata.order === "asc" ? 1 : -1;
            switch (this.sortdata.key) {
                case "Skill":
                    rows.sort((x,y) => order * (this.getSkill(x).name.localeCompare(this.getSkill(y).name)));
                    break;
                case "Data":
                    var sortFunc = x => {
                        var value = this.sortdata.sortFunc(x);
                        if (value === 0) {
                            if (order > 0) {
                                value = 1e15;
                            } else {
                                value = -1e15;
                            }
                        }
                        return value;
                    };
                    rows.sort((x,y) => order * (sortFunc(x) - sortFunc(y)));
                    break;
                default:
                    return null;
                    break;
            }
            return rows;
        },
    }
};

var colSliderComponent = function (perpage, names = null) {
    let data;
    let methods;
    if (names !== null) {
        data = function () {
            let res = {};
            for (let i = 0; i < names.length; i++) {
                res[names[i] + "ColStructure"] = {
                    offset: 0,
                    perpage: perpage,
                };
            }
            return res;
        };
        methods = {};
        for (let i = 0; i < names.length; i++) {
            methods["isIn" + names[i][0].toUpperCase() + names[i].slice(1) + "ColPage"] = function (index) {
                return (
                    index >= this[names[i] + "ColStructure"].offset &&
                    index < this[names[i] + "ColStructure"].offset + this[names[i] + "ColStructure"].perpage
                );
            }
        }
    } else {
        data = function () {
            return {
                colStructure: {
                    offset: 0,
                    perpage: perpage,
                },
            };
        };
        methods = {
            isInColPage: function (index) {
                return (
                    index >= this.colStructure.offset &&
                    index < this.colStructure.offset + this.colStructure.perpage
                );
            },
        }
    }
    return {
        data: data,
        methods: methods
    };
};

var rowSliderComponent = function (perpage) {
    return {
      data: function () {
        return {
          rowStructure: {
              offset: 0,
              perpage: perpage
          },
        };
      },
      methods: {
        isInRowPage: function (index) {
          return (
            index >= this.rowStructure.offset &&
            index < this.rowStructure.offset + this.rowStructure.perpage
          );
        },
      },
    };
  };

// Requires graphComponent and damageGraphComponent
var targetTabGraphComponent = {   
    data: function () {
        return {
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
        this.data = [];
        this.targetOffset += computeRotationData(this.rotationData, images, this.data, this.phase, this.target, 1);
        var oldOffset = this.targetOffset;
        this.targetOffset += computeBuffData(this.boonGraph, this.data);
        var hasBuffs = oldOffset !== this.targetOffset;
        this.targetOffset += addTargetLayout(this.data, this.target, this.breakbarStates, "breakbar", "breakbar", this.phase.breakbarPhase);
        this.targetOffset += addTargetLayout(this.data, this.target, this.barrierStates, "barrier", "barrier", false);
        this.targetOffset += addTargetLayout(this.data, this.target, this.healthStates, "hp", "health", true);
        this.data.push({
            x: this.phase.times,
            y: [],
            mode: 'lines',
            line: {
                shape: 'spline'
            },
            yaxis: 'y3',
            hoverinfo: 'name+y+x',
            name: 'Total'
        });
        this.layout = getActorGraphLayout(images, this.light ? '#495057' : '#cccccc', hasBuffs);
        computePhaseMarkups(this.layout.shapes, this.layout.annotations, this.phase, this.light ? '#495057' : '#cccccc');
        this.updateVisibily(this.layout.images, this.phase.start, this.phase.end);
    },
    activated: function () {
        var div = document.getElementById(this.graphid);
        var layout = this.layout;
        var images = layout.images;
        var _this = this;
        div.on('plotly_relayout', function (evt) {
            var x0 = layout.xaxis.range[0];
            var x1 = layout.xaxis.range[1];
            //console.log("re-layout " + x0 + " " + x1);
            if (_this.updateVisibily(images, x0, x1)) {
                layout.datarevision = new Date().getTime();
                //console.log("re-drawing");
            }
        });
    },
    computed: {
        healthStates: function () {
            return this.graph.targets[this.phaseTargetIndex].healthStates;
        },
        breakbarStates: function () {
            return this.graph.targets[this.phaseTargetIndex].breakbarPercentStates;
        },
        barrierStates: function () {
            return this.graph.targets[this.phaseTargetIndex].barrierStates;
        },
        target: function () {
            return logData.targets[this.targetindex];
        },
        phaseTargetIndex: function () {
            return this.phase.targets.indexOf(this.targetindex);
        },
        damageGraphName: function () {
            switch (this.graphdata.damagemode) {
                case DamageType.All:
                    return "total";
                case DamageType.Power:
                    return "totalPower";
                case DamageType.Condition:
                    return "totalCondition";
                default:
                    throw new Error("unknown enum in damage graph name");
            }
        },
        graphname: function () {
            var name = getDamageGraphName(this.graphdata.damagemode, this.graphdata.graphmode);
            switch (this.graphdata.dpsmode) {
                case 0:
                    name = "Full " + name;
                    break;
                case -1:
                    name = "Phase " + name;
                    break;
                default:
                    name = this.graphdata.dpsmode + "s " + name;
                    break;
            }
            return name;
        },
        computeData: function () {
            this.layout.datarevision = new Date().getTime();
            this.layout.yaxis3.title = graphTypeEnumToString(this.graphdata.graphmode);
            var res = this.data;
            var data = this.computeDPSRelatedData();
            for (var i = 0; i < data.length; i++) {
                this.data[this.targetOffset - i].y = data[i];
            }
            return res;
        },
        rotationData: function() {
            return this.target.details.rotation[this.phaseindex];
        }
    },
    methods: {
        computeDPSData: function () {
            var cacheID = getDPSGraphCacheID(this.graphdata.dpsmode, this.graphdata.damagemode, this.graphdata.graphmode, [], this.phaseindex, null);
            if (this.dpsCache.has(cacheID)) {
                return this.dpsCache.get(cacheID);
            }
            //var before = performance.now();
            var res;
            var damageData = this.graph.targets[this.phaseTargetIndex][this.damageGraphName];
            if (this.graphdata.dpsmode >= 0) {
                res = computeTargetDPS(this.target, damageData, this.graphdata.dpsmode, null, cacheID, this.phase.times, this.graphdata.graphmode);
            } else {
                res = computeTargetDPS(this.target, damageData, 0, this.computePhaseBreaks, cacheID, this.phase.times, this.graphdata.graphmode);
            }
            this.dpsCache.set(cacheID, res);
            return res;
        },
        computeDPSRelatedData: function () {
            var cacheID = getDPSGraphCacheID(this.graphdata.dpsmode, this.graphdata.damagemode, this.graphdata.graphmode, [], this.phaseindex, null);
            if (this.dataCache.has(cacheID)) {
                return this.dataCache.get(cacheID);
            }
            var dpsData = this.computeDPSData();
            var res = [dpsData.dps];
            addPointsToGraph(res, this.healthStates, dpsData.maxDPS);               
            addPointsToGraph(res, this.barrierStates, dpsData.maxDPS);
            addPointsToGraph(res, this.breakbarStates, dpsData.maxDPS);
            this.dataCache.set(cacheID, res);
            return res;
        },
    }
}
