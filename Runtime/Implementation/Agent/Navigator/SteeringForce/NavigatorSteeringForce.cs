using UnityEngine;
using System.Collections.Generic;

namespace XDay.AI
{
    internal class NavigatorSteeringForce : INavigatorSteeringForce
    {
        public void OnDestroy()
        {
        }

        public void SetAgent(IAgent agent)
        {
            m_Agent = agent;
        }

        public void AddSteeringForce(ISteeringForce sf)
        {
            m_SteeringForces.Add(sf);
        }

        public void GetSteeringForce<Type>() where Type : ISteeringForce
        {
            Debug.Assert(false, "todo");
        }

        public void Update(float dt)
        {
            Vector3 totalForce = Vector3.zero;
            foreach (var sf in m_SteeringForces)
            {
                if (sf.Enable)
                {
                    totalForce += sf.Calculate(m_Agent, dt);
                }
            }
            ClampForce(ref totalForce, m_Agent.MaxLinearAcceleration);
            m_Agent.AddForce(totalForce, ForceMode.Acceleration);
        }

        public void DrawGizmos()
        {
            foreach (var sf in m_SteeringForces)
            {
                sf.DrawGizmos();
            }
        }

        private void ClampForce(ref Vector3 totalForce, float maxAcceleration)
        {
            var len = totalForce.sqrMagnitude;
            if (len > maxAcceleration * maxAcceleration)
            {
                len = Mathf.Sqrt(len);
                totalForce *= maxAcceleration / len;
            }
        }

        private IAgent m_Agent;
        private readonly List<ISteeringForce> m_SteeringForces = new();
    }
}
