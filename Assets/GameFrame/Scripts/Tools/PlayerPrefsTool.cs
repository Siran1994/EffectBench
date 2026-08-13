using UnityEngine;

public static class PlayerPrefsTool 
{
    public static void SetString(string valueName, string s)
    {
        PlayerPrefs.SetString(valueName, s);
    }
    public static string GetString(string valueName)
    {
        return PlayerPrefs.GetString(valueName);
    }
    public static void SetFloat(string valueName, float i)
    {
        PlayerPrefs.SetFloat(valueName, i);
    }
    public static float GetFloat(string valueName)
    {
        return PlayerPrefs.GetFloat(valueName);
    }
    public static void SetInt(string valueName, int i)
    {
        PlayerPrefs.SetInt(valueName, i);
    }
    public static int GetInt(string valueName)
    {
        return PlayerPrefs.GetInt(valueName);
    }
    public static void SetBool(string valueName, bool b)
    {
        SetInt(valueName, b ? 1 : 0);
    }
    public static bool GetBool(string valueName)
    {
        return GetInt(valueName) == 0 ? false : true;
    }
    public static void SetVector2(string valueName, Vector2 v3)
    {
        SetFloat(valueName + "_x", v3.x);
        SetFloat(valueName + "_y", v3.y);
    }
    public static Vector2 GetVector2(string valueName)
    {
        return new Vector2(
            GetFloat(valueName + "_x"),
            GetFloat(valueName + "_y")
        );
    }
    public static void SetVector3(string valueName, Vector3 v3)
    {
        SetFloat(valueName + "_x", v3.x);
        SetFloat(valueName + "_y", v3.y);
        SetFloat(valueName + "_z", v3.z);
    }
    public static Vector3 GetVector3(string valueName)
    {
        return new Vector3(
            GetFloat(valueName + "_x"),
            GetFloat(valueName + "_y"),
            GetFloat(valueName + "_z")
        );
    }
    public static void SetQuaternion(string valueName, Quaternion q4)
    {
        SetFloat(valueName + "_x", q4.x);
        SetFloat(valueName + "_y", q4.y);
        SetFloat(valueName + "_z", q4.z);
        SetFloat(valueName + "_w", q4.w);
    }
    public static Quaternion GetQuaternion(string valueName)
    {
        return new Quaternion(
            GetFloat(valueName + "_x"),
            GetFloat(valueName + "_y"),
            GetFloat(valueName + "_z"),
            GetFloat(valueName + "_w")
        );
    }
    public static void Save()
    {
        PlayerPrefs.Save();
    }
    public static void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }
    public static void DeleteKey(string n)
    {
        PlayerPrefs.DeleteKey(n);
    }
}
