/*jshint esversion: 6 */
"use strict";

function computeGradient(left, percent, right) {
    var template = "linear-gradient(to right, $fill$, $middle$, $black$)";
    var res = percent;
    var fillPercent = left + " " + res + "%";
    var blackPercent = right + " " + (100 - res) + "%";
    var middle = res + "%";
    template = template.replace("$fill$", fillPercent);
    template = template.replace("$black$", blackPercent);
    template = template.replace("$middle$", middle);
    return template;
};

function computeSliderGradient(color, fillColor, startPercent, endPercent) {
    var template = "linear-gradient(to right, $left$, $left2$, $middle$, $middle2$, $right$, $right2$)";
    var left = color + " " + 0 + "%";
    var left2 = color + " " + startPercent + "%";
    var right = color + " " + endPercent + "%";
    var right2 = color + " " + 100 + "%";
    var middle = fillColor + " " + startPercent + "%";
    var middle2 = fillColor + " " + endPercent + "%";
    template = template.replace("$left$", left);
    template = template.replace("$left2$", left2);
    template = template.replace("$right$", right);
    template = template.replace("$right2$", right2);
    template = template.replace("$middle$", middle);
    template = template.replace("$middle2$", middle2);
    return template;
};

function buildFallBackURL(skill) {
    if (!skill.icon || skill.fallBack) {
        return;
    }
    var apiIcon = skill.icon;
    if (!apiIcon.includes("render")) {
        return;
    }
    var splitIcon = apiIcon.split('/');
    var signature = splitIcon[splitIcon.length - 2];
    var id = splitIcon[splitIcon.length - 1].split('.')[0] + "-64px.png";
    skill.icon = "https://darthmaim-cdn.de/gw2treasures/icons/" + signature + "/" + id;
    skill.fallBack = true;
}

function findSkill(isBuff, id) {
    var skill;
    if (isBuff) {
        skill = logData.buffMap['b' + id] || {};
        skill.condi = true;
    } else {
        skill = logData.skillMap["s" + id] || {};
    }
    skill.id = id;
    if (!apiRenderServiceOkay) {
        buildFallBackURL(skill);
    }
    return skill;
}

function getTargetCacheID(activetargets) {
    var id = 0;
    for (var i = 0; i < activetargets.length; i++) {
        id += Math.pow(2, activetargets[i]);
    }
    return id;
}

function getDPSGraphCacheID(dpsmode, damagemode, graphmode, activetargets, phaseIndex, extra) {
    return "dps" + dpsmode + '-'+ damagemode + '-' + graphmode + '-' + getTargetCacheID(activetargets) + '-' + phaseIndex + (extra !== null ? '-' + extra : '');
}

function graphTypeEnumToString(mode) {
    var name = "";
    switch (mode) {
        case GraphType.DPS:
            name = "DPS";
            break;
        case GraphType.CenteredDPS:
            name = "Centered DPS";
            break;
        case GraphType.Damage:
            name = "Damage";
            break;
        default:
            break;
    }
    return name;
}

function addPointsToGraph(res, graph, max) {
    if (!graph) {
        return;
    }
    var points = [];
    for (var j = 0; j < graph.length; j++) {
        points[j] = graph[j][1] * max / 100.0;
    }
    res.push(points);
}

function addMechanicsToGraph(data, phase, phaseIndex) {
    for (var i = 0; i < graphData.mechanics.length; i++) {
        var mech = graphData.mechanics[i];
        var mechData = logData.mechanicMap[i];
        var chart = {
            x: [],
            mode: 'markers',
            visible: mech.visible ? null : 'legendonly',
            type: 'scatter',
            marker: {
                symbol: mech.symbol,
                color: mech.color,
                size: mech.size ? mech.size : 15
            },
            text: [],
            name: mechData.name,
            hoverinfo: 'text'
        };
        if (mechData.enemyMech) {
            for (var j = 0; j < mech.points[phaseIndex].length; j++) {
                var pts = mech.points[phaseIndex][j];
                var tarId = phase.targets[j];
                if (tarId >= 0) {
                    var target = logData.targets[tarId];
                    for (var k = 0; k < pts.length; k++) {
                        var time = pts[k];
                        chart.x.push(time);
                        chart.text.push(time + 's: ' + target.name);
                    }
                } else {
                    for (var k = 0; k < pts.length; k++) {
                        var time = pts[k][0];
                        chart.x.push(time);
                        chart.text.push(time + 's: ' + pts[k][1]);
                    }
                }
            }
        } else {
            for (var j = 0; j < mech.points[phaseIndex].length; j++) {
                var pts = mech.points[phaseIndex][j];
                var player = logData.players[j];
                for (var k = 0; k < pts.length; k++) {
                    var time = pts[k];
                    chart.x.push(time);
                    chart.text.push(time + 's: ' + player.name);
                }
            }
        }
        data.push(chart);
    }
}

function updateMechanicsYValues(res, phase, phaseIndex, phaseGraphData, max) {
    for (var i = 0; i < graphData.mechanics.length; i++) {
        var mech = graphData.mechanics[i];
        var mechData = logData.mechanicMap[i];
        var chart = [];
        res.push(chart);
        if (mechData.enemyMech) {
            for (var j = 0; j < mech.points[phaseIndex].length; j++) {
                var pts = mech.points[phaseIndex][j];
                var tarId = phase.targets[j];
                if (tarId >= 0) {
                    var health = phaseGraphData.targets[j].healthStates;
                    for (var k = 0; k < pts.length; k++) {
                        chart.push(findState(health, pts[k], 0, health.length - 1) * max / 100.0);
                    }
                } else {
                    for (var k = 0; k < pts.length; k++) {
                        chart.push(max * 0.5);
                    }
                }
            }
        } else {
            for (var j = 0; j < mech.points[phaseIndex].length; j++) {
                var pts = mech.points[phaseIndex][j];
                for (var k = 0; k < pts.length; k++) {
                    var time = pts[k];
                    var ftime = Math.floor(time);
                    var y = res[j][ftime];
                    var yp1 = res[j][ftime + 1];
                    chart.push(interpolatePoint(ftime, ftime + 1, y, yp1, time));
                }
            }
        }
    }
}


function interpolatePoint(x1, x2, y1, y2, x) {
    if (typeof y2 !== "undefined") {
        return y1 + (y2 - y1) / (x2 - x1) * (x - x1);
    } else {
        return y1;
    }
}

function damageTypeEnumToString(mode) {
    var name = "";
    switch (mode) {
        case DamageType.All:
            name = "All";
            break;
        case DamageType.Power:
            name = "Power";
            break;
        case DamageType.Condition:
            name = "Condition";
            break;
        case DamageType.Breakbar:
            name = "Breakbar";
            break;
        default:
            break;
    }
    return name;
}

function getDamageGraphName(damageMode, graphMode) {
    return damageTypeEnumToString(damageMode) + " " + graphTypeEnumToString(graphMode) + " Graph";
}

function computeRotationData(rotationData, images, data, phase, actor, yAxis) {
    if (rotationData) {
        var rotaTrace = {
            x: [],
            base: [],
            y: [],
            name: actor.name,
            text: [],
            orientation: 'h',
            mode: 'markers',
            type: 'bar',
            textposition: "none",
            width: [],
            hoverinfo: 'text',
            hoverlabel: {
                namelength: '-1'
            },
            yaxis: yAxis === 0 ? 'y' : 'y' + (yAxis + 1),
            marker: {
                color: [],
                width: '5',
                line: {
                    color: [],
                    width: '2.0'
                }
            },
            showlegend: false
        };
        for (var i = 0; i < rotationData.length; i++) {
            var item = rotationData[i];
            var x = item[0];
            var skillId = item[1];
            var duration = item[2];
            var endType = item[3];
            var quick = item[4];
            var skill = findSkill(false, skillId);
            var aa = false;
            var icon;
            var name = '???';
            if (skill) {
                aa = skill.aa;
                icon = skill.icon;
                name = skill.name;
            }

            if (!icon.includes("render") && !icon.includes("darthmaim")) {
                icon = null;
            }

            var fillColor;
            var originalDuration = duration;
            if (endType === RotationStatus.REDUCED) { 
                fillColor = 'rgb(0,0,255)'; 
            } else if (endType === RotationStatus.CANCEL) { 
                fillColor = 'rgb(255,0,0)'; 
            } else if (endType === RotationStatus.FULL) { 
                fillColor = 'rgb(0,255,0)'; 
            } else if (endType === RotationStatus.INSTANT) { 
                fillColor = 'rgb(0,255,255)'; 
                duration = 50; // so that the quad is visible
            } else { // UNKNOWN
                fillColor = 'rgb(255,255,0)'; 
            }

            var clampedX = Math.max(x, 0);
            var diffX = clampedX - x;
            var clampedWidth = Math.min(x + duration / 1000.0, phase.duration / 1000.0) - x - diffX;
            if (!aa && icon) {
                images.push({
                    source: icon,
                    xref: 'x',
                    yref: yAxis === 0 ? 'y' : 'y' + (yAxis + 1),
                    x: clampedX,
                    y: 0.0,
                    sizex: 1.0,
                    sizey: 1.0,
                    xanchor: 'middle',
                    yanchor: 'bottom'
                });
            }

            rotaTrace.x.push(clampedWidth - 0.001);
            rotaTrace.base.push(clampedX);
            rotaTrace.y.push(1.2);
            var text = `${name} at ${x}s`;
            rotaTrace.text.push(endType === RotationStatus.INSTANT ? text : text + ` for ${originalDuration}ms`);
            rotaTrace.width.push(aa ? 0.5 : 1.0);
            rotaTrace.marker.color.push(fillColor);

            var outlineR = quick > 0.0 ? quick * quickColor.r + (1.0 - quick) * normalColor.r : -quick * slowColor.r + (1.0 + quick) * normalColor.r;
            var outlineG = quick > 0.0 ? quick * quickColor.g + (1.0 - quick) * normalColor.g : -quick * slowColor.g + (1.0 + quick) * normalColor.g;
            var outlineB = quick > 0.0 ? quick * quickColor.b + (1.0 - quick) * normalColor.b : -quick * slowColor.b + (1.0 + quick) * normalColor.b;
            rotaTrace.marker.line.color.push('rgb(' + outlineR + ',' + outlineG + ',' + outlineB + ')');
        }
        data.push(rotaTrace);
        return 1;
    }
    return 0;
}

function computePhaseMarkupSettings(currentArea, areas, annotations) {
    var y = 1;
    var textbg = '#0000FF';
    var x = (currentArea.end + currentArea.start) / 2;
    for (var i = annotations.length - 1; i >= 0; i--) {
        var annotation = annotations[i];
        var area = areas[i];
        if ((area.start <= currentArea.start && area.end >= currentArea.end) || area.end >= currentArea.start - 2) {
            // current area included in area OR current area intersects area
            if (annotation.bgcolor === textbg) {
                textbg = '#FF0000';
            }
            y = annotation.y === y && area.end > currentArea.start ? 1.09 : y;
            break;
        }
    }
    return {
        y: y,
        x: x,
        textbg: textbg
    };
}

function computePhaseMarkups(shapes, annotations, phase, linecolor) {
    if (phase.markupAreas) {
        for (var i = 0; i < phase.markupAreas.length; i++) {
            var area = phase.markupAreas[i];
            var setting = computePhaseMarkupSettings(area, phase.markupAreas, annotations);
            if (area.label) {
                annotations.push({
                    x: setting.x,
                    y: setting.y,
                    xref: 'x',
                    yref: 'paper',
                    xanchor: 'center',
                    yanchor: 'bottom',
                    text: area.label + '<br>' + '(' + Math.round(1000 * (area.end - area.start)) / 1000 + ' s)',
                    font: {
                        color: '#ffffff'
                    },
                    showarrow: false,
                    bordercolor: '#A0A0A0',
                    borderwidth: 2,
                    bgcolor: setting.textbg,
                    opacity: 0.8
                });
            }

            if (area.highlight) {
                shapes.push({
                    type: 'rect',
                    xref: 'x',
                    yref: 'paper',
                    x0: area.start,
                    y0: 0,
                    x1: area.end,
                    y1: 1,
                    fillcolor: setting.textbg,
                    opacity: 0.2,
                    line: {
                        width: 0
                    },
                    layer: 'below'
                });
            }
        }
    }
    if (phase.markupLines) {
        for (var i = 0; i < phase.markupLines.length; i++) {
            var x = phase.markupLines[i];
            shapes.push({
                type: 'line',
                xref: 'x',
                yref: 'paper',
                x0: x,
                y0: 0,
                x1: x,
                y1: 1,
                line: {
                    color: linecolor,
                    width: 2,
                    dash: 'dash'
                },
                opacity: 0.6,
            });
        }
    }
}


function computePlayerDPS(player, damageGraphs, lim, phasebreaks, activetargets, cacheID, times, graphMode, damageMode) {
    if (player.dpsGraphCache.has(cacheID)) {
        return player.dpsGraphCache.get(cacheID);
    }
    var totalDamage = 0;
    var totalDamageTaken = 0;
    var targetDamage = 0;
    var totalDPS = [0];
    var cleaveDPS = [0];
    var targetDPS = [0];
    var takenDPS = [0];
    var maxDPS = {
        total: 0,
        cleave: 0,
        target: 0,
        taken: 0
    };
    var centeredDPS = graphMode === GraphType.CenteredDPS && times[times.length - 1] > 2;
    if (centeredDPS) {
        lim /= 2;
    }
    var end = times.length;
    var left = 0, right = 0, targetid, k;
    var roundingToUse = damageMode === DamageType.Breakbar ? numberComponent.methods.round1 : numberComponent.methods.round;
    for (var j = 0; j < end; j++) {
        var time = times[j];
        if (lim > 0) {
            left = Math.max(Math.round(time - lim), 0);
        } else if (phasebreaks && phasebreaks[j]) {
            left = j;
        }
        right = j;    
        if (centeredDPS) {
            if (lim > 0) {
                right = Math.min(Math.round(time + lim), end - 1);
            } else if (phasebreaks) {
                for (var i = left + 1; i < phasebreaks.length; i++) {
                    if (phasebreaks[i]) {
                        right = i;
                        break;
                    }
                }
            } else {
                right = end - 1;
            }
        }          
        var div = graphMode !== GraphType.Damage ? Math.max(times[right] - times[left], 1) : 1;
        totalDamage = damageGraphs.total[right] - damageGraphs.total[left];
        totalDamageTaken = damageGraphs.taken[right] - damageGraphs.taken[left];
        targetDamage = 0;
        for (k = 0; k < activetargets.length; k++) {
            targetid = activetargets[k];
            targetDamage += damageGraphs.targets[targetid][right] - damageGraphs.targets[targetid][left];
        }
        totalDPS[j] = roundingToUse(totalDamage / div);
        targetDPS[j] = roundingToUse(targetDamage / div);
        cleaveDPS[j] = roundingToUse((totalDamage - targetDamage) / div);
        takenDPS[j] = roundingToUse(totalDamageTaken / div);
        maxDPS.total = Math.max(maxDPS.total, totalDPS[j]);
        maxDPS.target = Math.max(maxDPS.target, targetDPS[j]);
        maxDPS.cleave = Math.max(maxDPS.cleave, cleaveDPS[j]);
        maxDPS.taken = Math.max(maxDPS.taken, takenDPS[j]);
    }
    if (maxDPS.total < 1e-6) {
        maxDPS.total = 10;
    }
    if (maxDPS.target < 1e-6) {
        maxDPS.target = 10;
    }
    if (maxDPS.cleave < 1e-6) {
        maxDPS.cleave = 10;
    }
    if (maxDPS.taken < 1e-6) {
        maxDPS.taken = 10;
    }
    var res = {
        dps: {
            total: totalDPS,
            target: targetDPS,
            cleave: cleaveDPS,
            taken: takenDPS
        },
        maxDPS: maxDPS
    };
    player.dpsGraphCache.set(cacheID, res);
    return res;
}

function findState(states, timeS, start, end) {
    // when the array exists, it covers from 0 to fightEnd by construction
    var id = Math.floor((end + start) / 2);
    if (id === start || id === end) {
        return states[id][1];
    }
    var item = states[id];
    var itemN = states[id + 1];
    var x = item[0];
    var xN = itemN[0];
    if (timeS < x) {
        return findState(states, timeS, start, id);
    } else if (timeS > xN) {
        return findState(states, timeS, id, end);
    } else {
        return item[1];
    }
}

function getActorGraphLayout(images, color, hasBuffs, noIncoming) {
    let layout =  {
        barmode: 'stack',
        yaxis2: {
            title: 'Rotation',
            domain: hasBuffs ? [0.45, 0.54] : [0.0, 0.09],
            fixedrange: true,
            showgrid: false,
            showticklabels: false,
            color: color,
            range: [0, 2]
        },      
        legend: {
            traceorder: 'reversed'
        },
        hovermode: 'x',
        hoverdistance: 150,
        yaxis: {
            title: 'Duration Buffs',
            domain: hasBuffs ? [0.0, 0.44] : [0.0, 0.0],
            color: color,
            gridcolor: color,
            tickformat: ",d",
            fixedrange: true,
            side: 'right',
            range: [0, 1.5],
            nticks: 1
        },
        yaxis4: {
            title: 'Intensity Buffs',
            domain: hasBuffs ? [0.0, 0.44] : [0.0, 0.0],
            color: color,
            gridcolor: color,
            tickformat: ",d",
            fixedrange: true,
            overlaying: 'y',
            nticks: 10,
        },
        images: images,
        font: {
            color: color
        },
        xaxis: {
            title: 'Time(sec)',
            color: color,
            rangemode: 'nonnegative',
            gridcolor: color,
            tickmode: 'auto',
            nticks: 8,
            xrangeslider: {},
            domain: [0.0, 0.95],
        },
        paper_bgcolor: 'rgba(0,0,0,0)',
        plot_bgcolor: 'rgba(0,0,0,0)',
        shapes: [],
        annotations: [],
        autosize: true,
        width: 1300,
        height: 850,
        datarevision: new Date().getTime(),
    };
    if (noIncoming) {
        Object.assign(layout, {
            yaxis3: {
                title: 'Outgoing',
                domain: hasBuffs ? [0.55, 1.0] : [0.1, 1.0],
                color: color,
                gridcolor: color,
                tickformat: ",d",
            },
        });
    } else {
        Object.assign(layout, {
            yaxis3: {
                title: 'Outgoing',
                domain: hasBuffs ? [0.68, 1.0] : [0.23, 1.0],
                color: color,
                gridcolor: color,
                tickformat: ",d",
            },
            yaxis5: {
                title: 'Incoming',
                domain: hasBuffs ? [0.55, 0.67] : [0.1, 0.22],
                color: color,
                gridcolor: color,
                tickformat: ",d",
                nticks: 2,
            },
        });
    }
    return layout;
}

function _computeTargetGraphData(graph, targets, phase, data, yaxis, jsonGraphName, percentName, graphName, visible) {
    var count = 0;
    for (var i = 0; i < graph.targets.length; i++) {
        var graphData = graph.targets[i][jsonGraphName];
        if (!graphData) {
            continue;
        }
        count++;
        var texts = [];
        var times = [];
        var target = targets[phase.targets[i]];
        for (var j = 0; j < graphData.length; j++) {
          texts[j] = graphData[j][1] + "% " + percentName + " - " + target.name;
          times[j] = graphData[j][0];
        }
        var res = {
          x: times,
          text: texts,
          mode: "lines",
          line: {
            dash: "dashdot",
            shape: "hv",
          },
          hoverinfo: "text",
          visible: visible ? true : "legendonly",
          name: target.name + " " + graphName,
        };
        if (yaxis) {
            res.yaxis = yaxis;
        }
        data.push(res);
    }
    return count;
}

function computeTargetHealthData(graph, targets, phase, data, yaxis) {
    return _computeTargetGraphData(graph, targets, phase, data, yaxis, "healthStates", "hp", "health", !logData.wvw);
}

function computeTargetBarrierData(graph, targets, phase, data, yaxis) {
    return _computeTargetGraphData(graph, targets, phase, data, yaxis, "barrierStates", "barrier", "barrier", false);
}

function computeTargetBreakbarData(graph, targets, phase, data, yaxis) {
    return _computeTargetGraphData(graph, targets, phase, data, yaxis, "breakbarPercentStates", "breakbar", "breakbar", phase.breakbarPhase);
}

function _computePlayerGraphData(graph, player, data, yaxis, graphName, percentName) {
    if (!graph) {
        return 0;
    }
    var texts = [];
    var times = [];
    for (var j = 0; j < graph.length; j++) {
        texts[j] = graph[j][1] + "%" + percentName + " - " + player.name;
        times[j] = graph[j][0];
    }
    var res = {
        x: times,
        text: texts,
        mode: 'lines',
        line: {
            dash: 'dashdot',
            shape: 'hv'
        },
        hoverinfo: 'text',
        name: player.name + ' ' + graphName,
        visible: 'legendonly',
    };
    if (yaxis) {
        res.yaxis = yaxis;
    }
    data.push(res);
    return 1;
}

function computePlayerHealthData(healthGraph, player, data, yaxis) {
    return _computePlayerGraphData(healthGraph, player, data, yaxis, "health", "hp");
}

function computePlayerBarrierData(barrierGraph, player, data, yaxis) {
    return _computePlayerGraphData(barrierGraph, player, data, yaxis, "barrier", "barrier");
}

function computeBuffData(buffData, data) {
    if (buffData) {
        for (var i = 0; i < buffData.length; i++) {
            var boonItem = buffData[i];
            var boon = findSkill(true, boonItem.id);
            var line = {
                x: [],
                y: [],
                text: [],
                yaxis: boon.stacking ? 'y4' : 'y',
                type: 'scatter',
                visible: boonItem.visible ? null : 'legendonly',
                line: {
                    color: boonItem.color,
                    shape: 'hv'
                },
                hoverinfo: 'text+x',
                fill: 'tozeroy',
                name: boon.name.substring(0, 20)
            };
            for (var p = 0; p < boonItem.states.length; p++) {
                line.x.push(boonItem.states[p][0]);
                line.y.push(boonItem.states[p][1]);
                line.text.push(boon.name + ': ' + boonItem.states[p][1]);
            }
            data.push(line);
        }
        return buffData.length;
    }
    return 0;
}

function computeTargetDPS(target, damageGraphs, lim, phasebreaks, cacheID, times, graphMode) {
    if (target.dpsGraphCache.has(cacheID)) {
        return target.dpsGraphCache.get(cacheID);
    }
    var totalDamage = 0;
    var totalDPS = [0];
    var maxDPS = 0;
    var left = 0, right = 0;
    var end = times.length;
    var centeredDPS = graphMode === GraphType.CenteredDPS && times[times.length - 1] > 2;
    if (centeredDPS) {
        lim /= 2;
    }
    for (var j = 0; j < end; j++) {
        var time = times[j];
        if (lim > 0) {
            left = Math.max(Math.round(time - lim), 0);
        } else if (phasebreaks && phasebreaks[j]) {
            left = j;
        }
        right = j;
        if (gcenteredDPS) {
            if (lim > 0) {
                right = Math.min(Math.round(time + lim), end - 1);
            } else if (phasebreaks) {
                for (var i = left + 1; i < phasebreaks.length; i++) {
                    if (phasebreaks[i]) {
                        right = i;
                        break;
                    }
                }
            } else {
                right = end - 1;
            }
        }
        var div = graphMode !== GraphType.Damage ? Math.max(times[right] - times[left], 1) : 1;
        totalDamage = damageGraphs[right] - damageGraphs[left];
        totalDPS[j] = Math.round(totalDamage / (div));
        maxDPS = Math.max(maxDPS, totalDPS[j]);
    }
    if (maxDPS < 1e-6) {
        maxDPS = 10;
    }
    var res = {
        dps: totalDPS,
        maxDPS: maxDPS
    };
    target.dpsGraphCache.set(cacheID, res);
    return res;
}

function addTargetLayout(data, target, states, percentName, graphName, visible) {
    if (!states) {
        return 0;
    }
    var texts = [];
    var times = [];
    for (var j = 0; j < states.length; j++) {
        texts[j] = states[j][1] + "% " + percentName;
        times[j] = states[j][0];
    }
    var res = {
        x: times,
        text: texts,
        mode: 'lines',
        line: {
            dash: 'dashdot',
            shape: 'hv'
        },
        hoverinfo: 'text',
        visible: visible ? true : 'legendonly',
        name: target.name + ' ' + graphName,
        yaxis: 'y3'
    };
    data.push(res);
    return 1;
}


/*function getActorGraphLayout(images, boonYs, stackingBoons) {
    var layout = {
        barmode: 'stack',
        yaxis: {
            title: 'Rotation',
            domain: [0, 0.1],
            fixedrange: true,
            showgrid: false,
            showticklabels: false,
            color: '#cccccc',
            range: [0, 2]
        },
        legend: {
            traceorder: 'reversed'
        },
        hovermode: 'compare',
        images: images,
        font: {
            color: '#cccccc'
        },
        xaxis: {
            title: 'Time(sec)',         
            rangemode: 'nonnegative',
            color: '#cccccc',
            tickmode: 'auto',
            nticks: 8,
            gridcolor: '#cccccc',
            xrangeslider: {}
        },
        paper_bgcolor: 'rgba(0,0,0,0)',
        plot_bgcolor: 'rgba(0,0,0,0)',
        shapes: [],
        annotations: [],
        autosize: true,
        width: 1100,
        height: 1100,
        datarevision: new Date().getTime(),
    };
    layout['yaxis' + (2 + boonYs)] = {
        title: 'DPS',
        color: '#cccccc',
        gridcolor: '#cccccc',
        domain: [0.75, 1]
    };
    var perBoon = 0.65 / boonYs;
    var singleBuffs = boonYs;
    if (stackingBoons) {
        layout['yaxis' + (2 + boonYs - 1)] = {
            title: 'Stacking Buffs',
            color: '#cccccc',
            gridcolor: '#cccccc',
            domain: [0.70, 0.75]
        };
        perBoon = 0.6 / (boonYs - 1);
        singleBuffs--;
    }
    for (var i = 0; i < singleBuffs; i++) {
        layout['yaxis' + (2 + i)] = {
            title: '',
            color: '#cccccc',
            showgrid: false,
            showticklabels: false,
            domain: [0.1 + i * perBoon, 0.1 + (i + 1) * perBoon]
        };
    }
    return layout;
}*/

/*
function computeBuffData(buffData, data) {
    var ystart = 0;
    if (buffData) {
        var stackings = [];
        var i;
        for (i = buffData.length - 1; i >= 0; i--) {
            var boonItem = buffData[i];
            var boon = findSkill(true, boonItem.id);
            var line = {
                x: [],
                y: [],
                yaxis: boon.stacking ? 'stacking' : 'y' + (2 + ystart++),
                type: 'scatter',
                visible: boonItem.visible || !boon.stacking ? null : 'legendonly',
                line: {
                    color: boonItem.color,
                    shape: 'hv'
                },
                fill: boon.stacking ? 'tozeroy' : 'toself',
                name: boon.name,
                showlegend: boon.stacking ? true : false,
            };
            for (var p = 0; p < boonItem.states.length; p++) {
                line.x[p] = boonItem.states[p][0];
                line.y[p] = boonItem.states[p][1];
            }
            if (boon.stacking) {
                stackings.push(line);
            }
            data.push(line);
        }
        if (stackings.length) {
            var axis = 'y' + (2 + ystart++);
            for (i = 0; i < stackings.length; i++) {
                stackings[i].yaxis = axis;
            }
        }
        return {
            actorOffset: buffData.length,
            y: ystart,
            stacking: stackings.length > 0
        };
    }
    return {
        actorOffset: 0,
        y: 0,
        stacking: false
    };
}*/

function hasRotations() {
    return logData.players.length > 1;
}

function hasOutgoingDamageMods() {
    return Object.keys(logData.damageModMap).length !== 0;
}

function hasIncomingDamageMods() {
    return Object.keys(logData.damageIncModMap).length !== 0 ;
}

function hasDamageMods() {
    return hasOutgoingDamageMods() || hasIncomingDamageMods();
}

function playerMechanics() {
    var playerMechanics = [];
    for (var i = 0; i < logData.mechanicMap.length; i++) {
        var mech = logData.mechanicMap[i];
        if (mech.playerMech) {
            playerMechanics.push(mech);
        }
    }
    return playerMechanics;
}

function enemyMechanics() {
    var enemyMechanics = [];
    for (var i = 0; i < logData.mechanicMap.length; i++) {
        var mech = logData.mechanicMap[i];
        if (mech.enemyMech) {
            enemyMechanics.push(mech);
        }
    }
    return enemyMechanics;
}

function hasMechanics() {
    if (logData.mechanicMap.length > 0 && !logData.noMechanics) {
        return enemyMechanics().length > 0 || playerMechanics().length > 0;
    }
    return false;
}

function hasTargets() {
    return !logData.targetless ;
}

function hasOffBuffs() {
    return logData.offBuffs.length > 0;
};
function hasDefBuffs() {
    return logData.defBuffs.length > 0;
};
function hasSupBuffs() {
    return logData.supBuffs.length > 0;
};
function hasGearBuffs() {
    return logData.gearBuffs.length > 0;
};
function hasDebuffs() {
    return logData.debuffs.length > 0;
};
function hasConditions() {
    return logData.conditions.length > 0;
};
function hasNourishments() {
    return logData.nourishments.length > 0;
};
function hasEnhancements() {
    return logData.enhancements.length > 0;
};
function hasOtherConsumables() {
    return logData.otherConsumables.length > 0;
};
function hasPersBuffs() {
    var hasPersBuffs = false;
    if (logData.persBuffs) {
        for (var prop in logData.persBuffs) {
            if (logData.persBuffs.hasOwnProperty(prop) && logData.persBuffs[prop].length > 0) {
                hasPersBuffs = true;
                break;
            }
        }
    }
    return hasPersBuffs;
};

function showDeathRecap() {
    for (var i = 0; i < logData.players.length; i++) {
        if (!!logData.players[i].details.deathRecap) {
            return true;
        }
    }
    return false;
}

function hasBarrierExtension() {
    return !!barrierStatsExtension;
}

function validateStartPath(path) {  
    const setting = EIUrlParams.get("startPage");
    if (!setting) {
        return false;
    }
    return setting.startsWith(path);
}
