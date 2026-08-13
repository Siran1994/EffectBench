using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using System.Reflection;
using UnityEngine.Profiling;
using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
/*****************************************
文件:   EditorTools.cs
作者:   Siran
日期:   2020/12/24 15:12:57
功能:   编辑器拓展工具合集
*****************************************/
public class EditorTools:Editor
{
    [InitializeOnLoadMethod]
    static void InitializeOnLoadMethod()//Unity防关闭
    {
        EditorApplication.wantsToQuit -= Quit;
        EditorApplication.wantsToQuit += Quit;
    }
    static bool Quit()
    {
        var res = EditorUtility.DisplayDialog("Unity正在关闭中...", "确定吗?", "是", "否");
        if (res)
        {
            EditorPrefs.DeleteKey("FirstLoad");
            EditorPrefs.DeleteKey("CanLoad");//EditorPrefs中保存Unity的重要数据,比如SDK,NDK,JDK的路径
        }
        return res; //return true表示可以关闭unity编辑器
    }   

    [MenuItem("Window/便签工具 #n", false, 2)]
    private static void OpenNote()
    {
        ProjectNote.ShowWindow();
    }

    #region GameObject Tools  

    #region 一键生成预制体
    [MenuItem("GameObject/ObjTools/创建预制体", false, 1)]
    public static void Generate()
    {
        GameObject[] objectArray = Selection.gameObjects;

        foreach (GameObject gameObject in objectArray)
        {
            string localPath = "Assets/Resources/" + gameObject.name + ".prefab";

            string dir = "Assets/Resources/";
            if (!Directory.Exists(dir)) //创建存放资源的AssetBundles文件夹
            {
                Directory.CreateDirectory(dir);
            }
            PrefabUtility.SaveAsPrefabAssetAndConnect(gameObject, localPath, InteractionMode.AutomatedAction);
        }
    }

    [MenuItem("GameObject/ObjTools/子节点数量", false, 2)]
    private static void CalcChildCount()
    {
        if (Selection.activeGameObject != null)
            Debug.Log("子节点个数 : " + (Selection.activeGameObject.GetComponentsInChildren<Transform>().Length - 1));
    }
    #endregion

    [MenuItem("GameObject/ObjTools/关闭阴影", false, 3)]
    private static void RendererNoShadows()
    {
        Renderer[] _render = Selection.activeGameObject.transform.GetComponentsInChildren<Renderer>(true);

        for (int i = 0, icount = _render.Length; i < icount; i++)
        {
            _render[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            _render[i].receiveShadows = false;
        }
    }

    [MenuItem("GameObject/ObjTools/关闭射线", false, 4)]
    private static void CancleRaycastTarget()
    {
        UnityEngine.UI.Graphic[] raycast = GameObject.FindObjectsOfType<UnityEngine.UI.Graphic>();
        for (int i = 0, count = raycast.Length; i < count; i++)
        {
            raycast[i].raycastTarget = false;
        }
    }

    [MenuItem("GameObject/ObjTools/创建碰撞体", false, 7)]
    static void CreatParentBoxCollider()
    {
        Transform parent = Selection.activeGameObject.transform;
        Vector3 postion = parent.position;
        Quaternion rotation = parent.rotation;
        Vector3 scale = parent.localScale;
        parent.position = Vector3.zero;
        parent.rotation = Quaternion.Euler(Vector3.zero);
        parent.localScale = Vector3.one;

        Collider[] colliders = parent.GetComponentsInChildren<Collider>();
        foreach (Collider child in colliders)
        {
            DestroyImmediate(child);
        }
        Vector3 center = Vector3.zero;
        Renderer[] renders = parent.GetComponentsInChildren<Renderer>();
        foreach (Renderer child in renders)
        {
            center += child.bounds.center;
        }
        center /= parent.GetComponentsInChildren<Transform>().Length;
        Bounds bounds = new Bounds(center, Vector3.zero);
        foreach (Renderer child in renders)
        {
            bounds.Encapsulate(child.bounds);
        }
        BoxCollider boxCollider = parent.gameObject.AddComponent<BoxCollider>();
        boxCollider.center = bounds.center - parent.position;
        boxCollider.size = bounds.size;

        parent.position = postion;
        parent.rotation = rotation;
        parent.localScale = scale;
    }
    #endregion

    #region Developer Helper
    #region Data
    [MenuItem("开发者工具/数据清除", false, 1)]
    static void ClearPlayerPrefs()
    {
        if (System.Text.RegularExpressions.Regex.IsMatch(Application.companyName, @"[\u4e00-\u9fa5]"))
        {
            UnityEngine.Debug.LogError("公司名中包含中文，不能删除！");
            return;
        }
        else
        {
            PlayerPrefs.DeleteAll();
            EditorPrefs.DeleteKey("CanLoad");
            Debug.Log("Clear Success!");
        }
    }
    [MenuItem("开发者工具/打开缓存", false, 2)]
    private static void OpenDirectory()
    {
        //Process.Start(Application.persistentDataPath + "/");
        Application.OpenURL(Application.persistentDataPath + "/");
    }
    [MenuItem("开发者工具/清理缓存", false, 3)]
    private static void ClearData()
    {
        bool flag = EditorUtility.DisplayDialog("Develope Helper", "是否清空数据缓存目录？\n\n请确认已经将缓存数据替换为永久数据了。\n\n一旦清空无法恢复。", "确定", "取消");
        if (flag)
        {
            DirectoryInfo di = new DirectoryInfo(Application.persistentDataPath);
            di.Delete(true);
        }
    }
    [MenuItem("开发者工具/缓存转永久", false, 4)]
    private static void ReplaceFile()
    {
        string sourcePath = Application.persistentDataPath + "/";
        string targetPath = Directory.GetCurrentDirectory() + "/Assets/Resources/Data/";
        FileInfo[] fileName = new DirectoryInfo(sourcePath).GetFiles();
        for (int i = 0, count = fileName.Length; i < count; i++)
        {
            File.Copy(sourcePath + fileName[i].Name, targetPath + fileName[i].Name, true);
        }
        EditorUtility.DisplayDialog("UnityTools", "用缓存数据替换永久数据，替换成功。\n\n共替换了" + fileName.Length + "个文件。\n\n请手动属性Project视图！", "确定");
    }
    #endregion

    [MenuItem("开发者工具/竖屏 #p")]
    static void SetPSize()
    {
        int index = 18;
        typeof(Editor)
            .Assembly
            .GetType("UnityEditor.GameView")
            .GetProperty("selectedSizeIndex", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .SetValue(EditorWindow.GetWindow(typeof(Editor).Assembly.GetType("UnityEditor.GameView")), index, null);
    }
    [MenuItem("开发者工具/横屏 #l")]
    static void SetLSize()
    {
        int index = 19;
        typeof(Editor)
            .Assembly
            .GetType("UnityEditor.GameView")
            .GetProperty("selectedSizeIndex", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .SetValue(EditorWindow.GetWindow(typeof(Editor).Assembly.GetType("UnityEditor.GameView")), index, null);
    }

    [MenuItem("开发者工具/清空日志 #c", false, 1001)]
    public static void ClearLog()
    {
        var assembly = Assembly.GetAssembly(typeof(ActiveEditorTracker));
        var type = assembly.GetType("UnityEditorInternal.LogEntries");
        if (type == null)
        {
            type = assembly.GetType("UnityEditor.LogEntries");
        }
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }   

    [MenuItem("开发者工具/删除Miss C#", false, 101)]
    static void CleanupMissingScript()
    {
        GameObject[] pAllObjects = (GameObject[])Resources.FindObjectsOfTypeAll(typeof(GameObject));

#if UNITY_2019 || UNITY_2020 || UNITY_2021
        for (int i = 0; i < pAllObjects.Length; i++)
        {
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(pAllObjects[i]);
        }
#endif
        int r;
        int j;
        for (int i = 0; i < pAllObjects.Length; i++)
        {
            if (pAllObjects[i].hideFlags == HideFlags.None)//HideFlags.None 获取Hierarchy面板所有Object
            {
                var components = pAllObjects[i].GetComponents<Component>();
                var serializedObject = new SerializedObject(pAllObjects[i]);
                var prop = serializedObject.FindProperty("m_Component");
                r = 0;

                for (j = 0; j < components.Length; j++)
                {
                    if (components[j] == null)
                    {
                        prop.DeleteArrayElementAtIndex(j - r);
                        r++;
                    }
                }

                serializedObject.ApplyModifiedProperties();
            }
        }
        Debug.Log("无效脚本清理成功!");
    }
    [MenuItem("开发者工具/创建默认文件夹", false, 201)]
    static void CreateFolders()
    {
        string[] folderNames = { "Animations", "Audio", "Fonts", "Plugins", "Textures", "Materials", "Resources", "Scenes", "Scripts", "Shaders", "Prefabs" };
        string path = Application.dataPath + "/";
        for (int i = 0; i < folderNames.Length; i++)
        {
            if (!Directory.Exists(path + folderNames[i]))
            {
                Directory.CreateDirectory(path + folderNames[i]);
            }
        }
        AssetDatabase.Refresh();
    }
    [MenuItem("开发者工具/空引用查找/当前场景", false, 301)]
    public static void FindMissingReferencesInCurrentScene()
    {
        var sceneObjects = GetSceneObjects();
        FindMissingReferences(EditorSceneManager.GetActiveScene().name, sceneObjects);
    }
    [MenuItem("开发者工具/空引用查找/所有场景", false, 301)]
    public static void MissingSpritesInAllScenes()
    {
        foreach (var scene in EditorBuildSettings.scenes.Where(s => s.enabled))
        {
            EditorSceneManager.OpenScene(scene.path);
            FindMissingReferencesInCurrentScene();
        }
    }
    [MenuItem("开发者工具/空引用查找/资源", false, 301)]
    public static void MissingSpritesInAssets()
    {
        var allAssets = AssetDatabase.GetAllAssetPaths().Where(path => path.StartsWith("Assets/")).ToArray();
        var objs = allAssets.Select(a => AssetDatabase.LoadAssetAtPath(a, typeof(GameObject)) as GameObject).Where(a => a != null).ToArray();

        FindMissingReferences("Project", objs);
    }
    [MenuItem("开发者工具/Shader/替换Default Shader")]
    public static void ResetShader()
    {
        var matGuids = AssetDatabase.FindAssets("t:Material", new string[] { "Assets" });
        for (var idx = 0; idx < matGuids.Length; ++idx)
        {
            var guid = matGuids[idx];
            EditorUtility.DisplayProgressBar(string.Format("批处理中...{0}/{1}", idx + 1, matGuids.Length), "替换shader", (idx + 1.0f) / matGuids.Length);
            var mat = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid));
            mat.shader = Shader.Find(mat.shader.name);
            new SerializedObject(mat).ApplyModifiedProperties();
        }

        AssetDatabase.SaveAssets();
        EditorUtility.ClearProgressBar();

        Debug.Log("replace all system shader is done!");
    }

    #region 空引用查找
    private static void FindMissingReferences(string context, GameObject[] objects)
    {
        foreach (var go in objects)
        {
            var components = go.GetComponents<Component>();

            foreach (var c in components)
            {
                if (!c)
                {
                    Debug.LogError("场景: " + EditorSceneManager.GetActiveScene().name + " " + "物体: " + GetFullPath(go) + " 有组件缺失", go);
                    continue;
                }

                SerializedObject so = new SerializedObject(c);
                var sp = so.GetIterator();

                while (sp.NextVisible(true))
                {
                    if (sp.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        if (sp.objectReferenceValue == null
                            && sp.objectReferenceInstanceIDValue != 0)
                        {
                            ShowError(context, go, c.GetType().Name, ObjectNames.NicifyVariableName(sp.name));
                        }
                    }
                }
            }
        }
    }
    private static GameObject[] GetSceneObjects()
    {
        return Resources.FindObjectsOfTypeAll<GameObject>()
            .Where(go => string.IsNullOrEmpty(AssetDatabase.GetAssetPath(go))
                   && go.hideFlags == HideFlags.None).ToArray();
    }
    private static void ShowError(string context, GameObject go, string componentName, string propertyName)
    {
        var ERROR_TEMPLATE = "Missing Ref in: [{3}]{0}. Component: {1}, Property: {2}";

        Debug.LogError(string.Format(ERROR_TEMPLATE, GetFullPath(go), componentName, propertyName, context), go);
    }
    private static string GetFullPath(GameObject go)
    {
        return go.transform.parent == null
            ? go.name
                : GetFullPath(go.transform.parent.gameObject) + "/" + go.name;
    }
    #endregion 
    
    #region 屏幕截图
    [MenuItem("开发者工具/截屏", false, 801)]
    static void ScreenShot()
    {
        string resolution = "" + Screen.width + "X" + Screen.height;
        string Path = Application.dataPath + "/";
        ScreenCapture.CaptureScreenshot(Path + resolution + "-" + PlayerPrefs.GetInt("number", 0) + ".png", 1);
        PlayerPrefs.SetInt("number", PlayerPrefs.GetInt("number", 0) + 1);
        AssetDatabase.Refresh();
    }
    #endregion

    #region 重启项目
    [MenuItem("开发者工具/重启Unity", false, 901)]
    static void RestartUnity()
    {
        EditorApplication.OpenProject(Application.dataPath.Replace("Assets", string.Empty));
    }
    #endregion    

    #region 合并网格
    [MenuItem("开发者工具/合并网格", false, 154)]
    private static void CombineMeshes()
    {
        if (Selection.activeGameObject.transform.childCount <= 0)
        {
            UnityEngine.Debug.LogError("该节点下的子节点为 0，合并失败");
            return;
        }

        MeshFilter[] meshFilters = Selection.activeGameObject.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        MeshRenderer[] meshRenderer = Selection.activeGameObject.GetComponentsInChildren<MeshRenderer>();  //获取自身和所有子物体中所有MeshRenderer组件
        Material[] mats = new Material[meshRenderer.Length];                    //新建材质球数组

        for (int i = 0; i < meshFilters.Length; i++)
        {

            mats[i] = meshRenderer[i].sharedMaterial;                           //获取材质球列表

            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }

        MeshFilter filterTransform = Selection.activeGameObject.GetComponent<MeshFilter>();
        if (filterTransform == null)
            Selection.activeGameObject.gameObject.AddComponent<MeshFilter>();
        MeshRenderer renderTransform = Selection.activeGameObject.GetComponent<MeshRenderer>();
        if (renderTransform == null)
            Selection.activeGameObject.gameObject.AddComponent<MeshRenderer>();

        filterTransform.sharedMesh = new Mesh();
        filterTransform.sharedMesh.CombineMeshes(combine, false);//为mesh.CombineMeshes添加一个 false 参数，表示并不是合并为一个网格，而是一个子网格列表

        renderTransform.sharedMaterials = mats;          //为合并后的GameObject指定材质

        filterTransform.gameObject.SetActive(true);
    }
    #endregion
    #endregion

    #region Assets
    #region 一键导包
    [MenuItem("Assets/自动导包", false, 0)]
    static void ExportPackage()
    {
        //  var path = EditorUtility.SaveFilePanel("Save unitypackage", "", "", "unitypackage");
        //  if (path == "")
        //    return;//文件保存面板

        var assetPathNames = new string[Selection.objects.Length];
        for (var i = 0; i < assetPathNames.Length; i++)
        {
            assetPathNames[i] = AssetDatabase.GetAssetPath(Selection.objects[i]);
        }

        assetPathNames = AssetDatabase.GetDependencies(assetPathNames);
        //if (Selection.activeObject.name== "DeveloperTools")
        //    PackageManager.DeleteFile(@"Assets/DeveloperTools/Editor/TempAssets/TmpImg.png");

        AssetDatabase.ExportPackage(assetPathNames,
            BuildSetting.GetOutPath(PlatformType.UnityPackage) + "/" + Selection.activeObject.name + ".unitypackage",
            ExportPackageOptions.Interactive
            | ExportPackageOptions.Recurse
            | ExportPackageOptions.IncludeDependencies);
        Application.OpenURL(BuildSetting.GetOutPath(PlatformType.UnityPackage));
    }
    #endregion

    #region 查看资源内存占用
    [MenuItem("Assets/内存检查", false, 1)]
    public static void CheckMemory()
    {
        Texture target = Selection.activeObject as Texture;

        var type = System.Reflection.Assembly.Load("UnityEditor.dll").GetType("UnityEditor.TextureUtil");
        MethodInfo methodInfo = type.GetMethod("GetStorageMemorySize", BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);

        Debug.LogError("内存占用：" + EditorUtility.FormatBytes(Profiler.GetRuntimeMemorySizeLong(Selection.activeObject)));
        Debug.LogError("硬盘占用：" + EditorUtility.FormatBytes((int)methodInfo.Invoke(null, new object[] { target })));
    }
    #endregion

    #region 特殊文件创建
    [MenuItem("Assets/Create/Files/xLua File", false, 1)]
    static void CreateXLuaFile()
    {
        CreateFile("lua.txt", "newxLua");
    }
    [MenuItem("Assets/Create/Files/Lua File", false, 2)]
    static void CreateLuaFile()
    {
        CreateFile("lua", "newlua");
    }
    [MenuItem("Assets/Create/Files/Text File", false, 3)]
    static void CreateTextFile()
    {
        CreateFile("txt", "newtxt");
    }
    [MenuItem("Assets/Create/Files/Ini Config File", false, 4)]
    static void CreateIniFile()
    {
        CreateFile("ini", "newconfig");
    }
    [MenuItem("Assets/Create/Files/Xml File", false, 5)]
    static void CreateXmlFile()
    {
        CreateFile("xml", "newxml", "<xml></xml>");
    }
    [MenuItem("Assets/Create/Files/Json File", false, 1005)]
    static void CreateJsonFile()
    {
        CreateFile("json", "newjson", "{" + "xx" + ":" + "xx" + "}");
    }
    static void CreateFile(string fileEx, string fileName = "newfile", string fileContain = "-- test")
    {
        var path = Application.dataPath + "/";
        var newFileName = fileName + "." + fileEx;
        var fullPath = path + newFileName;

        //如果是空白文件，编码并没有设成UTF-8
        File.WriteAllText(fullPath, fileContain, Encoding.UTF8);

        AssetDatabase.Refresh();
        //选中新创建的文件
        var asset = AssetDatabase.LoadAssetAtPath(fullPath, typeof(UnityEngine.Object));
        Selection.activeObject = asset;
    }
    #endregion

    #endregion   
}

#region 获取游戏对象路径
public class GetHierarchyPath : Editor
{
    static Transform selectedItem;
    static List<string> pathElements = new List<string>();
    static Transform nextParent;
    static TextEditor path;
    static string tmpPath;

    [MenuItem("GameObject/ObjTools/复制当前路径", false, 5)]
    public static void NewMenuOptions()
    {
        selectedItem = Selection.activeTransform;
        if (selectedItem != null)
        {
            path = new TextEditor();
            tmpPath = "";
            pathElements.Clear();
            nextParent = selectedItem;
            while (true)
            {
                pathElements.Add(nextParent.name);
                if (nextParent.parent != null)
                    nextParent = nextParent.parent;
                else
                    break;
            }
            for (int i = pathElements.Count - 1; i >= 0; i--)
            {
                tmpPath += pathElements[i];
                tmpPath += "/";
            }
            tmpPath = tmpPath.Remove(tmpPath.Length - 1);
            path.text = tmpPath;
            path.SelectAll();
            path.Copy();
            UnityEngine.Debug.Log("<color=#00ff00ff>当前游戏对象路径为:</color> " + "<color=#00ffffff><size=15>" + path.text + "</size></color>");
        }
        else
        {
            EditorUtility.DisplayDialog("错误!", "当前操作,需要选中一个游戏对象!", "确定");
        }
    }
}
#endregion

#region 子对象重命名
public class RenameChildren : EditorWindow
{
    public string baseName = "";
    static Transform[] parents;
    bool addIndexAtEnd;
    static RenameChildren window;
    string newName;
    public static void ShowWindow()
    {
        window = (RenameChildren)EditorWindow.GetWindow(typeof(RenameChildren), false, "子对象重命名", true);
        window.minSize = new Vector2(400, 200);
        window.titleContent = new GUIContent("子对象重命名");
        window.Show();
    }
    [MenuItem("GameObject/ObjTools/子对象重命名", false, 6)]
    public static void NewMenuOptions()
    {
        parents = Selection.transforms;
        if (parents != null && parents.Length > 0)
        {
            ShowWindow();
        }
        else
        {
            EditorUtility.DisplayDialog("错误!", "当前操作,需要选中一个游戏对象!", "确定");
        }
    }
    void OnGUI()
    {
        EditorGUILayout.Space();
        addIndexAtEnd = EditorGUILayout.ToggleLeft(new GUIContent("添加索引", "子索引将被附加到名称"), addIndexAtEnd);

        EditorGUILayout.Space();
        baseName = EditorGUILayout.TextField(" 名字: ", baseName);

        if (GUILayout.Button("重命名") || (Event.current != null && Event.current.keyCode == KeyCode.Return))
        {
            if (parents != null && parents.Length > 0)
            {
                RenameAllChildren(parents);
            }
            else
            {
                Debug.LogError("Double check failed");
            }
        }
    }
    void RenameAllChildren(Transform[] prnt)
    {
        for (int i = 0; i < prnt.Length; i++)
        {
            foreach (Transform item in prnt[i])
            {
                if (baseName == "")
                    baseName = "GameObject";

                if (addIndexAtEnd)
                {
                    newName = baseName + "_" + item.transform.GetSiblingIndex();
                }
                else
                {
                    newName = baseName;
                }
                item.name = newName;
                EditorUtility.SetDirty(item.gameObject);
            }
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        window.Close();
    }
    void OnLostFocus()
    {
        window.Close();
    }
    void OnDestroy()
    {
        parents = null;
    }
}
#endregion

#region 工程便签
public class ProjectNote : EditorWindow
{
    private Vector2 scrollPos;
    private string contentText = "";
    private DateTime lastWriteTime = DateTime.Now;
    private string fileName;

    public static void ShowWindow()
    {
        EditorWindow window = EditorWindow.GetWindow<ProjectNote>();
        window.position = new Rect(100, 100, 500, 400);
        window.titleContent = new GUIContent("工程便签");
        window.Show();
    }

    void Awake()
    {
        fileName = Application.dataPath + "/DeveloperTools/Editor/TempAssets/Note.txt";
        if (!File.Exists(fileName))
        {
            File.CreateText(fileName);
        }

        FileInfo info = new FileInfo(fileName);
        lastWriteTime = info.LastWriteTime;


        using (StreamReader sr = info.OpenText())
        {
            contentText = sr.ReadToEnd();
        }
    }

    void OnGUI()
    {
        GUILayout.Space(10);
        Label("工程便签", Color.yellow, 20, TextAnchor.MiddleCenter);
        GUILayout.Space(2);
        Label("上次修改时间：" + lastWriteTime.ToLocalTime(), Color.green, 12, TextAnchor.MiddleCenter);
        GUILayout.Space(6);

        scrollPos = GUILayout.BeginScrollView(scrollPos, false, false);
        {
            contentText = GUILayout.TextArea(contentText, GUILayout.MinHeight(350), GUILayout.MinWidth(450));
        }
        GUILayout.EndScrollView();

        GUILayout.Space(2);
        Label("Tips：支持 Ctrl+S、Ctrl+V、Ctrl+C 快捷键操作 \n关闭窗口自动保存", Color.white, 10, TextAnchor.MiddleCenter);
        GUILayout.Space(2);

        GUILayout.BeginHorizontal();

        //if (GUILayout.Button("粘贴"))
        //{
        //    contentText += GUIUtility.systemCopyBuffer;
        //}

        //if (GUILayout.Button("保存"))
        //{
        //    Save();
        //}

        GUILayout.EndHorizontal();

        SetCurrent();
    }

    void OnDestroy()
    {
        Save();
    }

    void Save()
    {
        using (FileStream fs = new FileStream(fileName, FileMode.Truncate))
        {
            using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
            {
                sw.Write(contentText);
            }
        }
    }

    /// <summary> 设置快捷操作 </summary>
    void SetCurrent()
    {
        Event e = Event.current;
        if (e.isKey)
        {
            bool EventDown = (e.modifiers & EventModifiers.Control) != 0;
            switch (e.keyCode)
            {
                case KeyCode.S:
                    if (EventDown)
                    {
                        e.Use();
                        Save();
                    }
                    break;
                //case KeyCode.A:
                //    if (EventDown)
                //    {
                //        e.Use();
                //        TextEditor te = new TextEditor();
                //        te.text = contentText;
                //        te.OnFocus();
                //    }
                //    break;
                case KeyCode.C:
                    if (EventDown)
                    {
                        e.Use();
                        TextEditor te = new TextEditor();
                        te.text = contentText;
                        te.OnFocus();
                        te.Copy();
                    }
                    break;
                case KeyCode.V:
                    if (EventDown)
                    {
                        e.Use();
                        contentText += GUIUtility.systemCopyBuffer; // 粘贴
                        GUIUtility.systemCopyBuffer = null; // 粘贴后清空粘贴板 
                    }
                    break;
            }
        }
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
}
#endregion

#region SVN
namespace SVN
{
    public class UnitySVN
    {
        [MenuItem("SVN/提交项目", false, 1)]
        static void GitCommitProject()
        {
            RunCmd("TortoiseProc.exe", string.Format("/command:commit /path:\"{0}\\{1}\" /closeonend:0", System.Environment.CurrentDirectory, "Assets"));
        }

        [MenuItem("SVN/同步项目", false, 2)]
        private static void SvnUpdateAssets()
        {
            RunCmd("TortoiseProc.exe", string.Format("/command:update /path:\"{0}\\{1}\" /closeonend:0", System.Environment.CurrentDirectory, "Assets"));
        }

        #region 提交指定文件

        [MenuItem("SVN/提交指定文件", false, 3)]
        private static bool CheckCommit()
        {
            if (Selection.activeObject == null)
                return false;
            else
                return true;
        }

        [MenuItem("SVN/提交指定文件夹", false, 4)]
        private static void SvnCommitThisFile()
        {
            RunCmd("TortoiseProc.exe", string.Format("/command:commit /path:\"{0}\\{1}\" /closeonend:0", System.Environment.CurrentDirectory, AssetDatabase.GetAssetPath(Selection.activeObject)));
        }

        #endregion

        #region 更新指定文件

        [MenuItem("SVN/更新指定文件", false, 5)]
        private static bool CheckUpdate()
        {
            if (Selection.activeObject == null)
                return false;
            else
                return true;
        }

        [MenuItem("SVN/更新指定文件夹", false, 6)]
        private static void SvnUpdateThisFile()
        {
            RunCmd("TortoiseProc.exe", string.Format("/command:update /path:\"{0}\\{1}\" /closeonend:0", System.Environment.CurrentDirectory, AssetDatabase.GetAssetPath(Selection.activeObject)));
        }

        #endregion
        /// <summary>
        /// 运行外部程序 
        /// </summary>
        /// <param name="cmdExe">指定应用程序的完整路径，如果该程序在系统环境变量中，只需要填写对用的程序名称就可以</param>
        /// <param name="cmdStr">执行命令行参数</param>
        private static bool RunCmd(string cmdExe, string cmdStr, bool iswait = true)
        {
            bool result = false;
            try
            {
                using (Process myPro = new Process())
                {
                    //指定启动进程是调用的应用程序和命令行参数
                    ProcessStartInfo psi = new ProcessStartInfo(cmdExe, cmdStr);
                    myPro.StartInfo = psi;
                    myPro.Start();
                    // 是否加上这句话，看个人需求。如果加上的话，我们必须关掉弹出的SVN窗口才能继续操作。如果不加上，则可以弹出SVN，也可以继续修改unity项目。个人建议加上比较好
                    if (iswait)
                        myPro.WaitForExit();
                    result = true;
                }
            }
            catch
            {
            }
            return result;
        }
    }
}
#endregion

#region Git
namespace Git
{
    using System.Linq;
    using System.Text;
    using System.Diagnostics;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;

    public class UnityGitWindow : EditorWindow
    {
        private readonly string[] tabTitles = { "Log", "Status", "Commit" };

        private List<LogItem> parsedGitLog = new List<LogItem>();

        private List<StatusItem> parsedStatusLog = new List<StatusItem>();

        private string commitMessage = string.Empty;

        private Vector2 scrollPosition;

        private TreeViewState gitLogTreeViewState;

        private int currentTab;

        private MultiColumnHeaderState multiColumnHeaderState;

        [MenuItem("Git/Git #g")]
        public static UnityGitWindow GetWindow()
        {
            var window = GetWindow<UnityGitWindow>();
            window.titleContent = new GUIContent("Git");
            window.minSize = new Vector2(350, 630);
            window.maxSize = new Vector2(800, 680);
            window.Focus();
            window.Repaint();
            return window;
        }

        public void OnGUI()
        {
            currentTab = GUILayout.Toolbar(currentTab, tabTitles, GUILayout.Height(25));

            switch (currentTab)
            {
                case 0:
                    DisplayGitLog();
                    break;
                case 1:
                    DisplayGitStatus();
                    break;
                case 2:
                    DisplayCommit();
                    break;
            }
        }

        /// <summary>
        /// Call the parse to initialize everything.
        /// </summary>
        public void Awake()
        {
            ParseGitLog();
            ParseGitStatus();
        }

        #region Display Methods
        /// <summary>
        /// Display the git log
        /// </summary>
        public void DisplayGitLog()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            EditorGUILayout.BeginVertical();

            GUIStyle style = CreateColoredBackground();

            if (parsedGitLog != null && parsedGitLog.Count > 0)
            {
                for (int i = 0; i < parsedGitLog.Count; i++)
                {
                    // alternate the background color for each entry
                    if (i % 2 == 0)
                        EditorGUILayout.BeginVertical();
                    else
                        EditorGUILayout.BeginVertical(style);

                    foreach (string displayString in parsedGitLog[i].parsedLogItem)
                    {
                        DisplayLabel(displayString);
                    }
                    EditorGUILayout.EndVertical();
                }
            }
            else
            {
                DisplayLabel("There is not a repository!");
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        public void DisplayGitStatus()
        {            // display all of the installed packages
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            EditorGUILayout.BeginVertical();

            GUIStyle style = CreateColoredBackground();

            if (parsedStatusLog != null && parsedStatusLog.Count > 0)
            {
                for (int i = 0; i < parsedStatusLog.Count; i++)
                {
                    // alternate the background color for each package
                    if (i % 2 == 0)
                        EditorGUILayout.BeginVertical();
                    else
                        EditorGUILayout.BeginVertical(style);

                    EditorGUILayout.BeginHorizontal();
                    {
                        DisplayLabel(parsedStatusLog[i].status);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.EndVertical();
                }
            }
            else
            {
                DisplayLabel("There are no files modified!");
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();

            DisplayStatusHeader();
        }

        /// <summary>
        /// Display the head for the status tab
        /// </summary>
        public void DisplayStatusHeader()
        {
            EditorGUILayout.BeginVertical(GUILayout.Height(50));
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(80);
                if (GUILayout.Button("Add All", GUILayout.Width(200), GUILayout.Height(25)))
                {
                    CallProcess("git", "add -A");
                    ParseGitStatus();
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Display the commit tab
        /// </summary>
        public void DisplayCommit()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            //EditorStyles.label.fontStyle = FontStyle.Bold;
            //EditorStyles.label.fontSize = 12;            
            commitMessage = EditorGUILayout.TextField("Message: ", commitMessage, GUILayout.Height(20));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            if (GUILayout.Button("Commit"))
            {
                CallProcess("git", "commit -m" + "\"" + commitMessage + "\"");
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Push"))
            {
                CallProcess("git", "push");
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Pull"))
            {
                CallProcess("git", "pull origin master");
            }
            EditorGUILayout.EndVertical();

            Space(80);
            EditorGUILayout.BeginVertical(GUILayout.Height(50));
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(80);
                if (GUILayout.Button("Close Window", GUILayout.Width(200), GUILayout.Height(25)))
                {
                    GetWindow<UnityGitWindow>().Close();
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Generic method to display a label
        /// </summary>
        public void DisplayLabel(string label)
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.green;
                // EditorStyles.label.fontStyle = FontStyle.Bold;
                // EditorStyles.label.fontSize = 12;
                EditorGUILayout.LabelField(string.Format("{0}", label), style, GUILayout.Height(15));
            }
            EditorGUILayout.EndHorizontal();
        }
        #endregion

        static void Space(int index)
        {
            for (int i = 0; i < index; i++)
            {
                EditorGUILayout.Space();
            }
        }
        /// <summary>
        /// Creates the alternating background color based upon if the Unity Editor is the free (light) skin or the Pro (dark) skin.
        /// </summary>
        /// <returns>The GUI style with the appropriate background color set.</returns>
        private GUIStyle CreateColoredBackground()
        {
            GUIStyle style = new GUIStyle();
            if (Application.HasProLicense())
            {
                style.normal.background = MakeTex(20, 20, new Color(0.3f, 0.3f, 0.3f));
            }
            else
            {
                style.normal.background = MakeTex(20, 20, new Color(0.6f, 0.6f, 0.6f));
            }

            return style;
        }

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }

        #region Parsing methods
        /// <summary>
        /// Call git and get the output from git log, split it to be easier to display.//"%h%an%ad%s"//"%h%x09%an%x09%ad%x09%s"//"%h%x09%an%x09%ae%08%ad%x09%s"
        /// </summary>
        public void ParseGitLog()
        {
            string gitLogOutput = CallProcess("git", "log --pretty=format:" + "\"" + "%s%x09%h%x09%an%x09%ae%x09%ad" + "\"" + "--date=short");

            gitLogOutput = gitLogOutput.Replace("--date=short", "");
            List<string> parsedOutput = gitLogOutput.Split(System.Environment.NewLine.ToCharArray()).ToList();         

            parsedGitLog.Clear();

            foreach (string parsed in parsedOutput)
            {
                List<string> splitParsed = parsed.Split('\t').ToList();
                // Log format:
                // hash username date comment
                LogItem item = new LogItem();

                item.parsedLogItem = splitParsed;

                parsedGitLog.Add(item);
            }
        }


        /// <summary>
        /// Call git and get the output from git status
        /// </summary>
        public void ParseGitStatus()
        {
            string gitOutput = CallProcess("git", "status --short");

            // Split on newlines
            List<string> splitOutput = gitOutput.Split(System.Environment.NewLine.ToCharArray()).ToList();

            // Prune empty strings
            splitOutput = splitOutput.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();

            parsedStatusLog.Clear();

            foreach (string stringOutput in splitOutput)
            {
                StatusItem item = new StatusItem();
                item.status = stringOutput;
                item.add = false;

                parsedStatusLog.Add(item);
            }
        }
        #endregion

        /// <summary>
        /// Generic method to call git.
        /// </summary>
        /// <returns>The output from git.</returns>
        /// <param name="fileName"></param>
        /// <param name="arguments"></param>
        public string CallProcess(string fileName, string arguments)
        {
            Process gitProcess = new Process();
            gitProcess.StartInfo.FileName = fileName;
            gitProcess.StartInfo.Arguments = arguments;
            gitProcess.StartInfo.UseShellExecute = false;
            gitProcess.StartInfo.RedirectStandardOutput = true;
            gitProcess.StartInfo.RedirectStandardError = true;
            gitProcess.StartInfo.CreateNoWindow = true;
            gitProcess.StartInfo.StandardOutputEncoding = Encoding.GetEncoding("utf-8");

            gitProcess.Start();

            string gitStatusOutput = gitProcess.StandardOutput.ReadToEnd();
            gitProcess.WaitForExit();

            return gitStatusOutput;
        }
    }
    public class LogItem
    {
        public List<string> parsedLogItem;
    }
    public class StatusItem
    {
        public string status;
        public bool add;
    }
}
#endregion