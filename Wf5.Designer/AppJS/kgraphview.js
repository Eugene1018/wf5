var kgraph = {};

(function () {
    kgraph.nodeWidth = 100;
    kgraph.nodeHeight = 50;
    kgraph.nodeNameHeight = 25;
    kgraph.connectorHeight = 35;
    kgraph.circleWidth = 40;
    kgraph.circleHeight = 20;

    kgraph.GraphView = function (graphData) {
        this.data = graphData;

        //finde node
        this.findNode = function (nodeID) {
            for (var i = 0; i < this.nodes.length; i++) {
                var node = this.nodes[i];
                if (node.data.id == nodeID) {
                    return node;
                }
            }
            throw new Error("Failed to find node " + nodeID);
        }

        //connector
        this.findInputConnector = function (nodeID, connectorIndex){
            var node = this.findNode(nodeID);

            if (!node.inputConnectors || node.inputConnectors.length <= connectorIndex) {
                throw new Error("Node" + nodeID + " has invalid input connectors.");
            }
            return node.inputConnectors[connectorIndex];
        }

        this.findOutputConnector = function (nodeID, connectorIndex) {
            var node = this.findNode(nodeID);

            if (!node.outputConnectors || node.outputConnectors.length <= connectorIndex) {
                throw new Error("Node" + nodeID + " has invalid output connectors.");
            }
            return node.outputConnectors[connectorIndex];
        }

        //create nodes
        var createNodes = function (snodes) {
            var nodes = [];
            if (snodes) {
                for (var i = 0; i < snodes.length; i++) {
                    nodes.push(new kgraph.Node(snodes[i]));
                }
            }
            return nodes;
        }
        this.nodes = createNodes(this.data.snodes);

        //create lines
        this.createLines = function (slines) {
            var lines = [];
            if (slines) {
                for (var i = 0; i < slines.length; i++) {
                    lines.push(this.createLine(slines[i]));
                }
            }
            return lines;
        }

        this.createLine = function (sline) {
            var sourceConnector = this.findOutputConnector(sline.source.nodeID, sline.source.connectorIndex);
            var destConnector = this.findInputConnector(sline.dest.nodeID, sline.dest.connectorIndex);

            return new kgraph.Line(sline, sourceConnector, destConnector);
        }

        this.lines = this.createLines(this.data.slines);

        //selection
        this.selectAll = function () {
            var ndoes = this.nodes;
            for (var i = 0; i < nodes.length; i++) {
                var ndoe = nodes[i];
                node.select();
            }

            var lines = this.lines;
            for (var i = 0; i < lines.length; i++) {
                var line = lines[i];
                line.select();
            }
        }

        this.deselectAll = function () {
            var nodes = this.nodes;
            for (var i = 0; i < nodes.length; i++) {
                var node = nodes[i];
                node.deselect();
            }

            var lines = this.lines;
            for (var i = 0; i < lines.length; i++) {
                var line = lines[i];
                line.deselect();
            }
        }

        //dragging
        this.updateSelectedNodesLocation = function (deltaX, deltaY) {
            var selectedNodes = this.getSelectedNodes();
            window.console.log("selected nodes length: " + selectedNodes.length);
            for (var i = 0; i < selectedNodes.length; i++) {
                var node = selectedNodes[i];
                node.data.x += deltaX;
                node.data.y += deltaY;
                window.console.log("node x: " + node.data.x + "node y: " + node.data.y);
            }
        };

        this.handleNodeClicked = function (node, ctrlKey) {
            if (ctrlKey) {
                node.toggleSelected();
            } else {
                this.deselectAll();
                node.select();
            }

            var nodeIndex = this.nodes.indexOf(node);
            if (nodeIndex == -1) {
                throw new Error("Failed to find node in view model!");
            }

            this.nodes.splice(nodeIndex, 1);
            this.nodes.push(node);
        };

        this.handleLineMouseDown = function (line, ctrlKey) {
            if (ctrlKey) {
                line.toggleSelected();
            } else {
                this.deselectAll();
                line.select();
            }
        };

        this.deleteSelected = function () {
            var newNodeViewModels = [];
            var newNodeDataModels = [];
            var deletedNodeIds = [];

            for (var nodeIndex = 0; nodeIndex < this.nodes.length; nodeIndex++) {
                var node = this.nodes[nodeIndex];
                if (!node.selected()) {
                    newNodeViewModels.push(node);
                    newNodeDataModels.push(node.data);
                } else {
                    deletedNodeIds.push(node.data.id);
                }
            }

            var newLineViewModels = [];
            var newLineDataModels = [];

            for (var lineIndex = 0; lineIndex < this.lines.length; lineIndex++) {
                var line = this.lines[lineIndex];
                if (!line.selected() &&
                    deletedNodeIds.indexOf(line.data.source.nodeID) === -1 &&
                    deletedNodeIds.indexOf(line.data.dest.nodeID) === -1) {
                    newLineViewModels.push(line);
                    newLineDataModels.push(line.data);
                }
            }
            this.nodes = newNodeViewModels;
            this.data.snodes = newNodeDataModels;
            this.lines = newLineViewModels;
            this.data.slines = newLineDataModels;
        }; //end delete selected

        this.getSelectedNodes = function () {
            var selectedNodes = [];
            for (var i = 0; i < this.nodes.length; i++) {
                var node = this.nodes[i];
                if (node.selected()) {
                    selectedNodes.push(node);
                }
            }
            return selectedNodes;
        }

        this.applySelectionRect = function (selectionRect) {
            this.deselectAll();

            for (var i = 0; i < this.nodes.length; i++) {
                var node = this.nodes[i];
                if (node.x() >= selectionRect.x &&
                    node.y() >= selectionRect.y &&
                    node.x() + node.width() <= selectionRect.x + selectionRect.width &&
                    node.y() + node.height() <= selectionRect.y + selectionRect.height) {
                    node.select();
                }
            }

            for (var i = 0; i < this.lines.length; i++) {
                var line = this.lines[i];
                if (line.source.parentNode().selected() &&
                    line.dest.parentNode().selected()) {
                    line.select();
                }
            }
        };

        this.getSelectedLines = function () {
            var selectedLines = [];

            for (var i = 0; i < this.lines.length; i++) {
                var line = this.lines[i];
                if (line.selected()) {
                    selectedLines.push(line);
                }
            }
            return selectedLines;
        };
    };  //end graph view

    kgraph.Node = function (snode) {
        this.data = snode;
        this._selected = false;

        this.name = function () {
            return this.data.name || "";
        }

        this.type = function () {
            return this.data.type;
        }

        this.x = function () {
            return this.data.x;
        }

        this.y = function () {
            return this.data.y;
        }

        this.width = function () {
            return kgraph.nodeWidth;
        }

        this.height = function () {
            return kgraph.nodeHeight;
        }

        this.select = function () {
            this._selected = true;
        }

        this.deselect = function () {
            this._selected = false;
        }

        this.toggleSelected = function () {
            this._selected = !this._selected;
        }

        this.selected = function () {
            return this._selected;
        }

        this.inputConnectors = createInputConnectors(this.data.inputConnectors, this);
        this.outputConnectors = createOutputConnectors(this.data.outputConnectors, this);
    };

    var createInputConnectors = function (sconnectors, parentNode) {
        var connectors = [];
        var x = 0, y = 0;

        if (parentNode.type() == "StartNode" || parentNode.type() == "EndNode") {
            x = -(kgraph.circleWidth / 2);
        } else {
            y = kgraph.nodeHeight / 2;
        }

        if (sconnectors) {
            for (var i = 0; i < sconnectors.length; i++) {
                var connector = new kgraph.Connector(sconnectors[i],
                    x,
                    y,
                    parentNode);
                connectors.push(connector);
            }
        }
        return connectors;
    };

    var createOutputConnectors = function (sconnectors, parentNode) {
        var connectors = [];
        var x = 0, y = 0;

        if (parentNode.type() == "StartNode" || parentNode.type() == "EndNode") {
            x = kgraph.circleWidth / 2;
        } else {
            x = kgraph.nodeWidth;
            y = kgraph.nodeHeight / 2;
        }

        if (sconnectors) {
            for (var i = 0; i < sconnectors.length; i++) {
                var connector = new kgraph.Connector(sconnectors[i],
                    x,
                    y,
                    parentNode);
                connectors.push(connector);
            }
        }
        return connectors;
    };

    //Connector
    kgraph.Connector = function (sconnector, x, y, parentNode) {
        this.data = sconnector;
        this._x = x;
        this._y = y;
        this._parent = parentNode;

        this.name = function () {
            return this.data.name;
        }

        this.x = function () {
            return this._x;
        }

        this.y = function () {
            return this._y;
        }

        this.parentNode = function () {
            return this._parent;
        }
    };

    //Line
    kgraph.Line = function (sline, sourceConnector, destConnector) {
        this.data = sline;
        this.source = sourceConnector;
        this.dest = destConnector;
        this._selected = false;

        this.sourceCoordX = function () {
            return this.source.parentNode().x() + this.source.x();
        }

        this.sourceCoordY = function () {
            return this.source.parentNode().y() + this.source.y();
        }

        this.sourceCoord = function () {
            return {
                x: this.sourceCoordX(),
                y: this.sourceCoordY()
            };
        }

        this.sourceTangentX = function () {
            return kgraph.computeLineSourceTangentX(this.sourceCoord(), this.destCoord());
        }

        this.sourceTangentY = function () {
            return kgraph.computeLineSourceTangentY(this.sourceCoord(), this.destCoord());
        }

        this.destCoordX = function () {
            return this.dest.parentNode().x() + this.dest.x();
        }

        this.destCoordY = function () {
            return this.dest.parentNode().y() + this.dest.y();
        }

        this.destCoord = function () {
            return {
                x: this.destCoordX(),
                y: this.destCoordY()
            };
        }

        this.destTangentX = function () {
            return kgraph.computeLineDestTangentX(this.sourceCoord(), this.destCoord());
        }

        this.destTangentY = function () {
            return kgraph.computeLineDestTangentY(this.sourceCoord(), this.destCoord());
        }

        this.select = function () {
            this._selected = true;
        }

        this.deselect = function () {
            this._selected = false;
        }

        this.toggleSelected = function () {
            this._selected = !this._selected;
        }

        this.selected = function () {
            window.console.log("line selected..." + this._selected)
            return this._selected;
        }
    };

    var computeLineTangentOffset = function (pt1, pt2) {
        return (pt2.x - pt1.x) / 2;
    }

    kgraph.computeLineSourceTangentX = function (pt1, pt2) {
        return pt1.x + computeLineTangentOffset(pt1, pt2);
    }

    kgraph.computeLineSourceTangentY = function (pt1, pt2) {
        return pt1.y;
    }

    kgraph.computeLineSourceTangent = function (pt1, pt2) {
        return {
            x: kgraph.computeLineSourceTangentX(pt1, pt2),
            y: kgraph.computeLineSourceTangentY(pt1, pt2)
        };

    }

    kgraph.computeLineDestTangentX = function (pt1, pt2) {
        return pt2.x - computeLineTangentOffset(pt1, pt2);
    }

    kgraph.computeLineDestTangentY = function (pt1, pt2) {
        return pt2.y;
    }

    kgraph.computeLineDestTangent = function (pt1, pt2) {
        return {
            x: kgraph.computeLineDestTangentX(pt1, pt2),
            y: kgraph.computeLineDestTangentY(pt1, pt2)
        }
    }
})()