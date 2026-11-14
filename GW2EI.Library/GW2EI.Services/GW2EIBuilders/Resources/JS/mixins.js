"use strict";

var numberComponent = {
    methods: {
        // https://stackoverflow.com/questions/16637051/adding-space-between-numbers
        integerWithSpaces: function (x) {
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
        },

        numberFormater: function (value, minimumFractionDigits, maximumFractionDigits) {
            if (isNaN(value) || !isFinite(value)) {
                return 0;
            }
            return value.toLocaleString(undefined, {
                minimumFractionDigits,
                maximumFractionDigits
            });
        },
        numberFormater0: function (value) {
            if (isNaN(value) || !isFinite(value)) {
                return 0;
            }
            return formatterCache.zeroZero.format(value);
        },
        numberFormater1: function (value) {
            if (isNaN(value) || !isFinite(value)) {
                return 0;
            }
            return formatterCache.zeroOne.format(value);
        },
        numberFormater2: function (value) {
            if (isNaN(value) || !isFinite(value)) {
                return 0;
            }
            return formatterCache.zeroTwo.format(value);
        },
        numberFormater3: function (value) {
            if (isNaN(value) || !isFinite(value)) {
                return 0;
            }
            return formatterCache.zeroThree.format(value);
        },

        percentFormater: function (value, minimumFractionDigits, maximumFractionDigits) {
            if (isNaN(value) || !isFinite(value)) {
                return '0%';
            }
            return value.toLocaleString(undefined, {
                style: "percent",
                minimumFractionDigits,
                maximumFractionDigits
            });
        },
        percentFormater0: function (value) {
            if (isNaN(value) || !isFinite(value)) {
                return '0%';
            }
            return formatterCache.percentZeroZero.format(value);
        },
        percentFormater1: function (value) {
            if (isNaN(value) || !isFinite(value)) {
                return '0%';
            }
            return formatterCache.percentZeroOne.format(value);
        },
        percentFormater2: function (value) {
            if (isNaN(value) || !isFinite(value)) {
                return '0%';
            }
            return formatterCache.percentZeroTwo.format(value);
        },
        percentFormater3: function (value) {
            if (isNaN(value) || !isFinite(value)) {
                return '0%';
            }
            return formatterCache.percentZeroThree.format(value);
        },

        formatNumberOrPercent3: function (isNumber, value) {
            return isNumber
                ? this.numberFormater3(value)
                : this.percentFormater3(value / 100);
        }
    }
};

var damageGraphComponent = {
    data: function () {
        return {
            graphdata: {
                dpsmode: 0,
                graphmode: logData.wvw ? GraphType.Damage : GraphType.DPS,
                damagemode: DamageType.All,
            }
        };
    },
};

const playerHeaderComponent = {
    methods: {
        showWeaponSet: function (weaponSetIndex, weaponSets, encounterPhase) {
            if (encounterPhase.type === PhaseTypes.INSTANCE) {
                return weaponSetIndex === 0;
            }
            let setToShow = null;
            for (let i = 0; i < weaponSets.length; i++) {
                const wepSet = weaponSets[i];
                const time = encounterPhase.end * 1000;
                if (wepSet.start <= time && wepSet.end >= time) {
                    setToShow = i;
                    break;
                }
            }
            return weaponSetIndex === setToShow;
        },
        getIcon: function (path) {
            return WeaponIcons[path];
        },
        getCommanderTooltip: function (player) {
            if (!player.isCommander) {
                return false;
            }
            let res = 'Commander';
            for (let i = 0; i < player.commanderStates.length; i++) {
                res = `
                        ${res}<br>
                        From ${player.commanderStates[i][0]} to ${player.commanderStates[i][1]}
                    `;
            }
            return res;
        },
    },
    computed: {

        phase: function () {
            return logData.phases[this.phaseindex];
        },
    }
};

var encounterPhaseComponent = {
    data: function () {
        return {
            encounters: reactiveLogdata.encounters
        }
    },
    computed: {
        encounterPhase: function () {
            const encounters = this.encounters;
            for (let i = 0; i < encounters.length; i++) {
                const encounter = encounters[i];
                if (encounter.active) {
                    return logData.phases[encounter.index];
                }
            }
            return logData.phases[0];
        },
    }
}

var graphComponent = {
    data: function () {
        return {
            data: [],
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
                image.visible = typeof x0 === "undefined" || ((image.x <= x1 + 10 && image.x >= x0 - 10) && (x1 - x0) < 75);
                redraw = redraw || image.visible !== old;
            }
            return redraw;
        },
        updateVisibilyInQuad: function (images, x0, x1, y0, y1) {
            var redraw = false;
            for (var i = 0; i < images.length; i++) {
                var image = images[i];
                var old = image.visible;
                image.visible = typeof x0 === "undefined" || (((image.x <= x1 + 10 && image.x >= x0 - 10) && (x1 - x0) < 75) && ((image.y <= y1 + 10 && image.y >= y0 - 10) && (y1 - y0) < 75));
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

const tooltipSweepComponent = {
    updated() {
        sweepOrphanedOrRefreshedTooltips();
    },
};

// Depends on encounter phase component
var buffComponent = {
    props: ["phaseindex"],
    computed: {
        phase: function () {
            return logData.phases[this.phaseindex];
        },
        boons: function () {
            var data = [];
            for (var i = 0; i < logData.boons.length; i++) {
                data[i] = findSkill(true, logData.boons[i]);
            }
            return data;
        },
        conditions: function () {
            var data = [];
            for (var i = 0; i < logData.conditions.length; i++) {
                data[i] = findSkill(true, logData.conditions[i]);
            }
            return data;
        },
        offs: function () {
            var data = [];
            for (var i = 0; i < logData.offBuffs.length; i++) {
                data[i] = findSkill(true, logData.offBuffs[i]);
            }
            return data;
        },
        defs: function () {
            var data = [];
            for (var i = 0; i < logData.defBuffs.length; i++) {
                data[i] = findSkill(true, logData.defBuffs[i]);
            }
            return data;
        },
        sups: function () {
            var data = [];
            for (var i = 0; i < logData.supBuffs.length; i++) {
                data[i] = findSkill(true, logData.supBuffs[i]);
            }
            return data;
        },
        nourishments: function () {
            var data = [];
            for (var i = 0; i < logData.nourishments.length; i++) {
                data[i] = findSkill(true, logData.nourishments[i]);
            }
            return data;
        },
        enhancements: function () {
            var data = [];
            for (var i = 0; i < logData.enhancements.length; i++) {
                data[i] = findSkill(true, logData.enhancements[i]);
            }
            return data;
        },
        otherConsumables: function () {
            var data = [];
            for (var i = 0; i < logData.otherConsumables.length; i++) {
                data[i] = findSkill(true, logData.otherConsumables[i]);
            }
            return data;
        },
        orderedSpecs: function () {
            var res = [];
            var aux = new Set();
            const players = getActivePlayersForPhase(this.encounterPhase);
            for (var i = 0; i < specs.length; i++) {
                var spec = specs[i];
                var pBySpec = [];
                for (var j = 0; j < players.length; j++) {
                    if (players[j] && players[j].profession === spec && logData.phases[0].buffsStatContainer.persBuffStats[j].data.length > 0) {
                        pBySpec.push(j);
                    }
                }
                if (pBySpec.length) {
                    aux.add(SpecToBase[spec]);
                    res.push({
                        ids: pBySpec,
                        name: spec
                    });
                }
            }
            this.bases = [];
            var _this = this;
            aux.forEach(function (value, value2, set) {
                _this.bases.push(value);
            });
            this.persBuffMode = this.bases[0];
            return res;
        },
        persBuffs: function () {
            var res = [];
            for (var i = 0; i < this.orderedSpecs.length; i++) {
                var spec = this.orderedSpecs[i];
                var data = [];
                for (var j = 0; j < logData.persBuffs[spec.name].length; j++) {
                    data.push(findSkill(true, logData.persBuffs[spec.name][j]));
                }
                res.push(data);
            }
            return res;
        },
        gears: function () {
            var data = [];
            for (var i = 0; i < logData.gearBuffs.length; i++) {
                data[i] = findSkill(true, logData.gearBuffs[i]);
            }
            return data;
        },
        debuffs: function () {
            var data = [];
            for (var i = 0; i < logData.debuffs.length; i++) {
                data[i] = findSkill(true, logData.debuffs[i]);
            }
            return data;
        },
        targetDebuffs: function () {
            var data = [];
            for (var i = 0; i < logData.targetDebuffs.length; i++) {
                data[i] = findSkill(true, logData.targetDebuffs[i]);
            }
            return data;
        },
        targetOtherBuffs: function () {
            var data = [];
            for (var i = 0; i < logData.targetOtherBuffs.length; i++) {
                data[i] = findSkill(true, logData.targetOtherBuffs[i]);
            }
            return data;
        },
        buffsStatContainer: function () {
            return this.phase.buffsStatContainer;
        },
        buffVolumesStatContainer: function () {
            return this.phase.buffVolumesStatContainer;
        },
    }
};

var buffPlayerWithActiveTimeComponent = {
    methods: {
        getPlayerWeight(playerIndex, active) {
            if (!active) {
                return 1;
            }
            return this.activeTimes[playerIndex];
        }
    },
    computed: {
        activeTimes: function() {
            return this.phase.playerActiveTimes;
        },
    }
};

var buffPlayerMinionsWithActiveTimeComponent = {
    methods: {
        getMinionInstanceWeight: function(instanceIndex, active) {
            if (!active || this.minionIndex < 0) {
                return 1;
            }
            return this.minionActiveTimes[instanceIndex];
        },
    },
    computed: {
        minionActiveTimes: function() {
            if (this.minionIndex < 0) {
                return null;
            }
            return this.phase.playerMinionActiveTimes[this.playerindex][this.minionIndex];
        },
    }
};

var buffPlayerMinionsComponent = {
    methods: {
        getCacheID: function() {
            return this.encounterPhase.id + '-' + this.phaseindex + '-' + this.activeduration + '-' + this.minionIndex + '-' + this.minionInstanceIndex;
        },
        getSumCacheID: function() {
            return this.phaseindex + '-' + this.activeduration + '-' + this.minionIndex + '-' + this.minionInstanceIndex;
        },
    },
    computed: {
        actor: function() {
            return this.player;
        },
        selectedMinionVolumeContainer: function () {
            if (this.minionIndex < 0) {
                return null;
            }
            return this.buffVolumesStatContainer.minionBuffVolumes[this.playerindex][this.minionIndex] ?? null;
        },
        selectedMinionBuffs: function () {
            if (this.minionIndex < 0) {
                return null;
            }
            return this.buffsStatContainer.minionBuffs[this.playerindex][this.minionIndex] ?? null;
        },
    }
};

var minionTabulatedComponent = {
    data: function () {
        return {
            minionIndex: -1,
            minionInstanceIndices: {},
            allowConsolidated: false, // For granular activation untill full support
        };
    },
    methods: {   
        getMinionInstanceIndexForMinionIndex: function(minionIndex) {
            let index = this.minionInstanceIndices[minionIndex];
            if (index === undefined) {
                index = -1;
            }
            if (index === -1 && !this.allowConsolidated) {
                // Until consolidated view is support, consolidate is first minion
                return 0;
            }
            return index;
        },
        getSelectedDropdownValueForMinionIndex: function(minionIndex) {
            return minionIndex + ":" + this.getMinionInstanceIndexForMinionIndex(minionIndex);
        },
        getDropdownValueForMinionInstance: function(minionIndex, minionInstanceIndex) {
            return minionIndex + ":" + minionInstanceIndex;
        },
        handleMinionChange: function (event) {
            const value = event.target.value;
            const [minionIndex, minionInstanceIndex] = value.split(':').map(Number);
            this.$set(this.minionInstanceIndices, minionIndex, minionInstanceIndex);
        },
        isSelectedMinionInstanceActive(phaseIndex, minionInstanceIndex) {
            const phase = logData.phases[phaseIndex];
            const minionInstance = this.selectedMinion.instances[minionInstanceIndex];
            return isMinionInstanceActive(phase, minionInstance);
        },
    },
    computed: {
        consolidatedMinionView: function() {
            return this.minionIndex >= 0 && this.minionInstanceIndex === -1;
        },
        minionsToUse: function () {
            let result = [];
            const actor = this.actor;
            const minions = actor.minions;
            for (let i = 0; i < minions.length; i++) {
                result.push({
                    minion: minions[i],
                    minionIndex: i
                });
            }
            return result;
        },
        minionsWithInstanceToUse: function () {
            let result = [];
            const actor = this.actor;
            const minions = actor.minions;
            for (let i = 0; i < minions.length; i++) {
                if (actor.minions[i].instances) {
                    result.push({
                        minion: minions[i],
                        minionIndex: i
                    });
                }
            }
            return result;
        },
        minionInstanceIndex: function () {
            const minionIndex = this.minionIndex;
            if (minionIndex < 0) {
                return -1;
            }
            // can't use getMinionInstanceIndexForMinionIndex, methods are not reactive
            if (!this.selectedMinion.instances || this.selectedMinion.instances.length === 1) {
                return 0;
            }
            let index = this.minionInstanceIndices[minionIndex];
            if (index === undefined) {
                index = -1;
            }
            if (index === -1 && !this.allowConsolidated) {
                // Until consolidated view is support, consolidate is first minion
                return 0;
            }
            return index;
        },
        selectedMinion: function () {
            if (this.minionIndex < 0) {
                return null;
            }
            return this.actor.minions[this.minionIndex] ?? null;
        },
    },
}

var crActorComponent = {
    props: ["actorIndex", "isTarget", "minionIndex", "minionInstanceIndex"],
    computed: {
        isMinion: function () {
            return this.minionIndex != null && this.minionInstanceIndex != null;
        },
        minionOwner: function () {
            if (!this.isMinion) {
                return null;
            }
            return this.isTarget ? logData.targets[this.actorIndex] : logData.players[this.actorIndex];
        },
        minionInstance: function () {
            if (!this.isMinion) {
                return null;
            }
            return this.minionInfo.instances[this.minionInstanceIndex];
        },
        minionInfo: function () {
            if (!this.isMinion) {
                return null;
            }
            return this.minionOwner.minions[this.minionIndex];
        },
        minionLabel: function () {
            if (!this.isMinion || !this.minionInfo) {
                return "Minion";
            }
            return this.minionInfo.name + "#" + (this.minionInstanceIndex + 1);
        },
        actor: function () {
            if (this.isMinion) {
                return this.minionInstance;
            }
            return this.isTarget ? logData.targets[this.actorIndex] : logData.players[this.actorIndex];
        },
    }
}

var extensionPresentComponent = {
    methods: {
        playerIsRunningHealingExtension: function (playerData) {
            return playerIsRunningHealingExtension(playerData);
        }
    }
}

var UIIconsComponent = {
    computed: {
        UIIcons: function () {
            return UIIcons;
        },
    },
}

var sortedTableComponent = {
    methods: {
        sortByBase: function (sortdata, key, index) {
            index = index >= 0 ? index : -1;
            if (sortdata.key !== key || index !== sortdata.index) {
                sortdata.order = "asc";
            } else {
                sortdata.order = sortdata.order === "asc" ? "desc" : "asc";
            }
            sortdata.key = key;
            sortdata.index = index;
        },
        sortByPlayerName: function (x, y) {
            return collatorWithNumerics.compare(x.player.name, y.player.name);
        },
        sortByPlayerProfession: function (x, y) {
            return collatorWithNumerics.compare(x.player.profession, y.player.profession);
        },
        sortByPlayerAccount: function (x, y) {
            return collatorWithNumerics.compare(x.player.acc, y.player.acc);
        },
        getHeaderClassBase: function (sortdata, key, index) {
            index = index >= 0 ? index : -1;
            if (sortdata.key === key && sortdata.index === index) {
                if (sortdata.order === "asc") {
                    return { "sorted_asc": true };
                } else {
                    return { "sorted_desc": true };
                }
            };
            return { 'sorted': true };
        },
        getBodyClassBase: function (sortdata, key, index) {
            index = index >= 0 ? index : -1;
            return { 'sorted': sortdata.key === key && sortdata.index === index };
        },
    }
};

var sortedDistributionComponent = {
    methods: {
        sortBy: function (key, index, func) {
            this.sortByBase(this.sortdata, key, index);
            this.sortdata.sortFunc = func ? func : null;
        },
        getHeaderClass: function (key, index) {
            return this.getHeaderClassBase(this.sortdata, key, index);
        },
        getBodyClass: function (key, index) {
            var classes = this.getBodyClassBase(this.sortdata, key, index);
            return classes;
        },
        getCastBodyClass: function (key, index, data) {
            var res = this.getBodyClass(key, index);
            var innacurate = { higherOrEqual: (!this.getSkill(data).condi && this.getCast(data)) && this.showInequality(data) };
            Object.assign(res, innacurate);
            return res;
        },
        getHitsPerCastBodyClass: function (key, index, data) {
            var res = this.getBodyClass(key, index);
            var innacurate = { lowerOrEqual: (!this.getSkill(data).condi && this.getConnectedHits(data) && this.getCast(data)) && this.showInequality(data) };
            Object.assign(res, innacurate);
            return res;
        },
        sortData: function (rows) {
            var order = this.sortdata.order === "asc" ? 1 : -1;
            switch (this.sortdata.key) {
                case "Skill":
                    rows.sort((x, y) => order * (collatorWithNumerics.compare(this.getSkill(x).name, this.getSkill(y).name)));
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
                    rows.sort((x, y) => order * (sortFunc(x) - sortFunc(y)) || (collatorWithNumerics.compare(this.getSkill(x).name, this.getSkill(y).name)));
                    break;
                default:
                    break;
            }
            return rows;
        },
    }
};

var colSliderComponent = function (perpage, names = null) {
    let data;
    let methods;
    if (names !== null) {
        data = function () {
            let res = {};
            for (let i = 0; i < names.length; i++) {
                res[names[i] + "ColStructure"] = {
                    offset: 0,
                    perpage: perpage,
                };
            }
            return res;
        };
        methods = {};
        for (let i = 0; i < names.length; i++) {
            methods["isIn" + names[i][0].toUpperCase() + names[i].slice(1) + "ColPage"] = function (index) {
                return (
                    index >= this[names[i] + "ColStructure"].offset &&
                    index < this[names[i] + "ColStructure"].offset + this[names[i] + "ColStructure"].perpage
                );
            }
        }
    } else {
        data = function () {
            return {
                colStructure: {
                    offset: 0,
                    perpage: perpage,
                },
            };
        };
        methods = {
            isInColPage: function (index) {
                return (
                    index >= this.colStructure.offset &&
                    index < this.colStructure.offset + this.colStructure.perpage
                );
            },
        }
    }
    return {
        data: data,
        methods: methods
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


// Expects an object of {name: {defaultValue: value, cb: fn}}, cb to be called when value has changed
// Currently only handles booleans
const localStoragedSettings = function (settings) {
    let data = {};
    let watch = {};
    let settingNames = Object.getOwnPropertyNames(settings);
    settingNames.forEach((settingName) => {
        data[settingName] = settings[settingName].defaultValue;
        watch[settingName] = function (newValue) {
            setValueToLocalStorage(settingName, newValue);
            if (settings[settingName].fn) {
                settings[settingName].fn(newValue);
            }
        }
    });
    return {
        data: function() {
            return data;
        },
        watch: watch
    };

}
// Requires graphComponent and damageGraphComponent
var targetTabGraphComponent = {
    data: function () {
        return {
            targetOffset: 0
        };
    },
    watch: {
        light: {
            handler: function () {
                var textColor = this.light ? '#495057' : '#cccccc';
                this.layout.yaxis.gridcolor = textColor;
                this.layout.yaxis.color = textColor;
                this.layout.yaxis2.gridcolor = textColor;
                this.layout.yaxis2.color = textColor;
                this.layout.yaxis3.gridcolor = textColor;
                this.layout.yaxis3.color = textColor;
                this.layout.xaxis.gridcolor = textColor;
                this.layout.xaxis.color = textColor;
                this.layout.font.color = textColor;
                for (var i = 0; i < this.layout.shapes.length; i++) {
                    this.layout.shapes[i].line.color = textColor;
                }
                this.layout.datarevision = new Date().getTime();
            }
        }
    },
    created: function () {
        this.computeLayout();
    },
    computed: {
        healthStates: function () {
            return this.graph.targets[this.phaseTargetIndex].healthStates;
        },
        breakbarStates: function () {
            return this.graph.targets[this.phaseTargetIndex].breakbarPercentStates;
        },
        barrierStates: function () {
            return this.graph.targets[this.phaseTargetIndex].barrierStates;
        },
        target: function () {
            return logData.targets[this.targetindex];
        },
        phaseTargetIndex: function () {
            return this.phase.targets.indexOf(this.targetindex);
        },
        damageGraphName: function () {
            switch (this.graphdata.damagemode) {
                case DamageType.All:
                    return "total";
                case DamageType.Power:
                    return "totalPower";
                case DamageType.Condition:
                    return "totalCondition";
                default:
                    throw new Error("unknown enum in damage graph name");
            }
        },
        graphname: function () {
            var name = getDamageGraphName(this.graphdata.damagemode, this.graphdata.graphmode);
            switch (this.graphdata.dpsmode) {
                case 0:
                    name = "Full " + name;
                    break;
                case -1:
                    name = "Phase " + name;
                    break;
                default:
                    name = this.graphdata.dpsmode + "s " + name;
                    break;
            }
            return name;
        },
        computeData: function () {
            this.layout.datarevision = new Date().getTime();
            this.layout.yaxis3.title = graphTypeEnumToString(this.graphdata.graphmode);
            var res = this.data;
            var data = this.computeDPSRelatedData();
            for (var i = 0; i < data.length; i++) {
                this.data[this.targetOffset - i].y = data[i];
            }
            return res;
        },
        rotationData: function () {
            return this.target.details.rotation[this.phaseindex];
        }
    },
    methods: {
        computeLayout() {
            const images = [];
            this.data = [];
            this.targetOffset += computeRotationData(this.rotationData, images, this.data, this.phase, this.target, 1);
            var oldOffset = this.targetOffset;
            this.targetOffset += computeBuffData(this.boonGraph, this.data);
            var hasBuffs = oldOffset !== this.targetOffset;
            this.targetOffset += addTargetLayout(this.data, this.target, this.breakbarStates, "breakbar", "breakbar", this.phase.breakbarPhase);
            this.targetOffset += addTargetLayout(this.data, this.target, this.barrierStates, "barrier", "barrier", false);
            this.targetOffset += addTargetLayout(this.data, this.target, this.healthStates, "hp", "health", true);
            this.data.push({
                x: this.phase.times,
                y: [],
                mode: 'lines',
                line: {
                    shape: 'spline'
                },
                yaxis: 'y3',
                hoverinfo: 'name+y+x',
                name: 'Total'
            });
            this.layout = getActorGraphLayout(images, this.light ? '#495057' : '#cccccc', hasBuffs, true);
            computePhaseMarkups(this.layout.shapes, this.layout.annotations, this.phase, this.light ? '#495057' : '#cccccc');
            this.updateVisibily(this.layout.images, this.phase.start, this.phase.end);
            
            const _this = this;
            this.layout._relayoutCB = function (evt) {
                const x0 = this.xaxis.range[0];
                const x1 = this.xaxis.range[1];
                //console.log("re-layout " + x0 + " " + x1);
                if (_this.updateVisibily(this.images, x0, x1)) {
                    this.datarevision = new Date().getTime();
                    //console.log("re-drawing");
                }
            }
        },
        computeDPSData: function () {
            const cacheID = getDPSGraphCacheID(this.graphdata.dpsmode, this.graphdata.damagemode, this.graphdata.graphmode, [], this.phaseindex, null);
            if (this.dpsCache.has(cacheID)) {
                return this.dpsCache.get(cacheID);
            }
            //var before = performance.now();
            var res;
            var damageData = this.graph.targets[this.phaseTargetIndex][this.damageGraphName];
            if (this.graphdata.dpsmode >= 0) {
                res = computeTargetDPS(this.target, damageData, this.graphdata.dpsmode, null, cacheID, this.phase.times, this.graphdata.graphmode);
            } else {
                res = computeTargetDPS(this.target, damageData, 0, this.computePhaseBreaks, cacheID, this.phase.times, this.graphdata.graphmode);
            }
            this.dpsCache.set(cacheID, res);
            return res;
        },
        computeDPSRelatedData: function () {
            var cacheID = getDPSGraphCacheID(this.graphdata.dpsmode, this.graphdata.damagemode, this.graphdata.graphmode, [], this.phaseindex, null);
            if (this.dataCache.has(cacheID)) {
                return this.dataCache.get(cacheID);
            }
            var dpsData = this.computeDPSData();
            var res = [dpsData.dps];
            addPointsToGraph(res, this.healthStates, dpsData.maxDPS);
            addPointsToGraph(res, this.barrierStates, dpsData.maxDPS);
            addPointsToGraph(res, this.breakbarStates, dpsData.maxDPS);
            this.dataCache.set(cacheID, res);
            return res;
        },
    }
}
