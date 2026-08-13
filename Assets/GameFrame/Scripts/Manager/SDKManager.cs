using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

#pragma warning disable 0618, 0649, 0414
/*****************************************
	 文件:   SDKManager.cs
	 作者:   Siran
	 日期:   2021/6/9 16:25:54
	 功能:   广告接入管理类
 *****************************************/
[HelpURL("https://github.com/Siran1994/MyTools")]
[DisallowMultipleComponent]
[RequireComponent(typeof(ColorObj))]
[RequireComponent(typeof(BatteryManager))]
[RequireComponent(typeof(LevelMgr))]
[DefaultExecutionOrder(-999)]
public class SDKManager : MonoSigleton<SDKManager>
{
    #region Unity方法
    protected override void Awake()
    {
        base.Awake();
        if (GameConfig.Instance.isRelease)
        {
            Debug.unityLogger.logEnabled = false;
            if (GameObject.Find("IngameDebugConsole"))
                Destroy(GameObject.Find("IngameDebugConsole"));
        }
        gameObject.name = "SDKManager";       
    }
    void Start() //广告初始化
    {
        if (GameConfig.Instance.isRelease == false)
            return;   
        //TODO
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }
    #endregion

    #region 公共方法
    public void ShowAd(UnityAction cb)
    {
        cb?.Invoke();
    }   
    #endregion
}

#if UNITY_EDITOR
public class SDKManagerEditor
{
    #region 创建高亮对象
    static GameObject go;
    [MenuItem("GameObject/ObjTools/SDKManager", false, 0)]
    static void Init()
    {
        if (!File.Exists("Assets/DeveloperTools/Textures/Icon.png") && !File.Exists("Assets/Icon.png"))
            EditorUtility.DisplayDialog("Error!!!", "Can`t find the Icon", "Confirm");
        else
        {
            if (!File.Exists("Assets/DeveloperTools/Icon/Icon.jpg"))
            {
                ColorObj.Icon = (Texture)AssetDatabase.LoadAssetAtPath("Assets/DeveloperTools/Textures/Icon.png", typeof(Texture));
            }
            else
            {
                ColorObj.Icon = (Texture)AssetDatabase.LoadAssetAtPath("Assets/DeveloperTools/Textures/Icon.jpg", typeof(Texture));
            }
        }
        if (GameObject.Find("SDKManager"))
        {
            UnityEngine.Object.DestroyImmediate(go);
            go = new GameObject("SDKManager");
            go.AddComponent<ColorObj>();
            go.AddComponent<SDKManager>();
        }
        else
        {
            go = new GameObject("SDKManager");
            go.AddComponent<ColorObj>();
            go.AddComponent<SDKManager>();
        }
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }
    #endregion
}
#region 对象高亮显示
[InitializeOnLoad]
public class ColorObjEditor
{
    static ColorObjEditor()
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
            GUI.Label(labelRect, GameConfig.Instance.AppName + ":(" + GameConfig.Instance.ChannelName + ") " + GameConfig.Instance.VersionNum);

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
#endregion
#endif