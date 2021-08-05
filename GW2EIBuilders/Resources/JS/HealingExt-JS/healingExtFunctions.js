/*jshint esversion: 6 */
"use strict";

function healingGraphTypeEnumToString(mode) {
    var name = "";
    switch (mode) {
        case GraphType.DPS:
            name = "HPS";
            break;
        case GraphType.CenteredDPS:
            name = "Centered HPS";
            break;
        case GraphType.Damage:
            name = "Healing";
            break;
        default:
            break;
    }
    return name;
}

function healingTypeEnumToString(mode) {
    var name = "";
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
        default:
            break;
    }
    return name;
}

function getHPSGraphCacheID(hpsmode, healingmode, graphmode, activetargets, phaseIndex, extra) {
    return "hps" + hpsmode + '-'+ healingmode + '-' + graphmode + '-' + getTargetCacheID(activetargets) + '-' + phaseIndex + (extra !== null ? '-' + extra : '');
}

function getHealingGraphName(healingMode, graphMode) {
    return healingTypeEnumToString(healingMode) + " " + healingGraphTypeEnumToString(graphMode) + " Graph";
}

function computePlayersHealthData(graph, data, yaxis) {
    var offset = 0;
    for (var i = 0; i < logData.players.length; i++) {
        var player = logData.players[i];
        if (player.isFake) {
            continue;
        }
        offset += computePlayerHealthData(graph.players[i].healthStates, player, data, yaxis)
    }
    return offset;
}
