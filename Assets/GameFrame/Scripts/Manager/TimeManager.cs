using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoSigleton<TimeManager>
{
    protected override void Awake()
    {
        base.Awake();
    }
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
    public void DelayCallBack(float time, Action action = null)
    {
        StartCoroutine(WaitForSecondsAction(time, action));
    }

    public void DelayForEndOfFrameAction(Action action = null)
    {
        StartCoroutine(WaitForEndOfFrameAction(action));
    }

    public static IEnumerator Timer(float Seconds, Action callback)
    {
        yield return new WaitForSeconds(Seconds);
        callback();
    }

    IEnumerator WaitForSecondsAction(float time, Action action = null)//会随对象销毁而销毁
    {
        yield return new WaitForSeconds(time);
        if (action != null)
        {
            action.Invoke();
        }
    }

    IEnumerator WaitForEndOfFrameAction(Action action = null)
    {
        yield return new WaitForEndOfFrame();
        if (action != null)
        {
            action.Invoke();
        }
    }

    public void DelayAction(float Delay, Action CallBack)//不会随对象销毁而销毁
    {
        Observable.Timer(TimeSpan.FromSeconds(Delay))
           .Subscribe(delegate
           {
               CallBack();
           })
           .AddTo(this);
    }

    IDisposable intervalDisposable = null;
    public void Timer(Text timeTxt, int totalTime, Action action)//倒计时
    {
        intervalDisposable = Observable.Interval(TimeSpan.FromSeconds(1))
         .Subscribe(_ =>
         {
             totalTime -= 1;
             timeTxt.text = MyTools.NumberToMinute(totalTime);

             if (totalTime <= 0)
             {
                 action.Invoke();
                 totalTime = 0;
                 intervalDisposable.Dispose();
             }
         }).AddTo(this);
    }
}
