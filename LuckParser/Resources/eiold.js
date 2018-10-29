function createBossCondiTable($target, boons, data, totalData) {
	if (!$target.length) return;
	var rows = [];
	var sums = [];

	var total = [];
	var avgCols = [];

	$.each(data, function (i, values) {
		var player = window.data.players[i];
		var g = player.group;
		rows.push({ player: player, data: values });
	});

	sums.push({ name: window.data.boss.name, data: totalData });

	var html = tmplBoonTable.render({ rows: rows, sums: sums, boons: boons }, { generation: true, condition: true });
	lazyTable2($target, html, { 'order': [[3, 'desc']] });
}

function createBossBoonTable($target, boons, data) {
	if (!$target.length) return;
	var row = { player: window.data.boss, data: data };

	var html = tmplBoonTable.render({ rows: [row], sums: [], boons: boons }, { generation: false });
	lazyTable2($target, html, { 'order': [[3, 'desc']] });
}


function findSkill(isBoon, id) {
	var skill;
	if (isBoon) {
		skill = window.data.boonMap['b' + id] || {};
	} else {
		skill = window.data.skillMap['s' + id] || {};
	}
	skill.condi = isBoon;
	return skill;
}

function createRotaTab($target, data) {
	var html = "";
	$.each(data, function (i, cast) {
		var skillName;
		var icon;
		var aa = false;
		var swapped = cast[1] == -2;
		var skill = window.data.skillMap['s' + cast[1]];
		if (skill) {
			skillName = skill.name;
			icon = skill.icon;
			aa = skill.aa;
		}
		if (icon && skillName) {
			html += '<span class="rot-skill' + (aa ? ' rot-aa' : '') + '"><img class="rot-icon'
				+ (cast[3] == 2 ? ' rot-cancelled' : '')
				+ '" src="'
				+ icon
				+ '" title= "' + skillName + ' Time: ' + cast[0] + 's, Dur: ' + cast[2] + 'ms"></span>';

			if (swapped) {
				html += '<br>';
			}
		}
	});
	var buildRota = function () {
		var $btns = $('<div style="margin-bottom: 8px; margin-top: 8px;"></div>').append(
			$('<button class="btn btn-primary btn-sm">').text('Switch size').click(function () {
				$('body').toggleClass('rot-small');
			})).append(' ').append($('<button class="btn btn-primary btn-sm">').text('Toggle AutoAttack').click(function () {
				$('body').toggleClass('rot-noaa');
			}));
		$target.append($btns);
		$target.append(html);
	};
	lazy($target, buildRota);
}

function createPlayerGraph($element, player, phaseIndex, playerIndex) {
	var plotData = [];
	var images = [];
	var dark = data.flags.dark;

	var rotationData = (player.details.rotation || {})[phaseIndex];
	if (rotationData) {
		$.each(rotationData, function (i, item) {
			var x = item[0];
			var skillId = item[1];
			var duration = item[2] / 1000.0;
			var endType = item[3];
			var quick = item[4];
			var skill = window.data.skillMap['s' + skillId];
			var aa = false;
			var icon;
			var name = '???';
			if (skill) {
				aa = skill.aa;
				icon = skill.icon;
				name = skill.name;
			}

			if (!aa && icon) {
				images.push({
					source: icon,
					xref: 'x',
					yref: 'y',
					x: x,
					y: 0,
					sizex: 1.1,
					sizey: 1.1,
					xanchor: 'left',
					yanchor: 'bottom'
				});
			}

			var fillColor;
			if (endType == 1) fillColor = 'rgb(40,40,220)';
			else if (endType == 2) fillColor = 'rgb(220,40,40)';
			else if (endType == 3) fillColor = 'rgb(40,220,40)';
			else fillColor = 'rgb(220,220,0)';

			plotData.push({
				x: [duration],
				y: [1.5],
				base: x,
				name: name,
				orientation: 'h',
				mode: 'markers',
				type: 'bar',
				width: aa ? 0.5 : 1,
				hoverinfo: 'name',
				hoverlabel: { namelength: '-1' },
				marker: {
					color: fillColor,
					width: '5',
					line: {
						color: quick ? 'rgb(220,40,220)' : 'rgb(20,20,20)',
						width: '1'
					}
				},
				showlegend: false
			});
		});
	}

	var boonData = (player.details.boonGraph || {})[phaseIndex];
	if (boonData) {
		$.each(boonData, function (i, boonItem) {
			var line = {
				x: [], y: [], yaxis: 'y2', type: 'scatter',
				visible: boonItem.visible ? null : 'legendonly',
				line: { color: boonItem.color, shape: 'hv' },
				fill: 'tozeroy',
				name: boonItem.name
			}
			for (var p = 0; p < boonItem.data.length; p++) {
				line.x[p] = boonItem.data[p][0];
				line.y[p] = boonItem.data[p][1];
			}
			plotData.push(line);
		});
	}

	var lines = [{ id: 'boss', name: 'DPS', color: 'colBoss' }, { id: 'cleave', name: 'Cleave DPS', color: 'colCleave' }, { id: 'total', name: 'TDPS', color: 'colTotal' }];

	var dpsData;
	if (playerIndex == -1) {
		dpsData = data.graphData[phaseIndex].boss;
	} else {
		dpsData = data.graphData[phaseIndex].players[playerIndex];
	}
	var seconds = dpsData.boss.full.length;
	var allX = [];
	for (var i = 0; i < seconds; i++) {
		allX[i] = i;
	}

	var maxDps = 100;

	for (var l = 0; l < lines.length; l++) {
		for (var t = 0; t < data.graphs.length; t++) {
			if (!dpsData[lines[l].id]) continue;
			var name = lines[l].name + ' ' + data.graphs[t].name;
			var points = dpsData[lines[l].id][data.graphs[t].id];

			var visible = null;
			var legendgroup = null;

			if (data.graphs[t].id != 'full') {
				visible = 'legendonly';
			} else {
				for (var h = 0; h < points.length; h++) {
					if (points[h] > maxDps) maxDps = points[h];
				}
			}
			plotData.push({
				x: allX,
				y: points,
				yaxis: 'y3',
				mode: 'lines',
				visible: visible,
				line: { shape: 'spline', color: player[lines[l].color] },
				name: name,
				legendgroup: data.graphs[t].id
			});
		}
	}

	$.each(data.dpsData, function (i, item) {
		var visible = null;
		var legendgroup = null;
		if (item.name.indexOf('10s') >= 0) {
			visible = 'legendonly';
		}
		else if (item.name.indexOf('30s') >= 0) {
			visible = 'legendonly';
		}
		plotData.push({
			x: allX,
			y: item.data,
			yaxis: 'y3',
			mode: 'lines',
			visible: visible,
			line: { shape: 'spline', color: item.color },
			name: item.name,
		});
	});

	// Boss HP line
	var hpPoints = [];
	var hpTexts = [];
	for (var i = 0; i < data.graphData[phaseIndex].bossHealth.length; i++) {
		hpPoints[i] = data.graphData[phaseIndex].bossHealth[i] * maxDps / 100.0;
		hpTexts[i] = data.graphData[phaseIndex].bossHealth[i] + "%";
	}

	plotData.push({
		y: hpPoints,
		x: allX,
		yaxis: 'y3',
		text: hpTexts,
		mode: 'lines',
		line: { shape: 'spline', dash: 'dashdot', color: '#808080' },
		hoverinfo: 'text+x+name',
		name: 'Boss health'
	});


	$.each(data.boonData, function (i, item) {
		var visible = item.name == "Might" || item.name == "Quickness" ? null : 'legendonly';
		plotData.push({
			x: allX,
			y: item.data,
			yaxis: 'y2',
			type: 'scatter',
			visible: visible,
			line: { color: item.color },
			fill: 'tozeroy',
			name: item.name
		});
	});

	var layout = {
		barmode: 'stack',
		yaxis: {
			title: 'Rotation',
			domain: [0, 0.09],
			fixedrange: true,
			showgrid: false,
			range: [0, 2]
		},
		legend: { traceorder: 'reversed' },
		hovermode: 'compare',
		yaxis2: { title: 'Boons', domain: [0.11, 0.50], fixedrange: true },
		yaxis3: { title: 'DPS', domain: [0.51, 1] },
		images: images,
		font: { color: dark ? '#ffffff' : '#000000' },
		paper_bgcolor: dark ? 'rgba(0,0,0,0)' : 'rgba(255, 255, 255, 0)',
		plot_bgcolor: dark ? 'rgba(0,0,0,0)' : 'rgba(255, 255, 255, 0)'
	};

	var callback = function () {
		Plotly.newPlot($element[0], plotData, layout);
	};

	lazy($element, callback);
}

function lazy($owner, callback) {
	if ('IntersectionObserver' in window) {
		let lazyTableObserver = new IntersectionObserver(function (entries, observer) {
			entries.forEach(function (entry) {
				if (entry.isIntersecting) {
					lazyTableObserver.unobserve(entry.target);
					callback();
				}
			});
		});
		lazyTableObserver.observe($owner[0]);
	} else {
		$(callback);
	}
}

function lazyTable($table, options) {
	lazy($table, function () {
		$table.DataTable(options);
	});
}

function createTable($target, tableHtml, options) {
	var $table = $(tableHtml);
	$target.append($table);
	$table.filter('table').DataTable(options);

	$target.find('[title]').tooltip({ html: true });
}

function lazyTable2($target, tableHtml, options) {
	lazy($target, function () {
		createTable($target, tableHtml, options);
	});
}


// Window generation

function buildTabs(tabLayout, parentId, level) {
	if (tabLayout.tabs.length == 1) {
		return buildContent(tabLayout.tabs[0].content, parentId, level);
	}
	var idPrefix = parentId ? parentId + '-' : 'tab';
	return tmplTabs.render(tabLayout, {
		idPrefix: idPrefix,
		level: level,
		buildContent: buildContent
	});
}

function buildTable(layout, parentId, level) {
	html = '<div id="' + layout.table + '"></div>';
	if (layout.caption) {
		html = '<h3>' + layout.caption + '</h3>' + html;
	}
	return html;
}

function extractDpsData(dmg, phaseBreaks) {
	var ret = { full: [0], s10: [0], s30: [0], phase: [0] };

	var count = dmg.length;
	var dmg_tot = 0;
	var dmg_10 = 0;
	var dmg_30 = 0;
	var dmg_phase = 0;
	var phaseBreak = 0;
	for (var i = 1; i < count; i++) {
		var lim10 = Math.max(i - 10, 0);
		var lim30 = Math.max(i - 30, 0);
		dmg_tot += dmg[i];
		dmg_10 += dmg[i];
		dmg_30 += dmg[i];
		dmg_phase += dmg[i];
		dmg_10 -= dmg[lim10];
		dmg_30 -= dmg[lim30];
		if (phaseBreaks && phaseBreaks[i-1]) {
			phaseBreak = i-1;
			dmg_phase = 0;
		}
		ret.full[i] = Math.round(dmg_tot / i);
		ret.s10[i] = Math.round(dmg_10 / (i - lim10));
		ret.s30[i] = Math.round(dmg_30 / (i - lim30));
		ret.phase[i] = Math.round(dmg_phase/(i-phaseBreak));
	}

	return ret;
}

function arrayAdd(a, b) {
	var c = [];
	var count = a.length;
	for (var i = 0; i < count; i++) {
		c[i] = a[i] + b[i];
	}
	return c;
}

function extractGraphData(graphData) {
	data.graphData = [];
	for (var i = 0; i < graphData.length; i++) {
		var phaseBreaks = [];
		if (i == 0) {
			for (var p = 1; p < window.data.phases.length; p++) {
				var phase = window.data.phases[p];
				phaseBreaks[Math.floor(phase.start)] = true;
				phaseBreaks[Math.floor(phase.end)] = true;
			}
		}

		data.graphData[i] = { bossHealth: graphData[i].bossHealth, players: [] };

		data.phases[i].graphFull = [];
		data.phases[i].graphS10 = [];
		data.phases[i].graphS30 = [];
		for (var p = 0; p < data.players.length; p++) {
			var graph = graphData[i].players[p]; // graph data for player p in phase i

			var bossDps = extractDpsData(graph.boss, phaseBreaks);
			var cleaveDps = extractDpsData(graph.cleave, phaseBreaks);
			var totDps = {
				full: arrayAdd(bossDps.full, cleaveDps.full),
				s10: arrayAdd(bossDps.s10, cleaveDps.s10),
				s30: arrayAdd(bossDps.s30, cleaveDps.s30),
				phase:arrayAdd(bossDps.phase,cleaveDps.phase)
			};

			data.graphData[i].players[p] = { boss: bossDps, cleave: cleaveDps, total: totDps };
		}
		var bossDps = extractDpsData(graphData[i].boss.boss);
		data.graphData[i].boss = { boss: bossDps };
	}
}

function createGraphs(graphData) {
	extractGraphData(graphData);

	for (var i = 0; i < data.phases.length; i++) {
		for (var t = 0; t < data.graphs.length; t++) {
			createGraph($('#DPSGraph' + i + '_' + data.graphs[t].id), data.graphData[i], i, data.graphs[t].id);

		}

		$.each(data.players, function (p, player) {
			createPlayerGraph($('#pgraph_' + p + '_' + i), player, i, p);
		});
		createPlayerGraph($('#boss_graph' + i), data.boss, i, -1);
	}
}

function createGraph($target, phaseData, phase, type) {
	if (!$target || !$target.length) return;
	var lines = [];
	var xAxis = [];
	var seconds = phaseData.players[0].boss[type].length;
	var maxDps = 0;
	var allPlayerDps = [];
	for (var i = 0; i < seconds; i++) xAxis[i] = i;
	for (var p = 0; p < window.data.players.length; p++) {
		var refPoints = phaseData.players[p].boss[type];
		for (var h = 0; h < refPoints.length; h++) {
			if (refPoints[h] > maxDps) maxDps = refPoints[h];
			allPlayerDps[h] = (allPlayerDps[h] || 0) + refPoints[h];
		}

		var player = window.data.players[p];
		lines.push({ y: phaseData.players[p].boss[type], x: xAxis, mode: 'lines', line: { shape: 'spline', color: player.colBoss }, name: player.name + ' DPS' });
		lines.push({ y: phaseData.players[p].total[type], x: xAxis, mode: 'lines', line: { shape: 'spline', color: player.colTotal }, visible: 'legendonly', name: player.name + ' TDPS' });
		lines.push({ y: phaseData.players[p].cleave[type], x: xAxis, mode: 'lines', line: { shape: 'spline', color: player.colCleave }, visible: 'legendonly', name: player.name + ' Cleave DPS' });
	}

	var layout = {
		yaxis: {
			title: 'DPS',
			fixedrange: false,
			rangemode: 'tozero',
			color: window.data.flags.dark ? '#cccccc' : '#000000'
		},
		xaxis: {
			title: 'Time(sec)',
			color: window.data.flags.dark ? '#cccccc' : '#000000',
			xrangeslider: {}
		},
		hovermode: 'compare',
		legend: { orientation: 'h', font: { size: 15 } },
		font: { color: window.data.flags.dark ? '#cccccc' : '#000000' },
		paper_bgcolor: 'rgba(0,0,0,0)',
		plot_bgcolor: 'rgba(0,0,0,0)',
		staticPlot: true,
		displayModeBar: false,
		shapes: [],
		annotations: []
	};

	var hpPoints = [];
	var hpTexts = [];
	for (var i = 0; i < phaseData.bossHealth.length; i++) {
		hpPoints[i] = phaseData.bossHealth[i] * maxDps / 100.0;
		hpTexts[i] = phaseData.bossHealth[i] + "%";
	}

	lines.push({ x: xAxis, y: allPlayerDps, mode: 'lines', line: { shape: 'spline' }, visible: 'legendonly', name: 'All Player Dps' });
	$.each(window.data.mechanics, function (i, mechanic) {
		var chart = { x: [], y: [], mode: 'markers', visible: mechanic.visible ? null : 'legendonly', type: 'scatter', marker: { symbol: mechanic.symbol, color: mechanic.color, size: 15 }, text: [], name: mechanic.name, hoverinfo: 'text' };
		if (mechanic.enemyMech) {
			var l = mechanic.data[phase].length;
			$.each(mechanic.data[phase][l - 1], function (pd, time) {
				chart.x.push(time);
				var y = hpPoints[Math.floor(time)];
				if (!y) y = 0;
				chart.y.push(y);
				chart.text.push(time + 's: ' + window.data.boss.name);
			});
		} else {
			$.each(mechanic.data[phase], function (p, pdata) {
				$.each(pdata, function (pd, time) {
					chart.x.push(time);
					var y = phaseData.players[p].boss[type][Math.floor(time)];
					if (!y) y = 0;
					chart.y.push(y);
					chart.text.push(time + 's: ' + window.data.players[p].name);
				});
			});
		}
		lines.push(chart);
	});

	lines.push({
		y: hpPoints,
		x: xAxis,
		text: hpTexts,
		mode: 'lines',
		line: { shape: 'spline', dash: 'dashdot', color: '#808080' },
		hoverinfo: 'text+x+name',
		name: 'Boss health',
		_yaxis: 'y2'
	});


	$.each(data.phases[phase].markupAreas, function (i, area) {
		if (area.label) {
			layout.annotations.push({
				x: (area.end + area.start) / 2,
				y: 1,
				xref: 'x',
				yref: 'paper',
				xanchor: 'center',
				yanchor: 'bottom',
				text: area.label + '<br>' + '(' + Math.round(area.end - area.start) + ' s)',
				showarrow: false
			});
		}
		if (area.highlight) {
			layout.shapes.push({
				type: 'rect',
				xref: 'x',
				yref: 'paper',
				x0: area.start,
				y0: 0,
				x1: area.end,
				y1: 1,
				fillcolor: '#808080',
				opacity: 0.125,
				line: { width: 0 }
			});
		}
	});

	$.each(data.phases[phase].markupLines, function (i, x) {
		layout.shapes.push({
			type: 'line',
			xref: 'x',
			yref: 'paper',
			x0: x,
			y0: 0,
			x1: x,
			y1: 1,
			opacity: 0.35,
			line: { color: '#00c0ff', width: 2, dash: 'dash' }
		});
	});

	var callback = function () {
		Plotly.newPlot($target.attr('id'), lines, layout);
	};

	lazy($target, callback);
}