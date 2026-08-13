using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;


/*****************************************
	 文件:   AssetBundleMgr.cs
	 作者:   Siran
	 日期:   2024/11/25 18:9:59
	 功能:   Nothing
 *****************************************/
[HelpURL("https://github.com/Siran1994")]
public class AssetBundleMgr : MonoSigleton<AssetBundleMgr>
{
    new void Awake()
    {
        base.Awake();
    }

    public string url = "http://allgame.test.efunent.com/group2/test1/";


    public void LoadAssetsBundle<T>(string bundleName, string assetsName, UnityAction<T> fun) where T : Object
    {
        StartCoroutine(LoadBundle(bundleName, assetsName, fun));
    }

    public void LoadSceneBundle(string bundleName, int sceneId, UnityAction fun)
    {
        StartCoroutine(LoadScene(bundleName, sceneId, fun));
    }

    IEnumerator LoadBundle<T>(string bundleName, string assetsName, UnityAction<T> fun) where T : Object
    {
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(new System.Uri(url + bundleName));
        yield return request.SendWebRequest();

        if (request.error != null)
            print("下载失败" + request.error);
        else
        {
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
            AssetBundleRequest loadOperation = bundle.LoadAssetAsync<T>(assetsName);
            yield return loadOperation;
            if (loadOperation.asset && loadOperation.asset is GameObject)
                fun(Instantiate(loadOperation.asset) as T);
            else
                fun(loadOperation.asset as T);
            bundle.Unload(false);
        }
    }

    private AsyncOperation asyncOperation;

    IEnumerator LoadScene(string bundleName, int sceneId, UnityAction fun)
    {
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(new System.Uri(url + bundleName));
        yield return request.SendWebRequest();

        if (request.error != null)
            print("下载失败" + request.error);
        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
        //LevelMgr.Instance.StartCoroutine(LevelMgr.Instance.LoadSceneAsync(sceneId, () =>
        //{
        //    fun();
        //    print("场景加载成功!");
        //}));
    }
}
