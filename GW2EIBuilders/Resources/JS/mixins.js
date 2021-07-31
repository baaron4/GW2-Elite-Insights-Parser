"use strict";

var numberComponent = {
    methods: {
        // https://stackoverflow.com/questions/16637051/adding-space-between-numbers
        integerWithSpaces: function(x) {
            return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, " ");
        },
        round: function (value) {
            if (isNaN(value)) {
                return 0;
            }
            return Math.round(value);
        },
        round1: function (value) {
            if (isNaN(value)) {
                return 0;
            }
            var mul = 10;
            return Math.round(mul * value) / mul;
        },
        round2: function (value) {
            if (isNaN(value)) {
                return 0;
            }
            var mul = 100;
            return Math.round(mul * value) / mul;
        },
        round3: function (value) {
            if (isNaN(value)) {
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
