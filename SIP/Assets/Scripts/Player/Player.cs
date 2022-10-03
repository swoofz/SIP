using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public int speed = 5;
    public AbilityBook book;
    public List<AbilityBook.AbilityData> Abilities { get { return abilities; } private set { } }
    public int Level { get; private set; }

    [HideInInspector] public int abilityIndex = 0;

    private Transform tf;
    private List<AbilityBook.AbilityData> abilities = new();

    private void Awake() {
        tf = GetComponent<Transform>();
        Level = 1;
    }


    private void Start() {
        foreach (Ability _ability in book.abilities) { 
            abilities.Add(new AbilityBook.AbilityData { 
                ability = _ability,
                weights = new[] { 0f, 1f, 1f }, // timeUsed, power, effective
                selfStats = book.GetStats(_ability.attrs.self),
                friendlyStats = book.GetStats(_ability.attrs.friendly),
                enemyStats = book.GetStats(_ability.attrs.enemy),
            }); 
        }
    }


    private void Update() {
        Movement();

        if (Input.GetKeyDown(KeyCode.Space)) { UseAbility(); }
    }


    private void Movement() {
        float x = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float y = Input.GetAxis("Vertical") * speed * Time.deltaTime;

        tf.position += new Vector3(x, 0, y);
    }

    private void UseAbility() {
        if (!book) return;

        Ability ability = abilities[abilityIndex].ability;
        var useNode = ability.onUse.rootNode as UseNode;

        useNode.option = UseNode.Options.OnUse;
        ability.onUse.Run(SendData());
    }

    private OnUseData SendData() {
        OnUseData data = new() {
            abilities = abilities,
            abilityIndex = abilityIndex,
            target_ele = ElementTpye.Water
        };

        return data;
    }
}
