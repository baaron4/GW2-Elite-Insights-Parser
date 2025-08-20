/*jshint esversion: 6 */
"use strict";
function compileTemplates() {
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
                const charCode = (evt.which) ? evt.which : evt.keyCode;
                if ((charCode > 31 && charCode < 48) || charCode > 57) {
                    return false;
                }
                return true;
            }
        },
        mounted() {
            $("#" + this.id).on("input ", function () {
                const max = parseInt($(this).attr('max')) || 1e12;
                const min = parseInt($(this).attr('min'));
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
                const res = {
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
        mixins: [encounterPhaseComponent],
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
                return getActivePlayersForPhase(this.encounterPhase);
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

function getDefaultEncounter() {
    const setting = EIUrlParams.get("encounter");
    if (!setting || !IsMultiEncounterLog) {
        return 0;
    }
    return parseInt(setting);
}

function mainLoad() {
    if (!apiRenderServiceOkay) {
        for (let key in WeaponIcons) {
            WeaponIcons[key] = _buildFallBackURL(WeaponIcons[key]);
        }
        for (let key in UIIcons) {
            UIIcons[key] = _buildFallBackURL(UIIcons[key]);
        }
    }
    // make some additional variables reactive
    for (let i = 0; i < logData.phases.length; i++) {
        const phase = logData.phases[i];
        phase.durationS = phase.duration / 1000.0
        const times = [];
        const dur = phase.end - phase.start;
        const floorDur = Math.floor(dur);
        phase.needsLastPoint = dur > floorDur + 1e-3;
        for (let j = 0; j <= floorDur; j++) {
            times.push(j);
        }
        if (phase.needsLastPoint) {
            times.push(phase.end - phase.start);
        }
        phase.id = i;
        phase.encounterID = -1;
        phase.times = times;
        reactiveLogdata.phases.push({
            active: false,
            index: i,
            focus: -1
        });
        if (phase.type === PhaseTypes.INSTANCE || phase.type === PhaseTypes.ENCOUNTER) {
            phase.encounterID = reactiveLogdata.encounters.length;
            reactiveLogdata.encounters.push({
                active: false,
                index: i
            });
            reactiveLogdata.activeEncounterPhaseData.push({
                phase: i,
                player: -1,
            });
        }
    }
    IsMultiEncounterLog = reactiveLogdata.encounters.length > 1;
    const activeEncounterIndex = getDefaultEncounter();
    for (let i = 0; i < reactiveLogdata.encounters.length; i++) {
        reactiveLogdata.encounters[i].active = i === activeEncounterIndex;
    }
    let encounterPhases = getPhasesForSelectedEncounter(reactiveLogdata.phases, reactiveLogdata.encounters).filter(x => {
        const logPhase = logData.phases[x.index];
        return !logPhase.breakbarPhase;
    });
    const activePhaseIndex = getDefaultPhase();
    for (let i = 0; i < encounterPhases.length; i++) {
        encounterPhases[i].active = i === activePhaseIndex;
        if (encounterPhases[i].active) {
            reactiveLogdata.activeEncounterPhaseData[activeEncounterIndex].phase = encounterPhases[i].index;
        }
    }
    for (let i = 0; i < logData.targets.length; i++) {
        const target = logData.targets[i];
        const activeArray = [];
        reactiveLogdata.targets.push(activeArray);
        for (let j = 0; j < logData.phases.length; j++) {
            const phase = logData.phases[j];
            const phaseTarget = phase.targets.indexOf(i);
            const priority = phase.targetPriorities[phaseTarget];
            activeArray.push({
                active: typeof priority !== "undefined" && priority < 2,
                secondary: typeof priority === "undefined" || priority > 0,
                index: i,
            });
        }
        target.id = i;
        target.dpsGraphCache = new Map();
    }
    let activeFound = false;
    for (let i = 0; i < logData.players.length; i++) {
        const playerData = logData.players[i];
        const active = !activeFound && !!playerData.isPoV;
        reactiveLogdata.players.push({
            active: active,
            index: i,
            targetActive: !playerData.isFake
        });
        if (active) {
            activeFound = true;
            for (let j = 0; j < reactiveLogdata.encounters.length; j++) {
                reactiveLogdata.activeEncounterPhaseData[j].player = i;
            }
        }
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
            healingExt: !!healingStatsExtension,
            reactiveLogdata: reactiveLogdata
        },
        methods: {
            switchTheme: function (state) {
                if (state === this.light) {
                    return;
                }
                const style = this.light ? 'yeti' : 'slate';
                this.light = state;
                const newStyle = this.light ? 'yeti' : 'slate';
                document.body.classList.remove("theme-" + style);
                document.body.classList.add("theme-" + newStyle);
                if (storeTheme) storeTheme(newStyle);
                const theme = document.getElementById('theme');
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
                const res = [
                    { 
                        key: "DPS Reports Link (EI)", 
                        url: "" 
                    },
                    { 
                        key: "Raidar Link", 
                        url: "" 
                    }
                ];
                let hasAny = false;
                for (let i = 0; i < logData.uploadLinks.length; i++) {
                    const link = logData.uploadLinks[i];
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
            },
            UIIcons: function () {
                return UIIcons;
            }
        },
        mounted() {
            const element = document.getElementById("loading");
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
    const imgOfficialAPI = document.createElement("img");
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
        const imgDarthmaim = document.createElement("img");
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
