using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakNode : BoolNode {
    protected override void OnStart(OnUseData _data) {
        data = _data;
    }

    protected override void OnStop() {
        
    }

    protected override State OnUpdate() {
        var ability = data.abilities[data.abilityIndex].ability;
        CreateLogData(LogData());

        if (ability.level + 5 < data.target_lvl) return True ? True.Run(data) : State.Success;
        return False ? False.Run(data) : State.Success;
    }

    protected override string[] LogData() {
        var ability = data.abilities[data.abilityIndex].ability;
        string path;
        if (ability.level + 5 < data.target_lvl) path = True ? "True" : "Finished";
        else path = False ? "False" : "Finished";

        return new[] { "Weak", $"Player Level({ability.level}) is to weaker vs Target Level({data.target_lvl})?", $"{path}(path)" };
    }
}
