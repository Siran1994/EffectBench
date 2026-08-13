
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
[CreateAssetMenu(menuName = "Create ItemConfig", fileName = "ItemConfig")]
public class ItemConfig : ScriptableObject
{
    private static ItemConfig instance;
    public static ItemConfig Instance
    {
        get
        {
            if (instance == null)
            {
                ItemConfig ItemConfig = (ItemConfig)Resources.Load("ItemConfig");
                instance = ItemConfig;
            }
            return instance;
        }
    }
    [Header("关卡表")]
    public TextAsset LvTable;

    [Header("垃圾分类配置表")]
    public GarbageInfo[] garbageInfos;

    public GarbageInfo GetGarbageInfo(GarbageType garbageType)
    {
        return garbageInfos[(int)garbageType];
    }

    public ItmeInfo GetItmeInfo(ItemType itemType)
    {
        ItmeInfo itmeInfo = new ItmeInfo();
        for (int i = 0; i < garbageInfos.Length; i++)
        {
            for (int j = 0; j < garbageInfos[i].itemList.Length; j++)
            {
                if (garbageInfos[i].itemList[j].ItemType == itemType)
                {
                    itmeInfo = garbageInfos[i].itemList[j];
                }
            }
        }
        return itmeInfo;
    }

    public TrashCanInfo GetTrashCanInfoByLv(List<int> lvInfo, List<int> itemList)
    {
        TrashCanInfo trashCanInfo = new TrashCanInfo();

        if (lvInfo != null)
        {
            List<int> mergedList = new List<int>();
            ItemType type = ItemType.None;
            if (itemList.Count > 0)
            {
                mergedList.AddRange(lvInfo);
                mergedList.AddRange(itemList);
                type = (ItemType)mergedList[Random.Range(0, mergedList.Count - 1)];
            }
            else
            {
                type = (ItemType)lvInfo[Random.Range(0, lvInfo.Count - 1)];
            }

            for (int i = 0; i < garbageInfos.Length; i++)
            {
                for (int j = 0; j < garbageInfos[i].itemList.Length; j++)
                {
                    if (garbageInfos[i].itemList[j].ItemType == type)
                    {
                        trashCanInfo.garbageType = garbageInfos[i].garbageType;
                        trashCanInfo.itmeInfo = garbageInfos[i].itemList[j];
                    }
                }
            }
        }
        return trashCanInfo;
    }

    public GameObject ItemFactory(ItemType itemType)
    {
        return Instantiate(GetItmeInfo(itemType).itemGo);
    }
    public int[] GetLvData(int lv)
    {
        if (lv <= 99)
        {
            string[] AllLv = LvTable.text.Split('\n');
            var datas = AllLv[lv - 1].Split(',');
            int[] intArray = datas.Select(c => int.Parse(c.ToString())).ToArray();//string[]=>int[]
            return RandomReGroup(intArray);
        }
        return null;
    }

    public int[] ReGroup(int[] inputArray)//顺序重组
    {
        int[] newArray = new int[inputArray.Length * 3];
        for (int i = 0; i < inputArray.Length; i++)
        {
            newArray[i * 3] = inputArray[i];
            newArray[i * 3 + 1] = inputArray[i];
            newArray[i * 3 + 2] = inputArray[i];
        }
        return newArray;
    }

    public int[] RandomReGroup(int[] inputArray)//随机重组
    {
        List<int> result = new List<int>();

        foreach (int num in inputArray)
        {
            result.Add(num);
            result.Add(num);
            result.Add(num);
        }
        System.Random rng = new System.Random();
        int n = result.Count;

        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            int value = result[k];
            result[k] = result[n];
            result[n] = value;
        }
        return result.ToArray();
    }
}

[System.Serializable]
public struct GarbageInfo
{
    public GarbageType garbageType;
    [ShowAssetPreview(64, 64)]
    public Material skin;
    public ItmeInfo[] itemList;
}

[System.Serializable]
public struct ItmeInfo
{
    public ItemType ItemType;
    [ShowAssetPreview(64, 64)]
    public Sprite itemIcon;
    [ShowAssetPreview(64, 64)]
    public GameObject itemGo;
    public float scale;
    public Vector3 ros;
    public string audioName;
}

[System.Serializable]
public struct TrashCanInfo
{
    public GarbageType garbageType;
    public ItmeInfo itmeInfo;
}
