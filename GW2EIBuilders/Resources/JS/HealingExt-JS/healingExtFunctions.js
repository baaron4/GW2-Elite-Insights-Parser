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
        default:
            break;
    }
    return name;
}

function getHealingGraphName(healingMode, graphMode) {
    return healingGraphTypeEnumToString(healingMode) + " " + healingTypeEnumToString(graphMode) + " Graph";
}
