/*jshint esversion: 6 */
"use strict";

function compileHealingExtTemplates() {
    TEMPLATE_HEALING_EXT_COMPILE
};

var healingGraphComponent = {
    data: function() {
        return {     
            graphhealingdata: {
                hpsmode: 0,
                graphmode: GraphType.Damage,
                healingmode: HealingType.All,
            },
        };
    },
};

const HealingType = {
    All: 0,
    HealingPower: 1,
    Conversion: 2,
    Hybrid: 3
};
