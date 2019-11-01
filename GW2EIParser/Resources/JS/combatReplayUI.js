/*jshint esversion: 6 */

var compileCombatReplayUI = function () {
    Vue.component("combat-replay-ui-component", {
        props: ["mode", "light"],
        template: `${tmplCombatReplayUI}`,
        data: function () {
            return {
                animationStatus: {
                    time: 0,
                    selectedPlayer: null,
                    selectedPlayerID: null,
                    animated: false
                }
            };
        },
        mounted() {
            animator = new Animator(logData.crData, this.animationStatus);
        },
        watch: {
            mode: {
                handler: function () {
                    if (this.animationStatus.animated && animator != null) {
                        if (this.mode === 1) {
                            animator.startAnimate(false);
                        } else {
                            animator.stopAnimate(false);
                        }
                    }
                },
                deep: true
            }
        },
    });

    Vue.component("combat-replay-animation-control-component", {
        props: ["light", "animated"],
        template: `${tmplCombatReplayAnimationControl}`,
        data: function () {
            return {
                speeds: [0.125, 0.25, 0.5, 1.0, 2.0, 4.0, 8.0, 16.0],
                selectedSpeed: 1,
                backwards: false,
                canvas: {
                    x: logData.crData.sizes[0],
                    y: logData.crData.sizes[1]
                },
                maxTime: logData.crData.maxTime,
            };
        },
        computed: {
            phases: function() {
                return logData.phases;
            }
        },
        methods: {
            toggleBackwards: function () {
                this.backwards = animator.toggleBackwards();
            },
            toggleAnimate: function () {
                animator.toggleAnimate();
            },
            restartAnimate: function () {
                animator.restartAnimate();
            },
            setSpeed: function (speed) {
                animator.setSpeed(speed);
                this.selectedSpeed = speed;
            },
            updateTime: function (value) {
                animator.updateTime(value);
            },
            updateInputTime: function (value) {
                animator.updateInputTime(value);
            }
        },
    });

    Vue.component("combat-replay-player-select-component", {
        props: ['selectedplayerid', "light"],
        template: `${tmplCombatReplayPlayerSelect}`,
        methods: {
            selectActor: function (id) {
                animator.selectActor(id);
            }
        },
        computed: {
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
                        noUndefinedGroups.push({ id: i, players: aux[i] });
                    }
                }
                return noUndefinedGroups;
            }
        }
    });

    Vue.component("combat-replay-range-select-component", {
        props: ["light"],
        template: `${tmplCombatReplayRangeSelect}`,
        data: function () {
            return {
                rangeSelect: [130, 180, 240, 300, 360, 400, 480, 600, 900, 1200, 1500],
                selectedRanges: [],
            };
        },
        methods: {
            toggleRange: function (range) {
                var active = animator.toggleRange(range);
                if (active) {
                    this.selectedRanges = this.selectedRanges.concat([range]);
                } else {
                    this.selectedRanges = this.selectedRanges.filter(x => x != range);
                }
            },
        },
        computed: {
            rangeSelectArrays: function () {
                var res = [];
                var cols = Math.ceil(this.rangeSelect.length / 3);
                for (var col = 0; col < cols; col++) {
                    var offset = 3 * col;
                    var column = [];
                    for (var i = 0; i < Math.min(3, this.rangeSelect.length - offset); i++) {
                        column.push(this.rangeSelect[offset + i]);
                    }
                    res.push(column);
                }
                return res;
            }
        }
    });

    Vue.component("combat-replay-mechanics-list-component", {
        props: ['selectedplayerid'],
        template: `${tmplCombatReplayMechanicsList}`,
        data: function () {
            var mechanicEvents = [];
            var phase = logData.phases[0];
            var phaseTargets = phase.targets;
            for (var mechI = 0; mechI < graphData.mechanics.length; mechI++) {
                var graphMechData = graphData.mechanics[mechI];
                var logMechData = logData.mechanicMap[mechI];
                var mechData = {name: logMechData.name, shortName: logMechData.shortName};
                var pointsArray = graphMechData.points[0];
                // players
                if (!logMechData.enemyMech) {
                    for (var playerI = 0; playerI < pointsArray.length; playerI++) {
                        var points = pointsArray[playerI];
                        var player = logData.players[playerI];
                        for (var i = 0; i < points.length; i++) {
                            var time = points[i]; // when mechanic occured in seconds
                            mechanicEvents.push({
                                time: time * 1000,
                                actor: {name: player.name, enemy: false, id: player.combatReplayID},
                                mechanic: mechData,
                            });
                        }
                    }
                } else {
                    // enemy
                    for (var targetI = 0; targetI < pointsArray.length; targetI++) {
                        var points = pointsArray[targetI];
                        var tarId = phaseTargets[targetI];
                        // target tracked in phase
                        if (tarId >= 0) {
                            var target = logData.targets[tarId];
                            for (var i = 0; i < points.length; i++) {
                                var time = points[i]; // when mechanic occured in seconds
                                mechanicEvents.push({
                                    time: time * 1000,
                                    actor: {name: target.name, enemy: true, id: -1}, // target selection not supported
                                    mechanic: mechData,
                                });
                            }
                        } else {
                            // target not tracked in phase
                            for (var i = 0; i < points.length; i++) {
                                var time = points[i][0]; // when mechanic occured in seconds
                                mechanicEvents.push({
                                    time: time * 1000,
                                    actor: {name: points[i][1], enemy: true, id: -1},
                                    mechanic: mechData,
                                });
                            }
                        }
                    }
                }
            }

            mechanicEvents.sort(function(a, b) {
                return a.time - b.time;
            });

            var actors = {};
            var mechanics = {};
            for (var i = 0; i < mechanicEvents.length; i++) {
                var event = mechanicEvents[i];
                var mechName = event.mechanic.name;
                var actorName = event.actor.name;
                if (!mechanics[mechName]) {
                    mechanics[mechName] = Object.assign({}, event.mechanic, {included: true});
                }
                if (!actors[actorName]) {
                    actors[actorName] = Object.assign({}, event.actor, {included: true});
                }
            }

            var actorsList = Object.values(actors); // could be sorted for more clarity
            actorsList.sort(function(a, b) {
                if (a.enemy !== b.enemy) {
                    // Sort enemies before players
                    return a.enemy ? -1 : 1;
                }
                return a.name.localeCompare(b.name);
            });

            var mechanicsList = Object.values(mechanics);
            mechanicsList.sort(function(a, b) {
                return a.shortName.localeCompare(b.shortName);
            });

            return {
                showMechanics: false,
                mechanicEvents: mechanicEvents,
                actors: actors,
                actorsList: actorsList,
                mechanics: mechanics,
                mechanicsList: mechanicsList,
            };
        },
        methods: {
            selectMechanic: function (mechanic) {
                animator.updateTime(mechanic.time);
            },
        },
        computed: {
            filteredMechanicEvents: function() {
                return this.mechanicEvents.filter(function (event) {
                    var actor = this.actors[event.actor.name];
                    var mechanic = this.mechanics[event.mechanic.name];
                    if (actor && !actor.included) {
                        return false;
                    }
                    if (mechanic && !mechanic.included) {
                        return false;
                    }
                    return true;
                }.bind(this))
            },
        },
    });
};
