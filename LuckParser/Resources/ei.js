/*jshint esversion: 6 */

var mainComponent = null;

window.onload = function () {
    // make some additional variables reactive
    var i;
    var simpleLogData = {
        phases: [],
        players: [],
        targets: []
    };
    for (i = 0; i < logData.phases.length; i++) {
        var phase = logData.phases[i];
        var times = [];
        var dur = phase.end - phase.start;
        var floorDur = Math.floor(dur);
        phase.needsLastPoint = dur > floorDur + 1e-3;
        for (var j = 0; j <= floorDur; j++) {
            times.push(j);
        }
        if (phase.needsLastPoint) {
            times.push(phase.end - phase.start);
        }
        phase.times = times;
        simpleLogData.phases.push({
            active: i === 0,
            focus: null
        });
    }
    for (i = 0; i < logData.targets.length; i++) {
        simpleLogData.targets.push({
            active: true
        });
        logData.targets[i].id = i;
        logData.targets[i].dpsGraphCache = new Map();
    }
    for (i = 0; i < logData.players.length; i++) {
        simpleLogData.players.push({
            active: false
        });
        var playerData = logData.players[i];
        playerData.dpsGraphCache = new Map();
        playerData.icon = urls[playerData.profession];
        playerData.id = i;
    }

    var layout = compileLayout();
    compileCommons();
    compileHeader();
    compileGeneralStats();
    compileBuffStats();
    compileMechanics();
    compileGraphs();
    compilePlayerTab();
    compileTargetTab();
    if (typeof compileCombatReplay !== "undefined") {
        compileCombatReplay();
    }
    mainComponent = new Vue({
        el: "#content",
        data: {
            logdata: simpleLogData,
            layout: layout,
            datatypes: DataTypes,
            light: logData.lightTheme,
            mode: 0,
            animate: false,
            animationStatus: null
        },
        methods: {
            switchCombatReplayButtons: function (from, to) {
                var combatReplay = $('#combat-replay');
                if (combatReplay) {
                    var buttons = combatReplay.find('.' + from);
                    buttons.addClass(to).removeClass(from);
                }
            },
            switchTheme: function(state) {
                if (state === this.light) {
                    return;
                }
                var style = this.light ? 'yeti' : 'slate';
                this.light = state;
                var newStyle = this.light ? 'yeti' : 'slate';
                document.body.classList.remove("theme-"+style);
                document.body.classList.add("theme-"+newStyle);
                var theme = document.getElementById('theme');
                theme.href = themes[newStyle];
                this.switchCombatReplayButtons(this.light ? 'btn-dark' : 'btn-light', this.light ? 'btn-light' : 'btn-dark');     
            },
            changeMode: function(iMode) {
                if (this.mode === iMode) {
                    return;
                }
                var oldMode = this.mode;
                this.mode = iMode;
                if (this.mode !== 1 && oldMode === 1) {
                    this.animate = this.animator && this.animator.animation !== null;
                    this.animator && this.animator.stopAnimate();
                } else if (this.mode === 1 && this.animate) {
                    this.animator && this.animator.startAnimate();
                }
            },
        },
        computed: {
            animator: function () {
                return this.animationStatus ? animator : null;
            },
            activePhase: function () {
                var phases = this.logdata.phases;
                for (var i = 0; i < phases.length; i++) {
                    if (phases[i].active) {
                        return i;
                    }
                }
            },
            dataType: function () {
                var cur = this.layout.tabs;
                while (cur !== null) {
                    for (var i = 0; i < cur.length; i++) {
                        var tab = cur[i];
                        if (tab.active) {
                            if (tab.layout === null) {
                                return tab.dataType;
                            } else {
                                cur = tab.layout.tabs;
                                break;
                            }
                        }
                    }
                }
                return -1;
            },
            activePlayer: function () {
                var players = this.logdata.players;
                for (var i = 0; i < players.length; i++) {
                    if (players[i].active) {
                        return i;
                    }
                }
                return -1;
            },
            activePhaseTargets: function () {
                var res = [];
                var activePhase = logData.phases[this.activePhase];
                for (var i = 0; i < activePhase.targets.length; i++) {
                    var target = this.logdata.targets[activePhase.targets[i]];
                    if (target.active) {
                        res.push(i);
                    }
                }
                return res;
            }
        },
        mounted() {
            var element = document.getElementById("loading");
            element.parentNode.removeChild(element);
            if (this.light) {
                this.switchCombatReplayButtons('btn-dark', 'btn-light');
            }
        }
    });
    $("body").tooltip({
        selector: "[data-original-title]",
        html: true
    });
};
