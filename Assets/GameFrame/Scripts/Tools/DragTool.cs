using UnityEngine;
using UnityEngine.EventSystems;

public class DragTool : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler, IEndDragHandler
{
    public int toolId = 0;
    public string audioName;
    public float progress = 0.34f;
    public Vector3 OriPos;
    public Vector3 OriSca;
    Vector3 mousePos;
    Vector3 panelPos;
    Vector3 offset;
    RectTransform Rect;
    private void Awake()
    {
        OriPos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        OriSca = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
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

        transform.localScale = OriSca * 1.2f;
        ShowChild(true);
        if (MyTools.IsMouseOverUI(transform as RectTransform))
        {
            Messenger.Broadcast("OnPointerDown", eventData, toolId);
            AudioMgr.Instance.PlayLoop(audioName);
        }
        

    }
    public void OnDrag(PointerEventData eventData)
    {
        Vector3 worldPos;
        RectTransform canvasRect = UiManager.Instance.MainCanvas.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRect, Input.mousePosition, Camera.main, out worldPos);
        transform.position = worldPos;

        if (MyTools.IsMouseOverUI(transform as RectTransform) )
            Messenger.Broadcast("OnDrag", eventData);

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.localPosition = OriPos;
        transform.localScale = OriSca;
        ShowChild(false);
    }


    void ShowChild(bool isShow)
    {
        if (transform.childCount > 0)
        {
            transform.GetChild(0).SetActive(isShow);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.localPosition = OriPos;
        transform.localScale = OriSca;
        ShowChild(false);
        if (MyTools.IsMouseOverUI(transform as RectTransform) )
            Messenger.Broadcast("OnPointerUp", eventData);
        AudioMgr.Instance.PlayStopLoop(audioName);
    }
}