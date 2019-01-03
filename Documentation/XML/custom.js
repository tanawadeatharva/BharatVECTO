<!--/*--><root><![CDATA[<!--*/--> 
$(document).ready(function() {
	$('th.xmltype').append('<span class="expand"><a title="show XML Type"</span>').append('<span class="collapse"><a title="hide column"</span>')
	$('th.xmlpath').append('<span class="expand"><a title="show XPath"</span>').append('<span class="collapse"><a title="hide column"</span>')
	$('th.genericValues').append('<span class="expand"><a title="show Generic Values"</span>').append('<span class="collapse"><a title="hide column"</span>')
	
	$('table.parameters th span.expand').click(function(){
		var $col = $(this).parent('th').index()
		$(this).parents('th').removeClass('collapsed')
		$(this).parents('table').find('tr>td:nth-child('+($col+1)+')').removeClass('collapsed')
	})
	$('table.parameters th span.collapse').click(function(){
		var $col = $(this).parent('th').index()
		$(this).parents('th').addClass('collapsed')
		$(this).parents('table').find('tr>td:nth-child('+($col+1)+')').addClass('collapsed')
	})
	
	
	$('table.parameters th.parameterId').append('<span class="sortAsc"><a title="sort ascending"/></span><span class="sortDesc"><a title="sort descending"/></span>')
	$('table.parameters th.component').append('<span class="sortAsc"><a title="sort ascending"/></span><span class="sortDesc"><a title="sort descending"/></span>')
	$('table.parameters th.name').append('<span class="sortAsc"><a title="sort ascending"/></span><span class="sortDesc"><a title="sort descending"/></span>')
	$('table.parameters th.xmlpath').append('<span class="sortAttrAsc"><a title="sort ascending"/></span><span class="sortAttrDesc"><a title="sort descending"/></span>')

	$('table.parameters th span.sortAsc').click(function(){
		var table = $(this).parents('table').eq(0)
		var rows = table.find("tr:not(:has('th'))").toArray().sort(comparer($(this).parent('th').index()))
		for (var i = 0; i < rows.length; i++){table.append(rows[i])}
	})
	$('table.parameters th span.sortDesc').click(function(){
		var table = $(this).parents('table').eq(0)
		var rows = table.find("tr:not(:has('th'))").toArray().sort(comparer($(this).parent('th').index()))
		rows = rows.reverse()
		for (var i = 0; i < rows.length; i++){table.append(rows[i])}
	})
	$('table.parameters th span.sortAttrAsc').click(function(){
		var table = $(this).parents('table').eq(0)
		var rows = table.find("tr:not(:has('th'))").toArray().sort(comparerAttr($(this).parent('th').index(), 'sorting'))
		for (var i = 0; i < rows.length; i++){table.append(rows[i])}
	})
	$('table.parameters th span.sortAttrDesc').click(function(){
		var table = $(this).parents('table').eq(0)
		var rows = table.find("tr:not(:has('th'))").toArray().sort(comparerAttr($(this).parent('th').index(),'sorting'))
		rows = rows.reverse()
		for (var i = 0; i < rows.length; i++){table.append(rows[i])}
	})
	function comparer(index) {
		return function(a, b) {
			var valA = getCellValue(a, index), valB = getCellValue(b, index)
			return $.isNumeric(valA) && $.isNumeric(valB) ? valA - valB : valA.localeCompare(valB)
		}
	}
	function comparerAttr(index, attr) {
		return function(a, b) {
			var valA = getCellAttrValue(a, index, attr), valB = getCellAttrValue(b, index, attr)
			return $.isNumeric(valA) && $.isNumeric(valB) ? valA - valB : valA.localeCompare(valB)
		}
	}
	function getCellValue(row, index){ return $(row).children('td').eq(index).html() }
	function getCellAttrValue(row, index, attr){ return $(row).children('td').eq(index).attr(attr) }
	
	
	$('table.parameters').before('<div class="filter"><span class="filterDecl"><span class="icon"/>Declaration</span><span class="filterEng"><span class="icon"/>Engineering</span></div>')
	$('.filter .filterEng').click(function() {
		if ($(this).hasClass('hidden')) {
			$('table.parameters tbody tr').filter(function() {
				return ($(this).hasClass('Engineering') && !$(this).hasClass('Declaration') ) || $(this).attr('class') =='hideMode' || !$(this).attr('class')
			}).removeClass('hideMode')
			$('table.parameters td div.engineering').removeClass('hideMode')
			$('span.optionalEngineering').show();
		} else {
			$('table.parameters tbody tr').filter(function() {
				return ($(this).hasClass('Engineering') && !$(this).hasClass('Declaration') )  || !$(this).attr('class')
			}).addClass('hideMode')	
			$('table.parameters td div.engineering').addClass('hideMode')			
			$('span.optionalEngineering').hide();			
		}
		$(this).toggleClass('hidden')
	})
	$('.filter .filterDecl').click(function() {
		if ($(this).hasClass('hidden')) {
			$('table.parameters tbody tr').filter(function() {
				return $(this).hasClass('Declaration') 
			}).removeClass('hideMode')
		} else {
			$('table.parameters tbody tr').filter(function() {
				return $(this).hasClass('Declaration')
			}).addClass('hideMode')			
		}
		$(this).toggleClass('hidden')
	})
	
	var components = $('table.parameters tbody td.component').map(function(idx, el) {return $(el).html()}).toArray()
	components = components.sort().filter(function(el,i,a){if(i==a.indexOf(el))return 1;return 0})
	$('table.parameters').before('<div class="filter">'+ components.map(function(c) { return '<span class="filterComp"><span class="icon"/>' + c + '</span>'}).join('') + '<span class="filterAll hidden"><span class="icon"/>Hide All</span><span class="showAll"><span class="icon"/>Show All</span>' + '</div>')
	$('div.filter .filterComp').click(function() {
		var searchText = $(this).text();
		if ($(this).hasClass('hidden')) {
			$('table.parameters tbody tr').filter(function() {
				return $(this).find('td.component').text() == searchText
			}).removeClass('hideComp')
		} else {
			$('table.parameters tbody tr').filter(function() {
				return $(this).find('td.component').text() == searchText
			}).addClass('hideComp')
		}
		$(this).toggleClass('hidden')
	})
	$('div.filter .filterAll').click(function() {
		$('table.parameters tbody tr').addClass('hideComp')
		$('div.filter .filterComp').addClass('hidden')
	})
	$('div.filter .showAll').click(function() {
		$('table.parameters tbody tr').removeClass('hideComp')
		$('div.filter .filterComp').removeClass('hidden')
	})
	
	// initial view
	$('table.parameters th span.sortAttrAsc').trigger('click')
	$('div.filter span.filterEng').trigger('click')
	$('th.genericValues span.collapse').trigger('click')
	$('th.xmlpath span.collapse').trigger('click')
	$('th.xmltype span.collapse').trigger('click')
})

<!--/*-->]]></root><!--*/-->