var app = angular.module("canvasApp", ['kGraphApp']);

app.service('prompt', function () {
    return prompt;
});


app.controller('canvasCtrl', ['$scope', 'prompt', function canvasCtrl($scope, prompt) {
    var deleteKeyCode = 46;
    var ctrlKeyCode = 65;
    var ctrlDown = false;
    var aKeyCode = 17;
    var escKeyCode = 27;
    var nextNodeID = 10;

    $scope.keyDown = function (evt) {
        if (evt.keyCode === ctrlKeyCode) {
            ctrlDown = true;
            evt.stopPropagation();
            evt.preventDefault();
        }
    }

    $scope.keyUp = function (evt) {
        if (evt.keyCode === deleteKeyCode) {
            $scope.graphView.deleteSelected();
        }

        if (evt.keyCode == aKeyCode && ctrlDown) {
            $scope.graphView.selectAll();
        }

        if (evt.keyCode === escKeyCode) {
            $scope.graphView.deselectAll();
        }

        if (evt.keyCode === ctrlKeyCode) {
            ctrlDown = false;
            evt.stopPropagation();
            evt.preventDefault();
        }
    }

    var graphData = {
        snodes: [{
            name: "N1",
            id: 1,
            type: "TaskNode",
            x: 20,
            y: 20,
            inputConnectors: [{ name: "A" }],
            outputConnectors: [{ name: "X" }]
        }
        , {
            name: "N2",
            id: 2,
            type: "StartNode",
            x: 70,
            y: 70,
            inputConnectors: [{ name: "A" }],
            outputConnectors: [{ name: "X" }]
        }, {
            name: "N3",
            id: 3,
            type: "EndNode",
            x: 100,
            y: 100,
            inputConnectors: [{ name: "A" }],
            outputConnectors: [{ name: "X" }]
        }, {
            name: "N4",
            id: 4,
            type: "TaskNode",
            x: 200,
            y: 200,
            inputConnectors: [{ name: "A" }],
            outputConnectors: [{ name: "X" }]
        }
        ],
        slines: [{
            source: {
                nodeID: 1,
                connectorIndex: 0
            },
            dest: {
                nodeID: 4,
                connectorIndex: 0
            }
        }, {
            source: {
                nodeID: 4,
                connectorIndex: 0
            },
            dest: {
                nodeID: 1,
                connectorIndex: 0
            }
        }]
    };

    $scope.graphView = new kgraph.GraphView(graphData);

    $scope.addNode = function () {
        graphData.snodes[1].x += 10;
        graphData.snodes[1].y += 10;
        window.console.log("add node...");
    }
}]);
