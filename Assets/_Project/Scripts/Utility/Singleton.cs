using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    protected static T instance;
    public static bool HasInstance { get { return instance != null; } }

    public static T TryGetInstance() 
    {
        return HasInstance ? instance : null;
    }

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();

                if (instance == null)
                {
                    var gameObject = new GameObject(typeof(T).Name);
                    instance = gameObject.AddComponent<T>();

                }
            }

            return instance;
        }
    }

    protected virtual void Awake()
    {
        InitializeSingleton();
    }

    protected virtual void InitializeSingleton()
    {
        if (!Application.isPlaying) return;

        instance = this as T;
    }
}
