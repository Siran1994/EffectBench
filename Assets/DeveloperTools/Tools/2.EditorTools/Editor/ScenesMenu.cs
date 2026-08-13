using UnityEngine;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
/*****************************************
	 文件:   SceneMeunBuild.cs
	 作者:   Siran
	 日期:   2021/7/1 14:34:39
	 功能:   场景管理工具
 *****************************************/
[HelpURL("https://github.com/Siran1994")]
public static class ScenesMenu
{    
    static readonly string ScenesMenuPath = "DeveloperTools/Editor/TempAssets/SceneList.cs";
    static int index = 0;
    static string[] SceneList = null;
   [MenuItem("场景列表/更新场景列表")]
    public static void UpdateList()
    {
        string scenesMenuPath = Path.Combine(Application.dataPath, ScenesMenuPath);
        var stringBuilder = new StringBuilder();       
        stringBuilder.AppendLine("using UnityEditor;");       
        stringBuilder.AppendLine("public static class SceneList");
        stringBuilder.AppendLine("{");

        SceneList = BuildSetting.GetBuildScenes();
        //Build List
        for (int i = 0; i < SceneList.Length; i++)
        {
            string sceneFilename = SceneList[i]/*.Split('/')[SceneList.Length].Split('.')[0]*/;

            string sceneName = Path.GetFileNameWithoutExtension(SceneList[i]);
            string[] methodNames = sceneName.Split('/');
            string methodName = SceneList[i].Replace('/', '_').Replace('\\', '_').Replace('.', '_').Replace('-', '_');
            stringBuilder.AppendLine(string.Format("    [MenuItem(\"场景列表/{0}\", priority = 10)]", sceneName));
            stringBuilder.AppendLine(string.Format("    public static void {0}() {{ ScenesMenu.OpenScene(\"{1}\"); }}", methodNames[methodNames.Length - 1] + (index++).ToString(), sceneFilename));
        }
        //All Scene
        //foreach (string sceneGuid in AssetDatabase.FindAssets("t:Scene", new string[] { "Assets" }))
        //{
        //    string sceneFilename = AssetDatabase.GUIDToAssetPath(sceneGuid);
        //    string sceneName = Path.GetFileNameWithoutExtension(sceneFilename);
        //    string[] methodNames = sceneName.Split('/');
        //    string methodName = sceneFilename.Replace('/', '_').Replace('\\', '_').Replace('.', '_').Replace('-', '_');
        //    stringBuilder.AppendLine(string.Format("    [MenuItem(\"SceneManager/{0}\", priority = 10)]", sceneName));
        //    stringBuilder.AppendLine(string.Format("    public static void {0}() {{ ScenesMenu.OpenScene(\"{1}\"); }}", methodNames[methodNames.Length - 1] + (index++).ToString(), sceneFilename));
        //}
        stringBuilder.AppendLine("}");       
        Directory.CreateDirectory(Path.GetDirectoryName(scenesMenuPath));
        File.WriteAllText(scenesMenuPath, stringBuilder.ToString());
        AssetDatabase.Refresh();
    }

    public static void OpenScene(string filename)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            EditorSceneManager.OpenScene(filename);
    }
}
