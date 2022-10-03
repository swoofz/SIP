using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class OptionNode : TreeNode {
    public Dictionary<int, TreeNode> options = new();

    public abstract void SetNode(string portName, TreeNode node);
    public abstract void RemoveNode(string portName);
    public abstract List<TreeNode> GetOptions();
}
