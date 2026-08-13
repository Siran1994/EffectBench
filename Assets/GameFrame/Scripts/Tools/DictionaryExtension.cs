using System.Collections.Generic;
/*****************************************
	 文件:   DictionaryExtension.cs
	 作者:   Siran
	 日期:   2020/12/22 16:25:54
	 功能:   对Dic的拓展
 *****************************************/
public static class DictionaryExtension
{

    /// <summary>
    /// 尝试根据key得到value，得到了的话直接返回value，没有得到直接返回null
    /// this Dictionary<Tkey,Tvalue> dict 这个字典表示我们要获取值的字典
    /// </summary>
    public static Tvalue TryGet<Tkey, Tvalue>(this Dictionary<Tkey, Tvalue> dict, Tkey key)
    {
        Tvalue value;
        dict.TryGetValue(key, out value);
        return value;
    }

}