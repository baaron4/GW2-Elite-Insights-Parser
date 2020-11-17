/*jshint esversion: 6 */
"use strict";
function computeGradient(left, percent) {
    var template = "linear-gradient(to right, $fill$, $middle$, $black$)";
    var res = percent;
    var fillPercent = left + " " + res + "%";
    var blackPercent = "black " + (100 - res) + "%";
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
    if (!apiRenderServiceOkay) {
        buildFallBackURL(skill);
    }
    return skill;
}

function getTargetCacheID(activetargets) {
    var id = 0;
    for (var i = 0; i < activetargets.length; i++) {
        id |= 1 << activetargets[i];
    }
    return id;
}

function getDPSGraphCacheID(dpsmode, graphmode, activetargets, phaseIndex, extra) {
    return dpsmode + '-' + graphmode + '-' + getTargetCacheID(activetargets) + '-' + phaseIndex + (extra !== null ? '-' + extra : '');
}

const quickColor = {
    r: 220,
    g: 20,
    b: 220
};
const slowColor = {
    r: 220,
    g: 125,
    b: 30
};
const normalColor = {
    r: 125,
    g: 125,
    b: 125
};

function computeRotationData(rotationData, images, data, phase) {
    if (rotationData) {
        var rotaTrace = {
            x: [],
            base: [],
            y: [],
            name: 'Rotation',
            text: [],
            orientation: 'h',
            mode: 'markers',
            type: 'bar',
            width: [],
            hoverinfo: 'text',
            hoverlabel: {
                namelength: '-1'
            },
            marker: {
                color: [],
                width: '5',
                line: {
                    color: [],
                    width: '2.0'
                }
            },
            showlegend: false
        }
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
            if (endType == 1) { 
                fillColor = 'rgb(0,0,255)'; 
            }
            else if (endType == 2) { 
                fillColor = 'rgb(255,0,0)'; 
            }
            else if (endType == 3) { 
                fillColor = 'rgb(0,255,0)'; 
            }
            else if (endType == 4) { 
                fillColor = 'rgb(0,255,255)'; 
                duration = 50;
            }
            else { 
                fillColor = 'rgb(255,255,0)'; 
            }

            var clampedX = Math.max(x, 0);
            var diffX = clampedX - x;
            var clampedWidth = Math.min(x + duration / 1000.0, phase.duration / 1000.0) - x - diffX;
            if (!aa && icon) {
                images.push({
                    source: icon,
                    xref: 'x',
                    yref: 'y',
                    x: clampedX,
                    y: 0.0,
                    sizex: 1.1,
                    sizey: 1.1,
                    xanchor: 'middle',
                    yanchor: 'bottom'
                });
            }

            rotaTrace.x.push(clampedWidth - 0.001);
            rotaTrace.base.push(clampedX);
            rotaTrace.y.push(1.2);
            rotaTrace.text.push(name + ' at ' + x + 's for ' + originalDuration + 'ms');
            rotaTrace.width.push(aa ? 0.5 : 1.0);
            rotaTrace.marker.color.push(fillColor);

            var outlineR = quick > 0.0 ? quick * quickColor.r + (1.0 - quick) * normalColor.r : -quick * slowColor.r + (1.0 + quick) * normalColor.r;
            var outlineG = quick > 0.0 ? quick * quickColor.g + (1.0 - quick) * normalColor.g : -quick * slowColor.r + (1.0 + quick) * normalColor.r;
            var outlineB = quick > 0.0 ? quick * quickColor.b + (1.0 - quick) * normalColor.b : -quick * slowColor.r + (1.0 + quick) * normalColor.r;
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


function computePlayerDPS(player, damageData, lim, phasebreaks, activetargets, cacheID, times, graphMode) {
    if (player.dpsGraphCache.has(cacheID)) {
        return player.dpsGraphCache.get(cacheID);
    }
    var totalDamage = 0;
    var targetDamage = 0;
    var totalDPS = [0];
    var cleaveDPS = [0];
    var targetDPS = [0];
    var maxDPS = {
        total: 0,
        cleave: 0,
        target: 0
    };
    if (graphMode === GraphType.CenteredDPS) {
        lim /= 2;
    }
    var end = times.length;
    var left = 0, right = 0, targetid, k;
    var roundingToUse = graphMode === GraphType.BreakbarDamage ? roundingComponent.methods.round1 : roundingComponent.methods.round;
    for (var j = 0; j < end; j++) {
        var time = times[j];
        if (lim > 0) {
            left = Math.max(Math.round(time - lim), 0);
        } else if (phasebreaks && phasebreaks[j]) {
            left = j;
        }
        right = j;    
        if (graphMode === GraphType.CenteredDPS) {
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
        var div = graphMode !== GraphType.Damage && graphMode !== GraphType.BreakbarDamage ? Math.max(times[right] - times[left], 1) : 1;
        totalDamage = damageData.total[right] - damageData.total[left];
        targetDamage = 0;
        for (k = 0; k < activetargets.length; k++) {
            targetid = activetargets[k];
            targetDamage += damageData.targets[targetid][right] - damageData.targets[targetid][left];
        }
        totalDPS[j] = roundingToUse(totalDamage / div);
        targetDPS[j] = roundingToUse(targetDamage / div);
        cleaveDPS[j] = roundingToUse((totalDamage - targetDamage) / div);
        maxDPS.total = Math.max(maxDPS.total, totalDPS[j]);
        maxDPS.target = Math.max(maxDPS.target, targetDPS[j]);
        maxDPS.cleave = Math.max(maxDPS.cleave, cleaveDPS[j]);
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
    var res = {
        dps: {
            total: totalDPS,
            target: targetDPS,
            cleave: cleaveDPS
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

function getActorGraphLayout(images, color) {
    return {
        barmode: 'stack',
        yaxis: {
            title: 'Rotation',
            domain: [0, 0.09],
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
        yaxis2: {
            title: 'Buffs',
            domain: [0.11, 0.6],
            color: color,
            gridcolor: color,
            fixedrange: true
        },
        yaxis3: {
            title: 'DPS',
            color: color,
            gridcolor: color,
            domain: [0.61, 1]
        },
        images: images,
        font: {
            color: color
        },
        xaxis: {
            title: 'Time(sec)',
            color: color,
            gridcolor: color,
            xrangeslider: {}
        },
        paper_bgcolor: 'rgba(0,0,0,0)',
        plot_bgcolor: 'rgba(0,0,0,0)',
        shapes: [],
        annotations: [],
        autosize: true,
        width: 1100,
        height: 800,
        datarevision: new Date().getTime(),
    };
}

function computeTargetHealthData(graph, targets, phase, data, yaxis) {
    for (var i = 0; i < graph.targets.length; i++) {
        var health = graph.targets[i].healthStates;
        var hpTexts = [];
        var times = [];
        var target = targets[phase.targets[i]];
        for (var j = 0; j < health.length; j++) {
            hpTexts[j] = health[j][1] + "% hp - " + target.name;
            times[j] = health[j][0];
        }
        var res = {
            x: times,
            text: hpTexts,
            mode: 'lines',
            line: {
                dash: 'dashdot',
                shape: 'hv'
            },
            hoverinfo: 'text',
            name: target.name + ' health',
        };
        if (yaxis) {
            res.yaxis = yaxis;
        }
        data.push(res);
    }
    return graph.targets.length;
}

function computeTargetBreakbarData(graph, targets, phase, data, yaxis) {
    var count = 0;
    for (var i = 0; i < graph.targets.length; i++) {
        var breakbar = graph.targets[i].breakbarPercentStates;
        if (!breakbar) {
            continue;
        }
        count++;
        var breakbarTexts = [];
        var times = [];
        var target = targets[phase.targets[i]];
        for (var j = 0; j < breakbar.length; j++) {
            breakbarTexts[j] = breakbar[j][1] + "% breakbar - " + target.name;
            times[j] = breakbar[j][0];
        }
        var res = {
            x: times,
            text: breakbarTexts,
            mode: 'lines',
            line: {
                dash: 'dashdot',
                shape: 'hv'
            },
            hoverinfo: 'text',
            visible: phase.breakbarPhase ? true : "legendonly",
            name: target.name + ' breakbar',
        };
        if (yaxis) {
            res.yaxis = yaxis;
        }
        data.push(res);
    }
    return count;
}

function computePlayerHealthData(healthGraph, data, yaxis) {
    var health = healthGraph;
    var hpTexts = [];
    var times = [];
    for (var j = 0; j < health.length; j++) {
        hpTexts[j] = health[j][1] + "% hp - Player";
        times[j] = health[j][0];
    }
    var res = {
        x: times,
        text: hpTexts,
        mode: 'lines',
        line: {
            dash: 'dashdot',
            shape: 'hv'
        },
        hoverinfo: 'text',
        name: 'Player health',
        visible: 'legendonly',
    };
    if (yaxis) {
        res.yaxis = yaxis;
    }
    data.push(res);
    return 1;
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
                yaxis: 'y2',
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

function initTable (id, cell, order, orderCallBack) {
    var table = $(id);
    if (!table.length) {
        return;
    }
    /*if (lazyTableUpdater) {
        var lazyTable = document.querySelector(id);
        var lazyTableObserver = new IntersectionObserver(function (entries, observer) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    table.DataTable({
                        order: [
                            [cell, order]
                        ]
                    });
                    if (orderCallBack) {
                        table.DataTable().on('order.dt', orderCallBack);
                    }
                    observer.unobserve(entry.target);
                }
            });
        });
        lazyTableObserver.observe(lazyTable);
    } else {*/
    var data = {
        order: [
            [cell, order]
        ]
    };
    table.DataTable(data);
    if (orderCallBack) {
        table.DataTable().on('order.dt', orderCallBack);
    }
    //}
};

function updateTable(id) {
    /*if (lazyTableUpdater) {
        var lazyTable = document.querySelector(id);
        lazyTableUpdater.unobserve(lazyTable);
        lazyTableUpdater.observe(lazyTable);
    } else {*/
    var table = $(id);
    if ($.fn.dataTable.isDataTable(id)) {
        table.DataTable().rows().invalidate('dom');
        table.DataTable().draw();
    }
    //}
};

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
            color: '#cccccc',
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
