using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;

public class Ability : ScriptableObject {
    // Name of object is the name of the ability
    public string id;
    public Attributes attrs;
    public OnUse onUse;
    public Properties properties;
    public int level = 1;
}
