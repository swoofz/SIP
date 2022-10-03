using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseNode : OptionNode {

    public enum Options { OnHit, OnCrit, OnMiss, OnUse, None }
    public Options option = Options.None;

    public TreeNode onUse = null;
    public TreeNode onHit = null;
    public TreeNode onCrit = null;
    public TreeNode onMiss = null;

    public UseNode() {
        options.Add((int)Options.OnHit, null);
        options.Add((int)Options.OnCrit, null);
        options.Add((int)Options.OnMiss, null);
        options.Add((int)Options.OnUse, null);
    }

    protected override void OnStart(OnUseData _data) {
        data = _data;
    }

    protected override void OnStop() {
        state = State.Running;
        option = Options.None;
    }

    protected override State OnUpdate() {
        return RunPath();
    }

    protected override string[] LogData() {
        var path = "";
        switch (option) { 
            case Options.OnHit:
                path = "On Hit";
                break;
            case Options.OnCrit:
                path = "On Crit";
                break;
            case Options.OnMiss:
                path = "On Miss";
                break;
            case Options.OnUse:
                path = "On Use";
                break;
        }

        if (!options[(int)option]) path = "Finished";
        return new[] { "UseNode", $"CHOICE({option})", $"{path}(path)" };
    }

    public override void SetNode(string portName, TreeNode node) {
        switch (portName) {
            case "On Hit":
                options[(int)Options.OnHit] = node;
                onHit = node;
                break;

            case "On Crit":
                options[(int)Options.OnCrit] = node;
                onCrit = node;
                break;

            case "On Miss":
                options[(int)Options.OnMiss] = node;
                onMiss = node;
                break;

            case "On Use":
                options[(int)Options.OnUse] = node;
                onUse = node;
                break;
        }
    }

    public override void RemoveNode(string portName) {
        switch (portName) {
            case "On Hit":
                options[(int)Options.OnHit] = null;
                onHit = null;
                break;

            case "On Crit":
                options[(int)Options.OnCrit] = null;
                onCrit = null;
                break;

            case "On Miss":
                options[(int)Options.OnMiss] = null;
                onMiss = null;
                break;

            case "On Use":
                options[(int)Options.OnUse] = null;
                onUse = null;
                break;
        }
    }

    public override List<TreeNode> GetOptions() {
        UpdateOptions();
        List<TreeNode> lst = new();

        foreach (var option in options) {
            if (option.Key == (int)Options.None) continue;
            lst.Add(option.Value);
        }

        return lst;
    }

    private State RunPath() {
        CreateLogData(LogData());

        if (option == Options.None || !options[(int)option]) return State.Success;
        return options[(int)option].Run(data);
    }

    private void UpdateOptions() {
        options[(int)Options.OnHit] = onHit;
        options[(int)Options.OnMiss] = onMiss;
        options[(int)Options.OnCrit] = onCrit;
        options[(int)Options.OnUse] = onUse;
    }
}
