using System;
using UnityEngine;

namespace XDay.AI
{
    internal class SteeringForceArrive : SteeringForce, ISteeringForceArrive
    {
        public SteeringForceArrive()
        {
            m_DistanceOperator = (distance) =>
            {
                return distance * distance;
            };
        }

        public override Vector3 Calculate(IAgent agent, float dt)
        {
            Vector3 desired = GetTarget() - agent.Position;
            float distance = desired.magnitude;
            if (distance > 0)
            {
                desired.Normalize();
                if (distance < m_SlowDistance)
                {
                    var delta = distance / m_SlowDistance;
                    float speed = agent.MaxLinearSpeed * m_DistanceOperator(delta);
                    desired *= speed;
                }
                else
                {
                    desired *= agent.MaxLinearSpeed;
                }
                return desired - agent.LinearVelocity;
            }
            return Vector3.zero;
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

        internal bool ReachTarget(IAgent agent)
        {
            var delta = GetTarget() - agent.Position;
            var distanceSqr = delta.sqrMagnitude;
            if (distanceSqr <= agent.ReachColliderDistance * agent.ReachColliderDistance)
            {
                return true;
            }
            return false;
        }

        private Vector3 m_TargetPosition;
        private Transform m_TargetTransform;
        private float m_SlowDistance;
        private Func<float, float> m_DistanceOperator;
    }
}
