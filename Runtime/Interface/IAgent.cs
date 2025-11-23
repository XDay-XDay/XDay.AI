using System.Collections.Generic;
using UnityEngine;

namespace XDay.AI
{
    public class LineDetector
    {
        public Vector3 LocalDirection { get; set; }
        public Vector3 WorldDirection { get; internal set; }
        public float Length { get; set; }
        public int ColliderLayer { get; set; }
        public bool Hit { get; set; }
    }

    public struct HitInfo
    {
        public Vector3 Point;
        public Vector3 Normal;
        public float Distance;
    }

    public interface IAgent
    {
        int ID { get; }
        Vector3 Position { get; set; }
        Quaternion Rotation { get; set; }
        Vector3 LinearVelocity { get; set; }
        float MaxLinearAcceleration { get; set; }
        float MaxLinearSpeed { get; }
        float MaxAngularSpeed { get; }
        float ReachDistance { get; set; }
        float ReachColliderDistance { get; }
        float ColliderRadius { get; set; }
        INavigator Navigator { get; set; }
        IWorld World { get; }

        void OnDestroy();

        void AddForce(Vector3 force, ForceMode mode);

        void Update(float dt);

        List<LineDetector> GetLineDetectors();
        void AddLineDetector(LineDetector detector);
        void RemoveLineDetector(int index);

        void DrawGizmos();
    }

    public interface IAgentCreateInfo
    {
    }

    public class Physics3DAgentCreateInfo : IAgentCreateInfo
    {
        public UnityEngine.Rigidbody Rigidbody { get; set; }
        public Vector3 Position { get; set; }
        public float MaxLinearSpeed { get; set; } = 10f;
        public float MaxAngularSpeed { get; set; } = 360;
        public bool EnableCollision { get; set; } = false;
        public float ReachDistance { get; set; } = 0.5f;
        public float ColliderRadius { get; set; } = 0.5f;
    }
}