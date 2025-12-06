using UnityEngine;

namespace XDay.AI
{
    internal abstract class SteeringForce : ISteeringForce
    {
        public bool Enabled { get => m_Enable; set => m_Enable = value; }
        public float Priority { get => m_Priority; set => m_Priority = Mathf.Max(0, value); }

        public SteeringForce(SteeringForceConfig config)
        {
            Enabled = config.Enabled;
            Priority = config.Priority;
        }

        public abstract Vector3 Calculate(IAgent agent, float dt);

        public void DrawGizmos(IAgent agent)
        {
            if (Enabled)
            {
                var old = Gizmos.color;

                Gizmos.color = Color.green;
                Gizmos.DrawLine(agent.Position, agent.Position + agent.LinearVelocity);

                OnDrawGizmos(agent);

                Gizmos.color = old;
            }
        }

        protected virtual void OnDrawGizmos(IAgent agent) { }

        private bool m_Enable = true;
        private float m_Priority = 1f;
    }
}
