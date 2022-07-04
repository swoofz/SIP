using UnityEngine;

public class Colors {
    // The Colors used in this Window in normal RGB values
    private static Vector3[] palette = new Vector3[] {
        new Vector3(17,17,17),      // Near Black       - 0
        new Vector3(51,51,51),      // Darkest Grey     - 1
        new Vector3(85,85,85),      // Dark grey        - 2
        new Vector3(119,119,119),   // Grey             - 3
        new Vector3(153,153,153),   // Light Grey       - 4
    };

    public static Color GetColor(int index) {
        Vector3 color = palette[index] / 255f;
        return new Color(color.x, color.y, color.z);
    }

    public static Color ReturnColor(Vector3 color) {
        color = color / 255f;
        return new Color(color.x, color.y, color.z);
    }

    public static Texture2D GetColorTexture(int index) {
        Color color = GetColor(index);
        Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        texture.SetPixel(0, 0, color);
        return texture;
    }
}
