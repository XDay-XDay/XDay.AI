using System.Collections.Generic;
using UnityEngine;

namespace XDay.AI
{
    public class LineDetector
    {
        public Vector3 LocalDirection { get; set; }
        public Vector3 WorldDirection { get; internal set; }
        public float Length { get; set; }
        public int CollisionLayerMask { get; set; }
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
        /// <summary>
        /// 数据不再有效,用于对Agent的异步操作时判断Agent是否还存在
        /// </summary>
        bool Invalid { get; set; }
        int CurrentLOD { get; }
        string Name { get; set; }
        Vector3 Position { get; set; }
        Quaternion Rotation { get; set; }
        Vector3 LinearVelocity { get; set; }
        float MaxLinearAcceleration { get; set; }
        float MaxLinearHorizontalSpeed { get; }
        float MaxAngularSpeed { get; }
        float ReachDistance { get; set; }
        float ReachColliderDistance { get; }
        float ColliderRadius { get; set; }
        INavigator Navigator { get; }
        IWorld World { get; }
        IAgentTarget Target { get; set; }
        AgentConfig Config { get; }
        Vector3 TargetPosition { get; }
        Vector3 TargetVelocity { get; }
        /// <summary>
        /// 可能为空
        /// </summary>
        Transform Root { get; }

        void Init();

        void OnDestroy();

        void AddForce(Vector3 force, ForceMode mode) { }
        void Stop();

        void Update(float dt) { }
        void FixedUpdate() { }

        List<LineDetector> GetLineDetectors();
        void AddLineDetector(LineDetector detector);
        void RemoveLineDetector(int index);

        Vector3 LocalToWorld(Vector3 localPos);

        void DrawGizmos();
    }

    public interface ICharacterControllerAgent : IAgent
    {
        bool IsGrounded { get; }
        bool MoveByForce { get; }
        CharacterController Controller { get; }
    }

    public interface IAgentRenderer
    {
        IAgent Agent { get; }
        GameObject GameObject { get; }

        void ChangeLOD(int lod);
        void Init(IAgent agent);
        void OnDataChange(IAgentRenderer renderer);
        void OnDestroy();
        void Update(float dt);
        void SetActive(bool active);
        void SetGameObject(GameObject gameObject);
    }

    public interface IAgentTarget
    {
        Vector3 TargetPosition { get; set; }
        Vector3 TargetVelocity { get; }
        Transform TargetTransform { get; set; }
    }
}