'use strict';

/* Filters */

angular.module('Iris.filters', []).filter('titleCase', function () {
	return function (input) {
		return input.replace(/\w\S*/g, function (txt) { return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase(); });
	};
});