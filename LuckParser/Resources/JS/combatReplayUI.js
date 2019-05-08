/*jshint esversion: 6 */

var compileCombatReplayUI = function () {
    Vue.component("combat-replay-ui-component", {
        props: ["mode"],
        template: `${tmplCombatReplayDamageTable}`,
        data: function () {
            return {
                damageMode: 1,, 
                rangeSelect: [180, 240, 300, 600, 900, 1200],
                speeds: [0.125, 0.25, 0.5, 1.0, 2.0, 4.0, 8.0, 16.0],
                animationStatus: {
                   time: 0,
                   selectedPlayer: null,
                   selectedPlayerID: null
                }
            };
        },
        created() {
        },
        mounted() {
        },
        updated() {
        },
        computed: {
            rangeSelectArrays: function(){
               var res = [];
               var cols = Math.ceil(this.rangeSelect.length / 3);
               for (var col = 0; col < cols; col++) {
                  var offset = 3 * cols;
                  var column = [];
                  for (var i = 0; i < Math.min(3, cols - offset); i++) {
                     column.push(this.rangeSelect[offset + i]);
                  }
                  res.push(column);
               }
               return res;
            },
            groups: function () {
                var aux = [];
                var i = 0;
                for (i = 0; i < logData.players.length; i++) {
                    var playerData = logData.players[i];
                    if (playerData.isConjure) {
                        continue;
                    }
                    if (!aux[playerData.group]) {
                        aux[playerData.group] = [];
                    }
                    aux[playerData.group].push(playerData);
                }
                var noUndefinedGroups = [];
                for (i = 0; i < aux.length; i++) {
                    if (aux[i]) {
                        noUndefinedGroups.push({id: i, players: aux[i]});
                    }
                }
                return noUndefinedGroups;
            }
        }
    });
};
