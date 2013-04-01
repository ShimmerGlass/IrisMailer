'use strict';

angular.module('Iris', ['Iris.filters', 'Iris.services', 'Iris.directives']).
  config(['$routeProvider', function ($routeProvider) {
  	$routeProvider.when('/contents', { templateUrl: 'Content/Partials/Contents.html', controller: ContentsCtrl });
  	$routeProvider.otherwise({ redirectTo: '/view1' });
  }]);