using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Spell))]
public class SpellCEditor : Editor {

    private List<bool> foldouts = new List<bool>();
    private List<int> ids = new List<int>();

    private int attrLength = 5; // amounth of enum attr


    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        Spell spell = target as Spell;

        int index = 0;
        foreach (Spell.Attr attr in spell.attrs) {
            EditiableVariables(spell, attr, index);
            EditorGUILayout.Space();
            index++;
        }

        if (GUI.changed) {
            UpdateList(spell);
            EditorUtility.SetDirty(target);
        }
    }

    private void EditiableVariables(Spell spell, Spell.Attr attr, int index) {
        if (foldouts.Count == index) { foldouts.Add(false); }
        switch (attr.attribute) {
            case Spell.SpellAttributes.None:
                return;
            case Spell.SpellAttributes.Damage:
                foldouts[index] = EditorGUILayout.Foldout(foldouts[index], "Instant Damage");
                InstantDamage damage;

                int damIndex = DoesExist(attr.attribute, spell, attr.id);
                if (damIndex == -1) {
                    damage = new InstantDamage();
                    damage.id = attr.id;
                    damIndex = spell.Damage.Count;
                    spell.Damage.Add(damage);
                    ids.Add(damage.id);
                }
                damage = spell.Damage[damIndex];
                if (foldouts[index]) {
                    damage.type = (DamageType.Type)EditorGUILayout.EnumPopup("Damage Type", damage.type);
                    damage.min = EditorGUILayout.FloatField("Min Damage", damage.min);
                    damage.max = EditorGUILayout.FloatField("Max Damage", damage.max);
                }
                break;
            case Spell.SpellAttributes.Heal:
                foldouts[index] = EditorGUILayout.Foldout(foldouts[index], "Heal");
                InstantHeal heal;

                int healIndex = DoesExist(attr.attribute, spell, attr.id);
                if (healIndex == -1) {
                    heal = new InstantHeal();
                    heal.id = index;
                    healIndex = spell.Damage.Count;
                    spell.Heal.Add(heal);
                    ids.Add(heal.id);
                }
                heal = spell.Heal[healIndex];
                if (foldouts[index]) {
                    heal.min = EditorGUILayout.IntField("Min Healing", heal.min);
                    heal.max = EditorGUILayout.IntField("Max Healing", heal.max);
                }
                break;
            case Spell.SpellAttributes.DOT:
                foldouts[index] = EditorGUILayout.Foldout(foldouts[index], "Damage Over Time");
                DamageOverTime dot;

                int dotIndex = DoesExist(attr.attribute, spell, attr.id);
                if (dotIndex == -1) {
                    dot = new DamageOverTime();
                    dot.id = index;
                    dotIndex = spell.Damage.Count;
                    spell.DOT.Add(dot);
                    ids.Add(dot.id);
                }
                dot = spell.DOT[dotIndex];
                if (foldouts[index]) {
                    dot.type = (DamageType.Type)EditorGUILayout.EnumPopup("Damage Type", dot.type);
                    dot.tick = EditorGUILayout.IntField("Damage per tick", dot.tick);
                    dot.duration = EditorGUILayout.IntField("Duration", dot.duration);
                }
                break;
            case Spell.SpellAttributes.Cost:
                foldouts[index] = EditorGUILayout.Foldout(foldouts[index], "Spell Cost");
                SpellCost cost;

                int costIndex = DoesExist(attr.attribute, spell, attr.id);
                if (costIndex == -1) {
                    cost = new SpellCost();
                    cost.id = index;
                    costIndex = spell.Cost.Count;
                    spell.Cost.Add(cost);
                    ids.Add(cost.id);
                }
                cost = spell.Cost[costIndex];
                if (foldouts[index]) {
                    cost.type = (SpellCost.CostType)EditorGUILayout.EnumPopup("Cost Type", cost.type);
                    cost.costAmount = EditorGUILayout.IntField("Resources needed", cost.costAmount);
                }
                break;
            default:
                Debug.Log($"Inspector not setup for value. {attr}");
                return;
        }
    }



    // Find the index of the spell attribute if there is one
    private int DoesExist(Spell.SpellAttributes attribute, Spell spell, int id) {
        switch (attribute) {
            case Spell.SpellAttributes.Damage:
                for (int i=0; i < spell.Damage.Count; i++) {
                    if(spell.Damage[i].id == id) return i;
                }
                break;
            case Spell.SpellAttributes.Heal:
                for (int i = 0; i < spell.Heal.Count; i++) {
                    if (spell.Heal[i].id == id) return i;
                }
                break;
            case Spell.SpellAttributes.DOT:
                for (int i = 0; i < spell.DOT.Count; i++) {
                    if (spell.DOT[i].id == id) return i;
                }
                break;
            case Spell.SpellAttributes.Cost:
                for (int i = 0; i < spell.Cost.Count; i++) {
                    if (spell.Cost[i].id == id) return i;
                }
                break;
        }

        return -1;
    }

    // Update the changes to the public list for all list
    private void UpdateList(Spell spell) {
        if (!NeedsUpdate(spell)) return;

        int id = -1;
        for (int i = 0; i < ids.Count; i++) {
            bool found = false;
            foreach (Spell.Attr attr in spell.attrs) {
                if (ids[i] != attr.id) continue;
                found = true;
                break;
            }

            if (found) continue;
            id = ids[i];
            ids.Remove(id);
            break;
        }

        if (id == -1) return;
        int index;
        for (int i = 1; i < attrLength; i++) {
            switch (i) {
                case (int)Spell.SpellAttributes.Damage:
                    index = DoesExist(Spell.SpellAttributes.Damage, spell, id);
                    if (index != -1) spell.Damage.Remove(spell.Damage[index]);
                    break;
                case (int)Spell.SpellAttributes.Heal:
                    index = DoesExist(Spell.SpellAttributes.Heal, spell, id);
                    if (index != -1) spell.Heal.Remove(spell.Heal[index]);
                    break;
                case (int)Spell.SpellAttributes.DOT:
                    index = DoesExist(Spell.SpellAttributes.DOT, spell, id);
                    if (index != -1) spell.DOT.Remove(spell.DOT[index]);
                    break;
                case (int)Spell.SpellAttributes.Cost:
                    index = DoesExist(Spell.SpellAttributes.Cost, spell, id);
                    if (index != -1) spell.Cost.Remove(spell.Cost[index]);
                    break;
            }
        }
    }

    // Check if something changed in the list to need to do a update for all list
    private bool NeedsUpdate(Spell spell) {
        int idsCount = ids.Count;
        int spellAttrs = spell.attrs.Count;
        foreach (Spell.Attr attr in spell.attrs) {
            if (attr.attribute == Spell.SpellAttributes.None) { spellAttrs--; }
        }
        if (idsCount <= spellAttrs) return false;
        return true;
    }
}
