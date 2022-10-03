using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CompositeNode : TreeNode {
    public List<TreeNode> children = new();
}
