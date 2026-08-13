using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*****************************************
	 文件:   ResourcesManager.cs
	 作者:   Siran
	 日期:   2020/12/22 16:25:54
	 功能:   资源加载类 
 *****************************************/
public class ResourcesManager
{
    static Dictionary<string, GameObject> ResDict = new Dictionary<string, GameObject>();
    public static GameObject Load(string path)
    {
        if (ResDict.ContainsKey(path))  //从字典中存在就直接从字典中加载
        {
            return ResDict[path];
        }

        //不存在就从资源目录中加载,并加载到字典中
        GameObject go = Resources.Load(path) as GameObject;
        ResDict[path] = go;
        return go;
    }

    //非泛型容器
    static Hashtable resTable = new Hashtable();

    public static T Load<T>(string path) where T : UnityEngine.Object //泛型约束
    {
        if (resTable.ContainsKey(path))
        {
            return resTable[path] as T;
        }

        T t = Resources.Load<T>(path);
        resTable[path] = t;
        return t;
    }

}
