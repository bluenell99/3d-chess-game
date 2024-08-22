using UnityEngine;

namespace ChessGame
{
    public class RandomPointInCircle : PlacementStrategy
    {
        private float _radius;

        public RandomPointInCircle(float radius)
        {
            _radius = radius;
        }
        
        public override Vector3 GetPosition(Vector3 origin)
        {
            Debug.Log($"Origin: {origin}");
            Vector3 random = Random.insideUnitSphere * _radius;
            Debug.Log($"Random: {random}");
            Vector3 position = new Vector3(random.x, origin.y, random.z);
            Debug.Log($"Position: {position}");

            Vector3 final = origin + position;
            Debug.Log($"Position: {final}");
            return origin + position;
        }
    }
    
    
}