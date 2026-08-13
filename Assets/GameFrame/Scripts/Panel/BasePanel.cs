using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class BasePanel : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public Transform target = null;

    public void ShowPanel(float duration = 0.15f, UnityAction cb = null)
    {
        if (target.parent.name == "GamePanel")
            ShowFadeAni(duration, cb);
        else
            ShowScaleAni(duration, cb);
    }

    public void HidePanel(float duration = 0.1f, UnityAction cb = null)
    {
        HideScaleAni(duration, cb);
    }

    #region 位移动画 (下落)   
    public void ShowPosAni(float duration, UnityAction cb = null)
    {
        target.position = new Vector3(0, 3000, 0);
        gameObject.SetActive(true);
        target.DOLocalMoveX(0, duration).SetEase(Ease.OutQuint).OnComplete(() =>
        {
            target.position = Vector3.zero;
            cb?.Invoke();
        });
    }

    public void HidePosAni(float duration, UnityAction cb = null)
    {
        target.position = Vector3.zero;
        target.DOLocalMoveX(3000, duration).SetEase(Ease.OutQuint).OnComplete(() =>
        {
            target.position = new Vector3(0, 3000, 0);
            cb?.Invoke();
        });
    }
    #endregion

    #region 缩放动画
    public void ShowScaleAni(float duration, UnityAction cb = null)
    {
        target.localScale = Vector3.zero;
        gameObject.SetActive(true);
        target.DOScale(Vector3.one, duration).SetEase(Ease.OutQuint).OnComplete(() =>
        {
            target.localScale = Vector3.one;
            cb?.Invoke();
        });
    }

    public void HideScaleAni(float duration, UnityAction cb = null)
    {
        target.localScale = Vector3.one;
        target.DOScale(Vector3.zero, duration).SetEase(Ease.OutQuint).OnComplete(() =>
        {
            target.localScale = Vector3.zero;
            gameObject.SetActive(false);
            cb?.Invoke();
        });
    }
    #endregion

    #region 透明度
    public void ShowFadeAni(float duration, UnityAction cb = null)
    {
        canvasGroup.alpha = 0;
        gameObject.SetActive(true);
        canvasGroup.DOFade(1, duration).OnComplete(() =>
         {
             canvasGroup.alpha = 1;
             cb?.Invoke();
         });
    }

    public void HideFadeAni(float duration, UnityAction cb = null)
    {
        canvasGroup.alpha = 1;
        canvasGroup.DOFade(0, duration).OnComplete(() =>
         {
             canvasGroup.alpha = 0;
             gameObject.SetActive(false);
             cb?.Invoke();
         });
    }
    #endregion

}
