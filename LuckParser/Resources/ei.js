/*jshint esversion: 6 */

var onLoad = window.onload;

window.onload = function () {
    if (onLoad) {
        onLoad();
    }
    // make some additional variables reactive
    var i;
    for (i = 0; i < logData.phases.length; i++) {
        logData.phases[i].active = i === 0;
        logData.phases[i].focus = null;
    }
    for (i = 0; i < logData.targets.length; i++) {
        logData.targets[i].active = true;
    }
    for (i = 0; i < logData.players.length; i++) {
        var playerData = logData.players[i];
        playerData.active = false;
        playerData.icon = urls[playerData.profession];
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
    new Vue({
        el: "#content",
        data: {
            logdata: logData,
            layout: layout,
            datatypes: DataTypes,
            combatreplay: logData.combatReplay,
            light: logData.lightTheme,
            mode: 0
        },
        methods: {
            switchCombatReplayButtons: function(from, to) {          
                var combatReplay = $('#combat-replay');
                if (combatReplay) {
                    var buttons = combatReplay.find('.'+from);
                    buttons.addClass(to).removeClass(from);
                }
            },
            switchTheme: function(state) {
                if (state === this.light) {
                    return;
                }
                var style = this.light ? 'cosmo' : 'slate';
                this.light = state;
                var newStyle = this.light ? 'cosmo' : 'slate';
                document.body.classList.remove("theme-"+style);
                document.body.classList.add("theme-"+newStyle);
                var theme = document.getElementById('theme');
                theme.href = themes[newStyle];              
                this.switchCombatReplayButtons(this.light ? 'btn-dark' : 'btn-light', this.light ? 'btn-light' : 'btn-dark');
            }
        },
        computed: {
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
                var targets = this.logdata.targets;
                var activePhase = this.logdata.phases[this.activePhase];
                for (var i = 0; i < activePhase.targets.length; i++) {
                    var target = targets[activePhase.targets[i]];
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
