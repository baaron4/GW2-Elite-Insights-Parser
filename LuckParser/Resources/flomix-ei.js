$.extend( $.fn.dataTable.defaults, {searching: false, ordering: true,paging: false,dom:"t"} );
$.views.converters("dec",
  function(val) {
    return Math.round(val*100)/100;
  }
);
$.views.converters("round",
  function(val) {
    return Math.round(val);
  }
);

var urls = {
	'Warrior': 'https://wiki.guildwars2.com/images/4/43/Warrior_tango_icon_20px.png',
	'Berserker': 'https://wiki.guildwars2.com/images/d/da/Berserker_tango_icon_20px.png',
	'Spellbreaker': 'https://wiki.guildwars2.com/images/e/ed/Spellbreaker_tango_icon_20px.png',
	'Guardian': 'https://wiki.guildwars2.com/images/8/8c/Guardian_tango_icon_20px.png',
	'Dragonhunter': 'https://wiki.guildwars2.com/images/c/c9/Dragonhunter_tango_icon_20px.png',
	'DragonHunter': 'https://wiki.guildwars2.com/images/c/c9/Dragonhunter_tango_icon_20px.png',
	'Firebrand': 'https://wiki.guildwars2.com/images/0/02/Firebrand_tango_icon_20px.png',
	'Revenant': 'https://wiki.guildwars2.com/images/b/b5/Revenant_tango_icon_20px.png',
	'Herald': 'https://wiki.guildwars2.com/images/6/67/Herald_tango_icon_20px.png',
	'Renegade': 'https://wiki.guildwars2.com/images/7/7c/Renegade_tango_icon_20px.png',
	'Engineer': 'https://wiki.guildwars2.com/images/2/27/Engineer_tango_icon_20px.png',
	'Scrapper': 'https://wiki.guildwars2.com/images/3/3a/Scrapper_tango_icon_200px.png',
	'Holosmith': 'https://wiki.guildwars2.com/images/2/28/Holosmith_tango_icon_20px.png',
	'Ranger': 'https://wiki.guildwars2.com/images/4/43/Ranger_tango_icon_20px.png',
	'Druid': 'https://wiki.guildwars2.com/images/d/d2/Druid_tango_icon_20px.png',
	'Soulbeast': 'https://wiki.guildwars2.com/images/7/7c/Soulbeast_tango_icon_20px.png',
	'Thief': 'https://wiki.guildwars2.com/images/7/7a/Thief_tango_icon_20px.png',
	'Daredevil': 'https://wiki.guildwars2.com/images/e/e1/Daredevil_tango_icon_20px.png',
	'Deadeye': 'https://wiki.guildwars2.com/images/c/c9/Deadeye_tango_icon_20px.png',
	'Elementalist': 'https://wiki.guildwars2.com/images/a/aa/Elementalist_tango_icon_20px.png',
	'Tempest': 'https://wiki.guildwars2.com/images/4/4a/Tempest_tango_icon_20px.png',
	'Weaver': 'https://wiki.guildwars2.com/images/f/fc/Weaver_tango_icon_20px.png',
	'Mesmer': 'https://wiki.guildwars2.com/images/6/60/Mesmer_tango_icon_20px.png',
	'Chronomancer': 'https://wiki.guildwars2.com/images/f/f4/Chronomancer_tango_icon_20px.png',
	'Mirage': 'https://wiki.guildwars2.com/images/d/df/Mirage_tango_icon_20px.png',
	'Necromancer': 'https://wiki.guildwars2.com/images/4/43/Necromancer_tango_icon_20px.png',
	'Reaper': 'https://wiki.guildwars2.com/images/1/11/Reaper_tango_icon_20px.png',
	'Scourge': 'https://wiki.guildwars2.com/images/0/06/Scourge_tango_icon_20px.png'
};

function profImg(p) {
	return $('<img>')
		.attr('src',urls[p])
		.attr('alt',p)
		.attr('title',p)
		.attr('width',18)
		.attr('height',18);
}

function createProfessionCell($cell,profession) {
	$cell.empty().attr('data-sort', profession).append(profImg(profession));
}

function createDpsTable($target, data) {
	var rows = [];
	var sums = [];
	var total = [0,0,0,0,0,0,0,0,0,0,0,0,0];
	var groups = [];

	$.each(data, function(i, dps) {
		var p = window.data.players[i];
		rows.push({player:p,dps:dps});
		if (!groups[p.group]) groups[p.group] = [0,0,0,0,0,0,0,0,0,0,0,0,0];
		for (var j = 0; j < 13; j++) {
			total[j]+=dps[j];
			groups[p.group][j]+=dps[j];
		}
	});
	for (var i = 0; i < groups.length; i++) {
		if (groups[i]) sums.push({name:'Group '+i,dps:groups[i]});
	}
	sums.push({name:'Total',dps:total});
	
	var html = tmplDpsTable.render({rows:rows,sums:sums});
	lazyTable2($target, html, { 'order': [[4, 'desc']]})
}

function createDamageStatsTable($target, data) {
	var rows = [];
	var sums = [];
	var total = [0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0];
	var groups = [];
	$.each(data, function(i, dmg) {
		var player = window.data.players[i];
		rows.push({player:player,data:dmg});

		if (!groups[player.group]) groups[player.group] = [0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0];
		for (var j = 0; j < total.length; j++) {
			//total[j]+=dmg[j];
			//groups[player.group][j]+=dmg[j];
		}

	});

	for (var i = 0; i < groups.length; i++) {
		if (groups[i]) sums.push({name:'Group '+i,data:groups[i]});
	}
	sums.push({name:'Total',data:total});

	var html = tmplDmgTable.render({rows:rows,sums:sums});
	lazyTable2($target, html, { 'order': [[0, 'asc']]});
}

function createDefStatsTable($target, data) {
	var rows = [];
	var sums = [];
	var total = [0,0,0,0,0,0,0,0];
	var groups = [];
	$.each(data, function(i, def) {
		var player = window.data.players[i];
		rows.push({player:player,data:def});
		if (!groups[player.group])groups[player.group] = [0,0,0,0,0,0,0,0];
		for (var j = 0; j < 8; j++) {
			total[j]+=def[j];
			groups[player.group][j]+=def[j];
		}
	});

	for (var i = 0; i < groups.length; i++) {
		if (groups[i])sums.push({name:'Group '+i,data:groups[i]});
	}
	sums.push({name:'Total',data:total});

	var html = tmplDefTable.render({rows:rows,sums:sums});
	lazyTable2($target, html, { 'order': [[3, 'desc']]});
}


function createSupStatsTable($target, data) {
	var rows = [];
	var sums = [];
	var total = [0,0,0,0];
	var groups = [];
	$.each(data, function(i, dps) {
		var player = window.data.players[i];
		rows.push({player:player,data:dps});
		if (!groups[player.group])groups[player.group] = [0,0,0,0];
		for (var j = 0; j < 4; j++) {
			total[j]+=dps[j];
			groups[player.group][j]+=dps[j];
		}
	});

	for (var i = 0; i < groups.length; i++) {
		if (groups[i]) sums.push({name:'Group '+i,data:groups[i]});
	}
	sums.push({name:'Total',data:total});
	
	var html = tmplSupTable.render({rows:rows,sums:sums});
	lazyTable2($target, html, { 'order': [[3, 'desc']]});
}

function createBoonTable($target, boons, data, generation) {
	var rows = [];
	var sums = [];

	var total = [0,0,0,0];
	var groups = [];
	
	$.each(data, function(i, values) {
		var player = window.data.players[i];
		rows.push({player:player,data:values});
		if (!groups[player.group]) {
			groups[player.group] = [0,0,0,0];
		}
		for (var j = 0; j < 4; j++) {
			total[j]+=values[j];
			groups[player.group][j]+=values[j];
		}
	});

	for (var i = 0; i < groups.length; i++) {
		if (groups[i]) {
			//var $row = $tfoot.find('tr.groupsum.template').clone().removeClass('template');
			//$row.find('.group').text('Group ' + i);
			//$rowTotal.before($row);
		}
	}

	var html = tmplBoonTable.render({rows:rows,sums:sums,boons:boons}, {generation:generation});
	lazyTable2($target, html, { 'order': [[3, 'desc']]});
}

function createMechanicsTable($target, mechanics, data) {
	var rows = [];
	var sums = [];
	$.each(data, function(i, values) {
		var player = window.data.players[i];
		rows.push({player:player,data:values});
	});

	var html = tmplMechanicTable.render({rows:rows,mechanics:mechanics});
	lazyTable2($target, html, { 'order': [[2, 'asc']]});
}

function createDistTable($target, data) {
	var rows = [];
	$.each(data.data, function(i, values) {
		var skill={};
		if (values[0]) {
			skill = window.data.boonMap['b'+values[1]] || {};
		} else {
			skill = window.data.skillMap['s'+values[1]] || {};
		}
		skill.condi = values[0];
		rows.push({skill:skill,data:values});
	});
	var html = tmplDmgDistTable.render({rows:rows,contribution:data.contribution,totalDamage:data.totalDamage});
	lazyTable2($target, html, { 'order': [[2, 'desc']]});
}

function createPlayerGraph(elementId, data, dark) {
	var allX = [];
	var plotData = [];
	var images = [];

	var seconds = data.dpsData[0].data.length;
	for (var i = 0; i < seconds; i++) {
		allX[i] = i;
	}

	if (data.rotationData) {
		$.each(data.rotationData, function(i, item) {
			var x = item[0];
			var skillId = item[1];
			var duration = item[2];
			var endType = item[3];
			var alac = item[4];
			var skill = data.skillInfo['s'+skillId];
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
				orientation:'h',
				mode: 'markers',
				type: 'bar',
				width: aa ? 0.5 : 1,
				hoverinfo: 'name',
				hoverlabel:{namelength:'-1'},
				marker: {
					color: fillColor,
					width: '5',
					line:{
						color: alac ? 'rgb(220,40,220)' : 'rgb(20,20,20)',
						width: '1'
					}
				},
				showlegend: false
			});
		});
	}

	$.each(data.dpsData, function(i, item) {
		var visible = null;
		var legendgroup = null;
		if (item.name.indexOf('10s')>=0) {
			visible = 'legendonly';
		}
		else if (item.name.indexOf('30s')>=0) {
			visible = 'legendonly';
		}
		plotData.push({
			x: allX,
			y: item.data,
			yaxis: 'y3',
			mode: 'lines',
			visible: visible,
			line: {shape:'spline', color:item.color},
			name: item.name,
		});
	});

	$.each(data.boonData, function(i, item) {
		var visible = item.name == "Might" || item.name == "Quickness" ? null : 'legendonly';
		plotData.push({
			x: allX,
			y: item.data,
			yaxis: 'y2',
			type: 'scatter',
			visible: visible,
			line: {color:item.color},
			fill: 'tozeroy',
			name: item.name
		});
	});

	var layout = {
		barmode:'stack',
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

	Plotly.newPlot(elementId, plotData, layout);
}

function lazyTable($table, options) {
	if ('IntersectionObserver' in window) {
		let lazyTableObserver = new IntersectionObserver(function(entries, observer) {
			entries.forEach(function(entry) {
				if (entry.isIntersecting){
					$table.DataTable(options);
					lazyTableObserver.unobserve(entry.target);
				}
			});
		});
		lazyTableObserver.observe($table[0]);
	} else {
		$table.DataTable(options);
	}
}

function createTable($target, tableHtml, options) {
	var $table = $(tableHtml);
	$target.append($table);
	$table.DataTable(options);
	
	$target.find('[title]').tooltip({html:true});
}

function lazyTable2($target, tableHtml, options) {
	if ('IntersectionObserver' in window) {
		let lazyTableObserver = new IntersectionObserver(function(entries, observer) {
			entries.forEach(function(entry) {
				if (entry.isIntersecting){
					lazyTableObserver.unobserve(entry.target);
					createTable($target, tableHtml, options);
				}
			});
		});
		lazyTableObserver.observe($target[0]);
	} else {
		$(function() {
			createTable($target, tableHtml, options);
		});
	}
}






// Window generation

function buildTabs(tabLayout, parentId, level) {
	var idPrefix = parentId ? parentId+'-' : 'tab';
	return tmplTabs.render(tabLayout, {
		idPrefix:idPrefix,
		level:level,
		buildContent:buildContent
	});
}

function buildTable(layout,parentId,level) {
	return '<div id="'+layout.table+'"></div>';
}

function buildContent(layout, parentId, level) {
	if (layout.tabs) return buildTabs(layout,parentId,level);
	if (layout.table) return buildTable(layout,parentId,level);
	return layout;
}

function generateWindow(layout) {
	$.each(data.players, function(i, player) { player.icon = urls[player.profession]; });
	if (layout.tabs) {
		$('#content').html(buildTabs(layout,'',0));
	}
	
	$.each(data.phases, function(i, phaseData) {
		
		createDpsTable($('#dpsStats' + i), phaseData.dpsStats);
		createDamageStatsTable($('#dpsStatsBoss' + i), phaseData.dmgStatsBoss);
		createDamageStatsTable($('#dpsStatsAll' + i), phaseData.dmgStats);
		createDefStatsTable($('#defStats'+i), phaseData.defStats);
		createSupStatsTable($('#healStats'+i), phaseData.healStats);

		createMechanicsTable($('#mechanicStats'+i), data.mechanics, phaseData.mechanicStats);

		
		createBoonTable($('#boonsUptime'+i), data.boons, phaseData.boonStats);

		createBoonTable($('#boonsGenSelf'+i), data.boons, phaseData.boonGenSelfStats, true);
		createBoonTable($('#boonsGenGroup'+i), data.boons, phaseData.boonGenGroupStats, true);
		createBoonTable($('#boonsGenOGroup'+i), data.boons, phaseData.boonGenOGroupStats, true);
		createBoonTable($('#boonsGenSquad'+i), data.boons, phaseData.boonGenSquadStats, true);

		createBoonTable($('#offensiveUptime'+i), data.offBuffs, phaseData.offBuffStats);
		createBoonTable($('#offensiveGenSelf'+i), data.offBuffs, phaseData.offBuffGenSelfStats, true);
		createBoonTable($('#offensiveGenGroup'+i), data.offBuffs, phaseData.offBuffGenGroupStats, true);
		createBoonTable($('#offensiveGenOGroup'+i), data.offBuffs, phaseData.offBuffGenOGroupStats, true);
		createBoonTable($('#offensiveGenSquad'+i), data.offBuffs, phaseData.offBuffGenSquadStats, true);

		createBoonTable($('#defensiveUptime'+i), data.defBuffs, phaseData.defBuffStats);
		createBoonTable($('#defensiveGenSelf'+i), data.defBuffs, phaseData.defBuffGenSelfStats, true);
		createBoonTable($('#defensiveGenGroup'+i), data.defBuffs, phaseData.defBuffGenGroupStats, true);
		createBoonTable($('#defensiveGenOGroup'+i), data.defBuffs, phaseData.defBuffGenOGroupStats, true);
		createBoonTable($('#defensiveGenSquad'+i), data.defBuffs, phaseData.defBuffGenSquadStats, true);

		$.each(data.players, function(p, player) {
			createDistTable($('#dist_table_'+p+'_'+i+'_boss'), player.details.dmgDistributionsBoss[i]);
			createDistTable($('#dist_table_'+p+'_'+i), player.details.dmgDistributions[i]);
		});
	});

	$('[title]').tooltip({html:true});
}

function buildWindowLayout(data) {
	var tabs = [];
	var replayTab = {id:'replay',name: 'Combat Replay',content: 'Combat Replay'};
	$.each(data.phases, function(i, phase) {
		var playerSubtabs = [];
		$.each(data.players, function(p, player) {
			var playerTabs = [{name:player.name,content:{tabs: [
				{name:'Graph', content:'DPS/Rotation graph',noTitle:true},
				{name:'Boss', content:{table:'dist_table_'+p+'_'+i+'_boss'},noTitle:true},
				{name:'All', content:{table:'dist_table_'+p+'_'+i},noTitle:true}
			]},noTitle:true}];
			playerTabs.push({name:'Simple Rotation',content:'Simple Rotation',noTitle:true});
			$.each(player.minions, function(m,minion){
				playerTabs.push({name:minion.name,content:minion.name,noTitle:true});
			});
			playerTabs.push({name:'Damage Taken',content:'Damage Taken',noTitle:true});
			var playerContent = {tabs: playerTabs};
			playerSubtabs.push({name: player.name, icon: urls[player.profession], iconName: player.profession, content: playerContent});
		});
		var dpsGraphTabs = [];
			$.each(data.graphs, function (g,graph){
				dpsGraphTabs.push({
					name:graph.name,
					noTitle:true,
					content: '<div id="DPSGraph'+i+'_'+graph.id+'" style="height: 1000px; width:1200px;"></div>'});
			});
			var bossTabs = [{name:'Dhuum',content:'Dhuum'}];
			//TODO add boss minions
			bossTabs.push({name:'Deathling',content:'Deathling'});
			var phaseTabs = [
				{
					name:'Stats',content:{tabs: [
						{name:'DPS',noTitle:true, content: {table: 'dpsStats'+i}},
						{name:'Damage Stats',noTitle:true,content:{tabs: [
							{name:'Boss',noTitle:true,content:{table:'dpsStatsBoss'+i}},
							{name:'All',noTitle:true,content:{table:'dpsStatsAll'+i}}
						]}},
						{name:'Defensive Stats',noTitle:true,content:{table:'defStats'+i}},
						{name:'Heal Stats',noTitle:true,content:{table:'healStats'+i}}
					]}
				},
				{name:'Damage Graph',noTitle:true,content:{tabs: dpsGraphTabs}},
				{name:'Boons',content:{tabs:[
					{name:'Boons',noTitle:true,content:{tabs:[
						{name:'Uptime',noTitle:true,title:'Boon Uptime',content:{table:'boonsUptime'+i}},
						{name:'Generation (Self)',noTitle:true,title:'Boons generated by a character for themselves',content:{table:'boonsGenSelf'+i}},
						{name:'Generation (Group)',noTitle:true,title:'Boons generated by a character for their groupmates',content:{table:'boonsGenGroup'+i}},
						{name:'Generation (Off-Group)',noTitle:true,title:'Boons generated by a character for any subgroup that is not their own',content:{table:'boonsGenOGroup'+i}},
						{name:'Generation (Squad)',noTitle:true,title:'Boons generated by a character for their squadmates',content:{table:'boonsGenSquad'+i}}
					]}},
					{name:'Damage Buffs',noTitle:true,content:{tabs:[
						{name:'Uptime',noTitle:true,title:'Damage Buffs Uptime',content:{table:'offensiveUptime'+i}},
						{name:'Generation (Self)',noTitle:true,title:'Damage Buffs generated by a character for themselves',content:{table:'offensiveGenSelf'+i}},
						{name:'Generation (Group)',noTitle:true,title:'Damage Buffs generated by a character for their groupmates',content:{table:'offensiveGenGroup'+i}},
						{name:'Generation (Off-Group)',noTitle:true,title:'Damage Buffs generated by a character for any subgroup that is not their own',content:{table:'offensiveGenOGroup'+i}},
						{name:'Generation (Squad)',noTitle:true,title:'Damage Buffs generated by a character for their squadmates',content:{table:'offensiveGenSquad'+i}}
					]}},
					{name: 'Defensive Buffs',noTitle:true,content:{tabs:[
						{name:'Uptime',noTitle:true,title:'Defensive Buffs Uptime',content:{table:'defensiveUptime'+i}},
						{name:'Generation (Self)',noTitle:true,title:'Defensive Buffs generated by a character for themselves',content:{table:'defensiveGenSelf'+i}},
						{name:'Generation (Group)',noTitle:true,title:'Defensive Buffs generated by a character for their groupmates',content:{table:'defensiveGenGroup'+i}},
						{name:'Generation (Off-Group)',noTitle:true,title:'Defensive Buffs generated by a character for any subgroup that is not their own',content:{table:'defensiveGenOGroup'+i}},
						{name:'Generation (Squad)',noTitle:true,title:'Defensive Buffs generated by a character for their squadmates',content:{table:'defensiveGenSquad'+i}}
					]}}
				]}},
				{name:'Mechanics',content:{table:'mechanicStats'+i}},
				{name:'Player',content:'Player',subtabs: playerSubtabs},
				{name:'Boss',content:{tabs: bossTabs}},
				{name:'Estimates',content:'Estimates'}
			];
			if (data.phases.length == 1) {
				phaseTabs.push(replayTab);
			}
			var tab = {
				name: phase.name,
				title: phase.duration + " seconds",
				content: {tabs: phaseTabs}
			};
			tabs.push(tab);
	});
	if (data.phases.length > 1) {
		tabs.push(replayTab);
	}
	return {tabs: tabs};
}