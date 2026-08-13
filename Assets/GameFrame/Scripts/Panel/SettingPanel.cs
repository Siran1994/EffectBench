using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : BasePanel
{
    public Button SetBtn;
    public Toggle AudioBtn, MusicBtn;
    public GameObject Logo;
    int count = 0;

    void Start()
    {
        AudioBtn.onValueChanged.AddListener((bool isOn) =>
        {
            AudioMgr.Instance.Play("btn");
            if (isOn)
                GameData.SoundOn = 1;
            else
                GameData.SoundOn = 0;
            AudioMgr.Instance.UpdateState();
        });

        MusicBtn.onValueChanged.AddListener((bool isOn) =>
       {
           AudioMgr.Instance.Play("btn");
           if (isOn)
               GameData.MusicOn = 1;
           else
               GameData.MusicOn = 0;
           AudioMgr.Instance.UpdateState();
       });

        SetBtn.onClick.AddListener(delegate //暂停
        {
            this.count++;
            if (this.count >= 5)
            {
                this.Logo.SetActive(true);
                this.count = 0;
            }
        });      
    }

    void OnEnable()
    {
        init();
    }

    void init()
    {
        if (GameData.SoundOn == 1)
            this.AudioBtn.isOn = true;
        else
            this.AudioBtn.isOn = false;

        if (GameData.MusicOn == 1)
            this.MusicBtn.isOn = true;
        else
            this.MusicBtn.isOn = false;
    }
}
