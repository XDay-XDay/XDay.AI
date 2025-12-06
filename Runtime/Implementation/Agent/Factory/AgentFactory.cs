using System;
using System.Collections.Generic;
using UnityEngine;

namespace XDay.AI
{
    internal class AgentFactory
    {
        public void RegisterCreator(Type configType, Func<int, AgentConfig, IWorld, Vector3, IAgent> creator)
        {
            m_Creators.Add(configType, creator);
        }

        public IAgent CreateAgent(int id, AgentConfig createInfo, IWorld world, Vector3 position)
        {
            if (m_Creators.TryGetValue(createInfo.GetType(), out var creator))
            {
                return creator(id, createInfo, world, position);
            }
            return null;
        }

        private readonly Dictionary<Type, Func<int, AgentConfig, IWorld, Vector3, IAgent>> m_Creators = new();
    }
}
