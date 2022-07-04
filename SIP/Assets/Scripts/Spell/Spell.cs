using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour {

    public enum SpellType { 
        Melee,
        Spell_At,
        Spell_On
    }

    public enum SpellAttributes {
        None,
        Damage,
        Heal,
        DOT,
        Cost
    };

    public string spellName;
    [Tooltip("Melee - Weapon Abilities\n" +
        "Spell_At - Movable Spells\n" +
        "Spell_On - Debuffs, buffs, and any spell Non-moveable spell")]
    public SpellType spellType;
    public GameObject prefab;

    [System.Serializable]
    public struct Attr {
        public int id;
        public SpellAttributes attribute;
    }


    public List<Attr> attrs;
    public List<InstantDamage> Damage { get; set; }
    public List<InstantHeal> Heal { get; set; }
    public List<DamageOverTime> DOT { get; set; }
    public List<SpellCost> Cost { get; set; }
    public List<GameObject> clones { get; set; }
    public SkillWeights weights { get; set; }
}
