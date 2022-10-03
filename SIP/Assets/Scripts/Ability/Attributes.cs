using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Attributes : Node {
    public enum Options { self, friendly, enemy }

    public List<Attribute> enemy = new();
    public List<Attribute> friendly = new();
    public List<Attribute> self = new();

    public Attribute CreateAttribute(System.Type type, Vector2 pos) {
        var attr = CreateInstance(type) as Attribute;
        attr.guid = GUID.Generate().ToString();
        attr.name = attr.guid;
        attr.position = pos;

        AssetDatabase.AddObjectToAsset(attr, this);
        AssetDatabase.SaveAssets();
        return attr;    
    }

    public void DeleteAttribute(Attribute attr) {
        if(enemy.Contains(attr)) enemy.Remove(attr);
        if(friendly.Contains(attr)) friendly.Remove(attr);
        if(self.Contains(attr)) self.Remove(attr);

        AssetDatabase.RemoveObjectFromAsset(attr);
        AssetDatabase.SaveAssets();
    }

    public void AssignNode(Attribute child, string portName) {
        switch (portName) {
            case "Self":
                self.Add(child);
                break;
            case "Friendly":
                friendly.Add(child);
                break;
            case "Enemy":
                enemy.Add(child);
                break;
        }
    }

    public void UnassignNode(Attribute child, string portName) {
        switch (portName) {
            case "Self":
                self.Remove(child);
                break;
            case "Friendly":
                friendly.Remove(child);
                break;
            case "Enemy":
                enemy.Remove(child);
                break;
        }
    }

    public Dictionary<Options, List<Attribute>> GetAttributes() { 
        Dictionary<Options, List<Attribute>> allAttrs = new();

        allAttrs.Add(Options.self, new());
        allAttrs.Add(Options.friendly, new());
        allAttrs.Add(Options.enemy, new());

        foreach (Attribute attr in self) allAttrs[Options.self].Add(attr);
        foreach (Attribute attr in friendly) allAttrs[Options.friendly].Add(attr);
        foreach (Attribute attr in enemy) allAttrs[Options.enemy].Add(attr);

        return allAttrs;    
    }
}
