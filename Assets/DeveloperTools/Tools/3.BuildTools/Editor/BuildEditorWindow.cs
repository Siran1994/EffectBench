using System.IO;
using UnityEditor;
using UnityEngine;

/*****************************************
	 文件:   BuildEditorWindow.cs
	 作者:   Siran
	 日期:   2021/1/21 18:29:37
	 功能:   buildSystem 面板
 *****************************************/
[HelpURL("https://github.com/Siran1994/SuperToolsManager")]
public class BuildEditorWindow : EditorWindow
{
    [MenuItem("Window/打包工具 #b", false, 1)]
    static void OpenBuildToolsWindows()
    {
        OpenWindow();
    }

    static BuildEditorWindow Instance = null;   
    static string[] PlatformType = new string[] {"None", "Android", "IOS", "Windows" };
    static Texture2D logo;
    public static void OpenWindow()
    {
        if (Instance == null)
        {
            logo = (Texture2D)GameConfig.Instance.GetTexture("Icon");
            Instance = (BuildEditorWindow)EditorWindow.GetWindow(typeof(BuildEditorWindow));
            Instance.titleContent = new GUIContent("BuildWindow", logo, "欢迎使用本工具!,开发者:Siran,QQ:342093031,支付宝:boskbu@gmail.com");
            Instance.minSize = new Vector2(300, 700);
            Instance.maxSize = new Vector2(350, 750);
            BuildSetting.SetPlatform();
        }
        else
            Instance.Close();
    }
    void OnEnable()
    {
        Instance = this;
    }

    void OnDisable()
    {
        Instance = null;
    }

    #region GUI
    Vector2 scrollPos = Vector2.zero;
    int PlatformIndex = 0;
    void OnGUI()
    {
        BuildSetting.SwitchPlatform(PlatformIndex);

        Space();
        //绘制标题
        GUI.color = Color.green;
        GUI.skin.label.fontSize = 24;
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label("----------------------------");
        GUILayout.Label(string.Format("Build Tools({0})", EditorUserBuildSettings.activeBuildTarget));
        GUILayout.Label("----------------------------");
        GUI.skin.label.fontSize = 0;
        GUI.skin.label.alignment = 0;
        GUI.color = Color.white;

        if (EditorPrefs.GetString(GameConfig.Instance.AppName) == "")
            GUILayout.Label("Tips:Project never build!");
        else
            GUILayout.Label("Last Build: " + EditorPrefs.GetString(GameConfig.Instance.AppName));
       
        EditorGUILayout.BeginHorizontal("label", GUILayout.Height(25), GUILayout.Width(120));
        SpaceTab(3);
        EditorGUILayout.LabelField("Switch Platform: ", GUILayout.Width(100));
        PlatformIndex = EditorGUILayout.Popup(PlatformIndex, PlatformType, GUILayout.Width(80), GUILayout.Height(20));       
        EditorGUILayout.EndHorizontal();
        
        DrawHeadCommon();
        Space();
        DrawBuildChannelBtn();       
        Space();      
        DrawReadMe();        
    }

    static string TypeName = null;
    void DrawHeadCommon()
    {
        GUI.color = Color.green;
        GUI.skin.label.fontSize = 16;
        GUI.skin.label.fontStyle = FontStyle.Bold;
        GUILayout.Label("Settings:");
        GUI.skin.label.fontStyle = FontStyle.Normal;
        GUI.skin.label.fontSize = 0;
        GUI.color = Color.white;

        GUILayout.BeginHorizontal();
        SpaceTab();
        GUI.changed = false;
        GameConfig.Instance.isSetImg = EditorGUILayout.Toggle("Y/N FormatImg", GameConfig.Instance.isSetImg);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        SpaceTab();
        GUI.changed = false;
        GameConfig.Instance.isSetScreenLogo = EditorGUILayout.Toggle("Y/N SetScreenLogo", GameConfig.Instance.isSetScreenLogo);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        SpaceTab();
        GUI.changed = false;
        GameConfig.Instance.islockFPS = EditorGUILayout.Toggle("Y/N Lock Frame(60)", GameConfig.Instance.islockFPS);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        SpaceTab();
        GUI.changed = false;
        GameConfig.Instance.isRelease = EditorGUILayout.Toggle("Debug/Release", GameConfig.Instance.isRelease);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        SpaceTab();
        GUI.changed = false;
        GameConfig.Instance.is32 = EditorGUILayout.Toggle("IL2Cpp/Mono", GameConfig.Instance.is32);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        SpaceTab();
        GUI.changed = false;
        GameConfig.Instance.isBuildAab = EditorGUILayout.Toggle("Apk/Aab", GameConfig.Instance.isBuildAab);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        SpaceTab();
        GUI.changed = false;
        GameConfig.Instance.isOnlyBuild = EditorGUILayout.Toggle("Build/Build&Run", GameConfig.Instance.isOnlyBuild);        
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        SpaceTab();
       
        Space();
        GUILayout.EndHorizontal();

        Space(1);

        GUILayout.BeginHorizontal();
        SpaceTab(3);
        if (GUILayout.Button("Project Check"))
        {
            Debug.Log("<color=#ff00ffff>AppName:</color> " + "<color=#00ffffff><size=20>" + GameConfig.Instance.AppName + "</size></color>");
            Debug.Log("<color=yellow>ChannelName:</color> " + "<color=#00ffffff><size=20>" + GameConfig.Instance.ChannelName + "</size></color>");
            Debug.Log("<color=red>VersionNum:</color> " + "<color=#00ffffff><size=20>" + GameConfig.Instance.VersionNum + "</size></color>");
            Debug.Log("<color=#00ff00ff>BundleNum:</color> " + "<color=#00ffffff><size=20>" + GameConfig.Instance.BundleVersionNum + "</size></color>");          
        }       
        SpaceTab(3);
        GUILayout.EndHorizontal();

        Space(2);

        GUILayout.BeginHorizontal();
        SpaceTab(3);
        if (GUILayout.Button("Switch Mode"))
            BuildSetting.BuildOrBuildAndRun();
        SpaceTab(3);
        GUILayout.EndHorizontal();

        Space(2);

        TypeName = GameConfig.Instance.isBuildAab ? "Build Aab" : "Build Android";

        GUILayout.BeginHorizontal();       
        SpaceTab(3);
        if (GUILayout.Button(TypeName))
            BuildSystem.BulidAndroid(GameConfig.Instance.is32);
        SpaceTab(3);
        GUILayout.EndHorizontal();

        Space(2);

        GUILayout.BeginHorizontal();
        SpaceTab(3);
        if (GUILayout.Button("Build Apk&Aab"))
            BuildSystem.BuildApkAndAab();
        SpaceTab(3);
        GUILayout.EndHorizontal();

        Space(2);

        GUILayout.BeginHorizontal();
        SpaceTab(3);
        if (GUILayout.Button("Build IOS"))
            BuildSystem.BulidIOS();
        SpaceTab(3);
        GUILayout.EndHorizontal();

        Space(2);

        GUILayout.BeginHorizontal();
        SpaceTab(3);
        if (GUILayout.Button("Build Windows"))
            BuildSystem.BulidWindows();
        SpaceTab(3);
        GUILayout.EndHorizontal();
    }  

    bool bDrawBuildChannelBtn = false;
    void DrawBuildChannelBtn()
    {
        GUILayout.BeginHorizontal();
        GUI.skin.label.fontSize = 16;
        GUI.color = Color.green;
        GUI.skin.label.fontStyle = FontStyle.Bold;
        // bDrawBuildChannelBtn = EditorGUILayout.Foldout(bDrawBuildChannelBtn, "OutPath:");
        GUILayout.Label("OutPath:");
        GUI.skin.label.fontStyle = FontStyle.Normal;
        GUI.color = Color.white;
        EditorStyles.label.fontSize = 0;
        Space(5);
       
        if (GUILayout.Button("OpenPath"))
            Application.OpenURL(Path.GetFullPath(BuildSetting.GetOutPath(GameConfig.Instance.PT)));
        
        SpaceTab();
        GUILayout.EndHorizontal();


        if (!bDrawBuildChannelBtn)
            return;

        Space();       
        Space(1);
        GUILayout.BeginHorizontal();
        SpaceTab();      

        SpaceTab();
        GUILayout.EndHorizontal();

    }

    void DrawReadMe()
    {
        GUI.color = Color.green;
        GUI.skin.label.fontSize = 16;
        GUI.skin.label.fontStyle = FontStyle.Bold;
        GUILayout.Label("Tips:");
        GUI.skin.label.fontStyle = FontStyle.Normal;
        GUI.skin.label.fontSize = 0;
        GUI.color = Color.white;
        GUI.skin.label.fontSize = 12;
        GUILayout.Label("   1.Please check the 'GameConfig' in Resources"+"\n"+"     folder first When Build");     
        GUI.skin.label.fontSize = 0;

        GUILayout.BeginHorizontal();
        SpaceTab(6);
        if (GUILayout.Button("Open Asset"))
        {
            Selection.activeObject = GameConfig.Instance;
            // Application.OpenURL(@"Assets\DeveloperTools\Resources\");
            // AssetDatabase.FindAssets("GameConfig.asset");
            // EditorUtility.RevealInFinder(@"Assets\DeveloperTools\Resources\GameConfig.asset");         
            // EditorUtility.FindAsset(@"Assets\DeveloperTools\Resources\GameConfig.asset");
        }
        SpaceTab(6);
        GUILayout.EndHorizontal();

        Space(1);      
        if (GUILayout.Button("Close Window"))
            Instance.Close();      
    }
    #region Tools

    void Space(int n = 2)
    {
        for (int i = 0, iMax = n; i < iMax; i++)
        {
            EditorGUILayout.Space();
        }
    }
    void SpaceTab(int tabCount = 1)
    {
        GUILayout.Space(tabCount * 20);
    }

    Rect NormalRect = new Rect(100, 100, 800, 600);
   
    void Label(string text, Color color, int fontsize, TextAnchor alignment, params GUILayoutOption[] options)
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = fontsize;
        style.normal.textColor = color;
        style.alignment = alignment;
        style.wordWrap = true;
        GUILayout.Label(text, style, options);
    }
    
    void Label(string text, params GUILayoutOption[] options)
    {
        Label(text, Color.white, 16, TextAnchor.MiddleLeft, options);
    }
    #endregion
    #endregion
}
