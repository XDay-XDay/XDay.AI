using System;
using System.Collections.Generic;

namespace XDay.AI
{
    internal class AgentContainerFactory
    {
        public void RegisterCreator(Type createInfoType, Func<IAgentContainerCreateInfo, IAgentContainer> creator)
        {
            m_Creators.Add(createInfoType, creator);
        }

        public IAgentContainer CreateAgentContainer(IAgentContainerCreateInfo createInfo)
        {
            if (m_Creators.TryGetValue(createInfo.GetType(), out var creator))
            {
                return creator(createInfo);
            }
            return null;
        }

        private readonly Dictionary<Type, Func<IAgentContainerCreateInfo, IAgentContainer>> m_Creators = new();
    }
}
