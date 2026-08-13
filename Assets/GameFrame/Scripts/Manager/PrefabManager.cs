using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PrefabManager
{
    public static Dictionary<string, GameObject> effectMap = new Dictionary<string, GameObject>();
    public static Dictionary<string, GameObject> uiMap = new Dictionary<string, GameObject>();

    public static string Effect = "Effect";
    public static string Ui = "Ui";

    public static void loadPrefab(string name, string path, UnityAction cb = null)
    {
        var assets = Resources.LoadAll(path, typeof(GameObject));
        foreach (var t in assets)
        {
            switch (name)
            {
                case "Effect":
                    set(t.name, t as GameObject, effectMap);
                    break;
                case "Ui":
                    set(t.name, t as GameObject, uiMap);
                    break;
            }
        }
        cb?.Invoke();
    }

    public static void set(string key, GameObject value, Dictionary<string, GameObject> targetMap)
    {
        if (targetMap.ContainsKey(key))
            Debug.LogWarning("存入失败,资源已存在!");
        else
            targetMap.Add(key, value);
    }

    public static GameObject get(string key, Dictionary<string, GameObject> targetMap)
    {
        if (targetMap.ContainsKey(key))
            return targetMap[key];
        else
        {
            Debug.LogWarning("取出失败,资源不存在!");
            return null;
        }
    }
    public static void releaseAsset(string key, Dictionary<string, GameObject> targetMap)
    {
        if (targetMap.ContainsKey(key))
        {
            var asset = targetMap[key];
            targetMap.Remove(key);
            Resources.UnloadAsset(asset);
            Debug.Log("release asset with " + key);
        }
    }
    public static void releaseAllAsset()
    {
        effectMap.Clear();
        uiMap.Clear();
    }
}
