using RVO;
using System;
using UnityEngine;

namespace XDay.AI
{
    internal class NavigatorRVO : INavigatorRVO
    {
        public NavigatorRVO()
        {
            m_DistanceOperator = (distance) =>
            {
                return distance * distance;
            };
        }

        public void OnDestroy()
        {
            Simulator.Instance.removeAgent(m_RVOAgentID);
            m_RVOAgentID = -1;
        }

        public void SetAgent(IAgent agent)
        {
            Debug.Assert(agent != null, "Agent is null");
            m_Agent = agent;
            m_RVOAgentID = Simulator.Instance.addAgent(ToVector2(agent.Position), agent.MaxLinearSpeed, agent.ColliderRadius, ToVector2(agent.LinearVelocity));
        }

        public void Update(float dt)
        {
            var simulator = Simulator.Instance;
            m_Agent.Position = ToVector3(simulator.getAgentPosition(m_RVOAgentID));

            Vector3 desired = GetTarget() - m_Agent.Position;
            float distance = desired.magnitude;
            if (distance > 0)
            {
                desired.Normalize();
                if (distance < m_SlowDistance)
                {
                    var delta = distance / m_SlowDistance;
                    float speed = m_Agent.MaxLinearSpeed * m_DistanceOperator(delta);
                    desired *= speed;
                }
                else
                {
                    desired *= m_Agent.MaxLinearSpeed;
                }

                simulator.setAgentPrefVelocity(m_RVOAgentID, ToVector2(desired));
            }

            var rvoVelocity = simulator.getAgentVelocity(m_RVOAgentID);
            m_Agent.LinearVelocity = ToVector3(rvoVelocity);
        }

        public void DrawGizmos()
        {
        }

        public void SetSlowDistance(float distance)
        {
            m_SlowDistance = distance;
        }

        public void SetTarget(Vector3 position)
        {
            m_TargetPosition = position;
        }

        public void SetTarget(Transform target)
        {
            m_TargetTransform = target;
        }

        private Vector3 GetTarget()
        {
            if (m_TargetTransform == null)
            {
                return m_TargetPosition;
            }
            return m_TargetTransform.position;
        }

        public void SetDistanceOperator(Func<float, float> op)
        {
            m_DistanceOperator = op;
        }

        private Vector3 ToVector3(RVO.Vector2 v)
        {
            return new Vector3(v.x(), 0, v.y());
        }

        private RVO.Vector2 ToVector2(Vector3 pos)
        {
            return new RVO.Vector2(pos.x, pos.z);
        }

        private int m_RVOAgentID = -1;
        private Vector3 m_TargetPosition;
        private Transform m_TargetTransform;
        private float m_SlowDistance = 3f;
        private Func<float, float> m_DistanceOperator;
        private IAgent m_Agent;
    }
}
