using UnityEngine;
public class UiManager : MonoSigleton<UiManager>
{
    public Canvas MainCanvas;

    protected override void Awake()
    {
        base.Awake();
    }



    public void SetUiPos(Transform traget, GameObject Ui, float offset = -0.65f)//世界转Ui
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(traget.position);
        RectTransform canvasRect = MainCanvas.GetComponent<RectTransform>();
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRect, screenPosition, Camera.main, out Vector3 worldPoint))
        {
            Ui.transform.position = worldPoint + new Vector3(0, 0, offset);
        }
    }
}
