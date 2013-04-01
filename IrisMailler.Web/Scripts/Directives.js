'use strict';

/* Directives */

angular.module('Iris.directives', []).
	directive('folderView', function ($http) {
		return function (scope, elm, attrs) {
			var jEl = $(elm);
			var currentElementType;

			jEl.dynatree({
				onLazyRead: function (node) {
					$http.get('api/' + currentElementType.path + '/folder/' + node.data.key).success(function (data) {
						addNodes(node, data);
					});
				},
				onActivate: function (node) {
					if (node.data.href)
						window.location.href = node.data.href;
				},

				debugLevel: 1
			});

			function addNodes(parent, nodeData) {
				var i = 0;
				for (var key in nodeData) {
					for (var j in nodeData[key]) {
						parent.addChild({
							title: nodeData[key][j].Name,
							isFolder: i ? false : true,
							isLazy: i ? false : true,
							href: i ? '#/' + currentElementType.path + '/' + nodeData[key][j].Id : null,
							key: nodeData[key][j].Id
						});
					}
					i++;
				}
			}

			function initNodes(type) {
				jEl.dynatree('getRoot').removeChildren();
				$http.get('api/' + type + '/folder/').success(function(data) {
					addNodes(jEl.dynatree('getRoot'), data);
				});
			}
			
			initNodes(attrs.folderView);
		};
	}).directive('contenteditable', function () {
		return {
			restrict: 'A',
			require: '?ngModel',
			link: function (scope, element, attr, ngModel) {
				var read;
				if (!ngModel) {
					return;
				}
				ngModel.$render = function () {
					return element.html(ngModel.$viewValue);
				};
				element.bind('blur', function () {
					if (ngModel.$viewValue !== $.trim(element.html())) {
						return scope.$apply(read);
					}
				});
				return read = function () {
					console.log("read()");
					return ngModel.$setViewValue($.trim(element.html()));
				};
			}
		};
	});