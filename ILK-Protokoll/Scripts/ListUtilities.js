function ReplaceRow(list, rowId, data) {
	$('#' + list + '_' + rowId).replaceWith(data);
	RefreshTables(list);
}

function RemoveRow(list, rowId) {
	$('#' + list + '_' + rowId).remove();
	RefreshTables(list);
}

function AddRow(list, data) {
	$('#' + list + '_tbody').append(data);

	RefreshTables(list);
}

function RefreshTables(list) {
	if (list) {
		list += ' ';
	}
	$(list + 'table.table-sortable').trigger('update');
}