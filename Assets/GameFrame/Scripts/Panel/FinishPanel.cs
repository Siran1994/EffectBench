using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FinishPanel : BasePanel
{ 
    public Button NextLvBtn;//下一关
    public Button ReStartBtn;//重开
    public Button ReLiveBtn;//复活  

    bool isAddTime = false;
    void Start()
    {
        NextLvBtn.onClick.AddListener(delegate
       {
           AudioMgr.Instance.Play("btn");
           HidePanel(0.1f, delegate
           {
               GameData.Lv += 1;
               if (GameData.Lv > 6)
                   GameData.Lv = 1;
               LevelMgr.LoadLv(2);
           });
       });

        ReStartBtn.onClick.AddListener(delegate
       {
           AudioMgr.Instance.Play("btn");
           HidePanel(0.1f, delegate
           {
               LevelMgr.LoadLv(2);
           });
       });      

        ReLiveBtn.onClick.AddListener(delegate
        {
            AudioMgr.Instance.Play("btn");
            HidePanel(0.1f, delegate
            {
               
            });
        });
    }

    public void ReturnHome()
    {
        AudioMgr.Instance.Play("btn");
        HidePanel(0.1f, delegate
        {
            LevelMgr.LoadLv(1);
        });
    }

    public void ShowPanel(FinishType finishType, UnityAction cb = null)
    {
        ShowPanel();    
        GameManager.Instance.IsStart = false;
        switch (finishType)
        {
            case FinishType.成功:
                AudioMgr.Instance.Play("success");
             
                break;
            case FinishType.失败:
                AudioMgr.Instance.Play("fail");
             
                break;
            case FinishType.没位置:
                AudioMgr.Instance.Play("fail");
              
                isAddTime = false;
                break;         
        }
        cb?.Invoke();
    }
}
