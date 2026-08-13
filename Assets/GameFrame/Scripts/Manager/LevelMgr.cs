using UnityEngine;
using UnityEngine.SceneManagement;
using System.Diagnostics;
//using DG.Tweening;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class LevelMgr : MonoSigleton<LevelMgr>
{
    static Stopwatch sw = new Stopwatch();
    public static string LoadTime = string.Empty;

    static Image image;

    static float a = 1;
    static bool IsLoad = false;
    static bool IsComplete = false;
    new void Awake()
    {
        image = GameObject.Find("SDKManager/Loading").GetComponent<Image>();
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        LoadScene(GameData.Lv);
    }
    static void SetColorAlpha(float alpha)
    {
        Debug.Log(image.name);
        alpha = Mathf.Clamp01(alpha);
        var c = image.color;
        c.a = alpha;
        image.color = c;
    }
    static int tmpLv;
    private static void LoadScene(int lv)
    {
        SetColorAlpha(0);
        tmpLv = lv;
        //image.DOFade(1, 1).OnComplete(delegate
        //{
        //    int nextScene = lv % (SceneManager.sceneCountInBuildSettings - 1);
        //    if (nextScene == 0)
        //        nextScene = SceneManager.sceneCountInBuildSettings - 1;

        //    sw.Reset();
        //    sw.Start();
        //    SceneManager.LoadSceneAsync(nextScene);
        //    SceneManager.sceneLoaded += CallBack;
        //});
        IsLoad = true;
        a = 0;
    }
    private void Update()
    {
        if (IsLoad)
        {
            a += Time.deltaTime;
            SetColorAlpha(a);
            if (a >= 1)
            {
                a = 1;
                int nextScene = tmpLv % (SceneManager.sceneCountInBuildSettings - 1);
                if (nextScene == 0)
                    nextScene = SceneManager.sceneCountInBuildSettings - 1;
                sw.Reset();
                sw.Start();
                SceneManager.LoadSceneAsync(nextScene);
                SceneManager.sceneLoaded += CallBack;
                IsLoad = false;
            }
        }
        if (IsComplete)
        {
            a -= Time.deltaTime;
            SetColorAlpha(a);
            if (a <= 0)
            {
                a = 0;
                IsComplete = false;
            }
        }
    }
    public static void CallBack(Scene scene, LoadSceneMode sceneType)
    {
        a = 1;
        SetColorAlpha(1);
        //image.DOFade(0, 1);
        IsComplete = true;
        LoadTime = string.Format("加载: {0} ms", sw.ElapsedMilliseconds);
        sw.Stop();
    }
    public static void LoadLv(int lv)
    {
        LoadScene(lv);
    }
    public static void LoadNextLv()
    {
        LoadScene(++GameData.Lv);
    }
    public static void LoadNextLv(int lv)
    {
        LoadScene(lv);
    }
    public static void ReLoadLv()
    {
        LoadScene(GameData.Lv);
    }
}