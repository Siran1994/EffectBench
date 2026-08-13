using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

/*****************************************
	 文件:   BuildSystem.cs
	 作者:   Siran
	 日期:   2021/6/2 15:11:56
	 功能:   打包系统
 *****************************************/
[HelpURL("https://github.com/Siran1994")]
public class BuildSystem
{
    #region Build apk&aab
    public static void BuildApkAndAab()
    {
        Build(false);
        Build(true);
    }

    static void Build(bool IsAab = false, bool Is32 = false)
    {
        //------------------------------------------------------------项目设置-------------------------------------------------------
        //设置目标打包系统和工程目标       
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;

        //设置Cpu架构
        if (Is32)
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);//32位包
        else
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);//64位包
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
        }

        //设置build模式
        EditorUserBuildSettings.androidBuildType = AndroidBuildType.Release;//AndroidBuildType.Development
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;//使用Gradle打包模式
        EditorUserBuildSettings.connectProfiler = false;

        //Aab Or Apk
        EditorUserBuildSettings.buildAppBundle = IsAab ? true : false;

        //设置targetAPI
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel28;//Android 4.4        
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;//最高版本 

        //剥离等级
        PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android, ManagedStrippingLevel.High);

        //其他设置
        BuildSetting.OtherSetting();
        //设置Icon
        BuildSetting.SetIcons(BuildTargetGroup.Android);
        //设置开机图
        BuildSetting.SetScreenLogo();
        //设置签名
        BuildSetting.SetSignKey();

        //设置RuntimeVersion
#if (UNITY_2018 || UNITY_2017 || UNITY5 || UNITY_2019_2_3)
        {
            PlayerSettings.scriptingRuntimeVersion = ScriptingRuntimeVersion.Latest;
        }
#endif
        string suffix = IsAab ? ".aab" : ".apk";//后缀名        
        string app_name = PlayerSettings.productName + GameConfig.Instance.VersionNum + suffix;//全名
        string outputPath = BuildSetting.GetOutPath(PlatformType.Android) + "/" + app_name;//输出位置       
        PlayerSettings.bundleVersion = GameConfig.Instance.VersionNum;//内部版本号
        PlayerSettings.Android.bundleVersionCode = GameConfig.Instance.BundleVersionNum;//外部版本号

        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, GameConfig.Instance.PackageName);//设置包名

        BuildSetting.UpdateAndroidSettings();//配置AndroidSDK,NDK,JDK路径

        //---------------------------------------------------------开始打包----------------------------------------------------
        BuildReport report;
        if (GameConfig.Instance.isOnlyBuild)
            report = BuildPipeline.BuildPlayer(BuildSetting.GetBuildScenes(), outputPath, BuildTarget.Android, BuildOptions.CompressWithLz4HC);
        else
            report = BuildPipeline.BuildPlayer(BuildSetting.GetBuildScenes(), outputPath, BuildTarget.Android, BuildOptions.AutoRunPlayer | BuildOptions.CompressWithLz4HC);

        BuildSummary summary = report.summary;//打包结果反馈

        if (summary.result == BuildResult.Succeeded)
        {
            EditorPrefs.SetString(GameConfig.Instance.AppName, System.DateTime.Now.ToString());

            if (GameConfig.Instance.isRelease && IsAab)
            {
                GameConfig.Instance.VersionNum = BuildSetting.BundleVersionAdd(GameConfig.Instance.VersionNum);
                GameConfig.Instance.BundleVersionNum++;
            }
            if (IsAab)
            {
                var IsSuccess = EditorUtility.DisplayDialog("Build Success!", "Size:" + summary.totalSize / (4 * 1024 * 1024) + "M" + "\n" + "Time:" + summary.totalTime.TotalSeconds + "s", "Yes");
                if (IsSuccess)
                    Application.OpenURL(Path.GetFullPath(BuildSetting.GetOutPath(PlatformType.Android)));
            }
        }
        if (summary.result == BuildResult.Failed)
            EditorUtility.DisplayDialog("Build Fail!", "Error:" + summary.totalErrors + "s", "Yes");
    }
    #endregion

    #region Build Android
    public static void BulidAndroid(bool Is32 = false)
    {
        //------------------------------------------------------------项目设置-------------------------------------------------------
        //设置目标打包系统和工程目标       
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;

        //设置Cpu架构
        if (Is32)
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);//32位包
        else
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);//64位包
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
        }

        //设置build模式
        EditorUserBuildSettings.androidBuildType = AndroidBuildType.Release;//AndroidBuildType.Development
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;//使用Gradle打包模式
        EditorUserBuildSettings.connectProfiler = false;

        //Aab Or Apk
        EditorUserBuildSettings.buildAppBundle = GameConfig.Instance.isBuildAab ? true : false;

        //设置targetAPI
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel28;//Android 4.4        
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;//最高版本 

        //剥离等级
        PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android, ManagedStrippingLevel.High);

        //其他设置
        BuildSetting.OtherSetting();
        //设置Icon
        BuildSetting.SetIcons(BuildTargetGroup.Android);
        //设置开机图
        BuildSetting.SetScreenLogo();
        //设置签名
        BuildSetting.SetSignKey();

        //设置RuntimeVersion
#if (UNITY_2018 || UNITY_2017 || UNITY5 || UNITY_2019_2_3)
        {
            PlayerSettings.scriptingRuntimeVersion = ScriptingRuntimeVersion.Latest;
        }
#endif
        string suffix = GameConfig.Instance.isBuildAab ? ".aab" : ".apk";//后缀名        
        string app_name = PlayerSettings.productName + GameConfig.Instance.VersionNum + suffix;//全名
        string outputPath = BuildSetting.GetOutPath(PlatformType.Android) + "/" + app_name;//输出位置       
        PlayerSettings.bundleVersion = GameConfig.Instance.VersionNum;//内部版本号
        PlayerSettings.Android.bundleVersionCode = GameConfig.Instance.BundleVersionNum;//外部版本号

        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, GameConfig.Instance.PackageName);//设置包名

        BuildSetting.UpdateAndroidSettings();//配置AndroidSDK,NDK,JDK路径

        //---------------------------------------------------------开始打包----------------------------------------------------
        BuildReport report;
        if (GameConfig.Instance.isOnlyBuild)
        {
            report = BuildPipeline.BuildPlayer(BuildSetting.GetBuildScenes(), outputPath, BuildTarget.Android, BuildOptions.CompressWithLz4HC);
        }
        else
        {
            report = BuildPipeline.BuildPlayer(BuildSetting.GetBuildScenes(), outputPath, BuildTarget.Android, BuildOptions.AutoRunPlayer | BuildOptions.CompressWithLz4HC);
        }
        BuildSummary summary = report.summary;//打包结果反馈

        if (summary.result == BuildResult.Succeeded)
        {
            var IsSuccess = EditorUtility.DisplayDialog("Build Success!", "Size:" + summary.totalSize / (4 * 1024 * 1024) + "M" + "\n" + "Time:" + summary.totalTime.TotalSeconds + "s", "Yes");
            if (IsSuccess)
            {
                if (GameConfig.Instance.isRelease)
                {
                    GameConfig.Instance.VersionNum = BuildSetting.BundleVersionAdd(GameConfig.Instance.VersionNum);
                    GameConfig.Instance.BundleVersionNum++;
                }
                EditorPrefs.SetString(GameConfig.Instance.AppName, System.DateTime.Now.ToString());
                Application.OpenURL(Path.GetFullPath(BuildSetting.GetOutPath(PlatformType.Android)));
            }
        }
        if (summary.result == BuildResult.Failed)
        {
            EditorUtility.DisplayDialog("Build Fail!", "Error:" + summary.totalErrors + "s", "Yes");
        }
    }
    #endregion

    #region Build IOS
    public static void BulidIOS()
    {
        //------------------------------------------------------------项目设置-------------------------------------------------------
        //设置目标打包系统和工程目标       
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);

        //设置CPU架构
        PlayerSettings.SetArchitecture(BuildTargetGroup.iOS, 2);// 0 = ARMV7 1 = ARM64 2 = Universal
        //需要设置os最小版本为 9 否则上传appstore出错 执行文件大小太大
        PlayerSettings.iOS.targetOSVersionString = "10.0";

        //设置build模式
        EditorUserBuildSettings.iOSBuildConfigType = iOSBuildType.Release;//iOSBuildType.Development
        EditorUserBuildSettings.connectProfiler = false;

        //设置targetAPI
        PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.iOS, ApiCompatibilityLevel.NET_2_0_Subset);

        //设置Icon
        BuildSetting.SetIcons(BuildTargetGroup.iOS);
        //设置开机图
        BuildSetting.SetScreenLogo();
        //其他设置
        BuildSetting.OtherSetting();

        string app_name = BuildSetting.GetAppName() + GameConfig.Instance.VersionNum;//全名
        string outputPath = BuildSetting.GetOutPath(PlatformType.IOS) + "/" + app_name;//输出位置

        PlayerSettings.bundleVersion = GameConfig.Instance.VersionNum;//内部版本号
        PlayerSettings.iOS.buildNumber = GameConfig.Instance.BundleVersionNum.ToString();//外部版本号
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, GameConfig.Instance.PackageName);//设置包名

        if (Directory.Exists(outputPath))
        {
            Directory.Delete(outputPath, true);
        }
        //---------------------------------------------------------开始打包----------------------------------------------------
        if (true)
        {
            BuildReport report;
            if (GameConfig.Instance.isOnlyBuild)
            {
                report = BuildPipeline.BuildPlayer(BuildSetting.GetBuildScenes(), outputPath, BuildTarget.iOS, BuildOptions.CompressWithLz4HC);
            }
            else
            {
                report = BuildPipeline.BuildPlayer(BuildSetting.GetBuildScenes(), outputPath, BuildTarget.iOS, BuildOptions.AutoRunPlayer | BuildOptions.CompressWithLz4HC);
            }
            BuildSummary summary = report.summary;//打包结果反馈

            if (summary.result == BuildResult.Succeeded)
            {
                var IsSuccess = EditorUtility.DisplayDialog("Build Success!", "Size:" + summary.totalSize / (4 * 1024 * 1024) + "M" + "\n" + "Time:" + summary.totalTime.TotalSeconds + "s", "Yes");
                if (IsSuccess)
                {
                    if (GameConfig.Instance.isRelease)
                    {
                        GameConfig.Instance.VersionNum = BuildSetting.BundleVersionAdd(GameConfig.Instance.VersionNum);
                        GameConfig.Instance.BundleVersionNum++;
                    }
                    EditorPrefs.SetString(GameConfig.Instance.AppName, System.DateTime.Now.ToString());
                    Application.OpenURL(Path.GetFullPath(BuildSetting.GetOutPath(PlatformType.IOS)));
                }
            }
            if (summary.result == BuildResult.Failed)
            {
                EditorUtility.DisplayDialog("Build Fail!", "Error:" + summary.totalErrors + "s", "Yes");
            }
        }
    }
    #endregion

    #region Build Windows
    public static void BulidWindows()
    {
        //------------------------------------------------------------项目设置-------------------------------------------------------
        //设置目标打包系统和工程目标       
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);

        PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Disabled;
        PlayerSettings.usePlayerLog = false;
        PlayerSettings.visibleInBackground = true;
        PlayerSettings.forceSingleInstance = true;
        PlayerSettings.allowFullscreenSwitch = true;
        PlayerSettings.fullScreenMode = FullScreenMode.FullScreenWindow;

        //设置Icon
        BuildSetting.SetIcons(BuildTargetGroup.Standalone);

        //其他设置
        BuildSetting.OtherSetting();

        string app_name = BuildSetting.GetAppName();//全名
        string outputPath = BuildSetting.GetOutPath(PlatformType.Windows) + "/" + app_name + ".exe";//输出位置    

        if (Directory.Exists(outputPath))
        {
            Directory.Delete(outputPath, true);
        }
        //---------------------------------------------------------开始打包----------------------------------------------------
        if (true)
        {
            BuildReport report;
            if (GameConfig.Instance.isOnlyBuild)
            {
                report = BuildPipeline.BuildPlayer(BuildSetting.GetBuildScenes(), outputPath, BuildTarget.StandaloneWindows64, BuildOptions.None);
            }
            else
            {
                report = BuildPipeline.BuildPlayer(BuildSetting.GetBuildScenes(), outputPath, BuildTarget.StandaloneWindows64, BuildOptions.AutoRunPlayer);
            }
            BuildSummary summary = report.summary;//打包结果反馈

            if (summary.result == BuildResult.Succeeded)
            {
                var IsSuccess = EditorUtility.DisplayDialog("打包成功!", "项目大小为:" + summary.totalSize / (4 * 1024 * 1024) + "M" + "\n" + "耗时:" + summary.totalTime.TotalSeconds + "s", "确定");
                if (IsSuccess)
                {
                    EditorPrefs.SetString(GameConfig.Instance.AppName, System.DateTime.Now.ToString());
                    Application.OpenURL(Path.GetFullPath(BuildSetting.GetOutPath(PlatformType.Windows)));
                }
            }
            if (summary.result == BuildResult.Failed)
            {
                EditorUtility.DisplayDialog("打包失败!", "问题数:" + summary.totalErrors + "个", "确定");
            }
        }
    }
    #endregion
}
