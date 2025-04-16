/*jshint esversion: 6 */
"use strict";

function compileHealingExtTemplates() {
    TEMPLATE_HEALING_EXT_COMPILE
};

const healingGraphComponent = {
    data: function() {
        return {     
            graphhealingdata: {
                hpsmode: 0,
                graphmode: logData.wvw ? GraphType.Damage : GraphType.DPS,
                healingmode: HealingType.All,
            },
        };
    },
};

const HealingType = {
    All: 0,
    HealingPower: 1,
    Conversion: 2,
    Hybrid: 3,
    Downed: 4,
    Barrier: 5
};
