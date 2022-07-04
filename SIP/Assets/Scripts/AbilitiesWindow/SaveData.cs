using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

public class SaveData {

    public void Save(Abilities data) {
        string json = "{";

        json += @"""types"": " + SaveTypes(data.types) + ",";
        json += @"""stats"": " + SaveStats(data.stats);

        json += "}";
        SaveSystem.Save(json, "abilties.json");
    }

    public Abilities Load() {
        string saveFile = SaveSystem.Load("abilties.json");
        if (saveFile == null) { return null; }
        saveFile = RemoveFirstAndLast(saveFile);

        string statListData = "", typesListData = "";
        for (int i = 0; i < saveFile.Length; i++) {
            typesListData += saveFile[i];
            if (saveFile[i] == ']') {
                statListData = saveFile.Remove(0, i + 2);
                break;
            }
        }

        Abilities data = new Abilities();
        data.types = GetElements(typesListData);
        data.stats = GetStats(statListData);

        return data;
    }

    private string RemoveFirstAndLast(string str) {
        str = str.Remove(0, 1);
        return str.Remove(str.Length - 1);
    }

    private string GetKey(string str) {
        string key = "";
        for (int i = 0; i < str.Length; i++) {
            if (str[i] == ':') break;
            key += str[i];
        }
        return key;
    }

    private string RemoveKey(string str) {
        str = str.Remove(0, GetKey(str).Length+2);
        str = RemoveFirstAndLast(str);
        return str;
    }

    private List<string> SplitObjs(string str, bool isObj=true) {
        char beforesplitChar = ']';
        if (isObj) { beforesplitChar = '}'; }

        List<string> data = new List<string>();
        string placeholder = "";
        for (int i = 0; i < str.Length; i++) {
            placeholder += str[i];
            if (str[i] == '}') {
                if (str[i-1] == beforesplitChar && (i + 1 >= str.Length || str[i + 1] == ',')) {
                    placeholder = RemoveFirstAndLast(placeholder);
                    data.Add(placeholder);
                    placeholder = "";
                    i++;
                }
            }
        }
        return data;
    }

    private List<Abilities.Element> GetElements(string data) {
        List<Abilities.Element> types = new List<Abilities.Element>();
        data = RemoveKey(data);
        if (data == "") { return types; }

        foreach (string ele in data.Split(",")) {
            switch (RemoveFirstAndLast(ele)) {
                case "Water": types.Add(Abilities.Element.Water);
                    break;
                case "Fire": types.Add(Abilities.Element.Fire);
                    break;
                case "Air": types.Add(Abilities.Element.Air);
                    break;
                case "Earth": types.Add(Abilities.Element.Earth);
                    break;
                default:
                    break;
            }
        }

        return types;
    }

    private List<Stats> GetStats(string data) {
        List<Stats> stats = new List<Stats>();
        data = RemoveKey(data);
        if (data == "") { return stats; }

        foreach (string statdata in SplitObjs(data, false)) {
            Stats stat = new Stats();
            stat.attrs = GetAttrs(statdata);
            stats.Add(stat);
        }

        return stats;
    }

    private List<AbilityAttr> GetAttrs(string data) {
        List<AbilityAttr> attrs = new List<AbilityAttr>();
        data = RemoveKey(data);
        if (data == "") { return attrs; }

        foreach (string obj in SplitObjs(data)) { attrs.Add(GetAttr(obj)); }
        return attrs; 
    }

    private AbilityAttr GetAttr(string data) { 
        string key = GetKey(data);
        data = RemoveKey(data);
        List<string> values = new List<string>();
        foreach (string value in data.Split(",")) {
            values.Add(value.Split(":")[1]);
        }


        // TODO: Create a for loop to get the type and set the values of that type
        switch (RemoveFirstAndLast(key)) {
            case "Damage":
                return new AbilityAttr.Damage() { min = float.Parse(values[0]), max = float.Parse(values[1]) };
            case "Buff":
                return new AbilityAttr.Buff() { duration = float.Parse(values[0]) };
            case "Heal":
                return new AbilityAttr.Heal() { min = float.Parse(values[0]), max = float.Parse(values[1]) };
            case "DOT":
                return new AbilityAttr.DOT() { 
                    duration = float.Parse(values[0]), 
                    delay = float.Parse(values[1]), 
                    damage = float.Parse(values[2])
                };
            default:
                return null;
        }
    }

    // ---------------------- Save Helper Methods ---------------------------------------
    private string SaveTypes(List<Abilities.Element> types) {
        string value = "[";
        foreach (Abilities.Element element in types) {
            value += $@"""{element}""" + ","; // "Water"
        }

        if (value == "[") { return value + "]"; }
        return value.Remove(value.Length - 1) + "]";
    }

    private string SaveStats(List<Stats> stats) {
        string value = "[";
        foreach (Stats stat in stats) {
            value += @"{""attrs"": " + SaveAttrs(stat.attrs) + "},";
        }

        if (value == "[") { return value + "]"; }
        return value.Remove(value.Length - 1) + "]";
    }

    private string SaveAttrs(List<AbilityAttr> attrs) {
        string value = "[";
        foreach (AbilityAttr attr in attrs) {
            value += "{" + $@"""{attr.GetType().Name}""" + ": " + SaveAttrInfo(attr) + "},";
        }

        if (value == "[") { return value + "]"; }
        return value.Remove(value.Length - 1) + "]";
    }

    private string SaveAttrInfo(AbilityAttr attr) {
        string value = "{";
        foreach (FieldInfo field in attr.GetType().GetFields()) {
            value += $@"""{field.Name}"": {field.GetValue(attr)},";
        }

        if (value == "{") { return value + "}"; }
        return value.Remove(value.Length - 1) + "}";
    }
    // ---------------------- Save Helper End ---------------------------------------

}
