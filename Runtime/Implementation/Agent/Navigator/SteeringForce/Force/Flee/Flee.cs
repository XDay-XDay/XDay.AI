using UnityEngine;

namespace XDay.AI
{
    internal class SteeringForceFlee : SteeringForce, ISteeringForceFlee
    {
        public SteeringForceFlee(SteeringForceConfig config) : base(config)
        {
            var cfg = config as ISteeringForceFlee.Config;
            m_ThreatDistance = cfg.ThreatDistance;
        }

        public override Vector3 Calculate(IAgent agent, float dt)
        {
            Vector3 toTarget = agent.TargetPosition - agent.Position;
            var distance = toTarget.magnitude;

            if (distance > m_ThreatDistance)
            {
                return Vector3.zero;
            }

            var targetVelocity = agent.TargetVelocity;
            if (targetVelocity != Vector3.zero)
            {
                float relativeSpeed = Vector3.Dot(targetVelocity - agent.LinearVelocity, toTarget.normalized);
                relativeSpeed = Mathf.Max(relativeSpeed, agent.MaxLinearHorizontalSpeed * 0.1f);
                var predictedTime = distance / relativeSpeed;
                m_PredictedPosition = agent.TargetPosition + targetVelocity * predictedTime;
            }
            else
            {
                m_PredictedPosition = agent.TargetPosition;
            }

            Vector3 desiredVelocity = (agent.Position - m_PredictedPosition).normalized * agent.MaxLinearHorizontalSpeed;
            Vector3 steer = desiredVelocity - agent.LinearVelocity;

            return steer;
        }

        protected override void OnDrawGizmos(IAgent agent)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(agent.Position, m_PredictedPosition);
            Gizmos.DrawSphere(m_PredictedPosition, 0.3f);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(agent.Position, agent.TargetPosition);
        }

        private Vector3 m_PredictedPosition;
        private float m_ThreatDistance;
    }
}
