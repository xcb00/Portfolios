#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
public static class CustomEditorUtility
{
    public static GUIStyle DefineUnityGUIStyle(Vector2Int padding, Vector2Int border, NodeNumber number, NodeStyle style = NodeStyle.none, int fontSize = 10)
    {
        GUIStyle result = new GUIStyle();
        string name = string.Concat(number, style == NodeStyle.none ? string.Empty : $" {style}");
        result.normal.background = EditorGUIUtility.Load(name) as Texture2D;
        result.normal.textColor = Color.white;
        result.padding = new RectOffset(padding.x, padding.x, padding.y, padding.y);
        result.border = new RectOffset(border.x, border.x, border.y, border.y);
        result.fontSize = fontSize;
        return result;
    }

    public static GUIStyle DefineCustomGUIStyle(Vector2Int padding, Vector2Int border, string texturePath)
    {
        GUIStyle result = new GUIStyle();
        result.normal.background = Resources.Load<Texture2D>(texturePath);
        result.normal.textColor = Color.white;
        result.padding = new RectOffset(padding.x, padding.x, padding.y, padding.y);
        result.border = new RectOffset(border.x, border.x, border.y, border.y);
        return result;
    }

    public static GUIStyle DefineRectangleGUIStyle(Color color, float alpha)
    {
        GUIStyle result = new GUIStyle();
        Texture2D texture = new Texture2D(1, 1);
        Color _color = color;
        _color.a = alpha;
        texture.SetPixel(0, 0, color);
        texture.Apply();
        result.normal.background = texture;
        return result;
    }

    public static GUIStyle DefineRectangleGUIStyle(Color color)
    {
        GUIStyle result = new GUIStyle();
        Texture2D texture = new Texture2D(1, 1);
        Color _color = color;
        texture.SetPixel(0, 0, color);
        texture.Apply();
        result.normal.background = texture;
        return result;
    }
}
#endif