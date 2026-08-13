using UnityEngine;

public class GameData
{
    public static int Lv
    {
        get { return PlayerPrefs.GetInt("lv", 1); }
        set { PlayerPrefs.SetInt("lv", value); PlayerPrefs.Save(); }
    }
    public static int Coin
    {
        get { return PlayerPrefs.GetInt("Coin", 0); }
        set { PlayerPrefs.SetInt("Coin", value); PlayerPrefs.Save(); }
    }

    public static int SoundOn
    {
        get { return PlayerPrefs.GetInt("SoundOn", 0); }
        set { PlayerPrefs.SetInt("SoundOn", value); PlayerPrefs.Save(); }
    }

    public static int MusicOn
    {
        get { return PlayerPrefs.GetInt("MusicOn", 0); }
        set { PlayerPrefs.SetInt("MusicOn", value); PlayerPrefs.Save(); }
    }

    public static int GuideStep
    {
        get { return PlayerPrefs.GetInt("GuideStep", 3); }
        set { PlayerPrefs.SetInt("GuideStep", value); PlayerPrefs.Save(); }
    }

}