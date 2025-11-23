using UnityEngine;

namespace XDay.AI
{
    public interface IObstacleManager
    {
        bool Raycast(Vector3 position, Vector3 direction, float length, int layerMask, out HitInfo hitInfo);
    }

    public interface IObstacleManagerCreateInfo
    {
    }

    public class PhysicsObstacleManagerCreateInfo : IObstacleManagerCreateInfo
    {
    }
}
