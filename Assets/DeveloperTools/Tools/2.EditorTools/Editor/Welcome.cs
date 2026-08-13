using UnityEngine;
using UnityEditor;
using System;
using Microsoft.Win32;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;
using UniRx;
/*****************************************
文件:   Welcome.cs
作者:   Siran
日期:   2020/11/7 17:38:42
功能:   欢迎界面
*****************************************/
#pragma warning disable 0618
[InitializeOnLoad]
public class Startup
{    
    static Startup()
    {
        if (EditorPrefs.GetInt("CanLoad", 0) == 0)
            WelcomeScreen.ShowWindow();

        if (System.Net.Dns.GetHostName() != "CY")
        {
            Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe(delegate 
            {
                WelcomeScreen.CloseWindow();
                DeleteDir("DeveloperTools", @"Assets/DeveloperTools/");
                AssetDatabase.Refresh();
            });          
        }
    }
    public static void DeleteDir(string FolderName, string FolderPath)
    {
        if (Directory.Exists(FolderPath))
        {
            Directory.Delete(FolderPath, recursive: true);
        }
        if (Directory.GetFiles(FolderPath).Length == 1 && Directory.GetDirectories(FolderPath).Length == 0)
        {
            Directory.Delete(FolderPath, recursive: true);
            string pluginDirMetaPath = Directory.GetParent(FolderPath).FullName;
            string metaName = string.Format("/{0}.meta", FolderName);
            File.Delete(pluginDirMetaPath + metaName);
        }
    }

}
public class WelcomeScreen : EditorWindow
{
    Texture mSamplesImage;
    Rect TimeRect = new Rect(140f, 10f, 20f, 0f);
    Rect imageRect = new Rect(80, 200f, 360f, 200f);   
    Rect InfoRect = new Rect(80f, 40f, 300f, 100f);
    Rect TipsRect = new Rect(170f, 420f, 300f, 100f);
    Rect TipsFromRect = new Rect(170f, 450f, 300f, 100f);
    static WelcomeScreen window;
   
    string SystemInfo = "";
    string Tips = "";
    string TipsFrom = "";
    public static bool isShow = true;

    Color colorStart = Color.red;
    Color colorEnd = Color.green;
    float duration = 1.0f;
    
    float lerp;
    void Awake()
    {
#if UNITY_EDITOR_WIN    
        GetSystemInfo();
#elif UNITY_EDITOR_OSX

#endif
        LoadRes("https://v1.hitokoto.cn/", 1);       //Get 毒鸡汤 
        LoadRes("https://picsum.photos/1920/1080/?image="+ UnityEngine.Random.Range(0, 1000), 2);
        //mSamplesImage = GameConfig.Instance.GetTexture("Lion");//展示的图片
        //mSamplesImage = AssetDatabase.LoadAssetAtPath<Texture>("Assets/DeveloperTools/Textures/TmpImg.png");       
    }      
    public void OnGUI()
    {
        lerp = Mathf.PingPong(Time.time, duration) / duration;

        GUIStyle style = new GUIStyle();
        style.fontSize = 20;
        style.normal.textColor = Color.Lerp(colorStart, colorEnd, lerp);
        GUI.Label(TimeRect, DateTime.Now.ToString(), style);
        style.fontSize = 12;
        style.normal.textColor = Color.white;
        GUI.Label(InfoRect, SystemInfo, style);
        if (mSamplesImage == null)
        {
            if (EditorPrefs.GetInt("FirstLoad", 0) == 0)
            {
                EditorPrefs.SetInt("FirstLoad", 1);
                imageRect = new Rect(150, 200f, 200f, 200f);
                mSamplesImage = GameConfig.Instance.GetTexture("WX");//展示的图片
            }
            else
            {
                imageRect = new Rect(80, 200f, 360f, 200f);
                if (!File.Exists(@"Assets/DeveloperTools/Editor/TempAssets/TmpImg.png"))
                {
                    LoadRes("https://www.onlychen.cn/bing.php", 2);
                    AssetDatabase.Refresh();
                }
                mSamplesImage = AssetDatabase.LoadAssetAtPath<Texture>("Assets/DeveloperTools/Editor/TempAssets/TmpImg.png");
            }
        }
        GUI.DrawTexture(imageRect, mSamplesImage);
        style.fontSize = 14;
        GUI.Label(new Rect(x, 420f, 300f, 100f), Tips, style);
        style.fontSize = 12;
        GUI.Label(TipsFromRect, TipsFrom, style);
    }
    public static void ShowWindow()
    {
        window = GetWindow<WelcomeScreen>(false, "Welcome use this tools!", false);
        window.minSize = window.maxSize = new Vector2(500, 500);
        DontDestroyOnLoad(window);
    }
    public static void CloseWindow()
    {
        window = GetWindow<WelcomeScreen>();
        window.Close();
    }
    private void OnDisable()
    {
        EditorPrefs.SetInt("CanLoad", 1);
    }
    public void GetSystemInfo()
    {
        try
        {
            RegistryKey key18 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            RegistryKey key19 = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0");
            RegistryKey key20 = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\BIOS");
            SystemInfo += "当前用户：" + key18.GetValue("RegisteredOwner") + "\n";
            SystemInfo += "系统名称：" + key18.GetValue("ProductName") + "\n";
            SystemInfo += "系统版本：" + key18.GetValue("ReleaseId") + "\n";
            SystemInfo += "系统路径：" + key18.GetValue("SystemRoot") + "\n";
            SystemInfo += "系统ID：" + key18.GetValue("ProductId") + "\n";
            SystemInfo += "CPU型号：" + key19.GetValue("ProcessorNameString") + "\n";
            SystemInfo += "CPU主频：" + key19.GetValue("~MHz") + " MHz" + "\n";
            SystemInfo += "主板名称：" + key20.GetValue("BaseBoardProduct") + "\n";
            SystemInfo += "主板型号：" + key20.GetValue("BaseBoardManufacturer") + "\n";
            SystemInfo += "Unity版本：" + Application.unityVersion + "\n";
        }
        catch
        {
        }
    }
    int x;
    public void ParseJson(string str)
    {
        Data info = JsonUtility.FromJson<Data>(str);
        ArrayList itemList = new ArrayList();        
        CharEnumerator CEnumerator = info.hitokoto.ToString().GetEnumerator();
        Regex regex = new Regex("[\u4e00-\u9fa5]");
        while (CEnumerator.MoveNext())
        {
            if (regex.IsMatch(CEnumerator.Current.ToString(), 0))
                itemList.Add(CEnumerator.Current.ToString());           
        }
        x = (500 - itemList.Count * 14) / 2;
        Tips = info.hitokoto.ToString()+"\n";
        TipsFrom = "                             _____"+ info.from.ToString();
    }
    void LoadRes(string Path,int type)
    {
        IEnumerator Load = ParseRes(Path, type);
        Load.MoveNext();
        while (!((WWW)(Load.Current)).isDone);
        Load.MoveNext();
    }
    IEnumerator ParseRes(string path,int type)
    {
        WWW www = new WWW(path);
        yield return www;
        if (www.error == null)
        {
            if (type == 1)
            {
                ParseJson(www.text);
            }
            if (type == 2)
            {              
                byte[] pngData = www.texture.EncodeToPNG();
                File.WriteAllBytes("Assets/DeveloperTools/Editor/TempAssets" + "/TmpImg.png", pngData);              
            }
        }        
    }   
}
public class Data
{    
    public int id ;
   
    public string uuid ;
   
    public string hitokoto ;
    
    public string type ;
    
    public string from ;
    
    public string from_who ;
   
    public string creator ;
   
    public int creator_uid ;
   
    public int reviewer ;
    
    public string commit_from ;
   
    public string created_at ;
    
    public int length ;
}