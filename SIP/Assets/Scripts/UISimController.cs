using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class UISimController : MonoBehaviour {

    public AbilityBook book;
    public Player player;

    private DropdownField dropdown;
    private ScrollView log;
    private VisualElement root;

    private List<AbilityBook.AbilityData> abilities = new();

    private void Awake() {
        root = GetComponent<UIDocument>().rootVisualElement;
        dropdown = root.Q<DropdownField>("abilityField");
        log = root.Q<ScrollView>("logData");
    }

    // Start is called before the first frame update
    void Start() {
        AddAbilties();
        DisplayAbility();

        root.Q<Button>("attack").clicked += Attack;
        root.Q<Button>("use").clicked += UseAbility;
    }

    private void DisplayAbility() {
        DisplayDescription();
        DisplayAbilityStats();
    }

    public void LogData(string node, string operation, string nextPath) {
        log.Add(new Label($"<color=green>[{node}]</color> -> {operation} -> {nextPath}"));
    }

    private void UseAbility() {
        Ability ability = abilities[GetAbilityIndex()].ability;
        var useNode = ability.onUse.rootNode as UseNode;
        useNode.option = (UseNode.Options)Random.Range(0, 4);
        var abilityOldLvl = ability.level;

        var repeatSize = (120 - ability.name.Length);
        var adjustSize = 0;
        if (repeatSize % 2 != 0) adjustSize = 1;

        log.Add(new Label($"" +
            $"{string.Concat(Enumerable.Repeat("-", (repeatSize - adjustSize)/2 - 1))} " +
            $"{ability.name} " +
            $"{string.Concat(Enumerable.Repeat("-", (repeatSize - adjustSize) / 2 + adjustSize - 1))}"));

        useNode.Run(SendData());

        log.Add(new Label($"{string.Concat(Enumerable.Repeat("-", 120))}"));

        if (AbilityLeveled(abilityOldLvl)) DisplayAbilityStats();
        log.verticalScroller.value = log.verticalScroller.highValue > 0 ? log.verticalScroller.highValue : 0;
    }

    private void Attack() {
        var level = abilities[GetAbilityIndex()].ability.level;
        do {
            UseAbility();
        } while (!AbilityLeveled(level));
    }

    private bool AbilityLeveled(int level) {
        return level != abilities[GetAbilityIndex()].ability.level;
    }

    private OnUseData SendData() {
        OnUseData data = new() {
            abilities = abilities,
            abilityIndex = GetAbilityIndex(),
            target_ele = GetRandomElement(),
            target_lvl = GetRandomLevel(),
            log = this
        };

        return data;
    }

    private ElementTpye GetRandomElement() {
        int num = Random.Range(0, 3);
        return (ElementTpye)num;
    }
    private int GetRandomLevel() {
        return Random.Range(5, 15);
    }

    private void DisplayAbilityStats() {
        var abilityList = root.Q<ScrollView>("abilityData");
        abilityList.Clear();
        var index = GetAbilityIndex();

        abilityList.Add(TargetingLabel("Self"));
        abilityList.Add(DisplayAttrs(GetAttrsList(abilities[index].selfStats)));
        abilityList.Add(TargetingLabel("\nFriendly"));
        abilityList.Add(DisplayAttrs(GetAttrsList(abilities[index].friendlyStats)));
        abilityList.Add(TargetingLabel("\nEnemy"));
        abilityList.Add(DisplayAttrs(GetAttrsList(abilities[index].enemyStats)));
    }

    private Label TargetingLabel(string target) {
        var label = new Label($"{target} Targeting Stats");
        label.style.unityFontStyleAndWeight = FontStyle.Bold;

        return label;    
    }

    private string[] GetAttrsList(Dictionary<string, List<AbilityBook.Data>> stats) {
        string[] list = new[] { "", "", "Buffs: ", "Debuffs: " };
        foreach (var attr in stats) list = GetAttrs(list, attr.Value);

        return list;
    }

    private string[] GetAttrs(string[] list, List<AbilityBook.Data> attrs) {
        foreach (var attr in attrs) {
            switch (attr.values[0]) {
                case (int)AttributeType.Damage:
                    list[0] = $"Damage: {attr.values[1].ToString("0.00")} - {attr.values[2].ToString("0.00")}";
                    break;
                case (int)AttributeType.Heal:
                    list[1] = $"Heal: {attr.values[1]} - {attr.values[2]}";
                    break;
                case (int)AttributeType.Buff:
                    list[2] += $"\n  {(Effects.Effect)attr.values[1]}({attr.values[3].ToString("0.00")}%) - {attr.values[2].ToString("0.0")}s";
                    break;
                case (int)AttributeType.Debuff:
                    list[3] += $"\n  {(Effects.Effect)attr.values[1]}({attr.values[3].ToString("0.00")}%) - {attr.values[2].ToString("0.0")}s";
                    break;
            }
        }

        return list;
    }

    private VisualElement DisplayAttrs(string[] attrs) {
        string dam = attrs[0], heal = attrs[1], buff = attrs[2], debuff = attrs[3];

        if (dam == "") dam = "Damage: 0 - 0";
        if (heal == "") heal = "Heal: 0 - 0";

        var container = new VisualElement();
        container.Add(new Label(dam));
        container.Add(new Label(heal));
        container.Add(new Label(buff));
        container.Add(new Label(debuff));

        return container;
    }

    private void AddAbilties() {
        dropdown.choices.Clear();

        foreach (Ability _ability in book.abilities) {
            abilities.Add(new AbilityBook.AbilityData {
                ability = _ability,
                weights = new[] { 0f, 1f, 1f }, // timeUsed, power, effective
                selfStats = book.GetStats(_ability.attrs.self),
                friendlyStats = book.GetStats(_ability.attrs.friendly),
                enemyStats = book.GetStats(_ability.attrs.enemy),
            });

            dropdown.choices.Add(_ability.name);
        }

        dropdown.RegisterValueChangedCallback(e => DisplayAbility());
        dropdown.value = dropdown.choices[0];
    }

    private void DisplayDescription() {
        int index = GetAbilityIndex();

        var text = root.Q<Label>("description");
        text.text = abilities[index].ability.properties.description;
    }

    private int GetAbilityIndex() {
        return dropdown.choices.IndexOf(dropdown.value);
    }
}
