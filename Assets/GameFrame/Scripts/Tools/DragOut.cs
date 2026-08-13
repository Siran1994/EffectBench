using DG.Tweening;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragOut : MonoBehaviour, IDragHandler, IPointerDownHandler, IEndDragHandler
{
    public float progress = 0.34f;
    Vector3 mousePos;
    Vector3 panelPos;
    Vector3 offset;
    RectTransform Rect;

    public bool IsMoveOut = true;

    void Start()
    {
        Rect = GetComponent<RectTransform>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        mousePos = Input.mousePosition;
        panelPos = transform.position;
        offset = mousePos - panelPos;
        Rect.SetAsLastSibling();
        AudioMgr.Instance.Play("drag");
    }
    public void OnDrag(PointerEventData eventData)
    {
        Vector3 worldPos;
        RectTransform canvasRect = UiManager.Instance.MainCanvas.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRect, Input.mousePosition, Camera.main, out worldPos);
        transform.position = worldPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Messenger.Broadcast("ShowProgress", progress);      
        if (IsMoveOut)
            DOTweenTool.MoveToX(transform, 1200, 0.5f, delegate { Destroy(gameObject); });
        else
        {
            this.enabled = false;
            GetComponent<Image>().raycastTarget = false;
        }
    }
}
