using UnityEngine;

// Basically a CSS file
public class Styles {
    // Copy and Paste method to create new styles
    public static GUIStyle stylenamehere() {
        GUIStyle styles = new GUIStyle();
        return styles;
    }


    #region Attr Card Styles
    public static GUIStyle CardLabelText() {
        GUIStyle styles = new GUIStyle(GUI.skin.label);
        styles.alignment = TextAnchor.MiddleLeft;
        styles.fixedWidth = 100;
        styles.normal.textColor = Color.black;
        styles.hover.textColor = Color.black;
        styles.active.textColor = Color.black;
        styles.margin.left = 10;
        styles.fontSize = 13;

        return styles;
    }

    public static GUIStyle CardTitle() {
        GUIStyle styles = new GUIStyle(GUI.skin.label);
        styles.alignment = TextAnchor.MiddleCenter;
        styles.fontSize = 18;
        styles.normal.textColor = Color.black;
        styles.hover.textColor = Color.black;
        styles.active.textColor = Color.black;

        return styles;
    }

    public static GUIStyle CardBtnRemove(float width) {
        GUIStyle styles = new GUIStyle(GUI.skin.button);
        styles.fixedWidth = width / 3;
        styles.margin = new RectOffset((int)(width / 3), 0, 10, 3);

        return styles;
    }
    #endregion

    public static GUIStyle Column(int left) {
        GUIStyle styles = new GUIStyle();
        styles.margin = new RectOffset(left, 0, 20, 20);

        return styles;
    }

    public static GUIStyle Row(int padding) {
        GUIStyle styles = new GUIStyle();
        styles.normal.background = Colors.GetColorTexture(1);
        styles.margin.left = padding;

        return styles;
    }


}
