using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class GamePanel : BasePanel
{
    public Button PauseBtn;
    public Text TimerTxt;
 
    public DOTweenAnimation TimerTextEffect;
    public float GameTime = 0;

    public Button ClearBtn, PairBtn, DerageBtn;

    public Sprite add, red;


    void Init()
    {
        GameTime = Config.GameTime;      
    }
    void Start()
    {
        Init();

        PauseBtn.onClick.AddListener(delegate //暂停
        {
            AudioMgr.Instance.Play("btn");         
            GameManager.Instance.IsStart = false;
         
        });     

        PauseBtn.transform.DOLocalRotate(new Vector3(0, 0, 5), 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    private void OnEnable()
    {
        Messenger.AddListener<int, int>("UpdateTrashCanCount", UpdateTrashCanCount);
        Messenger.AddListener<int, bool>("ShowTrashInfo", ShowTrashInfo);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener<int, int>("UpdateTrashCanCount", UpdateTrashCanCount);
        Messenger.RemoveListener<int, bool>("ShowTrashInfo", ShowTrashInfo);
    }
    void UpdateTrashCanCount(int id, int count)
    {
    
    }

    void ShowTrashInfo(int id, bool isShow)
    {
      
    }

    void Update()
    {
        if (GameManager.Instance.IsStart)
        {
            Timer();
        }
    }

    void Timer()
    {
        if (GameTime > 0)
            GameTime -= Time.deltaTime;
        TimerTxt.text = MyTools.ToTimeFormat((int)GameTime);
        if (GameTime <= 10)
        {
            TimerTextEffect.DOPlay();
            if (TimerTxt.color != Color.red)
                TimerTxt.color = Color.red;
        }
        else
        {
            if (TimerTxt.color != Color.white)
                TimerTxt.color = Color.white;
        }
        if (GameTime <= 0)
        {
            TimerTextEffect.DOPause();
            GameTime = 0;
            //时间耗尽,复活           
        }
    }
}