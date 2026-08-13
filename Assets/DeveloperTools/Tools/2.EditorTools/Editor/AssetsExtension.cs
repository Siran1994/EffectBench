using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
/*****************************************
文件:   AssetsExtension.cs
作者:   Siran
日期:   2021/7/1 11:15:10
功能:   资源Inspector面板拓展
*****************************************/
[HelpURL("https://github.com/Siran1994")]

[CustomEditor(typeof(UnityEditor.SceneAsset))]
public class SceneInspector : Editor
{
    public override void OnInspectorGUI()
    {
        string path = AssetDatabase.GetAssetPath(target);
        GUI.enabled = true;       
        if (path.EndsWith(".unity"))
        {
            if (GUILayout.Button("Add To BuildList"))
            {
                List<EditorBuildSettingsScene> scenes = EditorBuildSettings.scenes.ToList();
                bool hasExist = false;
                foreach (EditorBuildSettingsScene scene in scenes) {
                    if (scene.path == path) {
                        hasExist = true;
                        break;
                    }
                }
                if (!hasExist)
                {
                    scenes.Add(new EditorBuildSettingsScene(path, true));
                    EditorBuildSettings.scenes = scenes.ToArray();                    
                }
            }
        }
    }    
}
[CustomEditor(typeof(UnityEditor.DefaultAsset))]
public class CustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        string path = AssetDatabase.GetAssetPath(target);
        GUI.enabled = true;       
        if (path.EndsWith(".txt"))
        {
            if (GUILayout.Button("Open File"))
            {
                AssetDatabase.OpenAsset(target);                              
            }            
        }
        else if (path.EndsWith(".abc"))
        {
            GUILayout.Button("abc");
        }       
        else if (path.EndsWith(""))//文件夹要放到最后，因为所有的结尾均为空
        {            
            if (GUILayout.Button("Open Folder"))
            {
                string tmp= path.Replace('/','\\');
                Application.OpenURL(tmp + '\\');
            }                    
        }
    }
}
