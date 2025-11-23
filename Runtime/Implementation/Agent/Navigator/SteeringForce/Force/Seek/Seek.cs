using UnityEngine;

namespace XDay.AI
{
    internal class SteeringForceSeek : SteeringForce, ISteeringForceSeek
    {
        public override Vector3 Calculate(IAgent agent, float dt)
        {
            var curVelocity = agent.LinearVelocity;
            var targetToMe = GetTarget() - agent.Position;
            targetToMe.Normalize();
            var desired = targetToMe * agent.MaxLinearAcceleration - curVelocity;
            return desired;
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

        private Vector3 m_TargetPosition;
        private Transform m_TargetTransform;
    }
}
