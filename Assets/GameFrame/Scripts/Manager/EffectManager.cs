
using UnityEngine;
using UnityEngine.Events;

public class EffectManager : MonoSigleton<EffectManager>
{
    protected override void Awake()
    {
        base.Awake();
    }
    public GameObject Effect = null;
    public GameObject Circle = null;

    void Start()
    {
        PoolManager.PrePool(Effect, transform, 10);
        PoolManager.PrePool(Circle, transform, 1);
    }

    public void PlayEffect(string name, Vector3 pos, UnityAction cb = null)
    {
        var go = PoolManager.GetNode(name);
        go.transform.position = pos;
        go.gameObject.SetActive(true);
        var Par = go.GetComponent<ParticleSystem>();
        Par.Play();
        if (cb != null)
        {
            TimeManager.Instance.DelayCallBack(Par.main.duration, delegate
            {
                PoolManager.PutNode(go);
                cb?.Invoke();
            });
        }
        else
        {
            TimeManager.Instance.DelayCallBack(Par.main.duration, delegate
            {
                PoolManager.PutNode(go);
            });
        }
    }
}
