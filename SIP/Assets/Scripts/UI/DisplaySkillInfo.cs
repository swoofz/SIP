using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplaySkillInfo : MonoBehaviour {

    public PlayerController player;
    public Text skillName;
    public Font font;

    private Transform tf;
    private List<Spell> spells;

    private GameObject damageText;
    private GameObject costText;
    private GameObject healText;
    private GameObject dOTText;

    private void Awake() {
        tf = GetComponent<Transform>();
    }

    // Start is called before the first frame update
    void Start() {
        spells = player.spellbook.spells;
        skillName.text = spells[0].name;
    }

    // Update is called once per frame
    void Update() {
        Vector3 pos = new Vector3(0, 80, 0);
        foreach (Spell.Attr attr in spells[0].attrs) {
            DisplayStats(attr.attribute, pos);
            pos.y -= 32;
        }
    }

    private void DisplayStats(Spell.SpellAttributes attributes, Vector3 pos) {
        string text = null;

        switch (attributes) {
            case Spell.SpellAttributes.None: return;
            
            case Spell.SpellAttributes.Damage:
                if (!damageText) {
                    damageText = new GameObject("Damage Text");
                    damageText.AddComponent<Text>();
                }
                float min = 0, max = 0;
                foreach (InstantDamage damage in spells[0].Damage) {
                    min += damage.min;
                    max += damage.max;
                }
                text = string.Format("{0,-14} {1,5}", "Damage:", $"{(int)min} - {(int)max}");
                CreateText(damageText.GetComponent<Text>(), text, pos);
                break;
            
            case Spell.SpellAttributes.DOT: 
                return;
            
            case Spell.SpellAttributes.Cost:
                if (!costText) {
                    costText = new GameObject("Cost Text");
                    costText.AddComponent<Text>();
                }
                    string type = spells[0].Cost[0].type.ToString() + ":";
                int ourCost = 0;
                foreach (SpellCost cost in spells[0].Cost) {
                    ourCost += cost.costAmount;
                }
                text = string.Format("{0,-14} {1,5}", type, ourCost);
                CreateText(costText.GetComponent<Text>(), text, pos);
                break;
            
            case Spell.SpellAttributes.Heal: 
                return;
        }

        //if (textObj == null || text == null) return;
        //CreateText(textObj.AddComponent<Text>(), text, pos);
    }

    private void CreateText(Text text, string ourText, Vector3 pos) {
        text.rectTransform.SetParent(tf, false);
        text.rectTransform.localPosition = pos;
        text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 32);
        text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 265);
        text.font = font;
        text.color = Color.black;
        text.fontSize = 20;
        text.text = ourText;
    }
}
