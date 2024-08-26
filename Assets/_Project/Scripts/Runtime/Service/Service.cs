using UnityEngine;

public abstract class Service : MonoBehaviour
{
    private void Register()
    {
        ServiceManager.AddService(this);
    }
    protected void Unregister()
    {
        ServiceManager.AddService(this);
    }
    protected virtual void Awake()
    {
        Register();
    }

    private void OnDestroy()
    {
        ServiceManager.RemoveService(this);
    }
}