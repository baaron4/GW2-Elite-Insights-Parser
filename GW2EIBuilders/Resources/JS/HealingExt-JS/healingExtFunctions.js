/*jshint esversion: 6 */
"use strict";

function healingGraphTypeEnumToString(mode, healingMode) {
    let name = "";
    switch (mode) {
        case GraphType.DPS:
            name = healingMode === HealingType.Barrier ? "BPS" : "HPS";
            break;
        case GraphType.CenteredDPS:
            name = healingMode === HealingType.Barrier ? "Centered BPS" : "Centered HPS";
            break;
        case GraphType.Damage:
            name = healingMode === HealingType.Barrier ? "Barrier" : "Healing";
            break;
        default:
            break;
    }
    return name;
}

function healingTypeEnumToString(mode) {
    let name = "";
    switch (mode) {
        case HealingType.All:
            name = "All";
            break;
        case HealingType.HealingPower:
            name = "Healing Power";
            break;
        case HealingType.Conversion:
            name = "Conversion";
            break;
        case HealingType.Hybrid:
            name = "Healing Power or Conversion";
            break;
        case HealingType.Downed:
            name = "Against Downed";
            break;
        case HealingType.Barrier:
            name = "Healing Power";
            break;
        default:
            break;
    }
    return name;
}

function getHPSGraphCacheID(hpsmode, healingmode, graphmode, activetargets, phaseIndex, extra) {
    return "hps" + hpsmode + '-'+ healingmode + '-' + graphmode + '-' + getTargetCacheID(activetargets) + '-' + phaseIndex + (extra !== null ? '-' + extra : '');
}

function getHealingGraphName(healingMode, graphMode) {
    return healingTypeEnumToString(healingMode) + " " + healingGraphTypeEnumToString(graphMode, healingMode) + " Graph";
}

function computePlayersHealingGraphData(graph, data, yaxis) {
    let offset = 0;
    for (let i = 0; i < logData.players.length; i++) {
        const player = logData.players[i];
        if (player.isFake) {
            continue;
        }
        offset += computePlayerHealthData(graph.players[i].healthStates, player, data, yaxis)
        offset += computePlayerBarrierData(graph.players[i].barrierStates, player, data, yaxis)
    }
    return offset;
}