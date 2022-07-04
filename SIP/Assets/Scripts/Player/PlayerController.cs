using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public int speed = 5;

    public Spellbook spellbook;

    private Transform tf;
    

    private void Awake() {
        tf = GetComponent<Transform>();
        SpellSystem.ResetAllCloneLists(spellbook.spells);
        foreach(Spell spell in spellbook.spells) {
            if(spell.weights != null) continue;
            spell.weights = new SkillWeights(); 
        }
    }

    // Start is called before the first frame update
    void Start() {
        //var fieldValues = spellbook.spells[0].GetType().GetFields();
        //for (int i = 0; i < fieldValues.Length; i++) { Debug.Log(fieldValues[i].ToString()); }
        //var fieldValues = tf.GetType().GetFields().Select(field => field.GetValue(Transform)).ToList();

        Type type = typeof(Spell);
        var p_values = type.GetProperties();
        //for (int i = 0; i < p_values.Length; i++) { Debug.Log(p_values[i]); }
    }

    // Update is called once per frame
    void Update() {
        Movement();
        SpellSystem.SetPositions(spellbook.spells, tf);

        if (Input.GetKeyDown(KeyCode.Space)) SpellSystem.UseSpell(spellbook.spells[0], tf);
    }

    void Movement() {
        float x = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float y = Input.GetAxis("Vertical") * speed * Time.deltaTime;

        tf.position += new Vector3(x, 0, y);
    }

    public Spell FindSpell(string spellName) {
        foreach (Spell spell in spellbook.spells) {
            if (spell.name == spellName) return spell;
        }

        Debug.Log("Couldn't find Spell");
        return null;
    }

    public int FindSpellIndex(string spellName) {
        for (int i = 0; i < spellbook.spells.Count; i++) {
            if(spellName == spellbook.spells[i].name) return i;
        }

        Debug.Log("Couldn't find Spell");
        return -1;
    }

    public void UpdateSpellDamage(int index) {
        foreach (InstantDamage damage in spellbook.spells[index].Damage) {
            damage.min += 1;
            damage.max += 2;
        }
    }
}
