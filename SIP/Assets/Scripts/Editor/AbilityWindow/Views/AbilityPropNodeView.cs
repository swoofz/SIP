using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

public static class AbilityPropNodeView  {

    public static NodeView GenerateProperties(string abilityId, Properties node) {
        var nodeView = new NodeView(node) {
            title = "Properties"
        };

        nodeView.contentContainer.Add(AddInspector(node));
        nodeView.contentContainer.style.backgroundColor = Color.black;

        nodeView.RefreshExpandedState();
        nodeView.RefreshPorts();
        nodeView.SetPosition(new Rect(300, 200, 100, 150));

        return nodeView;
    }

    private static IMGUIContainer AddInspector(Properties prop) {
        var editor = Editor.CreateEditor(prop);
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
