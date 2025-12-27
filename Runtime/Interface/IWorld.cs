using System;
using System.Collections.Generic;
using UnityEngine;
using XDay.WorldAPI;

namespace XDay.AI
{
    public interface IWorldTicker
    {
        void Update(float dt);
    }

    public class WorldCreateInfo
    {
        public IWorldAssetLoader AssetLoader { get; set; }
        public IAgentContainerCreateInfo ContainerCreateInfo { get; set; }
        public IAgentRendererContainerCreateInfo RendererContainerCreateInfo { get; set; }
        public IObstacleManagerCreateInfo ObstacleManagerCreateInfo { get; set; }
        public IWorldCullerCreateInfo WorldCullerCreateInfo { get; set; }
        public IWorldTicker WorldTicker { get; set; }
    }

    public interface IWorld
    {
        public event Action<IAgent> EventCreateAgent;
        public event Action<IAgent> EventRemoveAgent;
        public event Action<IAgent> EventUpdateAgent;
        public event Action<IAgent> EventShowAgent;
        public event Action<IAgent> EventHideAgent;
        public event Action<IAgent, int, int> EventChangeAgentLOD;
        public IWorldAssetLoader AssetLoader { get; }

        static IWorld Create(WorldCreateInfo createInfo)
        {
            return new World(createInfo); 
        }

        void OnDestroy();

        void Update(float deltaTime);
        void FixedUpdate();

        IAgent CreateAgent(AgentConfig config, Vector3 position);
        void RemoveAgent(IAgent agent);
        IAgent GetAgent(int id);
        void GetAllAgents(List<IAgent> allAgents);

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

        INavigator CreateNavigator(NavigatorConfig config, IAgent agent);

        void CreateRenderContainer(IAgentRendererContainerCreateInfo createInfo);
        void DestroyRenderContainer();
    }
}
