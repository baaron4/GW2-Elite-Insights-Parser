

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

	$.each(window.data.mechanics, function (i, mechanic) {
		var chart = { y: [],};
		if (mechanic.enemyMech) {
			var l = mechanic.data[phase].length;
			$.each(mechanic.data[phase][l - 1], function (pd, time) {
				var y = hpPoints[Math.floor(time)];
				if (!y) y = 0;
				chart.y.push(y);
			});
		} else {
			$.each(mechanic.data[phase], function (p, pdata) {
				$.each(pdata, function (pd, time) {
					var y = phaseData.players[p].boss[type][Math.floor(time)];
					if (!y) y = 0;
					chart.y.push(y);
				});
			});
		}
		lines.push(chart);
	});

	var callback = function () {
		Plotly.newPlot($target.attr('id'), lines, layout);
	};

	lazy($target, callback);
}