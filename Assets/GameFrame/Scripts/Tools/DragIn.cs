
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragIn : MonoBehaviour, IDragHandler, IPointerDownHandler, IEndDragHandler
{
    public Vector3 OriPos;
    public float progress = 0.34f;
    public Vector3 TargetPos;
    Vector3 mousePos;
    Vector3 panelPos;
    Vector3 offset;
    RectTransform Rect;
    private void Awake()
    {
        OriPos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
    }

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
        if (Vector3.Distance(transform.localPosition, TargetPos) <= 50)
        {
            transform.localPosition = TargetPos;
            Messenger.Broadcast("ShowProgress", progress);          
            GetComponent<Image>().raycastTarget = false;
            this.enabled = false;
        }
        else
        {
            transform.localPosition = OriPos;
        }
    }
}