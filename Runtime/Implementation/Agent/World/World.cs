using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using XDay.UtilityAPI;
using XDay.WorldAPI;

namespace XDay.AI
{
    internal class World : IWorld
    {
        public event Action<IAgent> EventCreateAgent;
        public event Action<IAgent> EventRemoveAgent;
        public event Action<IAgent> EventUpdateAgent;
        public event Action<IAgent> EventShowAgent;
        public event Action<IAgent> EventHideAgent;
        public event Action<IAgent, int, int> EventChangeAgentLOD;
        public IWorldAssetLoader AssetLoader => m_AssetLoader;

        public World(WorldCreateInfo createInfo)
        {
            m_WorldTicker = createInfo.WorldTicker;
            m_AssetLoader = createInfo.AssetLoader;

            RegisterAgentContainers();

            RegisterAgentRendererContainers();

            RegisterAgents();

            RegisterObstacleManagers();

            RegisterNavigators();

            RegisterWorldCullers();

            m_AgentContainer = m_AgentContainerFactory.CreateAgentContainer(createInfo.ContainerCreateInfo);
            m_ObstacleManager = m_ObstacleManagerFactory.CreateObstacleManager(createInfo.ObstacleManagerCreateInfo);
            m_WorldCuller = m_WorldCullerFactory.CreateWorldCuller(createInfo.WorldCullerCreateInfo);

            if (createInfo.RendererContainerCreateInfo != null)
            {
                m_AgentRendererContainer = m_AgentRendererContainerFactory.CreateAgentRendererContainer(createInfo.RendererContainerCreateInfo, this);
            }
        }

        public void OnDestroy()
        {
            Debug.LogError("todo");
        }

        public void Update(float dt)
        {
            m_WorldCuller.Update(dt);
            m_WorldTicker?.Update(dt);
            m_AgentContainer.Update(dt);
            m_AgentRendererContainer?.Update(dt);
        }

        public void FixedUpdate()
        {
            m_AgentContainer.FixedUpdate();
        }

        public void DrawGizmo()
        {
            m_AgentContainer.DrawGizmo();
            m_AgentRendererContainer?.DrawGizmo();
        }

        public void CreateRenderContainer(IAgentRendererContainerCreateInfo createInfo)
        {
            DestroyRenderContainer();

            m_AgentRendererContainer = m_AgentRendererContainerFactory.CreateAgentRendererContainer(createInfo, this);
        }

        public void DestroyRenderContainer() 
        {
            m_AgentRendererContainer?.OnDestroy();
            m_AgentRendererContainer = null;
        }

        public IAgent CreateAgent(AgentConfig config, Vector3 position, Quaternion rotation)
        {
            var agent = m_AgentFactory.CreateAgent(++m_NextID, config, this, position, rotation);
            agent.Init();
            m_AgentContainer.Add(agent);

            EventCreateAgent?.Invoke(agent);

            return agent;
        }

        public IAgent GetAgent(int id)
        {
            return m_AgentContainer.GetAgent(id);
        }

        public void QueryAgents(float minX, float minY, float maxX, float maxY, List<IAgent> outAgents)
        {
            m_AgentContainer.QueryAgents(minX, minY, maxX, maxY, outAgents);
        }

        public void QueryAgents(Vector3 center, float radius, List<IAgent> outAgents)
        {
            m_AgentContainer.QueryAgents(center, radius, outAgents);
        }

        public void RemoveAgent(IAgent agent)
        {
            if (agent == null)
            {
                return;
            }

            EventRemoveAgent?.Invoke(agent);

            m_AgentContainer.Remove(agent.ID);
            agent.OnDestroy();
        }

        public bool Raycast(Vector3 position, Vector3 direction, float length, int layerMask, out HitInfo hitInfo)
        {
            return m_ObstacleManager.Raycast(position, direction, length, layerMask, out hitInfo);
        }

        public INavigator CreateNavigator(NavigatorConfig config, IAgent agent)
        {
            return m_AgentNavigatorFactory.CreateAgentNavigator(config, agent);
        }

        private void RegisterAgents()
        {
            m_AgentFactory = new();

            var agentTypes = Helper.GetClassesWithAttribute<AgentLabel>();
            foreach (var type in agentTypes)
            {
                var attribute = type.GetCustomAttribute<AgentLabel>();
                m_AgentFactory.RegisterCreator(attribute.ConfigType, (id, createInfo, world, position, rotation) =>
                {
                    object[] constructorArgs = { id, createInfo, world, position, rotation };
                    return Activator.CreateInstance(type, constructorArgs) as IAgent;
                });
            }
        }

        private void RegisterAgentContainers()
        {
            m_AgentContainerFactory = new();

            var agentContainerTypes = Helper.GetClassesWithAttribute<AgentContainerLabel>();
            foreach (var type in agentContainerTypes)
            {
                var attribute = type.GetCustomAttribute<AgentContainerLabel>();
                m_AgentContainerFactory.RegisterCreator(attribute.CreateInfoType, (createInfo) =>
                {
                    object[] constructorArgs = { createInfo };
                    return Activator.CreateInstance(type, constructorArgs) as IAgentContainer;
                });
            }
        }

        private void RegisterAgentRendererContainers()
        {
            m_AgentRendererContainerFactory = new();

            var types = Helper.GetClassesWithAttribute<AgentRendererContainerLabel>();
            foreach (var type in types)
            {
                var attribute = type.GetCustomAttribute<AgentRendererContainerLabel>();
                m_AgentRendererContainerFactory.RegisterCreator(attribute.CreateInfoType, (createInfo, world) =>
                {
                    object[] constructorArgs = { createInfo, world };
                    return Activator.CreateInstance(type, constructorArgs) as IAgentRendererContainer;
                });
            }
        }

        private void RegisterNavigators()
        {
            m_AgentNavigatorFactory = new();

            var naviagorTypes = Helper.GetClassesWithAttribute<AgentNavigatorLabel>();
            foreach (var type in naviagorTypes)
            {
                var attribute = type.GetCustomAttribute<AgentNavigatorLabel>();
                m_AgentNavigatorFactory.RegisterCreator(attribute.ConfigType, (config, agent) =>
                {
                    object[] constructorArgs = { config, agent };
                    return Activator.CreateInstance(type, constructorArgs) as INavigator;
                });
            }
        }

        private void RegisterObstacleManagers()
        {
            m_ObstacleManagerFactory = new();

            var obstacleManagerTypes = Helper.GetClassesWithAttribute<ObstacleManagerLabel>();
            foreach (var type in obstacleManagerTypes)
            {
                var attribute = type.GetCustomAttribute<ObstacleManagerLabel>();
                m_ObstacleManagerFactory.RegisterCreator(attribute.CreateInfoType, (createInfo) =>
                {
                    object[] constructorArgs = { createInfo };
                    return Activator.CreateInstance(type, constructorArgs) as IObstacleManager;
                });
            }
        }

        private void RegisterWorldCullers()
        {
            m_WorldCullerFactory = new();

            var worldCullerTypes = Helper.GetClassesWithAttribute<WorldCullerLabel>();
            foreach (var type in worldCullerTypes)
            {
                var attribute = type.GetCustomAttribute<WorldCullerLabel>();
                m_WorldCullerFactory.RegisterCreator(attribute.CreateInfoType, (createInfo) =>
                {
                    object[] constructorArgs = { createInfo };
                    return Activator.CreateInstance(type, constructorArgs) as IWorldCuller;
                });
            }
        }

        public void GetAllAgents(List<IAgent> allAgents)
        {
            allAgents.Clear();
            allAgents.AddRange(m_AgentContainer.GetAgents());
        }

        private AgentFactory m_AgentFactory;
        private AgentContainerFactory m_AgentContainerFactory;
        private AgentRendererContainerFactory m_AgentRendererContainerFactory;
        private ObstacleManagerFactory m_ObstacleManagerFactory;
        private WorldCullerFactory m_WorldCullerFactory;
        private AgentNavigatorFactory m_AgentNavigatorFactory;
        private readonly IAgentContainer m_AgentContainer;
        private IAgentRendererContainer m_AgentRendererContainer;
        private readonly IWorldTicker m_WorldTicker;
        private readonly IWorldCuller m_WorldCuller;
        private readonly IObstacleManager m_ObstacleManager;
        private int m_NextID;
        private IWorldAssetLoader m_AssetLoader;
    }
}
