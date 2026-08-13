using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/*****************************************
	 文件:   BuildTools.cs
	 作者:   Siran
	 日期:   2021/1/21 16:29:49
	 功能:   打包设置
 *****************************************/
[HelpURL("https://github.com/Siran1994/SuperToolsManager")]
public class BuildSetting
{
    //设置平台
    public static void SetPlatform()
    {
        switch (GameConfig.Instance.PT)
        {
            case PlatformType.Android:
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
                break;
            case PlatformType.IOS:
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
                break;
            case PlatformType.Windows:
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
                break;           
        }
    }
    //其他设置
    public static void OtherSetting()
    {
        //设置横竖屏
        if (Screen.width > Screen.height)
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
        else
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;

        PlayerSettings.Android.renderOutsideSafeArea = true;//适配刘海和水滴屏
        if (GameConfig.Instance.islockFPS)
            QualitySettings.vSyncCount = 1;//0 开启锁帧
        else
            QualitySettings.vSyncCount = 0;//0 无锁模式
       
        PlayerSettings.muteOtherAudioSources = true;//禁用App后台音频 

        PlayerSettings.MTRendering = true;//设置多线程渲染        

        //公司名
        PlayerSettings.companyName = GameConfig.Instance.CompanyName;
        PlayerSettings.productName = GetAppName();//项目名

        //去掉未使用到的引擎代码（IOS 会奔溃）
        PlayerSettings.stripEngineCode = false;

        PlayerSettings.runInBackground = true;//后台运行         
    }
    //设置签名   
    public static void SetSignKey()
    {
#if (UNITY_2019 || UNITY_2020)
        PlayerSettings.Android.useCustomKeystore = true;
#endif       
        PlayerSettings.Android.keystoreName = "Assets/DeveloperTools/KeyStore/" + GameConfig.Instance.KeyName;
        PlayerSettings.Android.keyaliasName = GameConfig.Instance.KeyName.Split('.')[0];
        PlayerSettings.keystorePass = GameConfig.Instance.KeyPassWord;
        PlayerSettings.Android.keyaliasPass = PlayerSettings.keystorePass;
    }
    //设置Icon
    public static void SetIcons(BuildTargetGroup btg, string Icon_Path = "Assets/DeveloperTools/Textures/Icon.png")
    {
        if (!File.Exists(Icon_Path))
        {
            Icon_Path = "Assets/DeveloperTools/Textures/Icon.jpg";
            if (!File.Exists(Icon_Path))
            {
                EditorUtility.DisplayDialog("错误!!!", "没有找到该项目Icon", "确定");
            }
        }
        string iconPrefixName = "Icon";
        //获取所有的Icon尺寸
        int[] iconSizes = PlayerSettings.GetIconSizesForTargetGroup(btg);
        Texture2D[] texArray = new Texture2D[iconSizes.Length];
        for (int i = 0; i < iconSizes.Length; ++i)
        {
            int iconSize = iconSizes[i];
            //获得对应目录下的Icon，并转换成Texture2D
            Texture2D tex2D;
            if (GameConfig.Instance.GetTexture("Icon") != null)
                tex2D = GameConfig.Instance.GetTexture("Icon") as Texture2D;
            else
                tex2D = AssetDatabase.LoadAssetAtPath(string.Format(Icon_Path, iconPrefixName, iconSize),
              typeof(Texture2D)) as Texture2D;
            texArray[i] = tex2D;
        }
        //设置到PlayerSettings的各个Icon上
        PlayerSettings.SetIconsForTargetGroup(btg, texArray);
        AssetDatabase.SaveAssets();
    }
    //设置Logo
    public static void SetScreenLogo(string Logo_Path = "Assets/DeveloperTools/Textures/Splash.png")
    {
        //关闭闪屏 不能满足部分sdk渠道需求 提供logo和bg为一张大图
        if (GameConfig.Instance.isSetScreenLogo)
        {
            if (!File.Exists(Logo_Path))
            {
                EditorUtility.DisplayDialog("错误!!!", "没有找到健康忠告素材", "确定");
            }
            PlayerSettings.SplashScreen.showUnityLogo = true;// 屏蔽下方显示unity的logo (包含文字made with unity 和unity 的logo)
            PlayerSettings.SplashScreen.unityLogoStyle = PlayerSettings.SplashScreen.UnityLogoStyle.LightOnDark;
            PlayerSettings.SplashScreen.animationMode = PlayerSettings.SplashScreen.AnimationMode.Static;
            PlayerSettings.SplashScreen.drawMode = PlayerSettings.SplashScreen.DrawMode.AllSequential;
            PlayerSettings.SplashScreen.backgroundColor = Color.black;

            Sprite logo;
            if (GameConfig.Instance.GetTexture("Splash") != null)
                logo = Sprite.Create((Texture2D)GameConfig.Instance.GetTexture("Splash"), new Rect(0, 0, GameConfig.Instance.GetTexture("Splash").width, GameConfig.Instance.GetTexture("Splash").height), new Vector2(0.5f, 0.5f));
            else
                logo = AssetDatabase.LoadAssetAtPath(Logo_Path, typeof(Sprite)) as Sprite;

            var Unitylogo = PlayerSettings.SplashScreenLogo.Create(2, PlayerSettings.SplashScreenLogo.unityLogo);
            var Mylogo = PlayerSettings.SplashScreenLogo.Create(2, logo);
            PlayerSettings.SplashScreen.logos = new PlayerSettings.SplashScreenLogo[2] { Unitylogo, Mylogo };
        }
        else
        {
            PlayerSettings.SplashScreen.show = false;
        }       
    }
    //设置AndroidSDK,NDK,JDK路径
    public static void UpdateAndroidSettings()
    {
#if UNITY_2018 || UNITY_2017 || UNITY5
        {
            UpdateSetting("AndroidSdkRoot", GameConfig.Instance.SDKPath[0].SdkPath);
            UpdateSetting("AndroidNdkRootR16b", GameConfig.Instance.SDKPath[1].SdkPath);//Unity的版本不同,NDK有不同,keyName有所改动
            UpdateSetting("JdkPath", GameConfig.Instance.SDKPath[2].SdkPath);
        }
#endif
    }
    static void UpdateSetting(string key, string Path)
    {
        if (Directory.Exists(Path))
        {
            EditorPrefs.SetString(key, Path);
        }
        else
        {
            EditorUtility.DisplayDialog("提示!", "AndroidSDK,NDK,JDK路径错误!", "确定");
        }
    }
    //获取需要打包的场景
    public static string[] GetBuildScenes()
    {
        List<string> pathList = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                pathList.Add(scene.path);
            }
        }
        return pathList.ToArray();
    }
    //获取项目名称
    public static string GetAppName()
    {
        if (GameConfig.Instance.AppName == "")
        {
            EditorUtility.DisplayDialog("提示!", "未设置项目名称!", "确定");
            return "测试";
        }
        else
        {
            return GameConfig.Instance.AppName;
        }
    }    
    //获取输出路径
    public static string GetOutPath(PlatformType pt)
    {
        string outPath = "OutPath";
        switch (pt)
        {
            case PlatformType.Android:
                outPath += "/Android";
                break;
            case PlatformType.IOS:
                outPath += "/IOS";
                break;
            case PlatformType.Windows:
                outPath += "/Windows";
                break;
            case PlatformType.Dll:
                outPath += "/Dll";
                break;
            case PlatformType.UnityPackage:
                outPath += "/UnityPackage";
                break;
        }
        if (Directory.Exists(outPath) == false) //创建存放资源的AssetBundles文件夹
        {
            Directory.CreateDirectory(outPath);
        }
        return outPath;
    }   
    //模式切换
    public static void BuildOrBuildAndRun()
    {
        GameConfig.Instance.isOnlyBuild = !GameConfig.Instance.isOnlyBuild;
        if (GameConfig.Instance.isOnlyBuild)
            EditorUtility.DisplayDialog("Tips", "Current Mode:\n Only Build", "Yes");
        else
            EditorUtility.DisplayDialog("Tips", "Current Mode:\n BuildAndRun,\n Make sure your Android device is connected!", "Yes");
    }
    //平台切换
    public static void SwitchPlatform(int index)
    {
        switch (index)
        {
            case 0:
                break;
            case 1:
                GameConfig.Instance.PT = PlatformType.Android;
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
                break;
            case 2:
                GameConfig.Instance.PT = PlatformType.IOS;
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
                break;
            case 3:
                GameConfig.Instance.PT = PlatformType.Windows;
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
                break;           
        }       
    }
    //外部版本号处理    
    public static string BundleVersionAdd(string version)
    {
        var sp = version.Split('.');
        if (sp.Length <= 2)
            return "1.0.0";

        var nums = new int[sp.Length];       
        for (int i = 0; i < sp.Length; i++)
            nums[i] = int.Parse(sp[i]);

        nums[sp.Length - 1]++;
        if (nums[sp.Length - 1] >= 10)
        {

            nums[sp.Length - 1] = 0;
            nums[sp.Length - 2]++;
        }
        string ver = nums[0].ToString();
        for (int i = 1; i < sp.Length; i++)
            ver = $"{ver}.{nums[i]}";
        return ver;
    }
}
