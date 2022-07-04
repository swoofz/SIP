using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpdateWeights : MonoBehaviour {

    public PlayerController player;
    public TMP_InputField powerInput;
    public TMP_InputField knowledgeInput;
    public TMP_InputField efficiencyInput;

    public void UpdatePower() {
        Spell spell = GetSpell(0);
        spell.weights.power = float.Parse(powerInput.text);
    }

    public void UpdateKnowledge() {
        Spell spell = GetSpell(0);
        spell.weights.knowledge = float.Parse(knowledgeInput.text);
    }

    public void UpdateEfficiency() {
        Spell spell = GetSpell(0);
        spell.weights.efficiency = float.Parse(efficiencyInput.text);
    }

    public void ApplyWeight() {
        Spell spell = GetSpell(0);
        float power = spell.weights.power;
        float knowledge = spell.weights.knowledge;
        float efficiency = spell.weights.efficiency;

        if (power + knowledge + efficiency < 5) return;
        
        ApplyPower(spell, power / 5f);
        ApplyKnowledge(spell, knowledge / 5f);
        ApplyEfficiency(spell, efficiency / 5f);

        //Debug.Log($"{spell.weights.power}, {spell.weights.knowledge}, {spell.weights.efficiency}");
    }

    private Spell GetSpell(int index) {
        Spell spell = player.spellbook.spells[index];
        if (spell.weights == null) spell.weights = new SkillWeights();
        return spell;
    }

    private void ApplyPower(Spell spell, float power) {
        foreach (InstantDamage damage in spell.Damage) {
            damage.min = (damage.min * (power + 1)) - (damage.min * (1 - power));
            damage.max = (damage.max * (power + 1)) - (damage.max * (1 - power));
        }

        //spell.weights.power = 0;
    }

    private void ApplyKnowledge(Spell spell, float knowledge) {


        //spell.weights.knowledge = 0;
    }

    private void ApplyEfficiency(Spell spell, float efficiency) {
        foreach (SpellCost cost in spell.Cost) {
            float weightedValue = (cost.costAmount * (efficiency + 1)) - (cost.costAmount * (1 - efficiency));
            float different = weightedValue - cost.costAmount;
            cost.costAmount -= (int)different; 
        }
        //spell.weights.efficiency = 0;
    }
}
