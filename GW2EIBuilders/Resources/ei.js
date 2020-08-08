/*jshint esversion: 6 */
"use strict";
function compileTemplates() {
    Vue.component("graph-component", {
        props: ['id', 'layout', 'data'],
        template: '<div :id="id" class="d-flex flex-row justify-content-center"></div>',
        mounted: function () {
            var div = document.querySelector(this.queryID);
            Plotly.react(div, this.data, this.layout, { showEditInChartStudio: true, plotlyServerURL: "https://chart-studio.plotly.com" });
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
    TEMPLATE_COMPILE
};

function mainLoad() {
    // make some additional variables reactive
    var i;

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
    compileTemplates()
    new Vue({
        el: "#content",
        data: {
            light: typeof (window.theme) !== "undefined" ? (window.theme === 'yeti') : logData.lightTheme,
            mode: 0,
            cr: !!logData.crData
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
            },
            getLogData: function () {
                return logData;
            }
        },
        computed: {
            errorMessages: function () {
                return logData.logErrors;
            },
            uploadLinks: function () {
                var res = [
                    { 
                        key: "DPS Reports Link (EI)", 
                        url: "" 
                    },
                    { 
                        key: "DPS Reports Link (RH)", 
                        url: "" 
                    },
                    { 
                        key: "Raidar Link", 
                        url: "" 
                    }
                ];
                var hasAny = false;
                for (var i = 0; i < logData.uploadLinks.length; i++) {
                    var link = logData.uploadLinks[i];
                    if (link.length > 0) {
                        hasAny = true;
                        res[i].url = link;
                    }
                }
                return hasAny ? res : null;
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
    Vue.config.devtools = true
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
