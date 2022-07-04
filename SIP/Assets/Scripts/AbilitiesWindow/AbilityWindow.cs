using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AbilityWindow : EditorWindow {
    private string[] options = new string[] {
        Abilities.Element.Water.ToString(),
        Abilities.Element.Fire.ToString(),
        Abilities.Element.Air.ToString(),
        Abilities.Element.Earth.ToString()
    };

    public class SaveObject {
        public List<string> names = new List<string>();
    }

    public class ButtonData {
        public string text = "Enter Text Here";
        public int bg_index = 0;
    }

    public class BtnsData {
        public List<ButtonData> buttons = new List<ButtonData>();
        public Vector3[] bg_colors = new Vector3[] {
            new Vector3(95f, 98f, 99f),
            new Vector3(47f, 48f, 48f),
        };
        public Abilities data = new Abilities();
    }

    public BtnsData btnData = new BtnsData();
    public ButtonData selectedbutton;

    private bool isRenaming = false;
    private SaveData saveData = new SaveData();
    private Vector2 attrsScroll = Vector2.zero;
    private Vector2 attrButtonsScroll = Vector2.zero;
    private Vector2 abilityNamesScroll = Vector2.zero;


    [MenuItem("Window/Abilites")]
    public static void ShowWindow() { 
        AbilityWindow win = GetWindow<AbilityWindow>("Custom Abilites");
        win.minSize = new Vector2(900, 700);
        SaveSystem.Init();
    }

    // The window elements
    private void OnGUI() {
        AbiliityList();
        AbilityData();

        if (!EditorGUIUtility.editingTextField || Event.current.type == EventType.MouseDown) {
            EditorGUIUtility.editingTextField = false;
            isRenaming = false; 
            GUI.FocusControl(null); 
        }
    }

    private void OnEnable() {
        btnData.data = saveData.Load();
        if (btnData.data == null) { 
            btnData.data = new Abilities();
            return;
        }

        SaveObject savedName = JsonUtility.FromJson<SaveObject>(SaveSystem.Load("names.json"));
        foreach (string name in savedName.names) {
            ButtonData btn = AbilityButton();
            btn.text = name;
            btnData.buttons.Add(btn);
        }
    }

    private void OnDisable() {
        saveData.Save(btnData.data);
        SaveObject saveNames = new SaveObject();
        foreach (ButtonData btn in btnData.buttons) { saveNames.names.Add(btn.text); }
        SaveSystem.Save(JsonUtility.ToJson(saveNames), "names.json");
    }


    // ------------------------------ Ability List Display -------------------------------------------------
    private void AbiliityList() {
        float width = MinMaxValue(position.width * .15f, 145f, 180f);
        float height = position.height - 20f;
        Vector2 pos = new Vector2(10, 10);

        GUILayout.BeginArea(new Rect(pos.x, pos.y, width, height));     // List Container Starts here
            SetBackgroundColor(new Vector3(95, 98, 99));
            RemoveAbility(width * .025f, height * .937f, width * .95f);
            AddAbility(width * .025f, height * .97f, width * .95f, EditorGUIUtility.editingTextField);
        CreateTitle("Abilities", 18, new Rect(width / 2 - 37, 5, 100, 20));

            GUILayout.BeginArea(new Rect(10, 30, width - 20, height - 80));         // All Button Container Starts here
                ShowAllAbility(width);
            GUILayout.EndArea();                                                    // All Button Container ends here

        GUILayout.EndArea();                                            // List Container ends here
    }

    private void AddAbility(float x, float y, float width, bool isActive) {
        GUILayout.BeginArea(new Rect(x, y, width, 19));
            GUIStyle styles = new GUIStyle(GUI.skin.button);

            if (!isActive) {
                styles.hover.textColor = new Color(0f, 0f, 0f);
            }

            if (GUILayout.Button("Add New", styles)) {
                EditorGUIUtility.editingTextField = false;

                if (!isActive) {
                    InitAbility();
                }
            }
        GUILayout.EndArea();
    }

    private void RemoveAbility(float x, float y, float width) {
        GUILayout.BeginArea(new Rect(x, y, width, 19));
        GUIStyle styles = new GUIStyle(GUI.skin.button);

        if (GUILayout.Button("Remove", styles)) {
            int index = btnData.buttons.FindIndex(btn => btn == selectedbutton);
            btnData.buttons.Remove(selectedbutton);
            btnData.data.stats.Remove(btnData.data.stats[index]);
            btnData.data.types.Remove(btnData.data.types[index]);
            selectedbutton = null;
            // TODO: Set names values
            //btnData.data.names.Remove(btnData.data.names[index]);
        }
        GUILayout.EndArea();
    }

    private void ShowAllAbility(float width) {
        float y = 0;
        foreach (ButtonData btn in btnData.buttons) {
            GUILayout.BeginArea(new Rect(0, y, width - 20, 18));    // Each Button Area Starts here
                SetBackgroundColor(btnData.bg_colors[btn.bg_index]);
                if (!isRenaming || btn.bg_index == 0) {
                    if (GUILayout.Button("- " + btn.text, ClickableLabel())) {
                        btn.bg_index++;
                        ChangeSelectedButton(btn);
                        if (btn.bg_index >= btnData.bg_colors.Length) {
                            btn.bg_index = btnData.bg_colors.Length - 1;
                            EditorGUIUtility.editingTextField = true;
                            isRenaming = true;
                        }
                    }
                } else { btn.text = EditorGUILayout.TextField(btn.text, GUI.skin.textField); }
            GUILayout.EndArea();                                    // Each Button Area ends here
            y += 20;
        }
    }

    private void ChangeSelectedButton(ButtonData button) {
        if (selectedbutton != null) {
            if (selectedbutton == button) return;
            selectedbutton.bg_index = 0;
        }
        selectedbutton = button;
    }

    private ButtonData AbilityButton() { 
        ButtonData btn = new ButtonData();
        btn.bg_index = btnData.bg_colors.Length - 1;
        ChangeSelectedButton(btn);
        return btn;
    }

    private void InitAbility() {
        btnData.buttons.Add(AbilityButton());
        btnData.data.types.Add(Abilities.Element.Water);
        btnData.data.stats.Add(new Stats());
    }
    // ------------------------------ Ability List End -----------------------------------------------------


    // ------------------------------ Ability Attr Display -------------------------------------------------
    private void AbilityData() {
        float xOffset = 20f;
        float x = MinMaxValue(position.width*.15f+xOffset, 145f+xOffset, 180f+xOffset);
        float width = MinMaxValue(position.width-180f-xOffset*2, 725f, 1700f);
        float height = position.height - 20f;
        Vector2 pos = new Vector2(x, 10);

        GUILayout.BeginArea(new Rect(pos.x, pos.y, width, height));
            DisplayData(width, height);
        GUILayout.EndArea();
    }

    private void DisplayData(float conWidth, float conHeight) {
        if (selectedbutton == null) return;
        int index = btnData.buttons.FindIndex(btn => btn == selectedbutton);
        Abilities data = btnData.data;
        string abilityName = btnData.buttons[index].text;

        CreateTitle(abilityName, 26, new Rect(10,10,100,26));
        AddAttrBtns(data, index, conWidth-420);

        CreateTitle("Element: ", 12, new Rect(15, 40, 100, 16));
        data.types[index] = (Abilities.Element)EditorGUI.Popup(new Rect(70, 40, 70, 16), (int)data.types[index], options);

        DisplayAttrs(conWidth - 20, conHeight - 100, data.stats[index].attrs);
    }

    private void DisplayAttrs(float width, float height, List<AbilityAttr> attrs) {
        int wCard=275, hCard=175, xPadding=20;
        int cardsFitPerLine = (int)(width / wCard);
        int left = (int)((width - (cardsFitPerLine * wCard)) / 2);

        if ((xPadding + wCard)*cardsFitPerLine > width) { cardsFitPerLine -= 1; }

        GUILayout.BeginArea(new Rect(10, 100, width, height));
        SetBackgroundColor(Colors.GetColor(3));
        attrsScroll = GUILayout.BeginScrollView(attrsScroll);
        GUILayout.BeginVertical();

        float maxCols = Mathf.Ceil(attrs.Count / (float)cardsFitPerLine);
        for (int i = 0; i < maxCols; i++) {
            GUILayout.BeginHorizontal(Styles.Column(left - (xPadding*2)));
            for (int j = 0; j < cardsFitPerLine; j++) {
                if ((i * cardsFitPerLine) + j >= attrs.Count) { break; }
                GUILayout.BeginVertical(Styles.Row(xPadding), GUILayout.Width(wCard));
                DrawCard(attrs[i * cardsFitPerLine + j]);
                if (GUILayout.Button("Remove Attr", Styles.CardBtnRemove(wCard))) { attrs.Remove(attrs[i * cardsFitPerLine + j]); }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    private void AddAttrBtns(Abilities data, int index, float _x) {
        // TODO: Customize the way this is display with window resizing
        GUILayout.BeginArea(new Rect(_x, 10, 400, 80));
        SetBackgroundColor(Colors.GetColor(4));
        GUILayout.BeginVertical();
        attrButtonsScroll = GUILayout.BeginScrollView(attrButtonsScroll);
        //if (Event.current.type == EventType.Repaint || Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseUp) {
            //int x = 200, y = 20;
            GUILayout.BeginHorizontal();
            foreach (AbilityAttr type in GetAll().ToList()) {
                GUILayout.Button("Add " + type.GetType().Name);
                //if (GUILayout.Button("Add " + type.GetType().Name)) {
                //    data.stats[index].attrs.Add((AbilityAttr)Activator.CreateInstance(type.GetType()));
                //}
                //x += 110;
            }
            GUILayout.EndHorizontal();
        //}
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    private void DrawCard(AbilityAttr attr) {
        // TODO: Create the ability to handle different types and values coming in as fields
        GUILayout.Label(attr.GetType().Name, Styles.CardTitle());
        foreach (FieldInfo field in attr.GetType().GetFields()) {
            GUILayout.BeginHorizontal();
            GUILayout.Label(field.Name, Styles.CardLabelText());
            field.SetValue(attr, EditorGUILayout.FloatField((float)field.GetValue(attr)));
            GUILayout.EndHorizontal();
        }
    }
    // ------------------------------ Ability Attr End -------------------------------------------------


    // Our Helper Methods
    private void SetBackgroundColor(Vector3 color) {
        color = color / 255f;
        Texture2D texture = new Texture2D(1,1, TextureFormat.RGBA32, false);
        texture.SetPixel(0, 0, new Color(color.x, color.y, color.z));
        texture.Apply();
        GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), texture, ScaleMode.StretchToFill);
    }

    private void SetBackgroundColor(Color color) {
        Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), texture, ScaleMode.StretchToFill);
    }

    private void CreateTitle(string titleText, int fontsize, Rect position) {
        GUIStyle title = new GUIStyle();
        title.normal.textColor = Color.white;
        title.fontStyle = FontStyle.Bold;
        title.fontSize = fontsize;
        GUI.Label(position, titleText, title);
    }

    private GUIStyle ClickableLabel() {
        var stlyes = new GUIStyle();
        stlyes.fontSize = 13;
        stlyes.normal.textColor = Color.white;
        stlyes.border = new RectOffset(0, 0, 0, 0);
        return stlyes;
    }

    private float MinMaxValue(float value, float min, float max) {
        if (value <= min) return min;
        if (value >= max) return max;
        return value;
    }

    // Get all of the AbilityAttr subclasses
    private IEnumerable<AbilityAttr> GetAll() { 
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsSubclassOf(typeof(AbilityAttr)))
            .Select(type => Activator.CreateInstance(type) as AbilityAttr);
    }
}