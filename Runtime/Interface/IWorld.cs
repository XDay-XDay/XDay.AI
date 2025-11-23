using System.Collections.Generic;
using UnityEngine;

namespace XDay.AI
{
    public class WorldCreateInfo
    {
        public IAgentContainerCreateInfo ContainerCreateInfo { get; set; }
        public IObstacleManagerCreateInfo ObstacleManagerCreateInfo { get; set; }
    }

    public interface IWorld
    {
        static IWorld Create(WorldCreateInfo createInfo)
        {
            return new World(createInfo); 
        }

        void OnDestroy();

        void Update(float deltaTime);

        IAgent CreateAgent(IAgentCreateInfo createInfo);
        void RemoveAgent(IAgent agent);
        IAgent GetAgent(int id);

        /// <summary>
        /// query agents in rectangle range
        /// </summary>
        /// <param name="minX"></param>
        /// <param name="minY"></param>
        /// <param name="maxX"></param>
        /// <param name="maxY"></param>
        /// <param name="outAgents"></param>
        void QueryAgents(float minX, float minY, float maxX, float maxY, List<IAgent> outAgents);

        bool Raycast(Vector3 position, Vector3 direction, float length, int layerMask, out HitInfo hitInfo);
    }
}
