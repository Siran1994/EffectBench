using System;
using System.IO;
using UnityEditor;
using UnityEngine;
/*****************************************
	 文件:   ImportModels.cs
	 作者:   Siran
	 日期:   2021/7/5 15:47:11
	 功能:   模型自动导入
 *****************************************/
[HelpURL("https://github.com/Siran1994")]
public class ImportModels
{
    static FileInfo[] files;
    [MenuItem("模型工具/模型导入")]
    public static void CopyFile()
    {
        //string orginPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/Test";
        string orginPath = "Z:/【项目素材】/" + GameConfig.Instance.AppName /*+ "/3D模型"*/;

        if (Directory.Exists(orginPath))
        {
            DirectoryInfo direction = new DirectoryInfo(orginPath);
            files = direction.GetFiles("*.FBX", SearchOption.AllDirectories);
        }

        for (int i = 0; i < files.Length; i++)
        {
            string fileName = files[i].Name;
            string folderName = Application.dataPath + "/Models/" /*+ Path.GetFileNameWithoutExtension(fileName)*/;
            string targetPath = folderName + "/" + fileName;//目标文件夹

            if (!Directory.Exists(targetPath)) Directory.CreateDirectory(folderName);
            files[i].CopyTo(targetPath);
        }
    }

    [MenuItem("模型工具/模型设置")]
    public static void SetModelImporter()
    {
        string orginPath = Application.dataPath + "/Models";
        DirectoryInfo direction = new DirectoryInfo(orginPath);
        FileInfo[] files = direction.GetFiles("*.FBX", SearchOption.AllDirectories);
        if (files.Length == 0) return;

        for (int i = 0; i < files.Length; i++)
        {
            string[] pathNames = (files[i].FullName).Split(new string[] { "Assets" }, StringSplitOptions.RemoveEmptyEntries);
            string namePath = "Assets" + pathNames[1];
            ModelImporter modelImporter = AssetImporter.GetAtPath(namePath) as ModelImporter;
            modelImporter.generateSecondaryUV = true;
            modelImporter.SaveAndReimport();
        }
    }

    [MenuItem("模型工具/模型转预制体")]
    public static void CreatePrefab()
    {
        string orginPath = Application.dataPath + "/Models";
        DirectoryInfo direction = new DirectoryInfo(orginPath);
        FileInfo[] files = direction.GetFiles("*.FBX", SearchOption.AllDirectories);
        if (files.Length == 0) return;
        for (int i = 0; i < files.Length; i++)
        {
            string[] pathNames = (files[i].FullName).Split(new string[] { "Assets" }, StringSplitOptions.RemoveEmptyEntries);
            string namePath = "Assets" + pathNames[1];
            string fileName = Path.GetFileNameWithoutExtension(files[i].Name);
            GameObject gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(namePath);

            string localPath = @"Assets/Resources/" + gameObject.name + ".prefab";
            string dir = @"Assets/Resources/";

            if (!Directory.Exists(dir)) //创建存放资源的AssetBundles文件夹
                Directory.CreateDirectory(dir);

            var InstanceObj = PrefabUtility.InstantiatePrefab(gameObject);

            PrefabUtility.SaveAsPrefabAsset((GameObject)InstanceObj, localPath);
            UnityEngine.Object.DestroyImmediate(InstanceObj);
        }
    }
}
