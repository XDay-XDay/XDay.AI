using UnityEngine;

namespace XDay.AI
{
    internal class SteeringForceSeek : SteeringForce, ISteeringForceSeek
    {
        public SteeringForceSeek(SteeringForceConfig config) : base(config)
        {
        }

        public override Vector3 Calculate(IAgent agent, float dt)
        {
            var target = GetTarget(agent);
            if (target == null)
            {
                return Vector3.zero;
            }

            var curVelocity = agent.LinearVelocity;
            var targetToMe = target.TargetPosition - agent.Position;
            if (targetToMe == Vector3.zero)
            {
                return Vector3.zero;
            }
            targetToMe.Normalize();
            var desired = targetToMe * agent.MaxLinearAcceleration - curVelocity;
            return desired;
        }

        public void SetOverridenTarget(IAgentTarget target)
        {
            m_OverriddenTarget = target;
        }

        private IAgentTarget GetTarget(IAgent agent)
        {
            if (m_OverriddenTarget == null)
            {
                return agent.Target;
            }
            return m_OverriddenTarget;
        }

        private IAgentTarget m_OverriddenTarget;
    }
}
