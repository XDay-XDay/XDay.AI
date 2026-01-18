using System.Collections.Generic;
using UnityEngine;

namespace XDay.AI
{
    internal interface IAgentContainer
    {
        void Add(IAgent agent);
        IAgent Remove(int id);
        List<IAgent> GetAgents();
        IAgent GetAgent(int id);
        void QueryAgents(float minX, float minY, float maxX, float maxY, List<IAgent> outAgents);
        void QueryAgents(Vector3 center, float radius, List<IAgent> outAgents);
        void Update(float dt);
        void FixedUpdate();
        void DrawGizmo();
    }

    public interface IAgentContainerCreateInfo
    {
    }

    public class SimpleAgentContainerCreateInfo : IAgentContainerCreateInfo
    {
    }
}
