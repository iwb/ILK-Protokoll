/*

highlight v4

Highlights arbitrary terms.

<http://johannburkard.de/blog/programming/javascript/highlight-javascript-text-higlighting-jquery-plugin.html>

MIT license.

Johann Burkard
<http://johannburkard.de>
<mailto:jb@eaio.com>

*/

jQuery.fn.highlight = function(regexpattern) {
	function innerHighlight(node, regex) {
		var skip = 0;
		if (node.nodeType == 3) {
			var regexmatch = node.data.match(regex);

			console.log(node.data);
			console.log(regexmatch);

			if (regexmatch && regexmatch.index >= 0) {
				var pos = regexmatch.index;
				var spannode = document.createElement('span');
				spannode.className = 'highlight';
				var middlebit = node.splitText(pos);
				middlebit.splitText(regexmatch[0].length);
				var middleclone = middlebit.cloneNode(true);
				spannode.appendChild(document.createTextNode("\u200A")); // Hairspace
				spannode.appendChild(middleclone);
				spannode.appendChild(document.createTextNode("\u200A"));
				middlebit.parentNode.replaceChild(spannode, middlebit);
				skip = 1;
			}
		} else if (node.nodeType == 1 && node.childNodes && !/(script|style)/i.test(node.tagName)) {
			for (var i = 0; i < node.childNodes.length; ++i) {
				i += innerHighlight(node.childNodes[i], regex);
			}
		}
		return skip;
	}

	return this.length && regexpattern ? this.each(function() {
		innerHighlight(this, regexpattern);
	}) : this;
};

jQuery.fn.removeHighlight = function() {
	return this.find("span.highlight").each(function() {
		this.parentNode.firstChild.nodeName;
		with (this.parentNode) {
			replaceChild(this.firstChild, this);
			normalize();
		}
	}).end();
};