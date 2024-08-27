using UnityEngine;

namespace ChessGame
{
    public class RandomPointInCircle : PlacementStrategy
    {
        private float _radius;

        /// <summary>
        /// Gets a random point within a radius
        /// </summary>
        /// <param name="radius"></param>
        public RandomPointInCircle(float radius)
        {
            _radius = radius;
        }
        
        public override Vector3 GetPosition(Vector3 origin)
        {
            Vector3 random = Random.insideUnitSphere * _radius;
            Vector3 position = new Vector3(random.x, origin.y, random.z);
            
            return origin + position;
        }
    }
    
    
}