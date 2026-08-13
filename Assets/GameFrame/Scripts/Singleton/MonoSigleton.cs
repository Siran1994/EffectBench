using UnityEngine;

/*****************************************
	 文件:   MonoSigleton.cs
	 作者:   Siran
	 日期:   2021/3/8 11:30:21
	 功能:   继承自MonoBehavior的单例
 *****************************************/
public class MonoSigleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject(typeof(T).Name);
                instance = go.AddComponent<T>();
            }
            return instance;
        }
    }
    protected virtual void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this as T;
    }
}