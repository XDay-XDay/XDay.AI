using System.Collections.Generic;
using UnityEngine;

namespace XDay.AI
{
    public abstract class Agent : IAgent
    {
        public int ID => m_ID;
        public int CurrentLOD => m_CurrentLOD;
        public string Name { get => m_Name; set => m_Name = value; }
        public abstract Vector3 Position { get; set; }
        public abstract Quaternion Rotation { get; set; }
        public abstract Vector3 LinearVelocity { get; set; }
        public abstract float MaxLinearHorizontalSpeed { get; set; }
        public abstract float MaxLinearVerticalSpeed { get; set; }
        public abstract float MaxAngularSpeed { get; set; }
        public abstract Transform Root { get; }
        public virtual bool Invalid { get => m_Invalid; set => m_Invalid = value; }
        public float MaxLinearAcceleration { get => m_MaxLinearAcceleration; set => m_MaxLinearAcceleration = value; }
        public float ReachDistance { get => m_ReachDistance; set => m_ReachDistance = value; }
        public float ReachColliderDistance { get => m_ReachDistance + m_ColliderRadius; }
        public float ColliderRadius { get => m_ColliderRadius; set => m_ColliderRadius = value; }
        public INavigator Navigator { get => m_Navigator; }
        public IWorld World => m_World;
        public IAgentTarget Target { get => m_Target; set => m_Target = value; }
        public Vector3 TargetPosition
        {
            get
            {
                if (m_Target != null)
                {
                    return m_Target.TargetPosition;
                }
                return Vector3.zero;
            }
        }
        public Vector3 TargetVelocity
        {
            get
            {
                if (m_Target != null)
                {
                    return m_Target.TargetVelocity;
                }
                return Vector3.zero;
            }
        }
        public AgentConfig Config => m_Config;

        public Agent(int id, AgentConfig config, IWorld world, Vector3 position)
        {
            m_ID = id;
            m_Name = config.Name;
            m_ReachDistance = config.ReachDistance;
            m_ColliderRadius = config.ColliderRadius;
            m_World = world;
            m_Config = config;
            m_MaxLinearAcceleration = config.MaxLinearAcceleration;

            foreach (var lineDetector in config.LineDetectors)
            {
                var detector = new LineDetector()
                {
                    CollisionLayerMask = lineDetector.CollisionLayerMask,
                    Hit = false,
                    Length = lineDetector.Length,
                    LocalDirection = Quaternion.Euler(lineDetector.EulerAngle) * Vector3.forward,
                };
                AddLineDetector(detector);
            }
        }

        public virtual void Init()
        {
            var navigator = m_World.CreateNavigator(m_Config.Navigator, this);
            if (navigator == null)
            {
                Debug.LogError("Navigator is null");
            }
            m_Navigator = navigator;
        }

        public virtual void OnDestroy()
        {
            Invalid = true;
        }

        public virtual void Update(float dt)
        {
            m_Navigator?.Update(dt);

            var vel = LinearVelocity;
            vel.y = 0;
            if (vel != Vector3.zero)
            {
                var dir = Quaternion.LookRotation(vel, Vector3.up);
                Rotation = Quaternion.RotateTowards(Rotation, dir, MaxAngularSpeed * dt);
            }

            UpdateLineDetectors();
        }

        public virtual void FixedUpdate()
        {
        }

        public List<LineDetector> GetLineDetectors()
        {
            return m_LineDetectors;
        }

        public void AddLineDetector(LineDetector detector)
        {
            m_LineDetectors.Add(detector);
        }

        public void RemoveLineDetector(int index)
        {
            if (index >= 0 && index < m_LineDetectors.Count)
            {
                m_LineDetectors.RemoveAt(index);
            }
        }

        public virtual void DrawGizmos()
        {
            var old = Gizmos.color;
            foreach (var detector in m_LineDetectors)
            {
                if (detector.Hit)
                {
                    Gizmos.color = Color.red;
                }
                else
                {
                    Gizmos.color = Color.green;
                }
                Gizmos.DrawLine(Position, Position + detector.WorldDirection * detector.Length);
            }
            Gizmos.color = old;
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

        private void UpdateLineDetectors()
        {
            foreach (var lineDetector in m_LineDetectors)
            {
                lineDetector.WorldDirection = Rotation * lineDetector.LocalDirection;
            }
        }

        private readonly int m_ID;
        private int m_CurrentLOD = 0;
        private string m_Name;
        private readonly IWorld m_World;
        private INavigator m_Navigator;
        private IAgentTarget m_Target;
        private float m_ReachDistance = 0.5f;
        private float m_ColliderRadius = 0.5f;
        private float m_MaxLinearAcceleration = 100f;
        private bool m_Invalid = false;
        private readonly AgentConfig m_Config;
        private readonly List<LineDetector> m_LineDetectors = new();
    }
}