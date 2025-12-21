using System.Collections.Generic;

namespace XDay.AI
{
    internal interface IAgentContainer
    {
        void Add(IAgent agent);
        IAgent Remove(int id);
        List<IAgent> GetAgents();
        IAgent GetAgent(int id);
        void QueryAgents(float minX, float minY, float maxX, float maxY, List<IAgent> outAgents);
        void Update(float dt);
        void FixedUpdate();
    }

    public interface IAgentContainerCreateInfo
    {
    }

    public class SimpleAgentContainerCreateInfo : IAgentContainerCreateInfo
    {
    }
}
