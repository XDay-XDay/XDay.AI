using UnityEngine;

namespace XDay.AI
{
    internal abstract class SteeringForce : ISteeringForce
    {
        public bool Enable { get => m_Enable; set => m_Enable = value; }

        public abstract Vector3 Calculate(IAgent agent, float dt);

        public virtual void DrawGizmos()
        {
        }

        private bool m_Enable = true;
    }
}
