/*jshint esversion: 6 */

var roundingComponent = {
    methods: {
        round: function (value) {
            if (isNaN(value)) {
                return 0;
            }
            return Math.round(value);
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

var compileCommons = function () {
    Vue.component('rotation-legend-component', {
        template: `${tmplRotationLegend}`
    });
    
    Vue.component('target-data-component', {
        props: ['targetid'],
        template: `${tmplTargetData}`,
        computed: {
            target: function() {
                return logData.targets[this.targetid];
            }
        }
    });

    Vue.component('dmgtaken-component', {
        props: ['actor', 'tableid',
            'phaseindex'
        ],
        template: `${tmplDamageTaken}`,
        computed: {
            dmgtaken: function () {
                return this.actor.details.dmgDistributionsTaken[this.phaseindex];
            }
        },
    });

    Vue.component("graph-component", {
        props: ['id', 'layout', 'data'],
        template: '<div :id="id" class="d-flex flex-row justify-content-center"></div>',
        mounted: function () {
            var div = document.querySelector(this.queryID);
            Plotly.react(div, this.data, this.layout, {showEditInChartStudio: true, plotlyServerURL: "https://chart-studio.plotly.com"});
            var _this = this;
            div.on('plotly_animated', function () {
                Plotly.relayout(div, _this.layout);
            });
        },
        computed: {
            queryID: function () {
                return "#" + this.id;
            }
        },
        watch: {
            layout: {
                handler: function () {
                    var div = document.querySelector(this.queryID);
                    var duration = 1000;
                    Plotly.animate(div, {
                        data: this.data
                    }, {
                        transition: {
                            duration: duration,
                            easing: 'cubic-in-out'
                        },
                        frame: {
                            duration: 1.5 * duration
                        }
                    });
                },
                deep: true
            }
        }
    });
    Vue.component("buff-table-component", {
        props: ["buffs", "playerdata", "generation", "condition", "sums", "id", "playerindex"],
        template: `${tmplBuffTable}`,
        methods: {
            getAvgTooltip: function (avg) {
                if (avg) {
                    return (
                        "Average number of " +
                        (this.condition ? "conditions: " : "boons: ") +
                        avg
                    );
                }
                return false;
            },
            getCellTooltip: function (buff, val, uptime) {
                if (val instanceof Array) {
                    if (!uptime && this.generation && (val[1] > 0 || val[2] > 0 || val[3] > 0 || val[4] > 0)) {
                        var res = (val[1] || 0) + (buff.stacking ? "" : "%") + " with overstack";
                        if (val[4] > 0) {
                            res += "<br>";
                            res += val[4] + (buff.stacking ? "" : "%") + " by extension";
                        }
                        if (val[2] > 0) {
                            res += "<br>";
                            res += val[2] + (buff.stacking ? "" : "%") + " wasted";
                        }
                        if (val[5] > 0) {
                            res += "<br>";
                            res += val[5] + (buff.stacking ? "" : "%") + " extended";
                        }
                        if (val[3] > 0) {
                            res += "<br>";
                            res += val[3] + (buff.stacking ? "" : "%") + " extended by unknown source";
                        }
                        return res;
                    } else if (buff.stacking && val[1] > 0) {
                        return "Uptime: " + val[1] + "%";
                    } else {
                        return false;
                    }
                }
                return false;
            },
            getCellValue: function (buff, val) {
                var value = val;
                var force = false;
                if (val instanceof Array) {
                    value = val[0];
                    force = this.generation && (val[1] > 0 || val[2] > 0 ||val[3] > 0 || val[4] > 0);
                }
                if (value > 0 || force) {
                    return buff.stacking ? value : value + "%";
                }
                return "-";
            }
        },
        computed: {
            tooltipExpl: function () {
                return `<ul style='text-align:left;margin-block-end: 0.3em;'>
                        <li>The value shown in the row is "generation + extensions you are the source"</li>
                        <li>With overstack is "generation + extensions you are the source + stacks that couldn't make into the queue/stacks"</li>
                        <li>By extension is "extensions you are the source"</li>
                        <li>Waste is "stacks that were overriden/cleansed". If you have high waste values that could mean there is an issue with your composition as someone may be overriding your stacks non-stop.</li>
                        <li>Extended by unknown source is the extension value for which we were unable to find an src, not included in generation.</li>
                        <li>Extended is "extended by unknown source + extended by known source other than yourself". Not included in generation. This value is just here to indicate if you are a good seed.</li>
                        </ul>`
            }
        },
        mounted() {
            initTable("#" + this.id, 0, "asc");
        },
        updated() {
            updateTable("#" + this.id);
        }
    });

    Vue.component("damagedist-table-component", {
        props: ["dmgdist", "tableid", "actor", "isminion", "istarget", "phaseindex"],
        template: `${tmplDamageDistTable}`,
        data: function () {
            return {
                sortdata: {
                    order: "desc",
                    index: 2
                }
            };
        },
        mixins: [roundingComponent],
        mounted() {
            var _this = this;
            initTable(
                "#" + this.tableid,
                this.sortdata.index,
                this.sortdata.order,
                function () {
                    var order = $("#" + _this.tableid)
                        .DataTable()
                        .order();
                    _this.sortdata.order = order[0][1];
                    _this.sortdata.index = order[0][0];
                }
            );
        },
        beforeUpdate() {
            $("#" + this.tableid)
                .DataTable()
                .destroy();
        },
        updated() {
            var _this = this;
            initTable(
                "#" + this.tableid,
                this.sortdata.index,
                this.sortdata.order,
                function () {
                    var order = $("#" + _this.tableid)
                        .DataTable()
                        .order();
                    _this.sortdata.order = order[0][1];
                    _this.sortdata.index = order[0][0];
                }
            );
        },
        methods: {
            getSkill: function (isBoon, id) {
                return findSkill(isBoon, id);
            }
        },
        computed: {
            phase: function () {
                return logData.phases[this.phaseindex];
            }
        }
    });
};
