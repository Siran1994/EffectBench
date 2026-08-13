using System;
using PP = UnityEngine.PlayerPrefs;
/*****************************************
	 文件:   PlayerPrefsTime.cs
	 作者:   Siran
	 日期:   2021/7/1 14:34:39
	 功能:   时间数据拓展
 *****************************************/
public class PlayerPrefsTime
{    
    public static void SetDateTime(string name)
    {
        PP.SetString(name, DateTime.Now.ToShortTimeString());
    }
   
    public static void SetDateTime(string name, DateTime dt)
    {
        PP.SetString(name, dt.ToShortTimeString());
    }

    public static DateTime GetDateTime(string name, DateTime dt = default)
    {
        if (!PP.HasKey(name))
            return dt;
        return DateTime.Parse(PP.GetString(name));
    }


}
