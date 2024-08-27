using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ChessGame
{
    public class ServiceManager : Singleton<ServiceManager>
    {
        /// <summary>
        /// List of registered services
        /// </summary>
        private static readonly HashSet<Service> Services = new HashSet<Service>();

        /// <summary>
        /// Adds reference of this service to the Service Manager
        /// </summary>
        /// <param name="service"></param>
        public static void AddService(Service service)
        {
            Services.Add(service);
        }

        /// <summary>
        /// Removes reference of this service from the Service Manager
        /// </summary>
        /// <param name="service"></param>
        public static void RemoveService(Service service)
        {
            if (Services.Contains(service))
                Services.Remove(service);
        }

        /// <summary>
        /// Returns a service of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetService<T>() where T : Service
        {
            foreach (var service in Services.OfType<T>())
            {
                return service;
            }

            var gameObject = new GameObject(typeof(T).Name);
            gameObject.transform.parent = Instance.transform;
            return gameObject.AddComponent<T>();

        }
    }
}