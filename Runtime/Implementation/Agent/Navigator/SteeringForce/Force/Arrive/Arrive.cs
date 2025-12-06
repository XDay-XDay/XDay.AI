using System;
using UnityEngine;

namespace XDay.AI
{
    internal class SteeringForceArrive : SteeringForce, ISteeringForceArrive
    {
        public SteeringForceArrive(SteeringForceConfig config)
            : base(config)
        {
            var cfg = config as ISteeringForceArrive.Config;
            m_SlowDistance = cfg.SlowDistance;

            m_DistanceOperator = (distance) =>
            {
                return distance * distance;
            };
        }

        public override Vector3 Calculate(IAgent agent, float dt)
        {
            var target = GetTarget(agent);
            if (target == null)
            {
                return Vector3.zero;
            }

            Vector3 desired = target.TargetPosition - agent.Position;
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

        public void SetDistanceOperator(Func<float, float> op)
        {
            m_DistanceOperator = op;
        }

        public void SetOverriddenTarget(IAgentTarget target)
        {
            m_OverriddenTarget = target;
        }

        internal bool ReachTarget(IAgent agent)
        {
            var target = GetTarget(agent);
            var delta = target.TargetPosition - agent.Position;
            var distanceSqr = delta.sqrMagnitude;
            if (distanceSqr <= agent.ReachColliderDistance * agent.ReachColliderDistance)
            {
                return true;
            }

            return false;
        }

        private IAgentTarget GetTarget(IAgent agent)
        {
            if (m_OverriddenTarget == null)
            {
                return agent.Target;
            }
            return m_OverriddenTarget;
        }

        private float m_SlowDistance;
        private Func<float, float> m_DistanceOperator;
        private IAgentTarget m_OverriddenTarget;
    }
}
