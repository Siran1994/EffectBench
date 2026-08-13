using System;
using System.Collections;
using UnityEngine;

/*****************************************
	 文件:   CoroutineMgr.cs
	 作者:   Siran
	 日期:   2022/2/21 10:2:28
	 功能:   Nothing
 *****************************************/
 [HelpURL("https://github.com/Siran1994")]
public class CoroutineMgr : MonoSigleton<CoroutineMgr>
{
    CoroutineUnit mCorouineUnit;
    public void Init(GameObject go)
    {
        if (mCorouineUnit==null)
        {
            mCorouineUnit = go.AddComponent<CoroutineUnit>();
        }
    }

    public Coroutine StartCoroutine(IEnumerator coro)
    {
        if (null==mCorouineUnit||null==coro)
        {
            return null;
        }
        return mCorouineUnit.StartCoroutine(coro);
    }

    public void StopCoroutine(Coroutine coro)
    {
        if (null==mCorouineUnit)
        {
            return;
        }
        mCorouineUnit.StopCoroutine(coro);
    }

    public void StopAllCoroutine()
    {
        if (null ==mCorouineUnit)
        {
            return;
        }
        mCorouineUnit.StopAllCoroutines();
    }

    IEnumerator WaitForSecondsAction(float time, Action action = null)
    {
        yield return new WaitForSeconds(time);
        if (action !=null)
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

    public void DelayForSecondsAction(float time, Action action = null)
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
}
public class CoroutineUnit : MonoBehaviour
{
     
}