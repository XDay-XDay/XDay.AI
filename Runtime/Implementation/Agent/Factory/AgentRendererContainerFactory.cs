using System;
using System.Collections.Generic;

namespace XDay.AI
{
    internal class AgentRendererContainerFactory
    {
        public void RegisterCreator(Type createInfoType, Func<IAgentRendererContainerCreateInfo, IWorld, IAgentRendererContainer> creator)
        {
            m_Creators.Add(createInfoType, creator);
        }

        public IAgentRendererContainer CreateAgentRendererContainer(IAgentRendererContainerCreateInfo createInfo, IWorld world)
        {
            if (m_Creators.TryGetValue(createInfo.GetType(), out var creator))
            {
                return creator(createInfo, world);
            }
            return null;
        }

        private readonly Dictionary<Type, Func<IAgentRendererContainerCreateInfo, IWorld, IAgentRendererContainer>> m_Creators = new();
    }
}
