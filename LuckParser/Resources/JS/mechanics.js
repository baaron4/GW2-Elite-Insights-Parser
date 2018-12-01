/*jshint esversion: 6 */

var compileMechanics = function () {
    Vue.component("mechanics-stats-component", {
        props: ["phaseindex", "playerindex"],
        template: "#tmplMechanicsTable",
        data: function () {
            return {
                cacheP: new Map(),
                cacheE: new Map()
            };
        },
        mounted() {
            initTable("#playermechs", 0, "asc");
            //
            if (this.enemyMechHeader.length) {
                initTable("#enemymechs", 0, "asc");
            }
        },
        updated() {
            updateTable("#playermechs");
            //
            if (this.enemyMechHeader.length) {
                updateTable("#enemymechs");
            }
        },
        computed: {
            phase: function() {
                return logData.phases[this.phaseindex];
            },
            playerMechHeader: function () {
                var playerMechanics = [];
                for (var i = 0; i < mechanicMap.length; i++) {
                    if (mechanicMap[i].playerMech) {
                        playerMechanics.push(mechanicMap[i]);
                    }
                }
                return playerMechanics;
            },
            playerMechRows: function () {
                if (this.cacheP.has(this.phaseindex)) {
                    return this.cacheP.get(this.phaseindex);
                }
                var players = logData.players;
                var rows = [];
                for (var i = 0; i < players.length; i++) {
                    var player = players[i];
                    if (player.isConjure) {
                        continue;
                    }
                    rows.push({
                        player: player,
                        mechs: this.phase.mechanicStats[i]
                    });
                }
                this.cacheP.set(this.phaseindex, rows);
                return rows;
            },
            enemyMechHeader: function () {
                var enemyMechanics = [];
                for (var i = 0; i < mechanicMap.length; i++) {
                    if (mechanicMap[i].enemyMech) {
                        enemyMechanics.push(mechanicMap[i]);
                    }
                }
                return enemyMechanics;
            },
            enemyMechRows: function () {
                if (this.cacheE.has(this.phaseindex)) {
                    return this.cacheE.get(this.phaseindex);
                }
                var enemies = logData.enemies;
                var rows = [];
                for (var i = 0; i < enemies.length; i++) {
                    var enemy = enemies[i];
                    rows.push({
                        enemy: enemy.name,
                        mechs: this.phase.enemyMechanicStats[i]
                    });
                }
                this.cacheE.set(this.phaseindex, rows);
                return rows;
            }
        }
    });
};