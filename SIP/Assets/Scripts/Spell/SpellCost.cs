using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpellCost {
    public enum CostType {
        Mana,
        Energy,
        Stamia,
        Rage
    }

    public int id;
    public CostType type;
    public int costAmount;
}
