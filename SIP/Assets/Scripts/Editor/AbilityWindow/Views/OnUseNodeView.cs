using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public static class OnUseNodeView {
    public static List<string> FillNodeView(NodeView nodeView) {
        var options = new List<string>();

        if (nodeView.node is ApplyWeightNode) CreateApplyWeightsNode(nodeView);
        else if (IsBoolNode(nodeView.node)) options = new() { "True", "False" };

        return options;
    }

    public static NodeView GenerateUseNode(OnUse tree) {
        var node = ScriptableObject.CreateInstance<UseNode>();
        node.guid = GUID.Generate().ToString();
        node.name = node.guid;
        node.isRoot = true;

        var nodeView = new NodeView(node) { 
            title = "Use"
        };

        nodeView.RefreshExpandedState();
        nodeView.RefreshPorts();
        nodeView.SetPosition(new Rect(300, 200, 100, 150));

        tree.rootNode = node;
        AssetDatabase.AddObjectToAsset(node, tree);
        AssetDatabase.SaveAssets();

        return nodeView;
    }

    public static OnUse GenerateOnUseTree(string abilityId) {
        var tree = ScriptableObject.CreateInstance<OnUse>();
        tree.name = abilityId;

        AssetDatabase.CreateAsset(tree, $"Assets/Scripts/Ability/Abilities/On Use/{tree.name}.asset");
        AssetDatabase.SaveAssets();
        return tree;
    }

    private static bool IsBoolNode(Node node) {
        if (node is SameElementNode) return true;
        if (node is OppsiteNode) return true;
        if (node is OverpoweredNode) return true;
        if (node is WeakNode) return true;

        return false;
    }

    private static void CreateApplyWeightsNode(NodeView nodeView) {
        var node = nodeView.node as TreeNode;
        nodeView.outputContainer.Add(AddInspector(node));
    }

    private static IMGUIContainer AddInspector(TreeNode node) {
        var editor = Editor.CreateEditor(node);
        IMGUIContainer container = new(() => { editor.OnInspectorGUI(); });
        container.style.paddingLeft = 10;
        container.style.paddingRight = 10;
        container.style.marginLeft = 10;
        container.style.marginRight = 10;
        container.style.marginBottom = 10;
        container.style.marginTop = 10;
        return container;
    }
}
