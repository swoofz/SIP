using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;

public class OnUseGraphView : GraphView {

    private OnUse tree;

    public OnUseGraphView(OnUse _tree) {
        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;

        tree = _tree;
        if (!tree.rootNode) {
            var nodeView = OnUseNodeView.GenerateUseNode(tree);
            foreach (string name in new List<string>() { "On Hit", "On Crit", "On Miss", "On Use" }) {
                nodeView.outputContainer.Add(GenerateEntryPort(nodeView, name, false));
            }
            nodeView.RefreshExpandedState();
            nodeView.RefreshPorts();
            AddElement(nodeView);

        }

        AddElement(RootNode(tree.rootNode));
        tree.nodes.ForEach(n => CreateNodeView(n));
        EdgeConnection(tree.rootNode);
        tree.nodes.ForEach(node => EdgeConnection(node));
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt) {
        Vector2 pos = evt.mousePosition;

        evt.menu.AppendAction("Connector/Is_Oppsite(Element)", a => CreateNode(pos, typeof(OppsiteNode)));
        evt.menu.AppendAction("Connector/Is_Same(Element)", a => CreateNode(pos, typeof(SameElementNode)));
        evt.menu.AppendAction("Connector/Is_Overpowered(Level)", a => CreateNode(pos, typeof(OverpoweredNode)));
        evt.menu.AppendAction("Connector/Is_Weak(Level)", a => CreateNode(pos, typeof(WeakNode)));

        evt.menu.AppendAction("Apply_Weights", a => CreateNode(pos, typeof(ApplyWeightNode)));

        base.BuildContextualMenu(evt);
    }

    //internal void PopulateView(OnUse tree) {
    //    this.tree = tree;

    //    graphViewChanged -= OnGraphViewChanged;
    //    DeleteElements(graphElements);
    //    graphViewChanged += OnGraphViewChanged;

    //    AddElement(RootNode(tree.rootNode));
    //    tree.nodes.ForEach(n => CreateNodeView(n));

    //    EdgeConnection(tree.rootNode);
    //    tree.nodes.ForEach(node => EdgeConnection(node));
    //}

    private UnityEditor.Experimental.GraphView.GraphViewChange OnGraphViewChanged(UnityEditor.Experimental.GraphView.GraphViewChange graphViewChange) {
        if (graphViewChange.elementsToRemove != null) {
            graphViewChange.elementsToRemove.ForEach(e => {
                if (e is NodeView nodeView) {
                    tree.DeleteNode(nodeView.node as TreeNode);
                }

                if (e is UnityEditor.Experimental.GraphView.Edge edge) {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;
                    tree.RemoveChild(parentView.node as TreeNode, childView.node as TreeNode, edge.output.portName);
                }
            });
        }

        if (graphViewChange.edgesToCreate != null) {
            graphViewChange.edgesToCreate.ForEach(e => {
                NodeView parentView = e.output.node as NodeView;
                NodeView childView = e.input.node as NodeView;
                tree.AddChild(parentView.node as TreeNode, childView.node as TreeNode, e.output.portName);
            });
        }

        return graphViewChange;
    }

    private void CreateNode(Vector2 pos, Type type) { 
        var node = tree.CreateNode(type);
        node.position = pos;

        var nodeView = new NodeView(node) {
            title = type.Name.Split("Node")[0]
        };
        AddElement(nodeView);
    }

    private void CreateNodeView(TreeNode node) {
        var nodeView = new NodeView(node) {
            title = node.GetType().Name.Split("Node")[0]
        };

        AddElement(nodeView);
    }

    private NodeView RootNode(TreeNode rootNode) {
        var nodeView = new NodeView(rootNode) { 
            title = "Use"
        };

        foreach (string name in new List<string>() { "On Hit", "On Crit", "On Miss", "On Use" }) {
            var port = GenerateEntryPort(nodeView, name, false);
            nodeView.outputContainer.Add(port);
            nodeView.outputs.Add(port);
        }

        nodeView.RefreshExpandedState();
        nodeView.RefreshPorts();
        return nodeView;
    }

    private void EdgeConnection(TreeNode node) {
        var childern = tree.GetChildern(node);
        NodeView parentView = FindNodeView(node);
        var index = 0;
        childern.ForEach(child => {
            NodeView childView = null;
            if(child) childView = FindNodeView(child);

            if (childView != null && parentView.outputs.Count > 0) {
                UnityEditor.Experimental.GraphView.Edge edge = parentView.outputs[index].ConnectTo(childView.inputs[0]);
                AddElement(edge);
            }
            index++;
        });
    }
}
