using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MainPanel : MonoBehaviour
{
    public Button StartBtn, UpCycleBtn;
    public Button ShareBtn, RewardBtn, PosBtn, AddDeskBtn;
    public Animator LoadAni;
    public Text toDayNum;

    private void Awake()
    {       
           
    }

    void Start()
    {
        StartBtn.onClick.AddListener(delegate //暂停
        {
            AudioMgr.Instance.Play("btn");
        
        });       
    }

     void OnEnable()
    {
        toDayNum.text = 2110000 + Random.Range(1, 9999) + "人";
       
    }

    public void LoadingAni(UnityAction cb)
    {
        LoadAni.gameObject.SetActive(true);
        LoadAni.Play("loading");
        TimeManager.Instance.DelayCallBack(0.8f, delegate { cb?.Invoke(); });
        TimeManager.Instance.DelayCallBack(1.6f, delegate { LoadAni.gameObject.SetActive(false); });
    }

    void OnToggleChanged(Toggle changedToggle)
    {
        // 只在Toggle被选中时打印名称  
        if (changedToggle.isOn)
        {
            Debug.Log("Current Toggle name: " + changedToggle.gameObject.name);
           
        }
    }    
}
