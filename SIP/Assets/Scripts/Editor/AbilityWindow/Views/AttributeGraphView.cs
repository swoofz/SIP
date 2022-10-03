using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

public class AttributeGraphView : GraphView {

    public Attributes rootNode;

    public AttributeGraphView(Attributes root) {
        graphViewChanged -= OnUseGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnUseGraphViewChanged;

        rootNode = root;
        var rootPath = $"Assets/Scripts/Ability/Abilities/Attributes/{root.guid}.asset";

        if (rootNode) {
            AddElement(RootNode(rootNode));
            foreach (Attribute attr in AssetDatabase.LoadAllAssetRepresentationsAtPath(rootPath)) CreateNodeView(attr);
            EdgeConnection();
        }
        else {
            var nodeView = AttrNodeView.GenerateAttrNode("Targeting");
            rootNode = nodeView.node as Attributes;
            foreach (string name in new List<string>() { "Self", "Friendly", "Enemy" }) {
                var port = GenerateEntryPort(nodeView, name);
                nodeView.outputContainer.Add(port);
                nodeView.outputs.Add(port);
            }

            nodeView.RefreshExpandedState();
            nodeView.RefreshPorts();
            AddElement(nodeView);
        }
    }

    private UnityEditor.Experimental.GraphView.GraphViewChange OnUseGraphViewChanged(UnityEditor.Experimental.GraphView.GraphViewChange graphViewChange) {
        if (graphViewChange.elementsToRemove != null) {
            graphViewChange.elementsToRemove.ForEach(elem => {
                if (elem is NodeView nodeView) rootNode.DeleteAttribute(nodeView.node as Attribute);

                if (elem is UnityEditor.Experimental.GraphView.Edge edge) {
                    NodeView childView = edge.input.node as NodeView;
                    rootNode.UnassignNode(childView.node as Attribute, edge.output.portName);
                }
            });
        }

        if (graphViewChange.edgesToCreate != null) {
            graphViewChange.edgesToCreate.ForEach(edge => {
                NodeView childView = edge.input.node as NodeView;
                rootNode.AssignNode(childView.node as Attribute, edge.output.portName);
            });
        }

        return graphViewChange;
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt) {
        Vector2 pos = evt.mousePosition;
        evt.menu.AppendAction("Damage", a => { CreateNode(pos, typeof(Damage)); } );
        evt.menu.AppendAction("Heal", a => { CreateNode(pos, typeof(Heal)); ; } );
        evt.menu.AppendAction("Buff", a => { CreateNode(pos, typeof(Buff)); } );
        evt.menu.AppendAction("Debuff", a => { CreateNode(pos, typeof(Debuff)); } );

        base.BuildContextualMenu(evt);
    }

    private void CreateNode(Vector2 pos, System.Type type) {
        var attrNode = rootNode.CreateAttribute(type, pos);

        var nodeView = new NodeView(attrNode) {
            title = type.Name
        };
        AddElement(nodeView);
    }

    private void CreateNodeView(Attribute node) {
        var nodeView = new NodeView(node) {
            title = node.GetType().Name
        };

        AddElement(nodeView);
    }

    private NodeView RootNode(Attributes rootNode) {
        NodeView nodeView = new(rootNode) {
            title = "Targeting"
        };

        foreach (string name in new List<string>() { "Self", "Friendly", "Enemy" }) {
            var port = GenerateEntryPort(nodeView, name);
            nodeView.outputContainer.Add(port);
            nodeView.outputs.Add(port);
        }

        nodeView.RefreshExpandedState();
        nodeView.RefreshPorts();
        return nodeView;
    }

    private void EdgeConnection() {
        var childernDict = rootNode.GetAttributes();
        NodeView parentView = FindNodeView(rootNode);

        foreach (var attrLst in childernDict) {
            attrLst.Value.ForEach(attr => {
                NodeView childView = FindNodeView(attr);

                UnityEditor.Experimental.GraphView.Edge edge = parentView.outputs[(int)attrLst.Key].ConnectTo(childView.inputs[0]);
                AddElement(edge);
            });
        }
    }
}
