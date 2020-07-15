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
        created() {
            animator = new Animator(logData.crData, this.animationStatus);
        },
        mounted() {
            animator.attachDOM();
        },
        activated() {
            if (this.animationStatus.animated && animator != null) {
                animator.startAnimate(false);
            }
        },
        deactivated() {
            if (this.animationStatus.animated && animator != null) {
                animator.stopAnimate(false);
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
};
