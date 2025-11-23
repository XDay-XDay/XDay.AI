using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using XDay.UtilityAPI;

namespace XDay.AI
{
    internal class World : IWorld
    {
        public World(WorldCreateInfo createInfo)
        {
            RegisterContainers();

            RegisterAgents();

            RegisterObstacleManagers();

            m_Container = m_ContainerFactory.CreateAgentContainer(createInfo.ContainerCreateInfo);
            m_ObstacleManager = m_ObstacleManagerFactory.CreateObstacleManager(createInfo.ObstacleManagerCreateInfo);
        }

        public void OnDestroy()
        {
        }

        public void Update(float dt)
        {
            m_Container.Update(dt);
        }

        public IAgent CreateAgent(IAgentCreateInfo createInfo)
        {
            var agent = m_AgentFactory.CreateAgent(++m_NextID, createInfo, this);
            m_Container.Add(agent);
            return agent;
        }

        public IAgent GetAgent(int id)
        {
            return m_Container.GetAgent(id);
        }

        public void QueryAgents(float minX, float minY, float maxX, float maxY, List<IAgent> outAgents)
        {
            m_Container.QueryAgents(minX, minY, maxX, maxY, outAgents);
        }

        public void RemoveAgent(IAgent agent)
        {
            if (agent == null)
            {
                return;
            }
            m_Container.Remove(agent.ID);
            agent.OnDestroy();
        }

        private void RegisterAgents()
        {
            m_AgentFactory = new();

            var agentTypes = Helper.GetClassesWithAttribute<AgentLabel>();
            foreach (var type in agentTypes)
            {
                var attribute = type.GetCustomAttribute<AgentLabel>();
                m_AgentFactory.RegisterCreator(attribute.CreateInfoType, (id, createInfo, world) =>
                {
                    object[] constructorArgs = { id, createInfo, world };
                    return Activator.CreateInstance(type, constructorArgs) as IAgent;
                });
            }
        }

        private void RegisterContainers()
        {
            m_ContainerFactory = new();

            var agentContainerTypes = Helper.GetClassesWithAttribute<AgentContainerLabel>();
            foreach (var type in agentContainerTypes)
            {
                var attribute = type.GetCustomAttribute<AgentContainerLabel>();
                m_ContainerFactory.RegisterCreator(attribute.CreateInfoType, (createInfo) =>
                {
                    object[] constructorArgs = { createInfo };
                    return Activator.CreateInstance(type, constructorArgs) as IAgentContainer;
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

        public bool Raycast(Vector3 position, Vector3 direction, float length, int layerMask, out HitInfo hitInfo)
        {
            return m_ObstacleManager.Raycast(position, direction, length, layerMask, out hitInfo);
        }

        private AgentFactory m_AgentFactory;
        private AgentContainerFactory m_ContainerFactory;
        private ObstacleManagerFactory m_ObstacleManagerFactory;
        private readonly IAgentContainer m_Container;
        private IObstacleManager m_ObstacleManager;
        private int m_NextID;
    }
}
