using UnityEngine;

namespace ChessGame
{
    public abstract class Service : MonoBehaviour
    {
        /// <summary>
        /// Registers a new Service to the service manager
        /// </summary>
        private void Register()
        {
            ServiceManager.AddService(this);
        }

        /// <summary>
        /// Unregisters this serve from the service manager
        /// </summary>
        private void Unregister()
        {
            ServiceManager.RemoveService(this);
        }

        protected virtual void Awake()
        {
            Register();
        }

        private void OnDestroy()
        {
            Unregister();
        }
    }
}