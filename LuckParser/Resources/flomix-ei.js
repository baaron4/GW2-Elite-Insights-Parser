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

function createRotaTab($target, data) {
	var html = "";
	$.each(data, function(i, cast) {
		var skillName;
		var icon;
		var aa = false;
		var swapped = cast[1] == -2;
		var skill = window.data.skillMap['s'+cast[1]];
		if (skill) {
			skillName = skill.name;
			icon = skill.icon;
			aa = skill.aa;
		}
		if (icon && skillName) {
			html += '<span class="rot-skill'+(aa?' rot-aa':'')+'"><img class="rot-icon'
				+(cast[3]==2?' rot-cancelled':'')
				+'" src="'
				+icon
				+'" title= "'+skillName+' Time: '+cast[0]+ 's, Dur: '+cast[2]+'ms"></span>';

			if (swapped) {
				html += '<br>';
			}
		}
	});
	var buildRota = function() {
		var $btns = $('<div style="margin-bottom: 8px; margin-top: 8px;"></div>').append(
			$('<button class="btn btn-primary btn-sm">').text('Switch size').click(function() {
			$target.toggleClass('rot-small');
		})).append(' ').append($('<button class="btn btn-primary btn-sm">').text('Toggle AutoAttack').click(function() {
			$target.toggleClass('rot-noaa');
		}));
		$target.append($btns);
		$target.append(html);
	};
	lazy($target, buildRota);
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
			var quick = item[4];
			var skill = window.data.skillMap['s'+skillId];
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
						color: quick ? 'rgb(220,40,220)' : 'rgb(20,20,20)',
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

function lazy($owner, callback) {
	if ('IntersectionObserver' in window) {
		let lazyTableObserver = new IntersectionObserver(function(entries, observer) {
			entries.forEach(function(entry) {
				if (entry.isIntersecting){
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
	lazy($table, function() {
		$table.DataTable(options);
	});
}

function createTable($target, tableHtml, options) {
	var $table = $(tableHtml);
	$target.append($table);
	$table.DataTable(options);
	
	$target.find('[title]').tooltip({html:true});
}

function lazyTable2($target, tableHtml, options) {
	lazy($target, function() {
		createTable($target, tableHtml, options);
	});
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
			
			createRotaTab($('#rota_'+p+'_'+i), player.details.rotation[i]);
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
			if(data.flags.simpleRotation) {
				playerTabs.push({name:'Simple Rotation',content:{table:'rota_'+p+'_'+i},noTitle:true});
			}

			if (phase.deaths[p]>0) {
				playerTabs.push({name:'Death Recap',content:{table:'death_recap_'+p+'_'+i},noTitle:false});
			}

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


function extractDpsData(dmg) {
	var full = [0];
	var s10 = [0];
	var s30 = [0];

	var count = dmg.length;
	var dmg_tot = 0;
	var dmg_10 = 0;
	var dmg_30 = 0;
	for (var i = 1; i < count; i++) {
		var lim10 = Math.max(i-10,0);
		var lim30 = Math.max(i-30,0);
		dmg_tot+=dmg[i];
		dmg_10+=dmg[i];
		dmg_30+=dmg[i];
		dmg_10-=dmg[lim10];
		dmg_30-=dmg[lim30];
		full[i] = Math.round(dmg_tot/i);
		s10[i] = Math.round(dmg_10/(i-lim10));
		s30[i] = Math.round(dmg_30/(i-lim30));
	}
	
	return {full:full,s10:s10,s30:s30};
}

function arrayAdd(a,b) {
	var c = [];
	var count = a.length;
	for (var i=0;i<count;i++){
		c[i]=a[i]+b[i];
	}
	return c;
}

function extractGraphData(graphData) {
	data.graphData = [];
	for (var i = 0; i < graphData.length; i++) {
		data.graphData[i] = [];

		data.phases[i].graphFull = [];
		data.phases[i].graphS10 = [];
		data.phases[i].graphS30 = [];
		for (var p = 0; p < data.players.length;p++) {
			var graph = graphData[i][p]; // graph data for player p in phase i

			var bossDps = extractDpsData(graph.boss);
			var cleaveDps = extractDpsData(graph.cleave);
			var totDps = {
				full:arrayAdd(bossDps.full,cleaveDps.full),
				s10:arrayAdd(bossDps.s10,cleaveDps.s10),
				s30:arrayAdd(bossDps.s30,cleaveDps.s30)};
		
			data.graphData[i][p] = {boss:bossDps,cleave:cleaveDps,total:totDps};
		}
	}
}

function createGraphs(graphData) {
	extractGraphData(graphData);

	for (var i = 0; i < data.phases.length; i++) {
		for (var t = 0; t < data.graphs.length; t++) {
			createGraph($('#DPSGraph'+i+'_'+data.graphs[t].id), data.graphData[i], i, data.graphs[t].id);
		
		}
	}
}

function createGraph($target, phaseData, phase, type) {
	var lines = [];
	var xAxis = [];
	var seconds = phaseData[0].boss[type].length;
	for (var i = 0; i < seconds; i++) xAxis[i] = i;
	for (var p = 0; p < window.data.players.length; p++) { // Players in phase
		var player = window.data.players[p];
		lines.push({y: phaseData[p].boss[type],x: xAxis,mode: 'lines',line: {shape: 'spline',color:player.colBoss},name: player.name + ' DPS'});
		lines.push({y: phaseData[p].total[type],x: xAxis,mode: 'lines',line: {shape: 'spline',color:player.colTotal},visible:'legendonly',name: player.name + ' TDPS'});
		lines.push({y: phaseData[p].cleave[type],x: xAxis,mode: 'lines',line: {shape: 'spline',color:player.colTotal},visible:'legendonly',name: player.name + ' Cleave DPS'});
	}
	/*
	var mechanics = [
		{name: 'Golem Dmg',symbol:'square',color:'rgb(255,140,0)',players: [[],[229,285],[],[],[228,229],[],[],[],[],[],[]]},
		{name: 'Bomb DMG',symbol:'circle-open',color:'rgb(255,0,0)',players: [[68,84,100,116,160,176,192,208,250,340,356,368],[84,100,116,160,176,192,208,250,340,356,368],[68,84,100,116,160,176,192,208,250,340,356,368],[68,84,100,116,176,192,208,356,368],[68,84,100,116,160,176,192,208,250,340,356,368],[68,84,100,116,160,176,192,208,250,340,356,368],[68,84,100,116,160,176,192,208,250,340,356,368],[68,84,116,160,176,208,250,340,356],[68,84,100,116,160,176,192,208,250,340,356,368],[68,84,100,116,160,176,192,208,250,340,356,368]]},
		{name: 'Bomn',symbol:'circle',color:'rgb(255,0,0)',players: [[103],[],[195,371],[71,237],[163,211,343],[57],[87],[359],[147,327,417],[179]]},
		{name: 'Shackle',symbol:'diamond',color:'rgb(0,255,255)',players: [[],[],[248,308,338,368,398],[],[278],[],[428],[],[],[]]},
		{name: 'Shackle Dmg',symbol:'diamond-open',color:'rgb(0,255,255)',players: [[],[],[372],[372],[],[],[],[],[],[]]},
		{name: 'Cone',symbol:'triangle',color:'rgb(0,128,0)',players: [[],[],[],[],[],[],[],[],[],[]]},
		{name: 'Crack',symbol:'asterisk-open',color:'rgb(0,255,255)',players: [[],[],[],[],[],[],[433],[],[433],[]]},
		{name: 'Marks',symbol:'circle',color:'rgb(0,128,0)',players: [[134,143],[250,360,364,421],[232,421],[139,193,321,325,364,369],[133,189,280,285],[189,193,197,202,356,426],[241,245,272],[148,165,233,289,361],[181,233,329,406,421],[]]},
		{name: 'Suck Dmg',symbol:'circle-open',color:'rgb(255,140,0)',players: [[],[],[],[],[],[],[],[],[],[]]},
		{name: 'Dip Aoe',symbol:'hexagon',color:'rgb(255,140,0)',players: [[],[],[],[],[],[],[],[],[],[]]},
		{name: 'Knockback Dmg',symbol:'circle',color:'rgb(255,140,0)',players: [[213,293,373],[213,293,373],[213,293,373],[213,293,373],[213,373],[213,293,373],[213,293,373],[213,293],[293,373],[213,293,373]]},
		{name: 'Orbs CD',symbol:'square',color:'rgb(0,255,0)',players: [[435],[62,435],[435],[152,242,332,422,435],[435],[435],[435],[92,182,272,362,435],[32,122,212,302,392,435],[435]]},
		{name: 'DOWN',symbol:'cross',color:'rgb(255,0,0)',players: [[],[474.5],[228.5],[],[],[],[160.5,353.5],[],[],[]], visible: true},
		{name: 'DEAD',symbol:'x',color:'rgb(0,0,0)',players: [[],[],[],[],[],[],[],[],[],[]], visible: true}];
		*/

	var layout = {
		yaxis:{
			title:'DPS',
			fixedrange: false,
			rangemode: 'tozero',
			color: window.data.flags.dark?'#cccccc':'#000000'},
		xaxis:{
			title:'Time(sec)',
			color: window.data.flags.dark?'#cccccc':'#000000',
			xrangeslider: {}
		},
		hovermode: 'compare',
		legend: {orientation: 'h', font:{size: 15}},
		font: { color: window.data.flags.dark?'#cccccc':'#000000' },
		paper_bgcolor: 'rgba(0,0,0,0)',
		plot_bgcolor: 'rgba(0,0,0,0)',
		staticPlot: true,
		displayModeBar: false,
	};
	

	//data.push({x: xAxis, y: allPlayerDps, mode: 'lines',line: {shape: 'spline'},visible:'legendonly',name: 'All Player Dps'});
	$.each(window.data.mechanics, function(i, mechanic) {
		var chart = {x:[],y:[],mode:'markers',visible:mechanic.visible?null:'legendonly',type:'scatter',marker:{symbol:mechanic.symbol,color:mechanic.color,size:15},text:[],name:mechanic.name,hoverinfo:'text'};
		$.each(mechanic.data[phase], function(p,pdata) {
			$.each(pdata, function(pd,time){
				chart.x.push(time);
				var y = phaseData[p].boss[type][Math.floor(time)];
				if (!y)y = 0;
				chart.y.push(y);
				chart.text.push(time + 's: ' + window.data.players[p].name);
			});
		});
		lines.push(chart);
	});
  //TODO Boss health
	/*
	data.push({
		y: bossHealth,
		x: xAxis,
		mode: 'lines',
		line: {shape: 'spline', dash: 'dashdot'},
		hoverinfo: 'text',
		name: 'Boss health',
		_yaxis: 'y2'});
	*/
	
	var callback = function() {
		Plotly.newPlot($target.attr('id'), lines, layout);
	};
	
	
	
	var lazyplot = $target[0];
	if ('IntersectionObserver' in window) {
		let lazyPlotObserver = new IntersectionObserver(function(entries, observer) {
			entries.forEach(function(entry) {
				if (entry.isIntersecting){
					lazyPlotObserver.unobserve(entry.target);
					callback();
				}
			});
		});
		lazyPlotObserver.observe(lazyplot);
	} else {
		$(callback);
	}
}