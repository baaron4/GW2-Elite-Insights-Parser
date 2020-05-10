/*jshint esversion: 6 */
var apiRenderServiceOkay = true;

var mainLoad = function () {
    // make some additional variables reactive
    var i;
    var simpleLogData = {
        phases: [],
        players: [],
        targets: []
    };
    for (i = 0; i < logData.phases.length; i++) {
        var phase = logData.phases[i];
        phase.durationS = phase.duration / 1000.0
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
            focus: -1
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
        var playerData = logData.players[i];
        simpleLogData.players.push({
            active: !!playerData.isPoV
        });
        playerData.dpsGraphCache = new Map();
        playerData.icon = urls[playerData.profession];
        playerData.id = i;
    }

    var layout = compileLayout();
    compileCommons();
    compileHeader();
    compileGeneralStats();
    compileDamageModifiers();
    compileBuffStats();
    compileMechanics();
    compileGraphs();
    compilePlayerTab();
    compileTargetTab();
    if (logData.crData) {
        compileCombatReplay();
        compileCombatReplayUI();
    }
    new Vue({
        el: "#content",
        data: {
            logdata: simpleLogData,
            layout: layout,
            datatypes: DataTypes,
            light: typeof (window.theme) !== "undefined" ? (window.theme === 'yeti') : logData.lightTheme,
            mode: 0,
            cr: !!logData.crData,
			buffMode: 0
        },
        methods: {
            switchTheme: function (state) {
                if (state === this.light) {
                    return;
                }
                var style = this.light ? 'yeti' : 'slate';
                this.light = state;
                var newStyle = this.light ? 'yeti' : 'slate';
                document.body.classList.remove("theme-" + style);
                document.body.classList.add("theme-" + newStyle);
                if (storeTheme) storeTheme(newStyle);
                var theme = document.getElementById('theme');
                theme.href = themes[newStyle];
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
        }
    });
    $("body").tooltip({
        selector: "[data-original-title]",
        html: true
    });
};

window.onload = function () {
    // trick from
    var img = document.createElement("img");
    img.style.display = "none";
    document.body.appendChild(img);
    img.onload = function () {
        mainLoad();
        document.body.removeChild(img);
    };
    img.onerror = function () {
        apiRenderServiceOkay = false;
        console.warn("Warning: GW2 Render service unavailable, switching to darthmaim-cdn");
        console.warn("More info at https://dev.gw2treasures.com/services/icons");
        mainLoad();
        document.body.removeChild(img);
    };
    img.src = "https://render.guildwars2.com/file/2FA9DF9D6BC17839BBEA14723F1C53D645DDB5E1/102852.png";
}
