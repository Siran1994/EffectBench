using UnityEngine.Events;

public class GameManager : MonoSigleton<GameManager>
{
    protected override void Awake()
    {
        base.Awake();
    }

    public bool IsStart = false;

    void Start()
    {
        AudioMgr.Instance.playMusic("lvBg");      
    }
}