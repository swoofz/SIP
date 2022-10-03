using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OppsiteNode : BoolNode {

    protected override void OnStart(OnUseData _data) {
        data = _data;
    }

    protected override void OnStop() {

    }

    protected override State OnUpdate() {
        CreateLogData(LogData());
        if (IsOppsiteElement()) return True ? True.Run(data) : State.Success;
        return False ? False.Run(data) : State.Success;
    }

    private bool IsOppsiteElement() {
        var p_element = data.abilities[data.abilityIndex].ability.properties.element;

        switch (p_element) {
            case ElementTpye.Water:
                if (data.target_ele == ElementTpye.Fire) return true;
                break;
            case ElementTpye.Fire:
                if (data.target_ele == ElementTpye.Water) return true;
                break;
            case ElementTpye.Air:
                if (data.target_ele == ElementTpye.Earth) return true;
                break;
            case ElementTpye.Earth:
                if (data.target_ele == ElementTpye.Air) return true;
                break;
        }

        return false;
    }

    protected override string[] LogData() {
        // [Node] - operation - path
        var player_ele = data.abilities[data.abilityIndex].ability.properties.element;
        string path;

        if (IsOppsiteElement()) path = True ? "True" : "Finished";
        else path = False ? "False" : "Finished";

        return new[] { "Oppsite_Element", $"Player Element({player_ele}) is Oppsite of Target Element({data.target_ele})?", $"{path}(path)" };
    }
}
