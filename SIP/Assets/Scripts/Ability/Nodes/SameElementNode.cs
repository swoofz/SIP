using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SameElementNode : BoolNode {

    protected override void OnStart(OnUseData _data) {
        data = _data;        
    }

    protected override void OnStop() {
        
    }

    protected override State OnUpdate() {
        var p_element = data.abilities[data.abilityIndex].ability.properties.element;
        CreateLogData(LogData());

        if (p_element == data.target_ele) return True ? True.Run(data) : State.Success;
        return False ? False.Run(data) : State.Success;
    }

    protected override string[] LogData() {
        // [Node] - operation - path
        var player_ele = data.abilities[data.abilityIndex].ability.properties.element;
        string path;

        if (player_ele == data.target_ele) path = True ? "True" : "Finished";
        else path = False ? "False" : "Finished";

        return new[] { "Same_Element", $"Player Element({player_ele}) is same as Target Element({data.target_ele})?", $"{path}(path)" };
    }
}
