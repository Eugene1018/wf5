var kGraphApp = angular.module('kGraphApp', ['draggingApp']);

kGraphApp.directive('kGraph', function () {
	return {
		restrict: 'E',
		templateUrl: 'AppJS/template.html',
		replace: true,
		scope: {
			graph: '=graph'
		},
		controller: 'kGraphCtrl'
	}
});

kGraphApp.controller('kGraphCtrl', ['$scope', 'draggingService', '$element',
    function kGraphCtrl($scope, draggingService, $element) {
        var controller = this;
        this.document = document;

        this.jQuery = function (element) {
            return $(element);
        }

        $scope.draggingLine = false;
        $scope.connectorSize = 10;
        $scope.dragSelecting = false;

        $scope.dragSelectionRect = {
            x: 0, y: 0, width:0, height: 0
        };

        $scope.mouseOverConnector = null;
        $scope.mouseOverLine = null;
        $scope.mouseOverNode = null;

        this.lineClass = "line";
        this.connectorClass = "connector";
        this.nodeClass = "node";

        this.searchUp = function (element, parentClass) {
            if (element == null || element.length == 0) {
                return null;
            }

            if (hasClassSVG(element, parentClass)) {
                return element;
            }

            return this.searchUp(element.parent(), parentClass);
        }

        this.hitTest = function (clientX, clientY) {
            return this.document.elementFromPoint(clientX, clientY);
        }

        this.checkForHit = function (mouseOverElement, whichClass) {
            var hoverElement = this.searchUp(this.jQuery(mouseOverElement), whichClass);
            if (!hoverElement) {
                return null;
            }
        }

        this.translateCoordinates = function (x, y) {
            var svg_elem = $element.get(0);
            var matrix = svg_elem.getScreenCTM();
            var point = svg_elem.createSVGPoint();

            point.x = x;
            point.y = y;

            return point.matrixTransform(matrix.inverse());
        }

        $scope.mouseDown = function (evt) {
            $scope.graph.deselectAll();

            draggingService.startDrag(evt, {
                dragStarted: function (x, y) {
                    $scope.dragSelecting = true;
                    var startPoint = controller.translateCoordinates(x, y);
                    $scope.dragSelectionStartPoint = startPoint;
                    $scope.dragSelectionRect = {
                        x: startPoint.x,
                        y: startPoint.y,
                        width: 0,
                        height: 0
                    }
                },
                dragging: function (x, y) {
                    var startPoint = $scope.dragSelectionStartPoint;
                    var curPoint = controller.translateCoordinates(x, y);
                    $scope.dragSelectionRect = {
                        x: curPoint.x > startPoint.x ? startPoint.x : curPoint.x,
                        y: curPoint.y > startPoint.y ? startPoint.y : curPoint.y,
                        width: curPoint.x > startPoint.x ? curPoint.x - startPoint.x : startPoint.x - curPoint.x,
                        height: curPoint.y > startPoint.y ? curPoint.y - startPoint.y : startPoint.y - curPoint.y
                    };
                },
                dragEnd: function () {
                    $scope.dragSelecting = false;
                    $scope.graph.applySelectionRect($scope.dragSelectionRect);
                    delete $scope.dragSelectionStartPoint;
                    delete $scope.dragSelectionRect
                }
            })
        }   //end mousedown

        $scope.mouseMove = function (evt) {
            $scope.mouseOverLine = null;
            $scope.mouseOverConnector = null;
            $scope.mouseOverNode = null;

            var mouseOverElement = controller.hitTest(evt.clientX, evt.clientY);
            if (mouseOverElement == null) {
                return;
            }

            if (!$scope.draggingLine) {
                var scope = controller.checkForHit(mouseOverElement, controller.lineClass);
                if ($scope.mouseOverLine) {
                    return;
                }
            }

            var scope = controller.checkForHit(mouseOverElement, controller.lineClass);
            $scope.mouseOverConnector = (scope && scope.connector) ? scope.connector : null;

            if ($scope.mouseOverConnector) {
                return;
            }

            var scope = controller.checkForHit(mouseOverElement, controller.nodeClass);
            $scope.mouseOverNode = (scope && scope.node) ? scope.node : null;
        }   //end mouse move

        $scope.nodeMouseDown = function (evt, node) {
            var graph = $scope.graph;
            var lastMouseCoords;

            draggingService.startDrag(evt, {
                dragStarted: function (x, y) {
                    lastMouseCoords = controller.translateCoordinates(x, y);

                    if (!node.selected()) {
                        graph.deselectAll();
                        node.select();
                    }
                },
                dragging: function (x, y) {
                    var curCoords = controller.translateCoordinates(x, y);
                    var deltaX = curCoords.x - lastMouseCoords.x;
                    var deltaY = curCoords.y - lastMouseCoords.y;

                    //window.console.log("move deltaX: " + deltaX + " move deltaY: " + deltaY);
                    graph.updateSelectedNodesLocation(deltaX, deltaY);
                    lastMouseCoords = curCoords;
                },
                clicked: function () {
                    graph.handleNodeClicked(node, evt.ctrlKey);
                }
            });
        };   //  end node mouse down

        $scope.lineMouseDown = function (evt, line) {
            var graph = $scope.graph;
            graph.handleLineMouseDown(line, evt.ctrlKey);

            evt.stopPropgation();
            evt.preventDefault();
        };

        $scope.connectorMouseDown = function (evt, node, connector, connectorIndex, isInputConnector) {
            draggingService.startDrag(evt, {
                dragStarted: function (x, y) {
                    var curCoords = controller.translateCoordinates(x, y);
                    $scope.draggingLine = true;
                    $scope.dragPoint1 = graph.computeConnectorPos(node, connectorIndex, isInputConnector);
                    $scope.dragPoint2 = {
                        x: curCoords.x,
                        y: curCoords.y
                    };
                    $scope.dragTangent1 = graph.computeLineSourceTangent($scope.dragPoint1, $scope.dragPoint2);
                    $scope.dragTangent2 = graph.computLineDestTangent($scope.dragPoint1, $scope.dragPoint2);
                },
                dragging: function (x, y, evt) {
                    var startCoords = controller.translateCoordinates(x, y);
                    $scope.dragPoint1 = graph.computeConnectorPos(node, connectorIndex, isInputConnector);
                    $scope.dragPoint2 = {
                        x: startCoords.x,
                        y: startCoords.y
                    };
                    $scope.dragTangent1 = graph.computeLineSourceTangent($scope.dragPoint1, $scope.dragPoint2);
                    $scope.dragTangent2 = graph.computeLineDestTangent($scope.dragPoint1, $scope.dragPoint2);
                },
                dragEnded: function () {
                    if ($scope.mouseOverConnector &&
                        $scope.mouseOverConnector !== connector) {
                        $scope.graph.createNewConnector(connector, $scope.mouseOverConnector);
                    }
                    $scope.draggingLine = false;
                    delete $scope.dragPoint1;
                    delete $scope.dragPoint2;
                    delete $scope.dragTangent1;
                    delete $scope.dragTangent2;
                }
            });
        }   //end connector mouse down
}]);

