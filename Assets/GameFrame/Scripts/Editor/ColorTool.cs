using UnityEditor;
using UnityEngine;
/*****************************************
	 文件:   ColorTool.cs
	 作者:   Siran
	 日期:   2020/11/16 14:25:11
	 功能:   高亮展示项目核心游戏对象
 *****************************************/

[InitializeOnLoad]
public class ColorToolEditor
{
    static ColorToolEditor()
    {
        EditorApplication.hierarchyWindowItemOnGUI += EvaluateIcons;
    }
    private static void EvaluateIcons(int instanceId, Rect selectionRect)
    {
        GameObject go = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
        if (go != null)
        {
            ColorObj decorator = go.GetComponent<ColorObj>();

            if (decorator != null)
            {
                DrawDecoration(go, decorator, selectionRect);
            }
        }
    }
    private static void DrawDecoration(GameObject obj, ColorObj decorator, Rect rect)
    {
        if (decorator.applyBackgroundColor)
        {
            Texture2D t = new Texture2D(1, 1);
            Color c = decorator.backgroundColor;
            t.SetPixel(1, 1, c);
            t.Apply();
            GUI.DrawTexture(rect, t, ScaleMode.StretchToFill);
        }
        if (decorator.applyCustomTextColor)
        {
            GUI.contentColor = decorator.gameObjectTextColor;

            Rect nameOfObject = new Rect(rect.x + 16, rect.y + 1, rect.width, rect.height);
            GUI.Label(nameOfObject, obj.name);

            GUI.contentColor = Color.white;
        }
        if (decorator.applyDescription)
        {
            GUI.contentColor = decorator.descriptionTextColor;

            Rect labelRect = new Rect(rect.x + 150, rect.y, rect.width - 150, rect.height);
            GUI.Label(labelRect, decorator.description);

            GUI.contentColor = Color.white;
        }
        if (decorator.icon != null)
        {
            Rect r = new Rect(rect.x + rect.width - 35, rect.y, 50, 50);
            GUI.DrawTexture(r, decorator.icon);
        }
        EditorApplication.RepaintHierarchyWindow();
    }
}


