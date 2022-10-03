using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public static class AttrNodeView  {

    public static void FillNodeView(NodeView nodeView) {
        AddInspectorData(nodeView);
    }

    public static NodeView GenerateAttrNode(string abilityId) {
        var node = ScriptableObject.CreateInstance<Attributes>();
        node.name = abilityId;
        node.guid = GUID.Generate().ToString();
        node.isRoot = true;

        var nodeView = new NodeView(node) {
            title = "Targeting"
        };

        nodeView.RefreshExpandedState();
        nodeView.RefreshPorts();
        nodeView.SetPosition(new Rect(300, 200, 100, 150));

        AssetDatabase.CreateAsset(node, $"Assets/Scripts/Ability/Abilities/Attributes/{node.name}.asset");
        AssetDatabase.SaveAssets();

        return nodeView;
    }

    private static void AddInspectorData(NodeView nodeView) {
        var node = nodeView.node as Attribute;
        nodeView.outputContainer.Add(AddInspector(node));
    }

    private static IMGUIContainer AddInspector(Attribute attr) {
        var editor = Editor.CreateEditor(attr);
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
