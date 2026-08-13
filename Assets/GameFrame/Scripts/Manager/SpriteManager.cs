
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SpriteManager : MonoBehaviour
{
    public static Dictionary<string, Image> iconMap = new Dictionary<string, Image>();

    public static string Icon = "Icon";


    public static void loadPrefab(string name, string path, UnityAction cb = null)
    {
        var assets = Resources.LoadAll<Image>(path);
        foreach (var t in assets)
        {
            switch (name)
            {
                case "Icon":
                    set(t.name, t, iconMap);
                    break;
            }
        }
        cb?.Invoke();
    }

    public static void set(string key, Image value, Dictionary<string, Image> targetMap)
    {
        if (targetMap.ContainsKey(key))
            Debug.LogWarning("存入失败,资源已存在!");
        else
            targetMap.Add(key, value);
    }

    public static Image get(string key, Dictionary<string, Image> targetMap)
    {
        if (targetMap.ContainsKey(key))
            return targetMap[key];
        else
        {
            Debug.LogWarning("取出失败,资源不存在!");
            return null;
        }
    }
    public static void releaseAsset(string key, Dictionary<string, Image> targetMap)
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
        iconMap.Clear();
    }
}
