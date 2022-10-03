using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OnUse : Node {
    public TreeNode rootNode;
    public TreeNode.State treeState = TreeNode.State.Running;
    public List<TreeNode> nodes = new();

    public TreeNode.State Run(OnUseData data) {
        if (rootNode.state == TreeNode.State.Running) {
            treeState = rootNode.Run(data);
        }

        return treeState;
    }


    public TreeNode CreateNode(System.Type type) {
        var node = CreateInstance(type) as TreeNode;
        node.guid = GUID.Generate().ToString();
        node.name = node.guid;
        nodes.Add(node);


        AssetDatabase.AddObjectToAsset(node, this);
        AssetDatabase.SaveAssets();
        return node;
    }

    public void DeleteNode(TreeNode node) { 
        nodes.Remove(node);

        AssetDatabase.RemoveObjectFromAsset(node);
        AssetDatabase.SaveAssets();
    }

    public void AddChild(TreeNode parent, TreeNode child, string portName) {
        BoolNode boolean = parent as BoolNode;
        if (boolean) {
            boolean.SetNode(portName, child);
        }

        CompositeNode composite = parent as CompositeNode;
        if (composite) { 
            composite.children.Add(child);
        }

        OptionNode option = parent as OptionNode;
        if (option) {
            option.SetNode(portName, child);
        }
    }

    public void RemoveChild(TreeNode parent, TreeNode child, string portName) {
        BoolNode boolean = parent as BoolNode;
        if (boolean) {
            boolean.RemoveNode(portName);
        }

        CompositeNode composite = parent as CompositeNode;
        if (composite) {
            composite.children.Remove(child);
        }

        OptionNode option = parent as OptionNode;
        if (option) {
            option.RemoveNode(portName);
        }
    }

    public List<TreeNode> GetChildern(TreeNode parent) {
        List<TreeNode> children = new();

        BoolNode boolean = parent as BoolNode;
        if (boolean) {
            children.Add(boolean.True);
            children.Add(boolean.False);
        }

        CompositeNode composite = parent as CompositeNode;
        if (composite) {
            return composite.children;
        }

        OptionNode option = parent as OptionNode;
        if (option) {
            return option.GetOptions();
        }

        return children;
    }
}
