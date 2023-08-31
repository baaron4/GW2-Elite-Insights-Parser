/*jshint esversion: 6 */
"use strict";
function compileTemplates() {
    Vue.component("graph-component", {
        props: ['id', 'layout', 'data'],
        template: '<div :id="id" class="d-flex flex-row justify-content-center"></div>',
        activated: function () {
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
                    if (!div) {
                        return;
                    }
                    var duration = 1000;
                    Plotly.animate(div, {
                        data: this.data
                    }, {
                        transition: {
                            duration: duration,
                            easing: 'cubic-in-out'
                        },
                        frame: {
                            duration: 0.75 * duration
                        }
                    });
                },
                deep: true
            },
            data: {
                handler: function () {
                    var div = document.querySelector(this.queryID);
                    if (!div) {
                        return;
                    }
                    Plotly.react(div, this.data, this.layout, { showEditInChartStudio: true, plotlyServerURL: "https://chart-studio.plotly.com" });
                },
                deep: true
            }
        }
    });
    Vue.component("custom-numberform-component", {
        props: ["minValue", "maxValue", "id", "placeholderValue"],
        template: `
        <div>
            <input class="form-control" type="number" :id="id"
                @onkeypress="return isNumber(event)" onpaste="return false;" step="2" 
                    :value="placeholderValue" data-bind="value:replyNumber, fireChange: true"
                    :min="minValue" :max="maxValue">
        </div>
        `,
        methods: {
            isNumber: function (evt) {
                evt = (evt) ? evt : window.event;
                var charCode = (evt.which) ? evt.which : evt.keyCode;
                if ((charCode > 31 && charCode < 48) || charCode > 57) {
                    return false;
                }
                return true;
            }
        },
        mounted() {
            $("#" + this.id).on("input ", function () {
                var max = parseInt($(this).attr('max')) || 1e12;
                var min = parseInt($(this).attr('min'));
                if ($(this).val() > max) {
                    $(this).val(max);
                } else if ($(this).val() < min) {
                    $(this).val(min);
                }
            });
        }
    });
    Vue.component("table-scroll-component", {
        props: ["min", "max", "width", "height", "transform", "pagestructure"],
        template : `      
        <input 
            style="background-color: #888888;" 
            :style=getStyle()
            type="range" :min="min" :max="max" :value="min" class="slider" @input="updateOffset($event.target.value)">
        `,
        methods: {
            updateOffset: function(value) {
                this.pagestructure.offset = parseInt(value);
            },
            getStyle: function() {
                var res = {
                    width: this.width,
                    height: this.height,
                    transform: this.transform
                };
                return res;
            },
        }
    });
    Vue.component("targetperplayer-graphs-tab-component", {
        props: ["targetindex", "phaseindex", 'light', 'playerindex'],
        template : `      
        <div>            
            <keep-alive>  
                <targetperplayer-graph-tab-component v-for="(player, pId) in players" v-if="pId === playerindex"
                :key="phaseindex + 'perplayer' + pId" :targetindex="targetindex" :phaseindex="phaseindex" :light="light"
                :playerindex="playerindex">
                </targetperplayer-graph-tab-component>           
            <keep-alive>
        </div>
        `,
        computed: {
            players: function() {
                return logData.players;
            }
        }
    });
    TEMPLATE_COMPILE
};

function mainLoad() {
    // make some additional variables reactive
    var firstActive = logData.phases[0];
    for (var i = 0; i < logData.phases.length; i++) {
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
            active: firstActive === phase,
            focus: -1
        });
    }
    for (var i = 0; i < logData.targets.length; i++) {
        simpleLogData.targets.push({
            active: true
        });
        logData.targets[i].id = i;
        logData.targets[i].dpsGraphCache = new Map();
    }
    for (var i = 0; i < logData.players.length; i++) {
        var playerData = logData.players[i];
        simpleLogData.players.push({
            active: !!playerData.isPoV,
            targetActive: !playerData.isFake
        });
        playerData.dpsGraphCache = new Map();
        playerData.id = i;
    }
    compileTemplates();
    if (!!crData) {
        compileCRTemplates();
    }
    if (!!healingStatsExtension) {
        compileHealingExtTemplates();
    }
    new Vue({
        el: "#content",
        data: {
            light: typeof (window.theme) !== "undefined" ? (window.theme === 'yeti') : logData.lightTheme,
            mode: 0,
            cr: !!crData,
            healingExtShow: !!healingStatsExtension || logData.evtcVersion >= 20210701,
            healingExt: !!healingStatsExtension
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
            },
            usedExtensions: function () {
                if (!logData.usedExtensions) {
                    return null;
                }
                return logData.usedExtensions;
            }
        },
        mounted() {
            var element = document.getElementById("loading");
            element.parentNode.removeChild(element);
        }
    });
    $("body").tooltip({
        selector: "[data-original-title]",
        html: true,
        boundary: "window"
    });
};

window.onload = function () {
    Vue.config.devtools = true;
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
