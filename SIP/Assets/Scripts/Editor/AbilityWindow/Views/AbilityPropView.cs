using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor;

public class AbilityPropView : UnityEditor.Experimental.GraphView.GraphView {

    public string guid;

    public AbilityPropView(Properties prop) {
        AddElement(AbilityPropNodeView.GenerateProperties("Properties", prop));
        this.StretchToParentSize();
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt) {}
}