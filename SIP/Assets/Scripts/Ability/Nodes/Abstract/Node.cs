using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node : ScriptableObject {
    [HideInInspector] public bool isRoot = false;
    [HideInInspector] public string guid;
    [HideInInspector] public Vector2 position;
}
