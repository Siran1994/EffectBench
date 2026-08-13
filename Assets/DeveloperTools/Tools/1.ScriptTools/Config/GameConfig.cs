using System;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
/*****************************************
	 文件:   GameConfig.cs
	 作者:   Siran
	 日期:   2021/1/21 16:5:19
	 功能:   项目设置数据类
*****************************************/
public enum PlatformType  //目标平台
{    
    Android,   //安卓
    IOS,       //IOS
    Windows,    //Windows
    Dll,
    UnityPackage
}
[CreateAssetMenu(menuName = "Create Project Settings", fileName = "GameConfig")]
[Serializable]
public class GameConfig : ScriptableObject
{
    private static GameConfig instance;
    public static GameConfig Instance
    {
        get
        {
            if (instance == null)
            {
                GameConfig GameConfig = (GameConfig)Resources.Load("GameConfig");
                instance = GameConfig;
            }
            return instance;
        }
    }   

    [Header("CompanyName")]
    public string CompanyName; //公司名称

    [Header("AppName")]
    public string AppName; //项目名称

    [Header("ChannelName")]
    public string ChannelName; //渠道名称

    [Header("VersionNum")]
    public string VersionNum; //内部版本号PT

    [Header("BundleVersionNum")]
    public int BundleVersionNum; //外部版本号

    [Header("BundleId")]
    public string PackageName;//包名  

    [Header("Y/N FormatImg")]
    public bool isSetImg = true;// 是否处理图片

    [Header("Y/N SetScreenLogo")]
    public bool isSetScreenLogo = false;// 是否设置开机图

    [Header("Y/N Lock Frame(60)")]
    public bool islockFPS = false;// 是否锁定帧率

    [Header("Debug/Release")]
    public bool isRelease = false;// 是否为发布版本

    [Header("IL2Cpp/Mono")]
    public bool is32 = false;// 是否为32位包

    [Header("Apk/Aab")]
    public bool isBuildAab = false;// 包体类型

    [Header("Build/Build&Run")]
    public bool isOnlyBuild = true;// 是否为发布版本   

    [Header("PlatformType")]
    public PlatformType PT = PlatformType.Android; //平台类型  

    [Header("LocalAppPath")]
    public ExePath[] LocalExe; //本地程序地址   

    [Header("SDKPath")]
    public SDKPath[] SDKPath; //SDK地址    

    [Header("ImgAsset")]
    public Texture[] TexAsset; //图片资源
    public Texture GetTexture(string name)
    {
        Texture tmp = null;
        switch (name)
        {
            case "Icon":
                tmp = TexAsset[0];
                break;
            case "Splash":
                tmp = TexAsset[1];
                break;
            case "WX":
                tmp = TexAsset[2];
                break;
            case "Lion":
                tmp = TexAsset[3];
                break;
        }
        return tmp;
    }

    [Header("KeyStoreSetting")]
    public string KeyName = "Key.jks";

    [HideInInspector]
    public string KeyPassWord = "123456";
}

[Serializable]
public struct ExePath//程序安装路径
{
    public string ExeName;
    public string ExeLocalPath;
}

[Serializable]
public struct SDKPath//SDK安装地址
{
    public string SDKName;
    public string SdkPath;
}

#if UNITY_EDITOR
[CustomEditor(typeof(GameConfig))]
public class GameConfigGui : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GameConfig.Instance.KeyPassWord = EditorGUILayout.PasswordField("Key PassWord", GameConfig.Instance.KeyPassWord);
    }

    #region 打开本地应用
    [MenuItem("快速启动/阿里云", false, 0)]
    static void OpenOSS()
    {
        string ApplicationPath = "";
        System.Diagnostics.Process foo = new System.Diagnostics.Process();
#if UNITY_EDITOR    
        ApplicationPath = GameConfig.Instance.LocalExe[0].ExeLocalPath;
#endif
        foo.StartInfo.FileName = ApplicationPath;
        foo.Start();
    }

    [MenuItem("快速启动/腾讯云", false, 1)]
    static void OpenCOS()
    {
        string ApplicationPath = "";
        System.Diagnostics.Process foo = new System.Diagnostics.Process();
#if UNITY_EDITOR    
        ApplicationPath = GameConfig.Instance.LocalExe[1].ExeLocalPath;
#endif
        foo.StartInfo.FileName = ApplicationPath;
        foo.Start();
    }

    [MenuItem("快速启动/华为云", false, 2)]
    static void OpenOBS()
    {
        string ApplicationPath = "";
        System.Diagnostics.Process foo = new System.Diagnostics.Process();
#if UNITY_EDITOR    
        ApplicationPath = GameConfig.Instance.LocalExe[2].ExeLocalPath;
#endif
        foo.StartInfo.FileName = ApplicationPath;
        foo.Start();
    }

    [MenuItem("快速启动/Google浏览器", false, 3)]
    static void OpenGoogleBrowser()
    {
        string ApplicationPath = "";
        System.Diagnostics.Process foo = new System.Diagnostics.Process();
#if UNITY_EDITOR
        ApplicationPath = GameConfig.Instance.LocalExe[3].ExeLocalPath;
#endif
        foo.StartInfo.FileName = ApplicationPath;
        foo.Start();
    }
    [MenuItem("快速启动/有道翻译", false, 4)]
    static void OpenYouDao()
    {
        string ApplicationPath = "";
        System.Diagnostics.Process foo = new System.Diagnostics.Process();
#if UNITY_EDITOR
        ApplicationPath = GameConfig.Instance.LocalExe[4].ExeLocalPath;
#endif
        foo.StartInfo.FileName = ApplicationPath;
        foo.Start();
    }   
    [MenuItem("快速启动/比心云", false, 5)]
    static void OpenSSR()
    {
        string ApplicationPath = "";
        System.Diagnostics.Process foo = new System.Diagnostics.Process();
#if UNITY_EDITOR
        ApplicationPath = GameConfig.Instance.LocalExe[5].ExeLocalPath;
#endif
        foo.StartInfo.FileName = ApplicationPath;
        foo.Start();
    }

    [MenuItem("快速启动/CherryAi", false, 6)]
    static void OpenCherrAi()
    {
        string ApplicationPath = "";
        System.Diagnostics.Process foo = new System.Diagnostics.Process();
#if UNITY_EDITOR
        ApplicationPath = GameConfig.Instance.LocalExe[6].ExeLocalPath;
#endif
        foo.StartInfo.FileName = ApplicationPath;
        foo.Start();
    }

    [MenuItem("快速启动/UnityHub", false, 7)]
    static void OpenUnityHub()
    {
        string ApplicationPath = "";
        System.Diagnostics.Process foo = new System.Diagnostics.Process();
#if UNITY_EDITOR
        ApplicationPath = GameConfig.Instance.LocalExe[7].ExeLocalPath;
#endif
        foo.StartInfo.FileName = ApplicationPath;
        foo.Start();
    }

    [MenuItem("快速启动/GitHub", false, 8)]
    static void OpenGitHub()
    {
        string ApplicationPath = "";
        System.Diagnostics.Process foo = new System.Diagnostics.Process();
#if UNITY_EDITOR
        ApplicationPath = GameConfig.Instance.LocalExe[8].ExeLocalPath;
#endif
        foo.StartInfo.FileName = ApplicationPath;
        foo.Start();
    }


    [MenuItem("快速启动/AndroidStudio", false, 9)]
    static void OpenAS()
    {
        string ApplicationPath = "";
        System.Diagnostics.Process foo = new System.Diagnostics.Process();
#if UNITY_EDITOR
        ApplicationPath = GameConfig.Instance.LocalExe[9].ExeLocalPath;
#endif
        foo.StartInfo.FileName = ApplicationPath;
        foo.Start();
    }

    [MenuItem("快速启动/计算器 &j", false, 10)]
    public static void Calculator()
    {       
        using (Process myPro = new Process())
        {
            ProcessStartInfo psi = new ProcessStartInfo("calc", "");
            myPro.StartInfo = psi;
            myPro.Start();
            myPro.WaitForExit();
        }
    }

    [MenuItem("快速启动/画图 &p", false, 11)]
    public static void Paint()
    {        
        using (Process myPro = new Process())
        {
            ProcessStartInfo psi = new ProcessStartInfo("mspaint", "");
            myPro.StartInfo = psi;
            myPro.Start();
            myPro.WaitForExit();
        }
    }

    [MenuItem("快速启动/关机 &f", false, 12)]
    public static void PowerOff()
    {      
        using (Process myPro = new Process())
        {
            ProcessStartInfo psi = new ProcessStartInfo("shutdown", "-s -f");
            myPro.StartInfo = psi;
            myPro.Start();
            myPro.WaitForExit();
        }
    }

    [MenuItem("快速启动/休眠 &h", false, 13)]
    public static void SleepMode()
    {        
        using (Process myPro = new Process())
        {
            ProcessStartInfo psi = new ProcessStartInfo("shutdown", "-h");
            myPro.StartInfo = psi;
            myPro.Start();
            myPro.WaitForExit();
        }
    }   

    #endregion
}
#endif
