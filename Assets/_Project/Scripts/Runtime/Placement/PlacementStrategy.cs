using System;
using UnityEngine;

namespace ChessGame
{
    public class PlacementStrategy
    {
        /// <summary>
        /// Gets a placement position from a given origin point
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public virtual Vector3 GetPosition(Vector3 origin) => origin;

        public virtual Action DrawGizmo(Vector3 origin)
        {
            return () =>
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(origin, 0.2f);
            };
        }
    }
}