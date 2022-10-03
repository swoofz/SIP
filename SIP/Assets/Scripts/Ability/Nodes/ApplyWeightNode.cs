using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyWeightNode : ActionNode {
    [Range(-2f, 2f)]
    public float power = 0f; 
    [Range(-2f, 2f)]
    public float effiency = 0f;
    //[Range(-2f, 2f)]
    //public float knowlegde = 0f;

    protected override void OnStart(OnUseData _data) {
        data = _data;
    }

    protected override void OnStop() {

    }

    protected override State OnUpdate() {
        CreateLogData(LogData());
        //ApplyWeights(data.abilities[data.abilityIndex]);
        //ChangeStats(data.abilities[data.abilityIndex]);

        return State.Success;
    }

    protected override string[] LogData() {
        var oldWeights = data.abilities[data.abilityIndex].weights;
        var old = $"\n\t{oldWeights[1]}, {oldWeights[2]}";
        var levelAbility = "";

        ApplyWeights(data.abilities[data.abilityIndex]);
        if (ChangeStats(data.abilities[data.abilityIndex])) levelAbility = "<color=yellow>_LevelAbility</color>";
        var newWeights = data.abilities[data.abilityIndex].weights;
        var new_w = $"\n\t{newWeights[1]}, {newWeights[2]}";

        return new[] {"Apply_Weights", $"AddToWeights{levelAbility}", $"Finished{old}{new_w}" };
    }

    private void ApplyWeights(AbilityBook.AbilityData abilityData) {
        abilityData.weights[0]++;
        abilityData.weights[1] += (power/100);
        abilityData.weights[2] += (effiency/100);
        //abilityData.weights[3] += (knowlegde/100);
    }

    private bool ChangeStats(AbilityBook.AbilityData abilityData) {
        var ability = abilityData.ability;
        if (ability.properties.timesToUpdate <= abilityData.weights[0]) {
            //Debug.Log("-------------------------- SELF -------------------------------");
            foreach (var d in abilityData.selfStats) StatIncrease(d.Value);
            //Debug.Log("-------------------------- Frie -------------------------------");
            foreach (var d in abilityData.friendlyStats) StatIncrease(d.Value);
            //Debug.Log("-------------------------- Enem -------------------------------");
            foreach (var d in abilityData.enemyStats) StatIncrease(d.Value);
            ResetWeights(abilityData);
            ability.level++;
            return true;
        }
        return false;
    }

    private void StatIncrease(List<AbilityBook.Data> _data) {
        if (_data == null) return;

        foreach (var d in _data) {
            switch (d.values[0]) {
                case (int)AttributeType.Damage:
                    UpdateStats(d.values);
                    break;
                case (int)AttributeType.Heal:
                    UpdateStats(d.values);
                    break;
                case (int)AttributeType.Buff:
                    UpdateModifiers(d.values);
                    break;
                case (int)AttributeType.Debuff:
                    UpdateModifiers(d.values);
                    break;
            }
        }
    }

    private void ResetWeights(AbilityBook.AbilityData abilityData) {
        abilityData.weights[0] = 0;
        abilityData.weights[1] = 1f;
        abilityData.weights[2] = 1f;
        //abilityData.weights[3] += 1f;
    }

    private float[] UpdateStats(float[] values) {
        var weights = data.abilities[data.abilityIndex].weights;   // 1 = power, 2 = effective

        // power = increase values, effective = decrease value by a fraction
        // 0 = type, 1 = min, 2 = max
        values[1] = (values[1] * weights[1]) - (values[1] * (weights[2] - 1)); // min
        values[2] = (values[2] * weights[1]) - (values[2] * (weights[2] - 1)); // max

        return values;
    }

    private float[] UpdateModifiers(float[] values) {
        var weights = data.abilities[data.abilityIndex].weights;   // 1 = power, 2 = effective

        // Power effects percentage, effective effect duration
        // 0 = type, 1 = effect, 2 = duration, 3 = perectage change
        values[2] = values[2] + (weights[2] - 1); // Duration
        values[3] = values[3] + ((weights[1] -1) / 100); // Percentage

        return values;
    }
}
