using UnityEngine;

namespace XDay.AI
{
    internal class SteeringForceObstacleAvoidance : SteeringForce, ISteeringForceObstacleAvoidance
    {
        public float AvoidStrength { get => m_AvoidStrength; set => m_AvoidStrength = value; }

        public SteeringForceObstacleAvoidance(SteeringForceConfig config) : base(config)
        {
            var cfg = config as ISteeringForceObstacleAvoidance.Config;
            AvoidStrength = cfg.AvoidStrength;
        }

        public override Vector3 Calculate(IAgent agent, float dt)
        {
            Vector3 avoidanceForce = Vector3.zero;

            foreach (var detector in agent.GetLineDetectors())
            {
                detector.Hit = false;
                var world = agent.World;
                if (world.Raycast(agent.Position, detector.WorldDirection, detector.Length, detector.CollisionLayerMask, out var hitInfo))
                {
                    detector.Hit = true;

                    Vector3 avoidDirection = hitInfo.Normal;
                    avoidDirection.Normalize();

                    float distanceFactor = (detector.Length - hitInfo.Distance) / detector.Length;
                    avoidanceForce += distanceFactor * m_AvoidStrength * avoidDirection;
                }
            }

            return avoidanceForce;
        }

        private float m_AvoidStrength = 10.0f;
    }
}
