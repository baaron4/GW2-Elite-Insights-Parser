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

var colSliderComponent = function (perpage) {
  return {
    data: function () {
      return {
        colStructure: {
            offset: 0,
            perpage: perpage
        },
      };
    },
    methods: {
      isInColPage: function (index) {
        return (
          index >= this.colStructure.offset &&
          index < this.colStructure.offset + this.colStructure.perpage
        );
      },
    },
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
