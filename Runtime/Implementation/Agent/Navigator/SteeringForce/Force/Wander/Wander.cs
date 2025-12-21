using UnityEngine;

namespace XDay.AI
{
    internal class SteeringForceWander : SteeringForce, ISteeringForceWander
    {
        public SteeringForceWander(SteeringForceConfig config) : base(config)
        {
            var cfg = config as ISteeringForceWander.Config;
            m_CircleRadius = cfg.CircleRadius;
            m_CircleDistance = cfg.CircleDistance;
            m_AngleChange = cfg.AngleChange;
        }

        public override Vector3 Calculate(IAgent agent, float dt)
        {
            Vector3 circleCenter = Vector3.forward * m_CircleDistance;
            float wanderX = Mathf.Cos(m_WanderAngle) * m_CircleRadius;
            float wanderZ = Mathf.Sin(m_WanderAngle) * m_CircleRadius;
            Vector3 wanderPoint = circleCenter + new Vector3(wanderX, 0, wanderZ);

            Vector3 worldWanderPoint = agent.LocalToWorld(wanderPoint);
            Vector3 desiredVelocity = (worldWanderPoint - agent.Position).normalized * agent.MaxLinearHorizontalSpeed;
            Vector3 steer = desiredVelocity - agent.LinearVelocity;

            m_WanderAngle += Random.Range(-m_AngleChange, m_AngleChange) * Mathf.Deg2Rad;

            return steer;
        }

        protected override void OnDrawGizmos(IAgent agent)
        {
            Vector3 circleCenterLocal = Vector3.forward * m_CircleDistance;

            Gizmos.color = Color.yellow;
            for (int i = 0; i < 32; i++)
            {
                float angle1 = (i / 32.0f) * Mathf.PI * 2;
                float angle2 = ((i + 1) / 32.0f) * Mathf.PI * 2;
                Vector3 p1 = agent.LocalToWorld(circleCenterLocal + new Vector3(Mathf.Cos(angle1) * m_CircleRadius, 0, Mathf.Sin(angle1) * m_CircleRadius));
                Vector3 p2 = agent.LocalToWorld(circleCenterLocal + new Vector3(Mathf.Cos(angle2) * m_CircleRadius, 0, Mathf.Sin(angle2) * m_CircleRadius));
                Gizmos.DrawLine(p1, p2);
            }

            float wx = Mathf.Cos(m_WanderAngle) * m_CircleRadius;
            float wz = Mathf.Sin(m_WanderAngle) * m_CircleRadius;
            Vector3 wanderPointLocal = circleCenterLocal + new Vector3(wx, 0, wz);
            Vector3 wanderPointWorld = agent.LocalToWorld(wanderPointLocal);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(wanderPointWorld, 0.2f);
            Gizmos.DrawLine(agent.Position, wanderPointWorld);
        }

        private readonly float m_CircleDistance;
        private readonly float m_CircleRadius;
        private readonly float m_AngleChange;
        private float m_WanderAngle = 0;
    }
}
