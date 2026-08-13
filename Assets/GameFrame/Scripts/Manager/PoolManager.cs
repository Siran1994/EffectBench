using System.Collections.Generic;
using UnityEngine;
public class PoolManager
{
    private static Dictionary<string, Stack<GameObject>> dictPool = new Dictionary<string, Stack<GameObject>>();
    private static Dictionary<string, GameObject> dictPrefab = new Dictionary<string, GameObject>();
    public static void PrePool(GameObject prefab, Transform par, int nodeNum)
    {
        string name = prefab.name;
        Stack<GameObject> pool = new Stack<GameObject>();
        dictPool[name] = pool;

        for (int i = 0; i < nodeNum; i++)
        {
            GameObject node = GameObject.Instantiate(prefab);
            node.name = name;
            node.transform.parent = par;
            pool.Push(node);
            node.SetActive(false);
        }
    }

    public static GameObject GetNode(GameObject prefab, Transform parent)
    {
        string name = prefab.name;
        dictPrefab[name] = prefab;
        GameObject node = null;

        if (dictPool.ContainsKey(name))
        {
            Stack<GameObject> pool = dictPool[name];
            if (pool.Count > 0)
            {
                node = pool.Pop();
                node.SetActive(true);
            }
            else
                node = GameObject.Instantiate(prefab);
        }
        else
        {
            Stack<GameObject> pool = new Stack<GameObject>();
            dictPool[name] = pool;
            node = GameObject.Instantiate(prefab);
        }
        node.name = name;
        node.transform.parent = parent;
        return node;
    }

    public static GameObject GetNode(string name)
    {
        GameObject node = null;
        if (dictPool.ContainsKey(name))
        {
            Stack<GameObject> pool = dictPool[name];
            if (pool.Count > 0)
            {
                node = pool.Pop();
                node.SetActive(true);
            }
        }
        return node;
    }

    public static void PutNode(GameObject node)
    {
        string name = node.name;
        if (dictPool.ContainsKey(name))
        {
            Stack<GameObject> pool = dictPool[name];
            pool.Push(node);
            node.SetActive(false);
        }
        else
        {
            Stack<GameObject> pool = new Stack<GameObject>();
            pool.Push(node);
            dictPool[name] = pool;
        }
    }

    public static GameObject GetNodeInfo(string name)
    {
        if (dictPool.ContainsKey(name) && dictPrefab.ContainsKey(name))
        {
            return dictPrefab[name];
        }
        return null;
    }

    public static void PutNodeByName(string name)
    {
        GameObject node = GameObject.Find(name);
        if (node != null)
        {
            PutNode(node);
        }
    }

    public static void ClearPool(string name)
    {
        if (dictPool.ContainsKey(name))
        {
            dictPool[name].Clear();
        }
    }

    public static void Clear()
    {
        // PoolManager.ClearPool("Coin");
    }
}
