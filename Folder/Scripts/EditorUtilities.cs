// 유니티 에디터에서만 사용하는 클래스와 메소드이므로, data 영역의 메모리 사용을 줄이기 위해 전처리기 사용
# if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

public static class EditorUtilities
{
    #region PrintErrorMessage
    public static void PrintErrorMessage(string errorCode, string errorMessage, int stackIndex = 1)
    {
        System.Diagnostics.StackTrace stack = new System.Diagnostics.StackTrace(true);

        /*bool check = false;
        if (check)
        {
            for (int i = stack.FrameCount - 1; i >= 0; i--)
            {
                System.Diagnostics.StackFrame f = stack.GetFrame(i);
                Debug.Log($"[{i} : {f.GetFileName()}");
            }
        }*/

        System.Diagnostics.StackFrame frame = stack.GetFrame(stackIndex);
        Debug.LogError($"{errorCode} : {System.IO.Path.GetFileName(frame.GetFileName())} : {frame.GetFileLineNumber()}" +
                $"\n{errorMessage}");
    }
    #endregion

    #region Define GUI Style
    public static GUIStyle DefineUnityGUIStyle(Vector2Int padding, Vector2Int border, NodeNumber number, NodeStyle style = NodeStyle.none)
    {
        GUIStyle result = new GUIStyle();
        string name = string.Concat(Enums.ToString(number), style == NodeStyle.none ? string.Empty : $" {Enums.ToString(style)}");
        result.normal.background = EditorGUIUtility.Load(name) as Texture2D;
        result.normal.textColor = Color.white;
        result.padding = new RectOffset(padding.x, padding.x, padding.y, padding.y);
        result.border = new RectOffset(border.x, border.x, border.y, border.y);
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
    #endregion
}
#endif
