using UnityEngine;

namespace XDay.AI
{
    [ObstacleManagerLabel(typeof(PhysicsObstacleManagerCreateInfo))]
    internal class PhysicsObstacleManager : IObstacleManager
    {
        public PhysicsObstacleManager(PhysicsObstacleManagerCreateInfo createInfo)
        {
            Physics.queriesHitBackfaces = true;
        }

        public bool Raycast(Vector3 position, Vector3 direction, float length, int layerMask, out HitInfo hitInfo)
        {
            hitInfo = new();
            var hit = Physics.Raycast(position, direction, out var hi, length, layerMask);
            if (hit)
            {
                hitInfo.Distance = hi.distance;
                hitInfo.Normal = hi.normal;
                hitInfo.Point = hi.point;
            }

            return hit;
        }
    }
}
