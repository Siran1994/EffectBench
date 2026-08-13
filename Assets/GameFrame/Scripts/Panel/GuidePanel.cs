using UnityEngine;
using UnityEngine.UI;
public class GuidePanel : MonoBehaviour, ICanvasRaycastFilter
{
    public Canvas canvas;
    public Text tipTxt;
    public RectTransform mask, hand;
    public Image Target;
    private Material _material;

    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        if (Target == null)
            return true;
        return !RectTransformUtility.RectangleContainsScreenPoint(Target.rectTransform, sp, eventCamera);
    }

    void Start()
    {
        if (GameData.GuideStep >= 1)
            HideGuide(true);

        // showGuide(UiManager.Instance.gamePanel.PauseBtn.gameObject, "点击暂停按钮");
        // GameData.GuideStep += 1;
    }

    private void OnEnable()
    {
        Messenger.AddListener<GameObject>("UseGuide", UseGuide);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener<GameObject>("UseGuide", UseGuide);
        if (_material)
        {
            _material.SetVector("_Center", Vector4.zero);
            _material.SetFloat("_SliderX", 0);
            _material.SetFloat("_SliderY", 0);
        }
    }

    void UseGuide(GameObject target)
    {
        if (target == null)
            return;
        switch (GameData.GuideStep)
        {
            case 1:
                //showGuide(target, "点击暂停按钮");
                HideGuide(true);
                break;
            case 2:
                HideGuide(true);
                break;
        }
    }

    void HideGuide(bool isDes = false)
    {
        if (isDes)
        {
            DestroyImmediate(gameObject);
            return;
        }
        else
        {
            mask.SetActive(false);
            hand.SetActive(false);
        }
    }

    void showGuide(GameObject target, string tip)
    {
        mask.SetActive(true);
        hand.SetActive(true);
        Target = target.GetComponent<Image>();
        hand.position = target.transform.position;
        if (tip != "")
            tipTxt.text = tip;
        else
            tipTxt.gameObject.SetActive(false);

        _material = mask.GetComponent<Image>().material;
        Vector4 centerMat = new Vector4(hand.localPosition.x, hand.localPosition.y, 0, 0);
        _material.SetVector("_Center", centerMat);
        _material.SetFloat("_SliderX", Target.rectTransform.sizeDelta.x / 2 + 5);
        _material.SetFloat("_SliderY", Target.rectTransform.sizeDelta.y / 2 + 5);
    }
}
