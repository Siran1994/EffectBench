public abstract class Singleton<T> : System.IDisposable where T : new()
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new T();
            }
            return instance;
        }
    }

    public virtual void Start()
    {

    }

    public virtual void Dispose()
    {

    }
}