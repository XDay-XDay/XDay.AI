using System.Collections.Generic;
using UnityEngine;

namespace XDay.AI
{
    /// <summary>
    /// 无法移动的Agent
    /// </summary>
    public abstract class StaticAgent : IAgent
    {
        public int ID => m_ID;
        public int CurrentLOD => m_CurrentLOD;
        public string Name { get => m_Name; set => m_Name = value; }
        public Vector3 Position { get => m_Root.transform.position; set => m_Root.transform.position = value; }
        public Quaternion Rotation { get => m_Root.transform.rotation; set => m_Root.transform.rotation = value; }
        public Vector3 Scale { get => m_Root.transform.lossyScale; set => m_Root.transform.localScale = value; }
        public virtual Vector3 LinearVelocity { get => Vector3.zero; set { } }
        public virtual float MaxLinearHorizontalSpeed { get => 0; set { } }
        public virtual float MaxLinearVerticalSpeed { get => 0; set { } }
        public virtual float MaxAngularSpeed { get => 0; set { } }
        public virtual Transform Root => m_Root.transform;
        public virtual bool Invalid { get => m_Invalid; set => m_Invalid = value; }
        public float MaxLinearAcceleration { get => 0; set { } }
        public float ReachDistance { get => 0; set { } }
        public float ReachColliderDistance { get => 0; set { } }
        public float ColliderRadius { get => m_ColliderRadius; set => m_ColliderRadius = value; }
        public INavigator Navigator => null;
        public IWorld World => m_World;
        public IAgentTarget Target { get => null; set { } }
        public Vector3 TargetPosition => throw new System.NotImplementedException();
        public Vector3 TargetVelocity => throw new System.NotImplementedException();
        public AgentConfig Config => m_Config;
        public Quaternion TargetRotation
        {
            get => Rotation;
            set => Rotation = value;
        }
        public string Animation { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public StaticAgent(int id, AgentConfig config, IWorld world, Vector3 position, Quaternion rotation)
        {
            m_ID = id;
            m_Name = config.Name;
            m_World = world;
            m_Config = config;
            m_ColliderRadius = config.ColliderRadius;
            m_Root = new GameObject($"{m_Name}");
            Position = position;
            Rotation = rotation;
        }

        public virtual void Init()
        {
        }

        public virtual void OnDestroy()
        {
            Object.DestroyImmediate(m_Root);
            Invalid = true;
        }

        public virtual void Update(float dt)
        {
        }

        public virtual void FixedUpdate()
        {
        }

        public List<LineDetector> GetLineDetectors()
        {
            return null;
        }

        public void AddLineDetector(LineDetector detector)
        {
        }

        public void RemoveLineDetector(int index)
        {
        }

        public void DrawGizmos()
        {
        }

        public virtual void AddForce(Vector3 force, ForceMode mode)
        {
        }

        public virtual void Stop()
        {
        }

        public Vector3 LocalToWorld(Vector3 localPos)
        {
            var forward = Rotation * Vector3.forward;
            var right = Vector3.Cross(forward, Vector3.up);
            return Position + localPos.x * right + localPos.z * forward + localPos.y * Vector3.up;
        }

        private string m_Name;
        private readonly IWorld m_World;
        private readonly AgentConfig m_Config;
        private readonly int m_ID;
        private int m_CurrentLOD = 0;
        private float m_ColliderRadius;
        private GameObject m_Root;
        private bool m_Invalid = false;
    }
}