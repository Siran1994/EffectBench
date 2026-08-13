using UnityEngine;
using UnityEditor;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Reflection;
using System;
using System.Diagnostics;
using System.Threading;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net;
using System.Collections;
using UnityEngine.Networking;
using Random = UnityEngine.Random;
using Debug = UnityEngine.Debug;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#pragma warning disable 0618, 0649, 0414
/*****************************************
文件:   Util.cs
作者:   Siran
日期:   2020/9/27 
功能:   工具类
*****************************************/
public sealed class Util
{
    #region --- 文件操作 ---
    public static string Read(string _path)  //读取
    {
        try
        {
            StreamReader _sr = File.OpenText(_path);
            string _data = _sr.ReadToEnd();
            _sr.Close();
            return _data;
        }
        catch (System.Exception)
        {
            return "";
        }
    }
    public static void Write(string _data, string _path)//写入
    {
        try
        {
            FileStream fs = new FileStream(_path, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            sw.Write(_data);
            sw.Close();
            fs.Close();
#if UNITY_EDITOR
            AssetDatabase.Refresh();//资源刷新      
#endif

        }
        catch (Exception)
        {
            return;
        }
    }
    public static byte[] FileToByte(string path)//转Byte
    {
        if (File.Exists(path))
        {
            byte[] bytes = null;
            try
            {
                bytes = File.ReadAllBytes(path);
            }
            catch
            {
                return null;
            }
            return bytes;
        }
        else
        {
            return null;
        }
    }
    public static bool ByteToFile(byte[] bytes, string path)//转File
    {
        try
        {
            string parentPath = new FileInfo(path).Directory.FullName;
            CreateFolder(parentPath);
            FileStream fs = new FileStream(path,FileMode.Create, FileAccess.Write);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
            fs.Dispose();
            return true;
        }
        catch
        {
            return false;
        }
    }
    public static void CreateFolder(string _path)//创建文件夹
    {
        _path = GetFullPath(_path);
        if (Directory.Exists(_path))
            return;
        string _parentPath = new FileInfo(_path).Directory.FullName;
        if (Directory.Exists(_parentPath))
        {
            Directory.CreateDirectory(_path);
        }
        else
        {
            CreateFolder(_parentPath);
            Directory.CreateDirectory(_path);
        }
    }
    public static bool FileExist(string path)//判断文件是否存在
    {
        return !string.IsNullOrEmpty(path) && File.Exists(path);
    }
    public static bool DirectoryExist(string path)//判断路径是否存在
    {
        return !string.IsNullOrEmpty(path) && Directory.Exists(path);
    }
    public static bool FileOrDirectoryExist(string path)//文件判空
    {
        return FileExist(path) || DirectoryExist(path);
    }
    #endregion
    #region --- 路径处理 ---
    public static string FixPath(string _path)
    {
        _path = _path.Replace('\\', '/');
        _path = _path.Replace("//", "/");
        while (_path.Length > 0 && _path[0] == '/')
        {
            _path = _path.Remove(0, 1);
        }
        return _path;
    }
    public static string GetFullPath(string path)
    {
        return new FileInfo(path).FullName;
    }
    public static string RelativePath(string path)
    {
        path = FixPath(path);
        if (path.StartsWith("Assets"))
        {
            return path;
        }
        if (path.StartsWith(FixPath(Application.dataPath)))
        {
            return "Assets" + path.Substring(FixPath(Application.dataPath).Length);
        }
        else
        {
            return "";
        }
    }
    public static string CombinePaths(params string[] paths)
    {
        string path = "";
        for (int i = 0; i < paths.Length; i++)
        {
            path = Path.Combine(path, FixPath(paths[i]));
        }
        return FixPath(path);
    }
    public static string GetExtension(string path)//获取文件后缀
    {
        return Path.GetExtension(path);
    }
    public static string GetName(string path)
    {
        return Path.GetFileNameWithoutExtension(path);
    }
    public static string ChangeExtension(string path, string newEx)//修改文件后缀
    {
        return Path.ChangeExtension(path, newEx);
    }
    public static bool PathIsDirectory(string path)
    {
        FileAttributes attr = File.GetAttributes(path);
        return (attr & FileAttributes.Directory) == FileAttributes.Directory;
    }
    public static string GetRelativeParentPath(string path)
    {
        return RelativePath(new FileInfo(path).Directory.FullName);
    }
    public static string RenameForCreate(string path)
    {
        int currentIndex = 1;
        string rootPath = CombinePaths(
            GetRelativeParentPath(path),
            GetName(path)
        );
        string ex = GetExtension(path);
        while (FileExist(path))
        {
            path = rootPath + "_" + currentIndex.ToString() + ex;
            currentIndex++;
        }
        return path;
    }
    #endregion
    #region --- 日志记录 ---
#if UNITY_EDITOR
    public static bool Dialog(string title, string msg, string ok, string cancel = "")
    {
        EditorApplication.Beep();
        PauseWatch();
        if (string.IsNullOrEmpty(cancel))
        {
            bool sure = EditorUtility.DisplayDialog(title, msg, ok);
            RestartWatch();
            return sure;
        }
        else
        {
            bool sure = EditorUtility.DisplayDialog(title, msg, ok, cancel);
            RestartWatch();
            return sure;
        }
    }
    public static int DialogComplex(string title, string msg, string ok, string cancel, string alt)
    {
        EditorApplication.Beep();
        PauseWatch();
        int index = EditorUtility.DisplayDialogComplex(title, msg, ok, cancel, alt);
        RestartWatch();
        return index;
    }
    public static void ProgressBar(string title, string msg, float value)
    {
        value = Mathf.Clamp01(value);
        EditorUtility.DisplayProgressBar(title, msg, value);
    }
    public static void ClearProgressBar()
    {
        EditorUtility.ClearProgressBar();
    }
#endif
    public static void MakeToast(string str = "")
    {
        if (Application.platform != RuntimePlatform.Android)
            return;
        AndroidJavaObject currentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaClass Toast = new AndroidJavaClass("android.widget.Toast");
        currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
        {
            Toast.CallStatic<AndroidJavaObject>("makeText", currentActivity, str, Toast.GetStatic<int>("LENGTH_LONG")).Call("show");
        }));
    }
    #endregion
    #region --- 计时器 ---
    private static System.Diagnostics.Stopwatch TheWatch;

    public static void StartWatch()
    {
        TheWatch = new System.Diagnostics.Stopwatch();
        TheWatch.Start();
    }
    public static void PauseWatch()
    {
        if (TheWatch != null)
        {
            TheWatch.Stop();
        }
    }
    public static void RestartWatch()
    {
        if (TheWatch != null)
        {
            TheWatch.Start();
        }
    }
    public static double StopWatchAndGetTime()
    {
        if (TheWatch != null)
        {
            TheWatch.Stop();
            return TheWatch.Elapsed.TotalSeconds;
        }
        return 0f;
    }
    #endregion
    #region --- MD5加密 ---
    public static string MD5Encrypt(string str)//MD5加密
    {
        MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
        byte[] hashedDataBytes;
        hashedDataBytes = md5Hasher.ComputeHash(Encoding.Default.GetBytes(str));
        StringBuilder tmp = new StringBuilder();
        foreach (byte i in hashedDataBytes)
        {
            tmp.Append(i.ToString("x2"));//X2大写的16进制 x2小写的16进制 字符串
        }
        return tmp.ToString();
    }
    #endregion
    #region --- 反射 ---
    [Conditional("Debug")]
    [Obsolete("这个方法过时了,请使用NewMethod代替")]
    public static void MethodDo(string DllPath, string ClassName, string MethodName, string callKey = "直接调用", object[] paras = null)
    {
        Assembly asm = Assembly.LoadFrom(DllPath);//加载程序集
        Type t = asm.GetType(ClassName);//获取类名                                        
        object obj = Activator.CreateInstance(t);//实例化类型       
        try
        {
            if (callKey == "直接调用")
            {
                #region 方法一
                //直接调用
                MethodInfo method = t.GetMethod(MethodName);
                method.Invoke(obj, paras);
                #endregion
            }
            else
            {
                #region 方法二
                MethodInfo[] info = t.GetMethods();
                for (int i = 0; i < info.Length; i++)
                {
                    var md = info[i];
                    //方法名
                    string mothodName = md.Name;
                    //参数集合
                    ParameterInfo[] paramInfos = md.GetParameters();
                    //方法名相同且参数个数一样
                    if (mothodName == MethodName && paramInfos.Length == paras.Length)
                    {
                        md.Invoke(obj, paras);
                    }
                }
                #endregion
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    #endregion
    #region --- 多线程 ---
    public static void MyTask(Action action)
    {
        new Thread(new ThreadStart(action)).Start();
    }
    #endregion
    #region --- 正则表达式 ---
    /*只能输入数字： "^[0-9]*$" 。
    只能输入n位的数字："^\d{n}$"。
    只能输入至少n位的数字："^\d{n,}$"。
    只能输入m ~n位的数字："^\d{m,n}$"。
    只能输入零和非零开头的数字："^(0|[1-9][0-9]*)$"。
    只能输入有两位小数的正实数："^[0-9]+(.[0-9]{2})?$"。
    只能输入有1 ~3位小数的正实数："^[0-9]+(.[0-9]{1,3})?$"。
    只能输入非零的正整数："^\+?[1-9][0-9]*$"。
    只能输入非零的负整数："^\-[1-9][]0-9"*$。
    只能输入长度为3的字符："^.{3}$"。
    只能输入由26个英文字母组成的字符串："^[A-Za-z]+$"。
    只能输入由26个大写英文字母组成的字符串："^[A-Z]+$"。
    只能输入由26个小写英文字母组成的字符串："^[a-z]+$"。
    只能输入由数字和26个英文字母组成的字符串："^[A-Za-z0-9]+$"。
    只能输入由数字、26个英文字母或者下划线组成的字符串："^\w+$"。
    验证用户密码："^[a-zA-Z]\w{5,17}$"正确格式为：以字母开头，长度在6 ~18之间，只能包含字符、数字和下划线。
    验证是否含有^%&’,;=?$\"等字符："[^%&’,;=?$\x22]+"。
    只能输入汉字："^[\u4e00-\u9fa5]{0,}$"
    验证Email地址："^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"。
    验证InternetURL："^http://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?$"。
    提取图片地址：/(http(s)?\:\/\/)?(www\.)?(\w+\:\d+)?(\/\w+)+\.(png|gif|jpg|bmp|jpeg)/gi。
    验证电话号码："^(\(\d{3,4}-)|\d{3.4}-)?\d{7,8}$"正确格式为："XXX-XXXXXXX"、"XXXX-XXXXXXXX"、"XXX-XXXXXXX"、"XXX-XXXXXXXX"、"XXXXXXX"和"XXXXXXXX"。
    验证身份证号(15位或18位数字)："^\d{15}|\d{18}$"。
    验证一年的12个月："^(0?[1-9]|1[0-2])$"正确格式为："01"～"09"和"1"～"12"。
    验证一个月的31天："^((0?[1-9])|((1|2)[0-9])|30|31)$"正确格式为;"01"～"09"和"1"～"31"。*/
    //定位元字符  ^ 搜索字符串开头 $ 搜索字符串结尾
    public static string InsertStr(string targetStr = "目标字符串", string RegexStr = @"^", string ReplaceStr = "替换字符串")
    {
        return Regex.Replace(targetStr, RegexStr, ReplaceStr);//搜索字符串 符合正则表达式的情况.然后把所有符合位置,替换成后面的字符串
    }
    //@   不让编译器去解析其中的转义字符,而作为正则表达式的语法(元字符)存在.
    //^  表示开头  \d 表示数字 *表示 \d类型的字符有0个或多个 $ 表示以\d类型的字符结尾
    //  @"^\d*$"  匹配目标字符串是否是数字
    //  @"a*";   表示只要有a(1个或多个)字符组成的字符串都是符合规则的 
    //  @"^\W*$"  表示只允许输入除大小写字符,0-9的数字,下划线_以外的任何字      
    public static bool JudgeStr(string targetStr = "目标字符串", string RegexStr = @"^\d*$")//匹配目标字符串是否是数字
    {
        return Regex.IsMatch(targetStr, RegexStr);//搜索字符串 符合正则表达式的情况.然后把所有符合位置,替换成后面的字符串
    }
    // @"[^abc]" 查找除了abc以外的任意一个字符
    // @[ab]  匹配括号中的字符
    // @[a-z]  匹配括号中a到z之间的字符
    // @[^a]  匹配除了a之外的任意字符
    public static string ReplaceStr(string targetStr = "目标字符串", string RegexStr = @"[^abc]", string ReplaceStr = "替换字符串")
    {
        return Regex.Replace(targetStr, RegexStr, ReplaceStr);//搜索字符串 符合正则表达式的情况.然后把所有符合位置,替换成后面的字符串
    }
    // @{n}  匹配前面的字符n次
    // @{n,}  匹配前面的字符n次或者多余n次
    // @{n,m}  匹配前面的字符n到m次
    // @?  重复一次或0次
    // @+  重复一次或更多次
    // @*  重复0次或更多次
    // @"^\d{5,12}$"  校验QQ号是否合法(一般是5到12位,开头和结尾之间没有任何字母)
    // @"\(0\d{2,3}\)[-]?\d{7,8}|^0\d{2,3}[-]?\d{7,8}$)" 校验国内固定电话号码
    // @"^(((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?))$" 校验IP4地址
    public static bool JudgeNum(string targetStr = "QQNum", string RegexStr = @"^\d{5,12}$")//匹配目标字符串是否是数字
    {
        return Regex.IsMatch(targetStr, RegexStr);//搜索字符串 符合正则表达式的情况.然后把所有符合位置,替换成后面的字符串
    }

    // @| 将2个匹配条件进行逻辑"或(or)"运算
    // @"\d|[a-z]" 查找字符中的所有的数字和字母
    public static MatchCollection FindStrOrNum(string targetStr = "目标字符串", string RegexStr = @"\d|[a-z]")
    {
        return Regex.Matches(targetStr, RegexStr);//搜索字符串 符合正则表达式的情况.然后把所有符合位置,替换成后面的字符串
    }
    // @"[;,.]"  字符串分割
    // @"[;]|[,]|[.]"  字符串分割
    // @"(ab\w{2}){2}"==@"ab\w{2}ab\w{2}"
    public static string[] SplitStr(string targetStr = "目标字符串", string RegexStr = @"[;,.]")
    {
        return Regex.Split(targetStr, RegexStr);//搜索字符串 符合正则表达式的情况.然后把所有符合位置,替换成后面的字符串
    }
    #endregion
    #region --- 数据排序 ---
    public static List<int> Sort(List<int> data, bool IsUpSort = true)
    {
        for (int i = 0; i < data.Count - 1; i++)
        {
            for (int j = 0; j < data.Count - 1 - i; j++)
            {
                if (IsUpSort)
                {
                    if (data[j] > data[j + 1])
                    {
                        data[j] = data[j] + data[j + 1];
                        data[j + 1] = data[j] - data[j + 1];
                        data[j] = data[j] - data[j + 1];
                    }
                }
                else
                {
                    if (data[j] < data[j + 1])
                    {
                        data[j] = data[j] + data[j + 1];
                        data[j + 1] = data[j] - data[j + 1];
                        data[j] = data[j] - data[j + 1];
                    }
                }
            }
        }
        return data;
    }
    #endregion
    #region --- Win10获取IP地址 ---
    public static string GetLocalIp()
    {
        ///获取本地的IP地址
        string AddressIP = string.Empty;
        foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        {
            if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
            {
                AddressIP = _IPAddress.ToString();
            }
        }
        return AddressIP;
    }
    #endregion
    #region --- 获取时间戳(基于本地系统时间)
    public static long GetCreatetime()//获取时间戳(毫秒)
    {
        DateTime DateStart = new DateTime(1970, 1, 1, 8, 0, 0);
        return Convert.ToInt64((DateTime.Now - DateStart).TotalSeconds * 1000);
    }

    public static long GetCreatetime(DateTime dateTime)//获取时间戳(毫秒)
    {
        DateTime DateStart = new DateTime(1970, 1, 1, 8, 0, 0);
        return Convert.ToInt64((dateTime - DateStart).TotalSeconds * 1000);
    }

    public static DateTime UnixTimestampToDateTimeLocalTime(string timestamp)//时间戳转日期
    {
        DateTime dtStart = new DateTime(1970, 1, 1, 8, 0, 0);
        long lTime = long.Parse(timestamp + "0000000");
        TimeSpan toNow = new TimeSpan(lTime);
        return dtStart.Add(toNow);
    }
    #endregion
    #region --- 网络请求
    //Get请求
    public IEnumerator SendGet1(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
        if (request.isHttpError || request.isNetworkError)
            Debug.Log(request.error);
        else
        {
            Debug.Log(request.downloadHandler.text);
            // ParseJson(request.downloadHandler.text);
        }
    }
    public IEnumerator SendGet(string url)//Get请求
    {
        if (!string.IsNullOrEmpty(url))
        {
            WWW result = new WWW(url);

            yield return result;

            if (result.error != null)
            {
                Debug.Log("访问失败：" + result.error);
            }
            else
            {
                if (string.IsNullOrEmpty(result.text))
                {
                    Debug.LogError("返回值为空");
                }
                else
                {
                    Debug.Log(result.text);
                    // ParseJson(result.text);
                }
            }
        }
        else
        {
            Debug.LogError("URL不能为空");
        }

    }

    //Post请求
    public IEnumerator SendPost(string url, WWWForm wForm = null)//Post请求
    {
        if (!string.IsNullOrEmpty(url))
        {
            WWW result = new WWW(url, wForm);

            yield return result;

            if (result.error != null)
            {
                Debug.Log("访问失败：" + result.error);
            }
            else
            {
                if (string.IsNullOrEmpty(result.text))
                {
                    Debug.LogError("返回值为空");
                }
                else
                {
                    //Json解析
                    Debug.Log(result.text);
                    // ParseJson(result.text);
                }
            }
        }
        else
        {
            Debug.LogError("URL不能为空");
        }
    }
    #endregion
    #region --- 网络下载
    /// <summary>
    /// 各类型资源下载
    /// </summary>
    /// <param name="path">网络地址</param>
    /// <param name="ResType">资源类型</param>
    /// <param name="index">下标</param>    
    public static void LoadRes(string path, string ResType, int index = 0, string name = "")
    {
        IEnumerator Load = ParseRes(path, ResType, index, name);
        Load.MoveNext();
        while (!((WWW)(Load.Current)).isDone) ;
        Load.MoveNext();
    }
    static IEnumerator ParseRes(string path, string ResType, int index, string name)
    {
        WWW www = new WWW(path);
        yield return www;
        if (www.error == null)
        {
            switch (ResType)
            {
                case "Txt":
                    // ParseXML(www.text);
                    break;
                case "Tex":
                    switch (index)
                    {
                        case 1:
                            var img = www.texture;
                            break;
                    }
                    break;
                case "AB":
                    switch (index)
                    {
                        case 1:
                            var skin1 = (GUISkin)www.assetBundle.LoadAsset(name);
                            break;
                    }
                    www.assetBundle.Unload(false);
                    break;
                case "Audio":
                    switch (index)
                    {
                        case 1:
                            var clip = www.GetAudioClip(false, true, AudioType.OGGVORBIS);
                            break;
                    }
                    break;
                case "Video":
                    switch (index)
                    {
                        case 1:
                            // var video = www.GetMovieTexture();
                            break;
                    }
                    break;
            }
        }
    }
    #endregion
    #region --- 执行指令
    public static bool RunCmd(string cmdExe, string cmdStr, bool iswait = true)
    {
        bool result = false;
        try
        {
            using (Process myPro = new Process())
            {
                ProcessStartInfo psi = new ProcessStartInfo(cmdExe, cmdStr);
                myPro.StartInfo = psi;
                myPro.Start();
                if (iswait)
                    myPro.WaitForExit();
                result = true;
            }
        }
        catch
        {
        }
        return result;
    }
    #endregion
    #region --- 随机数
    public static int GetRandomNum()
    {
        System.Random random = new System.Random();
        return random.Next(1, 65535);
    }
    #endregion
}
public static class MyTools
{
    #region --- Data
    /// <summary>
    /// 将Bool数组中的变量重新设置为你需要的值
    /// </summary>
    /// <param name="array">需要设置的数组</param>
    /// <param name="value">具体参数</param>
    /// <returns></returns>
    public static void InitValue(this bool[] array, bool value)
    {
        for (int i = 0, count = array.Length; i < count; i++)
        {
            array[i] = value;
        }
    }

    /// <summary>
    /// 获取某一个节点下的全部子节点 
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static Transform[] GetChildArray(this Transform parent)
    {
        int count = parent.childCount;
        if (count == 0)
        {
            return null;
        }
        Transform[] list = new Transform[count];

        for (int i = 0; i < count; i++)
        {
            list[i] = parent.GetChild(i);
        }

        return list;
    }

    /// <summary>
    /// 根据name返回数组
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static List<Transform> GetChildListByName(this Transform parent, string name)
    {
        int count = parent.childCount;
        if (count == 0)
        {
            return null;
        }
        List<Transform> list = new List<Transform>();

        for (int i = 0; i < count; i++)
        {
            if (parent.GetChild(i).name.Contains(name))
            {
                list.Add(parent.GetChild(i));
            }
        }

        return list;
    }

    /// <summary>
    /// 将数组转换为list 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <returns></returns>
    public static List<T> ToList<T>(this T[] array)
    {
        List<T> temp = new List<T>();
        for (int i = 0, count = array.Length; i < count; i++)
        {
            temp.Add(array[i]);
        }
        return temp;
    }

    /// <summary>
    /// 字符串NullOrEmpty判断 
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool IsNullOrEmpty(this string str)
    {
        return string.IsNullOrEmpty(str);
    }


    public static Transform FindChild(this GameObject go, string name)
    {
        return go.transform.Find(name);
    }

    public static Transform FindChild(this MonoBehaviour mb, string name)
    {
        return mb.transform.Find(name);
    }

    /// <summary>
    /// 将字典中的值重写。
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="dict"></param>
    /// <param name="copy"></param>
    /// <param name="overwrite">该值决定是否重写</param>
    public static void AddRange<K, V>(this Dictionary<K, V> dict, Dictionary<K, V> copy, bool overwrite)
    {
        if (copy == null)
        {
            return;
        }

        foreach (KeyValuePair<K, V> pair in copy)
        {
            if (dict.ContainsKey(pair.Key) && overwrite)
            {
                dict[pair.Key] = pair.Value;
            }
            else
            {
                dict.Add(pair.Key, pair.Value);
            }
        }
    }

    /// <summary>
    /// 增加或者更新值
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="dict"></param>
    /// <param name="k"></param>
    /// <param name="v"></param>
    public static void AddOrUpdate<K, V>(this IDictionary<K, V> dict, K k, V v)
    {
        if (dict.ContainsKey(k))
        {
            dict[k] = v;
        }
        else
        {
            dict.Add(k, v);
        }
    }

    /// <summary>
    /// 设置游戏物体的状态，公共状态  
    /// </summary>
    /// <param name="_object"></param>
    /// <param name="_active"></param>
    public static void SetActive(this GameObject _object, bool _active)
    {
        if (_object.activeInHierarchy == _active)
            return;
        _object.SetActive(_active);
    }

    /// <summary>
    /// 设置游戏物体的状态，自身状态 
    /// </summary>
    /// <param name="_object"></param>
    /// <param name="_active"></param>
    public static void SetActiveSelf(this GameObject _object, bool _active)
    {
        if (_object.activeSelf == _active)
            return;
        _object.SetActive(_active);
    }

    /// <summary>
    /// 设置游戏物体的状态，公共状态  
    /// </summary>
    /// <param name="_object"></param>
    /// <param name="_active"></param>
    public static void SetActive(this Transform _transform, bool _active)
    {
        SetActive(_transform.gameObject, _active);
    }

    /// <summary>
    /// 设置游戏物体的状态，自身状态 
    /// </summary>
    /// <param name="_transform"></param>
    /// <param name="_active"></param>
    public static void SetActiveSelf(this Transform _transform, bool _active)
    {
        SetActiveSelf(_transform.gameObject, _active);
    }

    /// <summary>
    /// 将数组中的所有数据初始化为某一个值
    /// </summary>
    /// <param name="array"></param>
    /// <param name="value"></param>
    public static void InitOfValue(this bool[,] array, bool value)
    {
        for (int i = 0, ilength = array.GetLength(0); i < ilength; i++)
        {
            for (int j = 0, jlength = array.GetLength(1); j < jlength; j++)
            {
                array[i, j] = value;
            }
        }
    }

    /// <summary>
    /// 将二维数组中的所有数据初始化为某一个值
    /// </summary>
    /// <param name="array"></param>
    /// <param name="value"></param>
    public static void InitOfValue(this int[,] array, int value)
    {
        for (int i = 0, ilength = array.GetLength(0); i < ilength; i++)
        {
            for (int j = 0, jlength = array.GetLength(1); j < jlength; j++)
            {
                array[i, j] = value;
            }
        }
    }

    /// <summary>
    /// 判断物体是否在某一个摄像机的视角内 
    /// </summary>
    /// <param name="render"></param>
    /// <param name="camera"></param>
    /// <returns></returns>
    public static bool IsVisibleFrom(this Renderer render, Camera camera)
    {
        bool visible = true;
        // 取得给定的摄像机的视景并返回它的六个面。
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        // 判断边界框是否在平面内部 
        visible = GeometryUtility.TestPlanesAABB(planes, render.bounds);
        return visible;
    }

    /// <summary>
    /// 转换为Vector2，即去掉z轴。
    /// </summary>
    /// <param name="vec"></param>
    /// <returns></returns>
    public static Vector2 ToVector2(this Vector3 vec)
    {
        return new Vector2(vec.x, vec.y);
    }

    /// <summary>
    /// 转换为Vector3，使用某一个值填充 
    /// </summary>
    /// <param name="vec"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Vector3 ToVector3(this Vector2 vec, float value = 0)
    {
        return new Vector3(vec.x, vec.y, value);
    }

    /// <summary>
    /// 计算某一个数值的顺序，基于传入参数 
    /// </summary>
    /// <param name="curnum"></param>
    /// <param name="num1"></param>
    /// <param name="num2"></param>
    /// <returns></returns>
    public static bool Order(float curnum, float num1, float num2)
    {
        if (curnum > num1 && curnum < num2)
        {
            return true;
        }
        else if (curnum > num2 && curnum < num1)
        {
            return true;
        }
        else
            return false;
    }

    /// <summary>
    /// 判断某个数值是否在指定范围内
    /// </summary>
    /// <param name="_value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static bool Order(int _value, int min, int max)
    {
        if (_value >= min && _value <= max)
            return true;
        return false;
    }

    /// <summary>
    /// 格式化小数点后面的位数 
    /// </summary>
    /// <param name="num"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static float Format(this float num, int count)
    {
        return Mathf.FloorToInt(num * Mathf.Pow(10, count)) / Mathf.Pow(10, count);
    }

    /// <summary>
    /// 将右屏坐标转换为左屏坐标 
    /// </summary>
    /// <param name="rightpath"></param>
    /// <returns></returns>
    public static Vector3[] GetLeftPath(Vector3[] rightpath)
    {
        Vector3[] tempVec = rightpath;

        for (int i = 0, count = rightpath.Length; i < count; i++)
        {
            /*
             * 左边路径坐标和右边相差11.2个单位，因为左边坐标比右边坐标小，应该减去11.2个单位 
             */
            tempVec[i].x = rightpath[i].x - 11.2f;
        }

        return tempVec;
    }

    /// <summary>
    /// 弧度转角度
    /// </summary>
    /// <param name="radian"></param>
    /// <returns></returns>
    public static float RadianToAngle(float radian)
    {
        return radian * 180 / Mathf.PI;
    }

    /// <summary>
    /// 角度转弧度 
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static float AngleToRadian(float angle)
    {
        return angle * Mathf.PI / 180;
    }

    /// <summary>
    /// 计算两个点之间的夹角，仅用于2D界面(x，y轴)
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    public static float CalcAngleFromPosition(Vector3 sourcePos, Vector3 targetPos)
    {
        return RadianToAngle(Mathf.Atan((sourcePos.y - targetPos.y) / (sourcePos.x - targetPos.x)));
    }

    /// <summary>
    /// 计算两个点的距离，只计算x，y轴，用于2D平面 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static float CalcDistance2D(Vector3 source, Vector3 target)
    {
        return Vector2.Distance(source.ToVector2(), target.ToVector2());
    }

    /// <summary>
    /// 分解该数字 
    /// </summary>
    /// <param name="Num"></param>
    /// <returns></returns>
    public static int[] DecomposeNum(this int Num)
    {
        List<int> tempList = new List<int>();
        int tempNum = Num;

        for (int i = 0; i < 10; i++)
        {
            tempList.Add(tempNum % 10);
            tempNum /= 10;

            if (tempNum == 0)
                break;
        }

        tempList.Reverse();

        return tempList.ToArray();
    }

    /// <summary>
    /// 设置当前节点及其子节点全部Layer
    /// </summary>
    /// <param name="_trans"></param>
    /// <param name="_layer"></param>
    public static void SetLayer(this Transform _trans, int _layer)
    {
        Transform[] child = _trans.GetComponentsInChildren<Transform>();
        for (int i = 0, count = child.Length; i < count; i++)
        {
            child[i].gameObject.layer = _layer;
        }
    }

    /// <summary>
    /// 更新链表中的某一个值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_list"></param>
    /// <param name="index"></param>
    /// <param name="_t"></param>
    /// <returns></returns>
    public static bool UpdateListByItem<T>(this List<T> _list, int index, T _t)
    {
        if (_list.Count <= index)
            return false;

        _list[index] = _t;
        return true;
    }

    /// <summary>
    /// 返回浮点数的整数部分 
    /// </summary>
    /// <param name="_value"></param>
    /// <returns></returns>
    public static int ToInt(this float _value)
    {
        return Mathf.FloorToInt(_value);
    }

    public static Transform CreateChild(this Transform _trans, string childname = "")
    {
        return CreateChild(_trans.gameObject, childname).transform;
    }

    public static GameObject CreateChild(this GameObject _father, string childname = "")
    {
        GameObject child = new GameObject(childname.IsNullOrEmpty() ? "child" : childname);
        child.transform.SetParent(_father.transform);
        child.transform.localPosition = Vector3.zero;
        child.transform.localRotation = Quaternion.identity;
        child.transform.localScale = Vector3.one;

        return child;
    }

    /// <summary>
    /// 使用指定的预制体，生成子节点 
    /// </summary>
    /// <param name="_father"></param>
    /// <param name="_prefab"></param>
    /// <param name="childname"></param>
    /// <returns></returns>
    public static GameObject CreateChildForPrefab(this GameObject _father, GameObject _prefab, string childname = "")
    {
        GameObject child = GameObject.Instantiate(_prefab);
        if (!childname.IsNullOrEmpty())
        {
            child.name = childname;
        }
        child.transform.SetParent(_father.transform);
        child.transform.localPosition = Vector3.zero;
        child.transform.localRotation = Quaternion.identity;
        child.transform.localScale = Vector3.one;

        return child;
    }

    public static Transform CreateChildForPrefab(this Transform _father, GameObject _prefab, string childname = "")
    {
        return _father.gameObject.CreateChildForPrefab(_prefab, childname).transform;
    }
    #endregion

    #region 数组操作
    /// <summary>
    /// 返回第一个相同变量的序列号 
    /// </summary>
    /// <param name="array"></param>
    /// <param name="_t"></param>
    /// <returns></returns>
    public static int GetValueIndex(this string[] array, string _t)
    {
        int index = -1;
        for (int i = 0, count = array.Length; i < count; i++)
        {
            if (array[i].Equals(_t))
            {
                index = i;
                break;
            }
        }
        return index;
    }

    public static int[] GetRandomSequence2(int total, int n) //取不重复的任意随机数
    {
        //随机总数组
        int[] sequence = new int[total];
        //取到的不重复数字的数组长度
        int[] output = new int[n];
        for (int i = 1; i < total; i++)
        {
            sequence[i] = i;
        }
        int end = total - 1;
        for (int i = 0; i < n; i++)
        {
            //随机一个数，每随机一次，随机区间-1
            int num = Random.Range(1, end + 1);
            output[i] = sequence[num];
            //将区间最后一个数赋值到取到数上
            sequence[num] = sequence[end];
            end--;
            //执行一次效果如：1，2，3，4，5 取到2
            //则下次随机区间变为1,5,3,4;
        }
        return output;
    }

    public static T[] ConcatArrays<T>(T[] firstArray, T[] secondArray) //合并数组
    {
        T[] result = new T[firstArray.Length + secondArray.Length];
        Array.Copy(firstArray, result, firstArray.Length);
        Array.Copy(secondArray, 0, result, firstArray.Length, secondArray.Length);
        return result;
    }

    /// <summary>
    /// 数组的2个元素位置调换
    /// </summary>
    public static void Swap<T>(this T[] array, int index1, int index2)
    {
        T temp = array[index2];
        array[index2] = array[index1];
        array[index1] = temp;
    }
    /// <summary>
    /// 列表的2个元素位置调换
    /// </summary>
    public static void Swap<T>(this List<T> list, int index1, int index2)
    {
        T temp = list[index2];
        list[index2] = list[index1];
        list[index1] = temp;
    }

    /// <summary>
    /// 乱序排序数组
    /// </summary>
    public static void SortRandom<T>(this T[] array)
    {
        int randomIndex;
        for (int i = array.Length - 1; i > 0; i--)
        {
            randomIndex = Random.Range(0, i);
            array.Swap(randomIndex, i);
        }
    }
   
    #endregion

    #region 时间格式转换
    /// <summary>
    /// 时间格式 0分:0秒 秒转分秒
    /// </summary>
    public static string NumberToMinute(int num)
    {
        string timeStr = "";
        if (num <= 0) return "0分0秒";
        if (num / 60 > 0) timeStr += Mathf.Floor(num / 60).ToString() + "分";
        else timeStr += "0分";
        if (num % 60 < 10) timeStr += "0" + (num % 60) + "秒";
        else timeStr += (num % 60).ToString() + "秒";
        return timeStr;
    }/// <summary>
     /// 时间格式 00:00 秒转分秒
     /// </summary>
    public static string ToTimeFormat(this int time)
    {
        int seconds = time;
        int minute = seconds % 3600 / 60;
        seconds = seconds % 3600 % 60;
        return string.Format("{0:D2}:{1:D2}", minute, seconds);
    }
    #endregion

    #region ---贝塞尔工具类

    public static Vector3[] GetPath(Vector3 p0, Vector3 p1, Vector3 p2, int count = 30)
    {
        Vector3[] path = new Vector3[count]; // 假设我们需要300个点来形成平滑的曲线
        for (int i = 0; i < path.Length; i++)
        {
            path[i] = BezierCurve(p0, p1, p2, i / (float)(path.Length - 1));
        }
        return path;
    }

    /// <summary>
    /// 线性贝塞尔曲线
    /// </summary>
    public static Vector3 BezierCurve(Vector3 p0, Vector3 p1, float t)
    {
        Vector3 B = Vector3.zero;
        B = (1 - t) * p0 + t * p1;
        return B;
    }

    /// <summary>
    /// 二阶贝塞尔曲线
    /// </summary>
    public static Vector3 BezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        Vector3 B = Vector3.zero;
        float t1 = (1 - t) * (1 - t);
        float t2 = 2 * t * (1 - t);
        float t3 = t * t;
        B = t1 * p0 + t2 * p1 + t3 * p2;
        return B;
    }

    /// <summary>
    /// 三阶贝塞尔曲线
    /// </summary>
    public static Vector3 BezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        Vector3 B = Vector3.zero;
        float t1 = (1 - t) * (1 - t) * (1 - t);
        float t2 = 3 * t * (1 - t) * (1 - t);
        float t3 = 3 * t * t * (1 - t);
        float t4 = t * t * t;
        B = t1 * p0 + t2 * p1 + t3 * p2 + t4 * p3;
        return B;
    }

    /// <summary>
    /// n阶贝塞尔曲线
    /// </summary>
    public static Vector3 BezierCurve(List<Vector3> pointList, float t)
    {
        Vector3 B = Vector3.zero;
        if (pointList == null)
        {
            return B;
        }
        if (pointList.Count < 2)
        {
            return pointList[0];
        }

        List<Vector3> tempPointList = new List<Vector3>();
        for (int i = 0; i < pointList.Count - 1; i++)
        {
            Vector3 tempPoint = BezierCurve(pointList[i], pointList[i + 1], t);
            tempPointList.Add(tempPoint);
        }
        return BezierCurve(tempPointList, t);
    }
    #endregion

    /// <summary>
    /// 世界转Ui坐标
    /// </summary>
    public static void SetUiPos(Transform traget, RectTransform canvasRect, GameObject Ui, float offset = -0.65f)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(traget.position);
        //RectTransform canvasRect = MainCanvas.GetComponent<RectTransform>();
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRect, screenPosition, Camera.main, out Vector3 worldPoint))
        {
            Ui.transform.position = worldPoint + new Vector3(0, 0, offset);
        }
    }

    public static bool IsMouseOverUI(RectTransform rectTransform)//判定是在UI区域内
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;
        if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, pointerEventData.position, Camera.main))
            return true;
        return false;
    }

    public static bool CheckClipName(Animator animator, string clipName)//检测动画片段
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        for (int i = 0; i < clips.Length; i++)
        {
            if (clipName == clips[i].name)
                return true;
        }
        return false;
    }

    /// <summary>
    /// string 转枚举
    /// </summary>
    public static T ToEnum<T>(this string str)
    {
        return (T)Enum.Parse(typeof(T), str);
    }


    public static void RunTimePlatFromCallBack()
    {
        string platform = string.Empty;
#if UNITY_EDITOR
        platform = "hi,大家好,我是在unity编辑模式下";
#elif UNITY_XBOX360
       platform="hi，大家好,我在XBOX360平台";  
#elif UNITY_IPHONE
       platform="hi，大家好,我是IPHONE平台";  
#elif UNITY_ANDROID
       platform="hi，大家好,我是ANDROID平台";  
#elif UNITY_STANDALONE_OSX
       platform="hi，大家好,我是OSX平台";  
#elif UNITY_STANDALONE_WIN
       platform="hi，大家好,我是Windows平台";  
#endif
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            Debug.Log("Current Platform:" + platform);
        }
        if (Application.platform == RuntimePlatform.Android)
        {
            Debug.Log("Current Platform:" + platform);
        }
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Debug.Log("Current Platform:" + platform);
        }
        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            Debug.Log("Current Platform:" + platform);
        }
        Debug.Log("Current Platform:" + platform);
    }
}

public static class DOTweenTool
{
    //放大缩小
    public static void ScaleLoopOnce(Transform target, float toScale, float baScale, float toTime, float backTime, UnityAction cb = null)
    {
        target.DOScale(Vector3.one * toScale, toTime).OnComplete(delegate
        {
            target.DOScale(Vector3.one * baScale, backTime).OnComplete(delegate
            {
                cb?.Invoke();
            });
        });
    }

    public static void ScaleLoopOnce2(Transform target, float toScale, float baScale, float toTime, float backTime, UnityAction startCb = null, UnityAction endCb = null)
    {
        target.DOScale(Vector3.one * toScale, toTime)
        .OnStart(delegate
        {
            startCb?.Invoke();
        })
        .OnComplete(delegate
        {
            target.DOScale(Vector3.one * baScale, backTime).OnComplete(delegate
            {
                endCb?.Invoke();
            });
        });
    }

    public static void Scale(Transform target, float toScale, float toTime, UnityAction cb = null)
    {
        target.DOScale(Vector3.one * toScale, toTime).OnComplete(delegate
        {
            cb?.Invoke();
        });
    }

    public static void QScale(Transform target, float toScale, float toTime, UnityAction cb = null)
    {
        target.DOScale(Vector3.one * toScale, toTime).SetEase(Ease.InOutBack).OnComplete(delegate
        {
            cb?.Invoke();
        });
    }

    //移入移出
    public static void MoveToX(Transform target, float dis, float time, UnityAction cb = null)
    {
        target.DOLocalMoveX(dis, time).OnComplete(delegate
        {
            cb?.Invoke();
        });
    }
    public static void MoveToY(Transform target, float dis, float time, UnityAction cb = null)
    {
        target.DOLocalMoveY(dis, time).OnComplete(delegate
        {
            cb?.Invoke();
        });
    }
    public static void MoveToZ(Transform target, float dis, float time, UnityAction cb = null)
    {
        target.DOLocalMoveZ(dis, time).OnComplete(delegate
        {
            cb?.Invoke();
        });
    }


    //旋转
    public static void RotateZ(Transform target, float angle, float time)
    {
        target.DOLocalRotate(new Vector3(0, 0, angle), time).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
    }
    //淡出(变透明)
    public static void Fade(Image target, float value, float time, UnityAction cb = null)
    {
        target.DOFade(value, time).SetEase(Ease.Linear).OnComplete(delegate
        {
            cb?.Invoke();
        });
    }

    //插值动画
    public static Tweener DoTweenTo(float start, float end, float time, Action<float> update, Action finish)
    {
        return DOTween.To(() => start, x => start = x, end, time)
                 .SetEase(Ease.Linear)
                 .OnUpdate(delegate
                 {
                     update(start);
                 })
                 .OnComplete(delegate
                 {
                     finish();
                 });
    }
}
