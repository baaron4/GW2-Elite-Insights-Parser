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

function getDefaultMainComponent() {
    const setting = EIUrlParams.get("startPage");
    if (!setting) {
        return 0;
    }
    const mainCompo = setting.split('/')[0];
    switch (mainCompo) {
        case "Statistics":
            return 0;
        case "CombatReplay":
            return !!crData ? 1 : 0;
        case "HealingStatistics":
            return !!healingStatsExtension ? 2 : 0;
    }
    return 0;
}

const DEBUG = EIUrlParams.get("debug") === "true";

function getDefaultPhase() {
    const setting = EIUrlParams.get("phase");
    if (!setting) {
        return 0;
    }
    return parseInt(setting);
}

function mainLoad() {
    if (!apiRenderServiceOkay) {
        if (useDarthmaim) {
            Object.assign(WeaponIcons, {                     
                Axe: "https://icons-gw2.darthmaim-cdn.com/AE4909124900E1A3006CEA394670603D5B0C15EE/631536.png",
                Dagger: "https://icons-gw2.darthmaim-cdn.com/2F94A543C87EAEE701BE28B26564C7B3D19C0977/631546.png",
                Mace: "https://icons-gw2.darthmaim-cdn.com/6EA5EEBFDC1278F3F997A248362A6F9698CA09FD/631600.png",
                Pistol: "https://icons-gw2.darthmaim-cdn.com/51217142E12EB2FE19B1DB1CAE4F1D275CC9EA03/631608.png",
                Scepter: "https://icons-gw2.darthmaim-cdn.com/3832066C1A5B45F1C40930C703573C65CB53D73B/631624.pn",
                Sword: "https://icons-gw2.darthmaim-cdn.com/3C4AA1BD79DAB49201C81D934AC7567B286E711B/631658.png",
                Focus: "https://icons-gw2.darthmaim-cdn.com/3F2F9F46E00592FE966F0E976445A87536743513/631554.png",
                Shield: "https://icons-gw2.darthmaim-cdn.com/59060CD4B67508090C0F5F436499F07B71080E1B/631632.png",
                Torch: "https://icons-gw2.darthmaim-cdn.com/081557906F6FDA4160320E3AFD42D4B11FEDDC0B/631666.png",
                Warhorn: "https://icons-gw2.darthmaim-cdn.com/F4407BC09091D6042078B05D4B0757037300A333/631683.png",
                Greatsword: "https://icons-gw2.darthmaim-cdn.com/B1A52DB3FCD8A6C744144FD4770BCBE8F95A4CBA/631562.png",
                Hammer: "https://icons-gw2.darthmaim-cdn.com/A3455EC1C59AC001E12C65740DE32DA12645EFA5/631576.png",
                Longbow: "https://icons-gw2.darthmaim-cdn.com/773EB91B749EB947CBB277D3219090CC1BDCCAC4/631592.png",
                Rifle: "https://icons-gw2.darthmaim-cdn.com/9D0F6CE0C16A43FD0E66C55E3E27CCDF260779ED/631616.png",
                Shortbow: "https://icons-gw2.darthmaim-cdn.com/3D7A68807006A225D124A4315DDAFB10AA07CE0F/631634.pn",
                Staff: "https://icons-gw2.darthmaim-cdn.com/F86C3CD9FA20D20EE920590517993211C6F9B99C/631650.png",
                Speargun: "https://icons-gw2.darthmaim-cdn.com/5C473933354CB8F1542F9F0FF39A5B445877CC06/631642.png",
                Spear: "https://icons-gw2.darthmaim-cdn.com/C427A73B00AB091FE8049AC2FD7EDEB4AF9A093F/631584.png",
                Trident: "https://icons-gw2.darthmaim-cdn.com/434F5946A9020500C2EE2E1F0F38E2CF7F0654BC/631675.png",
            });
            Object.assign(UIIcons, {     
                Barrier: "https://icons-gw2.darthmaim-cdn.com/357922487919E8E84B914EAC13D5796DDDC42D14/1770209.png",
                Heal: "https://icons-gw2.darthmaim-cdn.com/D4347C52157B040943051D7E09DEAD7AF63D4378/156662.png",
                Crit: "https://icons-gw2.darthmaim-cdn.com/C2CEA567E0C43C199C782809544721AA12A6DF0A/2229323.png",
                Flank: "https://icons-gw2.darthmaim-cdn.com/44D4631FB427F09BE5B300BE0F537E6F2126BA0B/1012653.png",
                Damage: "https://icons-gw2.darthmaim-cdn.com/61AA4919C4A7990903241B680A69530121E994C7/156657.png",
                CC: "https://icons-gw2.darthmaim-cdn.com/1999B9DB355005D2DD19F66DFFBAA6D466057508/522727.png",
                ConditionDamage: "https://icons-gw2.darthmaim-cdn.com/0120CB042BFC2EA6A45BC3DB45155FECDDDE1910/2229318.png",
                Power: "https://icons-gw2.darthmaim-cdn.com/D6CAECEA0FD5FADE04DD6970384ADC5DE309C506/2229322.png",
                HealingPower: "https://icons-gw2.darthmaim-cdn.com/9B986DEADC035E58C364A1423975F5F538FC2202/2229321.png",
                Duration: "https://icons-gw2.darthmaim-cdn.com/7B2193ACCF77E56C13E608191B082D68AA0FAA71/156659.png",
                NumberOfTargets: "https://icons-gw2.darthmaim-cdn.com/BBE8191A494B0352259C10EADFDACCE177E6DA5B/1770208.png",
                StunBreak: "https://icons-gw2.darthmaim-cdn.com/DCF0719729165FD8910E034CA4E0780F90582D15/156654.png",
                Glance: "https://icons-gw2.darthmaim-cdn.com/6CB0E64AF9AA292E332A38C1770CE577E2CDE0E8/102853.png",
                Miss: "https://icons-gw2.darthmaim-cdn.com/09770136BB76FD0DBE1CC4267DEED54774CB20F6/102837.png",
                Interrupt: "https://icons-gw2.darthmaim-cdn.com/9AE125E930C92FEA0DD99E7EBAEDE4CF5EC556B6/433474.png",
                Block: "https://icons-gw2.darthmaim-cdn.com/DFB4D1B50AE4D6A275B349E15B179261EE3EB0AF/102854.png",
                Strip: "https://icons-gw2.darthmaim-cdn.com/D327055AA824ABDDAD70E2606E1C9AF018FF9902/961449.png",
                Cleanse: "https://icons-gw2.darthmaim-cdn.com/F6C2FD7E78EE0D9178AEAEF8B1666477D1E92C99/103544.png"
            });
        } else {
            Object.assign(WeaponIcons, {                     
                Axe: "https://assets.gw2dat.com/631536.png",
                Dagger: "https://assets.gw2dat.com//631546.png",
                Mace: "https://assets.gw2dat.com//631600.png",
                Pistol: "https://assets.gw2dat.com/631608.png",
                Scepter: "https://assets.gw2dat.com/631624.png",
                Sword: "https://assets.gw2dat.com/631658.png",
                Focus: "https://assets.gw2dat.com/631554.png",
                Shield: "https://assets.gw2dat.com/631632.png",
                Torch: "https://assets.gw2dat.com/631666.png",
                Warhorn: "https://assets.gw2dat.com/631683.png",
                Greatsword: "https://assets.gw2dat.com/631562.png",
                Hammer: "https://assets.gw2dat.com/631576.png",
                Longbow: "https://assets.gw2dat.com/631592.png",
                Rifle: "https://assets.gw2dat.com/631616.png",
                Shortbow: "https://assets.gw2dat.com/631634.png",
                Staff: "https://assets.gw2dat.com/631650.png",
                Speargun: "https://assets.gw2dat.com/631642.png",
                Spear: "https://assets.gw2dat.com/631584.png",
                Trident: "https://assets.gw2dat.com/631675.png",
            });
            Object.assign(UIIcons, {     
                Barrier: "https://assets.gw2dat.com/1770209.png",
                Heal: "https://assets.gw2dat.com/156662.png",
                Crit: "https://assets.gw2dat.com/2229323.png",
                Flank: "https://assets.gw2dat.com/1012653.png",
                Damage: "https://assets.gw2dat.com/156657.png",
                CC: "https://assets.gw2dat.com/522727.png",
                ConditionDamage: "https://assets.gw2dat.com/2229318.png",
                Power: "https://assets.gw2dat.com/2229322.png",
                HealingPower: "https://assets.gw2dat.com/2229321.png",
                Duration: "https://assets.gw2dat.com/156659.png",
                NumberOfTargets: "https://assets.gw2dat.com/1770208.png",
                StunBreak: "https://assets.gw2dat.com/156654.png",
                Glance: "https://assets.gw2dat.com/102853.png",
                Miss: "https://assets.gw2dat.com/102837.png",
                Interrupt: "https://assets.gw2dat.com/433474.png",
                Block: "https://assets.gw2dat.com/102854.png",
                Strip: "https://assets.gw2dat.com/961449.png",
                Cleanse: "https://assets.gw2dat.com/103544.png"
            });
        }
    }
    // make some additional variables reactive
    var activePhaseIndex = getDefaultPhase();
    var firstActive = logData.phases[activePhaseIndex] ? logData.phases[activePhaseIndex] : logData.phases[0];
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
        var target = logData.targets[i];
        var activeArray = [];
        simpleLogData.targets.push(activeArray);
        for (var j = 0; j < logData.phases.length; j++) {
            var phase = logData.phases[j];
            var phaseTarget = phase.targets.indexOf(i);
            activeArray.push({
                active: phaseTarget > -1 ? !phase.secondaryTargets[phaseTarget] : false,
                secondary: !!phase.secondaryTargets[phaseTarget]
            });
        }
        target.id = i;
        target.dpsGraphCache = new Map();
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
            mode: getDefaultMainComponent(),
            cr: !!crData,
            healingExtShow: !!healingStatsExtension || logData.evtcBuild >= 20210701,
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
    var imgOfficialAPI = document.createElement("img");
    imgOfficialAPI.style.display = "none";
    document.body.appendChild(imgOfficialAPI);
    imgOfficialAPI.onload = function () {
        console.info("Info: GW2 Render service available");
        mainLoad();
        document.body.removeChild(imgOfficialAPI);
    };
    imgOfficialAPI.onerror = function () {
        apiRenderServiceOkay = false;      
        document.body.removeChild(imgOfficialAPI);
        var imgDarthmaim = document.createElement("img");
        imgDarthmaim.style.display = "none";
        imgDarthmaim.onload = function () {
            console.warn("Warning: GW2 Render service unavailable, switching to https://icons-gw2.darthmaim-cdn.com");
            useDarthmaim = true;
            mainLoad();
            document.body.removeChild(imgDarthmaim);
        };
        imgDarthmaim.onerror = function() {
            console.warn("Warning: GW2 Render service unavailable, switching to https://assets.gw2dat.com");
            useDarthmaim = false;
            mainLoad();
            document.body.removeChild(imgDarthmaim);
        }
        imgDarthmaim.src = "https://icons-gw2.darthmaim-cdn.com/2FA9DF9D6BC17839BBEA14723F1C53D645DDB5E1/102852.png";
    };
    imgOfficialAPI.src = "https://render.guildwars2.com/file/2FA9DF9D6BC17839BBEA14723F1C53D645DDB5E1/102852.png";
}
