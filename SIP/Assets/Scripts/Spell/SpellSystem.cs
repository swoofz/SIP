using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellSystem : MonoBehaviour {

    public static void UseSpell(Spell spell, Transform player) {
        GameObject clone = Instantiate(spell.prefab, player.position + player.forward, Quaternion.identity);
        clone.name = spell.name;
        clone.transform.forward = player.forward;
        clone.GetComponent<SpellUser>().user = player.gameObject;
        spell.clones.Add(clone);
        Destroy(clone, 5);
    }

    public static void SetPositions(List<Spell> spells, Transform pos) {
        foreach (Spell spell in spells) {
            if (spell.spellType != Spell.SpellType.Spell_At) continue;
            if (spell.clones.Count == 0) continue;
            if(!spell.clones[0]) spell.clones.RemoveAt(0);
            UpdatePositions(spell);
        }
    }

    public static void ResetAllCloneLists(List<Spell> spells) {
        foreach (Spell spell in spells) {
            spell.clones.Clear();
        }
    }

    public static int TakeDamage(Spell spell) {
        int damage = 0;
        foreach (InstantDamage spellDamage in spell.Damage) { 
            int getDamage = Random.Range((int)spellDamage.min, (int)spellDamage.max);
            damage += getDamage;
        }

        return damage;
    }

    // Helper Functions
    private static void UpdatePositions(Spell spell) {
        foreach (GameObject spellObj in spell.clones) {
            spellObj.transform.position += spellObj.transform.forward * Time.deltaTime * 50;
        }
    }
}
