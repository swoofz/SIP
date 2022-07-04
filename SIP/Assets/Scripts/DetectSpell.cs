using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectSpell : MonoBehaviour {
    public enum WeightType {
        Power, Knowledge, Efficiency
    }
    public enum ApplyOptions {
        Opposite, Learning, Same, Overpowered,
        Weak, Immunity
    }

    [System.Serializable]
    public class Weight {
        public ApplyOptions options;
        public DetectSpell.WeightType type;
        public float value;
    }

    public DamageType.Type element;
    public List<Weight> ourWeights;

    private void OnCollisionEnter(Collision collision) {
        if (!collision.gameObject.GetComponent<SpellUser>()) return;
        
        GameObject user = collision.gameObject.GetComponent<SpellUser>().user;
        PlayerController player = user.GetComponent<PlayerController>();
        int spellIndex = player.FindSpellIndex(collision.gameObject.name);
        Destroy(collision.gameObject);

        if (spellIndex == -1) return;
        // Debug.Log(SpellSystem.TakeDamage(player.spellbook.spells[spellIndex]));
        //player.UpdateSpellDamage(spellIndex);
        Spell spell = player.spellbook.spells[spellIndex];
        ChangeWeights(spell);
        DisplayWeights(spell);
    }

    private void ChangeWeights(Spell spell) {
        foreach(Weight weight in ourWeights) {
            switch(weight.options) {
                case ApplyOptions.Opposite: 
                    Debug.Log("Opposite Option");
                    Opposite(spell);
                    break;
                case ApplyOptions.Immunity: 
                    Debug.Log("Immunity Option");
                    break;
                default: break;
            }
        }
    }

    private void Opposite(Spell spell) {
        foreach(InstantDamage damage in spell.Damage) {
            if(DamageType.isOppisiteType(element, damage.type)) {
                UpdateWeights(spell);
            }
        }
    }
    private void DisplayWeights(Spell spell) {
        SkillWeights weights = spell.weights;
        Debug.Log($"Power: {weights.power}, Knowledge: {weights.knowledge}, Efficiency: {weights.efficiency}");
    }

    private void UpdateWeights(Spell spell) {
        SkillWeights weights = spell.weights;
        foreach (Weight weight in ourWeights) {
            switch(weight.type) {
                case WeightType.Power: 
                    weights.power += weight.value;
                    break;
                case WeightType.Knowledge: 
                    weights.knowledge += weight.value;
                    break;
                case WeightType.Efficiency: 
                    weights.efficiency += weight.value;
                    break;
                default: break;
            }
        }
    }

}
