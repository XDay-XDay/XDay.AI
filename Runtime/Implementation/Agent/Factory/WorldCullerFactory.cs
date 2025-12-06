using System;
using System.Collections.Generic;

namespace XDay.AI
{
    internal class WorldCullerFactory
    {
        public void RegisterCreator(Type createInfoType, Func<IWorldCullerCreateInfo, IWorldCuller> creator)
        {
            m_Creators.Add(createInfoType, creator);
        }

        public IWorldCuller CreateWorldCuller(IWorldCullerCreateInfo createInfo)
        {
            if (m_Creators.TryGetValue(createInfo.GetType(), out var creator))
            {
                return creator(createInfo);
            }
            return null;
        }

        private readonly Dictionary<Type, Func<IWorldCullerCreateInfo, IWorldCuller>> m_Creators = new();
    }
}
