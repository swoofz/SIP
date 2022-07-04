using System.IO;
using UnityEngine;

public static class SaveSystem {

    public static readonly string saveFolder = Application.dataPath + "/Saves/";
    public static void Init() {
        if (!Directory.Exists(saveFolder)) { Directory.CreateDirectory(saveFolder); }
    }

    public static void Save(string data, string filename) {
        File.WriteAllText(saveFolder + filename, data);
    }

    public static string Load(string filename) {
        if (!File.Exists(saveFolder + filename)) return null;
        return File.ReadAllText(saveFolder + filename);
    }

}

