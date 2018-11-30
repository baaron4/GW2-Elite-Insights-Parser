/*jshint esversion: 6 */

var compileHeader = function () {
    Vue.component("encounter-component", {
        props: ["logdata"],
        template: "#tmplEncounter",
        methods: {
            getResultText: function (success) {
                return success ? "Success" : "Failure";
            },
            getResultClass: function (success) {
                return success ? ["text-success"] : ["text-warning"];
            }
        },
        computed: {
            encounter: function () {
                var logData = this.logdata;
                var targets = [];
                for (var i = 0; i < logData.phases[0].targets.length; i++) {
                    var targetData = logData.targets[logData.phases[0].targets[i]];
                    targets.push(targetData);
                }

                var encounter = {
                    name: logData.fightName,
                    success: logData.success,
                    icon: logData.fightIcon,
                    duration: logData.encounterDuration,
                    targets: targets
                };
                return encounter;
            }
        }
    });

    Vue.component("phase-component", {
        props: ["phases"],
        template: "#tmplPhase",
        methods: {
            select: function (phase) {
                for (var i = 0; i < this.phases.length; i++) {
                    this.phases[i].active = false;
                }
                phase.active = true;
            }
        }
    });

    Vue.component("target-component", {
        props: ["targets", "phase"],
        template: "#tmplTargets",
        methods: {
            show: function (target) {
                var index = this.targets.indexOf(target);
                return this.phase.targets.indexOf(index) !== -1;
            }
        }
    });

    Vue.component("player-component", {
        props: ["players"],
        template: "#tmplPlayers",
        methods: {
            getIcon: function (path) {
                return urls[path];
            },
            select: function (player, groups) {
                var oldStatus = player.active;
                for (var i = 0; i < groups.length; i++) {
                    var group = groups[i];
                    for (var j = 0; j < group.length; j++) {
                        group[j].active = false;
                    }
                }
                player.active = !oldStatus;
            }
        },
        computed: {
            groups: function () {
                var aux = [];
                var i = 0;
                for (i = 0; i < this.players.length; i++) {
                    var playerData = this.players[i];
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
                        noUndefinedGroups.push(aux[i]);
                    }
                }
                return noUndefinedGroups;
            }
        }
    });
};