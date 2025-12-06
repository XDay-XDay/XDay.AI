using System;
using System.Collections.Generic;

namespace XDay.AI
{
    internal class AgentNavigatorFactory
    {
        public void RegisterCreator(Type createInfoType, Func<NavigatorConfig, IAgent, INavigator> creator)
        {
            m_Creators.Add(createInfoType, creator);
        }

        public INavigator CreateAgentNavigator(NavigatorConfig config, IAgent agent)
        {
            if (m_Creators.TryGetValue(config.GetType(), out var creator))
            {
                return creator(config, agent);
            }
            return null;
        }

        private readonly Dictionary<Type, Func<NavigatorConfig, IAgent, INavigator>> m_Creators = new();
    }
}
