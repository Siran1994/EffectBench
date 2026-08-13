using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TipManager : MonoSigleton<TipManager>
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    bool isCanShow = true;

    public void ShowTips(string tips, float interval = 1, UnityAction cb = null)//提示
    {
        if (isCanShow)
        {
            isCanShow = false;
            var go = PoolManager.GetNode(PrefabManager.get("Tips", PrefabManager.uiMap), transform);
            if (go == null)
                return;
            go.GetComponentInChildren<Text>().text = tips;
            go.GetComponent<Image>().DOFade(1, 0f);
            go.GetComponentInChildren<Text>().DOFade(1, 0f);
            go.transform.localScale = Vector3.one;
            go.transform.rotation = new Quaternion(0, 0, 0, 0);
            go.transform.localPosition = new Vector3(0, 130, 0);
            go.transform.DOLocalMoveY(250f, 1f)
                 .SetEase(Ease.InOutQuad)
                 .OnComplete(delegate
                 {
                     go.GetComponent<Image>().DOFade(0, 0.5f);
                     go.GetComponentInChildren<Text>().DOFade(0, 0.5f)
                     .OnComplete(
                     delegate
                     {
                         cb?.Invoke();
                         PoolManager.PutNode(go);
                     });
                 });
            TimeManager.Instance.DelayCallBack(interval, () => { isCanShow = true; });
        }
    }

    public void MakeToast(string str = "暂无广告!!!")//吐司提示
    {
        Debug.Log(str);
        if (Application.platform != RuntimePlatform.Android)
            return;
        AndroidJavaObject currentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaClass Toast = new AndroidJavaClass("android.widget.Toast");
        currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
        {
            Toast.CallStatic<AndroidJavaObject>("makeText", currentActivity, str, Toast.GetStatic<int>("LENGTH_LONG")).Call("show");
        }));
    }
}
