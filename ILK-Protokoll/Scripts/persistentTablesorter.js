$(document).ready(function () {
	$.tablesorter.addWidget({
		// give the widget an id
		id: "sortPersist",
		// format is called when the on init and when a
		// sorting has finished
		format: function (table) {

			// Cookie info
			var cookieName = 'ILKprotokollSortCookie';
			var cookie = $.cookie(cookieName);
			var options = { path: '/' };

			var data = {};
			var sortList = table.config.sortList;
			var tableId = $(table).attr('id');
			var cookieExists = (typeof (cookie) != "undefined"
				 && cookie != null) && (typeof (tableId) != "undefined");

			// If the existing sortList isn't empty, set it into the cookie
			// and get out
			if (sortList.length > 0) {
				if (cookieExists) {
					data = $.evalJSON(cookie);
				}
				data[tableId] = sortList;
				$.cookie(cookieName, $.toJSON(data), options);
			} else {
				if (cookieExists) {

					// Get the cookie data
					var data = $.evalJSON($.cookie(cookieName));

					// If it exists
					if (typeof (data[tableId]) != "undefined"
						 && data[tableId] != null) {

						// Get the list
						sortList = data[tableId];

						// And finally, if the list is NOT empty, trigger
						// the sort with the new list
						if (sortList.length > 0) {
							$(table).trigger("sorton", [sortList]);
						}
					}
				}
			}
		}
	});
});