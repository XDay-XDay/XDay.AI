using System.Collections.Generic;
using UnityEngine;

namespace XDay.AI
{
    [AgentContainerLabel(typeof(SimpleAgentContainerCreateInfo))]
    internal class SimpleAgentContainer : IAgentContainer
    {
        public SimpleAgentContainer(IAgentContainerCreateInfo createInfo)
        {
        }

        public void Add(IAgent agent)
        {
            m_Agents.Add(agent.ID, agent);
        }

        public IAgent Remove(int id)
        {
            if (m_Agents.TryGetValue(id, out var agent))
            {
                m_Agents.Remove(agent.ID);
            }
            return agent;
        }

        public List<IAgent> GetAgents()
        {
            return new List<IAgent>(m_Agents.Values);
        }

        public IAgent GetAgent(int id)
        {
            m_Agents.TryGetValue(id, out var agent);
            return agent;
        }

        public void QueryAgents(float minX, float minY, float maxX, float maxY, List<IAgent> outAgents)
        {
            foreach (var agent in m_Agents.Values)
            {
                var pos = agent.Position;
                if (pos.x >= minX &&
                    pos.z >= minY &&
                    pos.x <= maxX &&
                    pos.z <= maxY)
                {
                    outAgents.Add(agent);
                }
            }
        }

        public void QueryAgents(Vector3 center, float radius, List<IAgent> outAgents)
        {
            var r2 = radius * radius;
            foreach (var agent in m_Agents.Values)
            {
                var pos = agent.Position;
                var dis = (center - pos).sqrMagnitude;
                if (dis <= r2)
                {
                    outAgents.Add(agent);
                }
            }
        }

        public void Update(float dt)
        {
            foreach (var agent in m_Agents.Values)
            {
                agent.Update(dt);
            }
        }

        public void FixedUpdate()
        {
            foreach (var agent in m_Agents.Values)
            {
                agent.FixedUpdate();
            }
        }

        public void DrawGizmo()
        {
            foreach (var agent in m_Agents.Values)
            {
                agent.DrawGizmos();
            }
        }

        private readonly Dictionary<int, IAgent> m_Agents = new();
    }
}
