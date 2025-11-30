using UnityEngine;

namespace XDay.AI
{
    internal abstract class SteeringForce : ISteeringForce
    {
        public bool Enabled { get => m_Enable; set => m_Enable = value; }
        public float Priority { get => m_Priority; set => m_Priority = Mathf.Max(0, value); }

        public abstract Vector3 Calculate(IAgent agent, float dt);

        public virtual void DrawGizmos()
        {
        }

        private bool m_Enable = true;
        private float m_Priority = 1f;
    }
}
