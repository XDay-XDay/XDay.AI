using UnityEngine;

namespace XDay.AI
{
    internal class SteeringForceChase : SteeringForce, ISteeringForceChase
    {
        public SteeringForceChase(SteeringForceConfig config) : base(config)
        {
        }

        public override Vector3 Calculate(IAgent agent, float dt)
        {
            Vector3 toTarget = agent.TargetPosition - agent.Position;

            var targetVelocity = agent.TargetVelocity;
            if (targetVelocity != Vector3.zero)
            {
                float relativeSpeed = Vector3.Dot(targetVelocity - agent.LinearVelocity, toTarget.normalized);
                relativeSpeed = Mathf.Max(relativeSpeed, agent.MaxLinearHorizontalSpeed * 0.1f);
                var predictedTime = toTarget.magnitude / relativeSpeed;
                m_PredictedPosition = agent.TargetPosition + targetVelocity * predictedTime;
            }
            else
            {
                m_PredictedPosition = agent.TargetPosition;
            }

            Vector3 desiredVelocity = (m_PredictedPosition - agent.Position).normalized * agent.MaxLinearHorizontalSpeed;
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
    }
}
