using UnityEngine;
using System.Collections.Generic;
using XDay.UtilityAPI;
using System;

namespace XDay.AI
{
    [AgentNavigatorLabel(typeof(NavigatorSteeringForceConfig))]
    internal class NavigatorSteeringForce : INavigatorSteeringForce
    {
        public NavigatorSteeringForce(NavigatorSteeringForceConfig config, IAgent agent)
        {
            SetAgent(agent);

            foreach (var sfc in config.ForceConfigs)
            {
                var label = Helper.GetClassAttribute<SteeringForceLabel>(sfc.GetType());
                object[] constructorArgs = { sfc };
                var force = Activator.CreateInstance(label.ForceType, constructorArgs) as ISteeringForce;
                AddSteeringForce(force);
            }
        }

        public void OnDestroy()
        {
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

            float totalPriority = 0f;
            foreach (var sf in m_SteeringForces)
            {
                totalPriority += sf.Priority;
            }

            foreach (var sf in m_SteeringForces)
            {
                if (sf.Enabled)
                {
                    totalForce += sf.Calculate(m_Agent, dt) * (sf.Priority / totalPriority);
                }
            }
            Helper.Clamp(ref totalForce, m_Agent.MaxLinearAcceleration);
            m_Agent.AddForce(totalForce, ForceMode.Acceleration);
        }

        public void DrawGizmos()
        {
            foreach (var sf in m_SteeringForces)
            {
                sf.DrawGizmos(m_Agent);
            }
        }

        private void SetAgent(IAgent agent)
        {
            m_Agent = agent;
        }

        private IAgent m_Agent;
        private readonly List<ISteeringForce> m_SteeringForces = new();
    }
}
