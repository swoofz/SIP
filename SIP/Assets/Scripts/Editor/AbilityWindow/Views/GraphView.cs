using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class GraphView : UnityEditor.Experimental.GraphView.GraphView {

    public string guid;

    public GraphView() {
        styleSheets.Add(Resources.Load<StyleSheet>("AbilityGraph"));
        Insert(0, new GridBackground());

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        this.AddManipulator(new ContentZoomer());

        this.StretchToParentSize();
    }

    private Port GeneratePort(NodeView node, Direction direction, Port.Capacity capacity = Port.Capacity.Multi) {
        return node.InstantiatePort(Orientation.Horizontal, direction, capacity, typeof(float));
    }

    protected Port GenerateEntryPort(NodeView node, string name, bool isCapMuli=true) {
        var capacity = isCapMuli ? Port.Capacity.Multi : Port.Capacity.Single;
        var port = GeneratePort(node, Direction.Output, capacity);
        port.portName = name;
        return port;
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) {
        var compatiblePorts = new List<Port>();

        ports.ForEach(port => {
            if (startPort != port && startPort.node != port.node) compatiblePorts.Add(port);
        });

        return compatiblePorts;
    }

    protected NodeView FindNodeView(Node node) {
        return GetNodeByGuid(node.guid) as NodeView;
    }
}
