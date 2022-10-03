using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BoolNode : TreeNode {
    public TreeNode True;
    public TreeNode False;

    public void SetNode(string portName, TreeNode node) {
        switch (portName) {
            case "True":
                True = node;
                break;
            case "False":
                False = node;
                break;
        }
    }

    public void RemoveNode(string portName) {
        switch (portName) {
            case "True":
                True = null;
                break;
            case "False":
                False = null;
                break;
        }
    }
}
