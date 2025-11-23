using System;
using System.Collections.Generic;

namespace XDay.AI
{
    internal class AgentFactory
    {
        public void RegisterCreator(Type createInfoType, Func<int, IAgentCreateInfo, IWorld, IAgent> creator)
        {
            m_Creators.Add(createInfoType, creator);
        }

        public IAgent CreateAgent(int id, IAgentCreateInfo createInfo, IWorld world)
        {
            if (m_Creators.TryGetValue(createInfo.GetType(), out var creator))
            {
                return creator(id, createInfo, world);
            }
            return null;
        }

        private readonly Dictionary<Type, Func<int, IAgentCreateInfo, IWorld, IAgent>> m_Creators = new();
    }
}
