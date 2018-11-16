/*jshint esversion: 6 */

var compileBuffStats = function () {
    Vue.component("personal-buff-table-component", {
        props: ['phase', 'persbuffs', 'players'],
        template: "#tmplPersonalBuffTable",
        data: function () {
            return {
                specs: [
                    "Warrior", "Berserker", "Spellbreaker", "Revenant", "Herald", "Renegade", "Guardian", "Dragonhunter", "Firebrand",
                    "Ranger", "Druid", "Soulbeast", "Engineer", "Scrapper", "Holosmith", "Thief", "Daredevil", "Deadeye",
                    "Mesmer", "Chronomancer", "Mirage", "Necromancer", "Reaper", "Scourge", "Elementalist", "Tempest", "Weaver"
                ],
                bases: [],
                specToBase: {
                    Warrior: 'Warrior',
                    Berserker: 'Warrior',
                    Spellbreaker: 'Warrior',
                    Revenant: "Revenant",
                    Herald: "Revenant",
                    Renegade: "Revenant",
                    Guardian: "Guardian",
                    Dragonhunter: "Guardian",
                    Firebrand: "Guardian",
                    Ranger: "Ranger",
                    Druid: "Ranger",
                    Soulbeast: "Ranger",
                    Engineer: "Engineer",
                    Scrapper: "Engineer",
                    Holosmith: "Engineer",
                    Thief: "Thief",
                    Daredevil: "Thief",
                    Deadeye: "Thief",
                    Mesmer: "Mesmer",
                    Chronomancer: "Mesmer",
                    Mirage: "Mesmer",
                    Necromancer: "Necromancer",
                    Reaper: "Necromancer",
                    Scourge: "Necromancer",
                    Elementalist: "Elementalist",
                    Tempest: "Elementalist",
                    Weaver: "Elementalist"
                },
                mode: "Warrior",
                cache: new Map()
            };
        },
        computed: {
            orderedSpecs: function () {
                var res = [];
                var aux = new Set();
                for (var i = 0; i < this.specs.length; i++) {
                    var spec = this.specs[i];
                    var pBySpec = [];
                    for (var j = 0; j < this.players.length; j++) {
                        if (this.players[j].profession === spec) {
                            pBySpec.push(j);
                        }
                    }
                    if (pBySpec.length) {
                        aux.add(this.specToBase[spec]);
                        res.push({
                            ids: pBySpec,
                            name: spec
                        });
                    }
                }
                this.bases = [];
                var _this = this;
                aux.forEach(function (value, value2, set) {
                    _this.bases.push(value);
                });
                this.mode = this.bases[0];
                return res;
            },
            data: function () {
                if (this.cache.has(this.phase)) {
                    return this.cache.get(this.phase);
                }
                var res = [];
                for (var i = 0; i < this.orderedSpecs.length; i++) {
                    var spec = this.orderedSpecs[i];
                    var dataBySpec = [];
                    for (var j = 0; j < spec.ids.length; j++) {
                        dataBySpec.push({
                            player: this.players[spec.ids[j]],
                            data: this.phase.persBuffStats[spec.ids[j]]
                        });
                    }
                    res.push(dataBySpec);
                }
                this.cache.set(this.phase, res);
                return res;
            },
            buffs: function () {
                var res = [];
                for (var i = 0; i < this.orderedSpecs.length; i++) {
                    var spec = this.orderedSpecs[i];
                    var data = [];
                    for (var j = 0; j < this.persbuffs[spec.name].length; j++) {
                        data.push(findSkill(true, this.persbuffs[spec.name][j]));
                    }
                    res.push(data);
                }
                return res;
            }
        }
    });

    Vue.component("buff-stats-component", {
        props: ['datatypes', 'datatype', 'phase', 'players', 'presentboons', 'presentoffs', 'presentdefs'],
        template: "#tmplBuffStats",
        data: function () {
            return {
                mode: 0,
                cache: new Map()
            };
        },
        computed: {
            boons: function () {
                var data = [];
                for (var i = 0; i < this.presentboons.length; i++) {
                    data[i] = findSkill(true, this.presentboons[i]);
                }
                return data;
            },
            offs: function () {
                var data = [];
                for (var i = 0; i < this.presentoffs.length; i++) {
                    data[i] = findSkill(true, this.presentoffs[i]);
                }
                return data;
            },
            defs: function () {
                var data = [];
                for (var i = 0; i < this.presentdefs.length; i++) {
                    data[i] = findSkill(true, this.presentdefs[i]);
                }
                return data;
            },
            buffData: function () {
                if (this.cache.has(this.phase)) {
                    return this.cache.get(this.phase);
                }
                var _this = this;
                var getData = function (stats, genself, gengroup, genoffgr, gensquad) {
                    var uptimes = [],
                        gens = [],
                        gengr = [],
                        genoff = [],
                        gensq = [];
                    var avg = [],
                        gravg = [],
                        totalavg = [];
                    var grcount = [],
                        totalcount = 0;
                    var i, k;
                    for (i = 0; i < _this.players.length; i++) {
                        var player = _this.players[i];
                        if (player.isConjure) {
                            continue;
                        }
                        uptimes.push({
                            player: player,
                            data: stats[i]
                        });
                        gens.push({
                            player: player,
                            data: genself[i]
                        });
                        gengr.push({
                            player: player,
                            data: gengroup[i]
                        });
                        genoff.push({
                            player: player,
                            data: genoffgr[i]
                        });
                        gensq.push({
                            player: player,
                            data: gensquad[i]
                        });
                        if (!gravg[player.group]) {
                            gravg[player.group] = [];
                            grcount[player.group] = 0;
                        }
                        totalcount++;
                        grcount[player.group]++;
                        for (var j = 0; j < stats[i].data.length; j++) {
                            totalavg[j] = (totalavg[j] || 0) + stats[i].data[j][0];
                            gravg[player.group][j] = (gravg[player.group][j] || 0) + stats[i].data[j][0];
                        }
                    }
                    for (i = 0; i < gravg.length; i++) {
                        if (gravg[i]) {
                            for (k = 0; k < gravg[i].length; k++) {
                                gravg[i][k] = Math.round(100 * gravg[i][k] / grcount[i]) / 100;
                            }
                            avg.push({
                                name: "Group " + i,
                                data: gravg[i],
                            });
                        }
                    }
                    for (k = 0; k < totalavg.length; k++) {
                        totalavg[k] = Math.round(100 * totalavg[k] / totalcount) / 100;
                    }
                    avg.push({
                        name: "Total",
                        data: totalavg
                    });
                    return [uptimes, gens, gengr, genoff, gensq, avg];
                };
                var res = {
                    boonsData: getData(this.phase.boonStats, this.phase.boonGenSelfStats,
                        this.phase.boonGenGroupStats, this.phase.boonGenOGroupStats, this.phase.boonGenSquadStats),
                    offsData: getData(this.phase.offBuffStats, this.phase.offBuffGenSelfStats,
                        this.phase.offBuffGenGroupStats, this.phase.offBuffGenOGroupStats, this.phase.offBuffGenSquadStats),
                    defsData: getData(this.phase.defBuffStats, this.phase.defBuffGenSelfStats,
                        this.phase.defBuffGenGroupStats, this.phase.defBuffGenOGroupStats, this.phase.defBuffGenSquadStats)
                };
                this.cache.set(this.phase, res);
                return res;
            }
        },
    });
};
