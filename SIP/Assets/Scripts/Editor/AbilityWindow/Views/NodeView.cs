using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NodeView : UnityEditor.Experimental.GraphView.Node {
    public Node node;

    public List<Port> inputs = new();
    public List<Port> outputs = new();

    public NodeView(Node node) {
        this.node = node;
        this.title = node.name;
        this.viewDataKey = node.guid;

        style.left = node.position.x;
        style.top = node.position.y;

        if (!node.isRoot) GenerateView(node);
    }

    public override void SetPosition(Rect newPos) {
        base.SetPosition(newPos);
        node.position.x = newPos.xMin;
        node.position.y = newPos.yMin;
    }

    private void GenerateView(Node node) {
        switch (GetNodeType(node)) {
            case 0:
                AttrNodeView.FillNodeView(this);
                GenerateInputPort(this);
                break;
            case 1:
                List<string> options = OnUseNodeView.FillNodeView(this);
                GenerateInputPort(this);
                if (options.Count > 0) GenerateOutputPort(this, options);
                break;
            default:
                break;
        }
    }

    private void GenerateInputPort(NodeView node) {
        var inputPort = GeneratePort(node, Direction.Input);
        inputPort.portName = "";
        node.inputContainer.Add(inputPort);
        inputs.Add(inputPort);
        node.RefreshExpandedState();
        node.RefreshPorts();
    }

    private void GenerateOutputPort(NodeView node, List<string> portNames) {
        foreach (string portName in portNames) {
            var outputPort = GeneratePort(node, Direction.Output);
            outputPort.portName = portName;
            outputs.Add(outputPort);
            node.outputContainer.Add(outputPort);
        }

        node.RefreshExpandedState();
        node.RefreshPorts();
    }

    private Port GeneratePort(NodeView node, Direction direction, Port.Capacity capacity = Port.Capacity.Multi) {
        return node.InstantiatePort(Orientation.Horizontal, direction, capacity, typeof(float));
    }

    private int GetNodeType(Node node) {
        if (node is Attribute) return 0;
        if (node is TreeNode) return 1;

        return -1;
    }
}
