using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


public class CustomAbilityEditor : EditorWindow {

    private Button addButton;
    private Button removeButton;
    private TextField edittingField;
    private ScrollView abilityScroll;
    private VisualElement graphContainer;

    private Dictionary<string, Ability> abilities = new Dictionary<string, Ability>();
    private Dictionary<string, AbilityView> abilityViews = new Dictionary<string, AbilityView>();
    private VisualElement graphView;

    private List<VisualElement> elements = new List<VisualElement>();

    private string abilitySavePATH = "Assets/Scripts/Ability/Abilities/";

    private string selectedBtnId = "";
    private string oldBtnName = "";
    private bool editting = false;
    private int abilityIndex = -1;

    [MenuItem("Tools/CustomAbilityEditor")]
    public static void OpenWindow() {
        CustomAbilityEditor wnd = GetWindow<CustomAbilityEditor>();
        wnd.titleContent = new GUIContent("CustomAbilityEditor");
        wnd.minSize = new Vector2(600, 500);
    }

    public void CreateGUI() {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/AbilityWindow/CustomAbilityEditor.uxml");
        visualTree.CloneTree(root);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Editor/AbilityWindow/CustomAbilityEditor.uss");
        root.styleSheets.Add(styleSheet);

        abilityScroll = rootVisualElement.Q<ScrollView>("ability-container");
        addButton = root.Q<Button>("add-ability-btn");
        addButton.clicked += AddNewAbility;
        removeButton = root.Q<Button>("remove-ability-btn");
        removeButton.clicked += RemoveAbility;
        graphContainer = root.Q<VisualElement>("tab-content");

        var tabs = root.Q<VisualElement>("tabs");
        for (int i = 0; i < tabs.childCount; i++) {
            var tab = tabs.contentContainer[i] as Button;
            tab.RegisterCallback<ClickEvent>(e => ChangeTab(tab));
        }

        if (graphView != null) graphContainer.Add(graphView);

        DisplayElements();
    }

    private void ChangeTab(Button tab) {
        if (graphView != null) graphContainer.Remove(graphView);
        graphView = abilityViews[selectedBtnId].views[tab.tabIndex];
        graphContainer.Add(graphView);
    }

    private void OnEnable() {
        // Create / Load Ability attr graph then display it on create in the tab-content Visual Element
        LoadAbility();
    }


    private void Update() {
        if (rootVisualElement.focusController.focusedElement == null) return;

        if (!editting) {
            edittingField = rootVisualElement.Q<TextField>("Editting");
            if (edittingField == null) return;
            oldBtnName = edittingField.value;
            editting = true;
            addButton.SetEnabled(false);
            removeButton.SetEnabled(false);
            edittingField.Focus();
            edittingField.Q(TextField.textInputUssName).RegisterCallback<KeyDownEvent>(e => ChangeAbilityName(e.keyCode));
        }
    }

    private void LoadAbility() {
        foreach (var file in Directory.GetFiles(abilitySavePATH, "*.asset")) {
            Ability ability = AssetDatabase.LoadAssetAtPath<Ability>(file);
            if (ability == null) continue;
            CreateAbilityButton(ability);
            GenerateAbilityViews(ability);
        }

        if (abilities.Count == 0) return;

        int focusIndex = 0;
        if (abilityIndex != -1) focusIndex = abilityIndex;

        selectedBtnId = elements[focusIndex].name;
        elements[focusIndex].AddToClassList("selected-label");
        elements[focusIndex].Focus();

        graphView = abilityViews[selectedBtnId].views[0];
    }

    void ChangeAbilityName(KeyCode key) {
        if (key != KeyCode.Return) return;
        if (!(edittingField.value == oldBtnName) && AbilityExist(edittingField.value)) {
            Debug.Log("Ability name already exist. Create new one.");
            return;
        }

        AssetDatabase.RenameAsset($"{abilitySavePATH}{oldBtnName}.asset", edittingField.value);
        editting = false;
        Button btn = new Button();
        btn.AddToClassList("label-btn");
        btn.name = selectedBtnId;
        btn.text = edittingField.value;
        btn.clicked += GetFocused;
        elements[abilityIndex] = btn;

        Ability ability = abilities[selectedBtnId];
        ability.name = btn.text;
        ability.attrs = AssetDatabase.LoadAssetAtPath<Attributes>($"{abilitySavePATH}Attributes/{selectedBtnId}.asset");
        ability.onUse = AssetDatabase.LoadAssetAtPath<OnUse>($"{abilitySavePATH}On Use/{selectedBtnId}.asset");
        ability.properties = AssetDatabase.LoadAssetAtPath<Properties>($"{abilitySavePATH}Properties/{selectedBtnId}.asset");

        btn.AddToClassList("selected-label");
        DisplayElements();
        btn.Focus();
        
        oldBtnName = "";
        edittingField.Q(TextField.textInputUssName).UnregisterCallback<KeyDownEvent>(e => ChangeAbilityName(e.keyCode));
        edittingField = null;
        addButton.SetEnabled(true);
        removeButton.SetEnabled(true);
        foreach (var element in elements) { if (element != btn) element.SetEnabled(true); }
    }

    private bool AbilityExist(string name) {
        return AssetDatabase.LoadAssetAtPath<Ability>($"{abilitySavePATH}{name}.asset") != null ? true : false;
    }

    void AddNewAbility() { 
        Ability ability = GenerateAbility();
        CreateAbilityButton(ability);
        GerenateNodeObjects(ability);
        GenerateAbilityViews(ability);
        if(graphView != null) graphContainer.Remove(graphView);
        graphView = abilityViews[ability.id].views[0];
        graphContainer.Add(graphView);
        DisplayElements();
    }

    void RemoveAbility() {
        Button btn = rootVisualElement.Q<Button>(selectedBtnId);
        Ability ability = abilities[selectedBtnId];
        elements.Remove(btn);
        abilities.Remove(selectedBtnId);
        selectedBtnId = "";
        DeleteAbilityAssets(ability);
        removeButton.SetEnabled(false);
        DisplayElements();
    }

    private void DeleteAbilityAssets(Ability ability) {
        AssetDatabase.DeleteAsset($"{abilitySavePATH}{ability.name}.asset");
        AssetDatabase.DeleteAsset($"{abilitySavePATH}Attributes/{ability.id}.asset");
        AssetDatabase.DeleteAsset($"{abilitySavePATH}On Use/{ability.id}.asset");
        AssetDatabase.DeleteAsset($"{abilitySavePATH}Properties/{ability.id}.asset");
    }

    void CreateAbilityButton(Ability ability) {
        Button btn = new Button();
        btn.AddToClassList("label-btn");
        abilities.Add(ability.id, ability);
        btn.text = ability.name;
        btn.name = ability.id;
        btn.clicked += GetFocused;

        elements.Add(btn);
        btn.Focus();
    }

    void DisplayElements() {
        abilityScroll.Clear();
        elements.ForEach(e => { abilityScroll.Add(e); });
    }

    void GetFocused() {
        Button btn = rootVisualElement.focusController.focusedElement as Button;
        removeButton.SetEnabled(true);
        if (btn.name != selectedBtnId) {
            if (selectedBtnId != "") {
                elements[elements.FindIndex(e => e.name == selectedBtnId)].RemoveFromClassList("selected-label");
                graphContainer.Remove(graphView);
                graphView = abilityViews[btn.name].views[0];
                graphContainer.Add(graphView);
            }

            selectedBtnId = btn.name;
            abilityIndex = elements.IndexOf(btn);
            btn.AddToClassList("selected-label");
        } else {
            foreach (var element in elements) { if (element != btn) element.SetEnabled(false); }
            TextField textField = new TextField();
            textField.value = abilities[selectedBtnId].name;
            textField.name = "Editting";
            elements[abilityIndex] = textField;
            DisplayElements();
            textField.Focus();
        }
    }


    int UnquieNameNumber() {
        int num = 0;
        bool isUnquie = false;

        while (!isUnquie) {
            isUnquie = true;
            foreach (Ability ability in abilities.Values) {
                if (ability.name != $"New Ability_{num}") continue;
                isUnquie = false;
                num++;
            }
        }

        return num;
    }

    Ability GenerateAbility() {
        int unquieNumber = UnquieNameNumber();
        Ability ability = CreateInstance<Ability>();
        ability.name = $"New Ability_{unquieNumber}";
        ability.id = GUID.Generate().ToString();

        AssetDatabase.CreateAsset(ability, $"{abilitySavePATH}{ability.name}.asset");
        AssetDatabase.SaveAssets();
        return ability;
    }

    void GenerateAbilityViews(Ability ability) {
        AbilityView abilityView = new() {
            name = ability.name,
            views = new VisualElement[3] {
                new AttributeGraphView(ability.attrs) { guid = ability.id, name = "Attribute Graph" },
                new OnUseGraphView(ability.onUse) { guid = ability.id, name = "On Use Graph" },
                new AbilityPropView(ability.properties) { guid = ability.id, name = "Properties" }
            }
        };

        abilityViews.Add(ability.id, abilityView);
    }

    void GerenateNodeObjects(Ability ability) { 
        ability.attrs = CreateRootNode(typeof(Attributes), ability.id) as Attributes;
        ability.onUse = CreateRootNode(typeof(OnUse), ability.id) as OnUse;
        ability.properties = CreateRootNode(typeof(Properties), ability.id) as Properties;

        AssetDatabase.CreateAsset(ability.attrs, $"{abilitySavePATH}Attributes/{ability.id}.asset");
        AssetDatabase.CreateAsset(ability.onUse, $"{abilitySavePATH}On Use/{ability.id}.asset");
        AssetDatabase.CreateAsset(ability.properties, $"{abilitySavePATH}Properties/{ability.id}.asset");
        AssetDatabase.SaveAssets();
    }

    private Node CreateRootNode(System.Type type, string id) { 
        var node = CreateInstance(type) as Node;
        node.name = id;
        node.guid = id;
        node.isRoot = true;

        return node;
    }
}