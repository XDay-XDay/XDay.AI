using System;
using System.Collections.Generic;

namespace XDay.AI
{
    internal class ObstacleManagerFactory
    {
        public void RegisterCreator(Type createInfoType, Func<IObstacleManagerCreateInfo, IObstacleManager> creator)
        {
            m_Creators.Add(createInfoType, creator);
        }

        public IObstacleManager CreateObstacleManager(IObstacleManagerCreateInfo createInfo)
        {
            if (m_Creators.TryGetValue(createInfo.GetType(), out var creator))
            {
                return creator(createInfo);
            }
            return null;
        }

        private readonly Dictionary<Type, Func<IObstacleManagerCreateInfo, IObstacleManager>> m_Creators = new();
    }
}
