using System.Collections.Generic;
using UnityEngine;

namespace XDay.AI
{
    [AgentLabel(typeof(Physics3DAgentCreateInfo))]
    internal class Physics3DAgent : IAgent
    {
        public int ID => m_ID;
        public Vector3 Position { get => m_Rigidbody.position; set => m_Rigidbody.position = value; }
        public Quaternion Rotation { get => m_Rigidbody.rotation; set => m_Rigidbody.rotation = value; }
        public Vector3 LinearVelocity { get => m_Rigidbody.linearVelocity; set => m_Rigidbody.linearVelocity = value; }
        public float MaxLinearSpeed { get => m_Rigidbody.maxLinearVelocity; set => m_Rigidbody.maxLinearVelocity = value; }
        public float MaxAngularSpeed { get => m_Rigidbody.maxAngularVelocity; set => m_Rigidbody.maxAngularVelocity = value; }
        public float MaxLinearAcceleration { get => m_MaxLinearAcceleration; set => m_MaxLinearAcceleration = value; }
        public float ReachDistance { get => m_ReachDistance; set => m_ReachDistance = value; }
        public float ReachColliderDistance { get => m_ReachDistance + m_ColliderRadius; }
        public float ColliderRadius { get => m_ColliderRadius; set => m_ColliderRadius = value; }
        public INavigator Navigator { get => m_Navigator; set => m_Navigator = value; }
        public IWorld World => m_World;

        public Physics3DAgent(int id, Physics3DAgentCreateInfo createInfo, IWorld world)
        {
            m_ID = id;
            m_Rigidbody = createInfo.Rigidbody;
            m_Rigidbody.detectCollisions = createInfo.EnableCollision;
            MaxLinearSpeed = createInfo.MaxLinearSpeed;
            MaxAngularSpeed = createInfo.MaxAngularSpeed;
            Position = createInfo.Position;
            ReachDistance = createInfo.ReachDistance;
            m_ColliderRadius = createInfo.ColliderRadius;
            m_World = world;
        }

        public void OnDestroy()
        {
        }

        public void AddForce(Vector3 force, ForceMode mode)
        {
            m_Rigidbody.AddForce(force, mode);
        }

        public void Update(float dt)
        {
            m_Navigator.Update(dt);

            var dir = Quaternion.LookRotation(LinearVelocity, Vector3.up);
            Rotation = Quaternion.RotateTowards(Rotation, dir, MaxAngularSpeed * dt);

            UpdateLineDetectors();
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

        private void UpdateLineDetectors()
        {
            foreach (var lineDetector in m_LineDetectors)
            {
                lineDetector.WorldDirection = Rotation * lineDetector.LocalDirection;
            }
        }

        public void DrawGizmos()
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

        private readonly int m_ID;
        private readonly IWorld m_World;
        private readonly UnityEngine.Rigidbody m_Rigidbody;
        private INavigator m_Navigator;
        private float m_MaxLinearAcceleration = 5f;
        private float m_ReachDistance = 0.5f;
        private float m_ColliderRadius = 0.5f;
        private readonly List<LineDetector> m_LineDetectors = new();
    }
}