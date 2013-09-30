/* Load this script using conditional IE comments if you need to support IE 7 and IE 6. */

window.onload = function() {
	function addIcon(el, entity) {
		var html = el.innerHTML;
		el.innerHTML = '<span style="font-family: \'e9ineIcons\'">' + entity + '</span>' + html;
	}
	var icons = {
			'e9ine-icon-untitled' : '&#x21;',
			'e9ine-icon-untitled-2' : '&#x22;',
			'e9ine-icon-untitled-3' : '&#x23;',
			'e9ine-icon-untitled-4' : '&#x24;',
			'e9ine-icon-untitled-5' : '&#x25;',
			'e9ine-icon-untitled-6' : '&#x26;',
			'e9ine-icon-untitled-7' : '&#x27;',
			'e9ine-icon-untitled-8' : '&#x28;',
			'e9ine-icon-untitled-9' : '&#x29;',
			'e9ine-icon-untitled-10' : '&#x2a;',
			'e9ine-icon-untitled-11' : '&#x2b;',
			'e9ine-icon-untitled-12' : '&#x2c;',
			'e9ine-icon-untitled-13' : '&#x2d;',
			'e9ine-icon-untitled-14' : '&#x2e;',
			'e9ine-icon-untitled-15' : '&#x2f;',
			'e9ine-icon-untitled-16' : '&#x30;',
			'e9ine-icon-untitled-17' : '&#x31;',
			'e9ine-icon-untitled-18' : '&#x32;',
			'e9ine-icon-untitled-19' : '&#x33;',
			'e9ine-icon-untitled-20' : '&#x34;',
			'e9ine-icon-untitled-21' : '&#x35;',
			'e9ine-icon-untitled-22' : '&#x36;',
			'e9ine-icon-untitled-23' : '&#x37;',
			'e9ine-icon-untitled-24' : '&#x38;',
			'e9ine-icon-untitled-25' : '&#x39;',
			'e9ine-icon-untitled-26' : '&#x3a;',
			'e9ine-icon-untitled-27' : '&#x3b;',
			'e9ine-icon-untitled-28' : '&#x3c;',
			'e9ine-icon-untitled-29' : '&#x3d;',
			'e9ine-icon-untitled-30' : '&#x3e;',
			'e9ine-icon-untitled-31' : '&#xa1;',
			'e9ine-icon-untitled-32' : '&#x40;',
			'e9ine-icon-untitled-33' : '&#x41;',
			'e9ine-icon-untitled-34' : '&#x42;',
			'e9ine-icon-untitled-35' : '&#x43;',
			'e9ine-icon-untitled-36' : '&#x44;',
			'e9ine-icon-untitled-37' : '&#x45;',
			'e9ine-icon-untitled-38' : '&#x46;',
			'e9ine-icon-untitled-39' : '&#x47;',
			'e9ine-icon-untitled-40' : '&#x48;',
			'e9ine-icon-untitled-41' : '&#x49;',
			'e9ine-icon-untitled-42' : '&#x4a;',
			'e9ine-icon-untitled-43' : '&#x4b;',
			'e9ine-icon-untitled-44' : '&#x4c;',
			'e9ine-icon-untitled-45' : '&#x4d;',
			'e9ine-icon-untitled-46' : '&#x4e;',
			'e9ine-icon-untitled-47' : '&#x4f;',
			'e9ine-icon-untitled-48' : '&#x50;',
			'e9ine-icon-untitled-49' : '&#x51;',
			'e9ine-icon-untitled-50' : '&#x52;',
			'e9ine-icon-untitled-51' : '&#x53;',
			'e9ine-icon-untitled-52' : '&#x54;',
			'e9ine-icon-untitled-53' : '&#x55;',
			'e9ine-icon-untitled-54' : '&#x56;',
			'e9ine-icon-untitled-55' : '&#x57;',
			'e9ine-icon-untitled-56' : '&#x58;',
			'e9ine-icon-untitled-57' : '&#x59;',
			'e9ine-icon-untitled-58' : '&#x5a;',
			'e9ine-icon-untitled-59' : '&#x5b;',
			'e9ine-icon-untitled-60' : '&#x5c;',
			'e9ine-icon-untitled-61' : '&#x5d;',
			'e9ine-icon-untitled-62' : '&#x5e;',
			'e9ine-icon-untitled-63' : '&#x5f;',
			'e9ine-icon-untitled-64' : '&#x60;',
			'e9ine-icon-untitled-65' : '&#x61;',
			'e9ine-icon-untitled-66' : '&#x62;',
			'e9ine-icon-untitled-67' : '&#x63;',
			'e9ine-icon-untitled-68' : '&#x64;',
			'e9ine-icon-untitled-69' : '&#x65;',
			'e9ine-icon-untitled-70' : '&#x66;',
			'e9ine-icon-untitled-71' : '&#x67;',
			'e9ine-icon-untitled-72' : '&#x68;',
			'e9ine-icon-untitled-73' : '&#x69;',
			'e9ine-icon-untitled-74' : '&#x6a;',
			'e9ine-icon-untitled-75' : '&#x6b;',
			'e9ine-icon-untitled-76' : '&#x6c;',
			'e9ine-icon-untitled-77' : '&#x6d;',
			'e9ine-icon-untitled-78' : '&#x6e;',
			'e9ine-icon-untitled-79' : '&#x6f;',
			'e9ine-icon-untitled-80' : '&#x70;',
			'e9ine-icon-untitled-81' : '&#x71;',
			'e9ine-icon-untitled-82' : '&#x72;',
			'e9ine-icon-untitled-83' : '&#x73;',
			'e9ine-icon-untitled-84' : '&#x74;',
			'e9ine-icon-untitled-85' : '&#x75;',
			'e9ine-icon-untitled-86' : '&#x76;',
			'e9ine-icon-untitled-87' : '&#x77;',
			'e9ine-icon-untitled-88' : '&#x78;',
			'e9ine-icon-untitled-89' : '&#x79;',
			'e9ine-icon-drawer' : '&#x7a;',
			'e9ine-icon-icons' : '&#x7b;',
			'e9ine-icon-list' : '&#x7c;',
			'e9ine-icon-grid' : '&#x7d;',
			'e9ine-icon-stats' : '&#x7e;'
		},
		els = document.getElementsByTagName('*'),
		i, attr, html, c, el;
	for (i = 0; ; i += 1) {
		el = els[i];
		if(!el) {
			break;
		}
		attr = el.getAttribute('data-icon');
		if (attr) {
			addIcon(el, attr);
		}
		c = el.className;
		c = c.match(/e9ine-icon-[^\s'"]+/);
		if (c && icons[c[0]]) {
			addIcon(el, icons[c[0]]);
		}
	}
};