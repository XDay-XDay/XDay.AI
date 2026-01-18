using System;
using System.Collections.Generic;

namespace XDay.AI
{
    [AgentRendererContainerLabel(typeof(AsyncAgentRendererContainerCreateInfo))]
    internal class AsyncAgentRendererContainer : IAgentRendererContainer
    {
        public AsyncAgentRendererContainer(IAgentRendererContainerCreateInfo createInfo, IWorld world)
        {
            m_World = world;

            world.EventCreateAgent += OnAgentCreated;
            world.EventRemoveAgent += OnAgentRemoved;
            world.EventUpdateAgent += OnAgentUpdated;
            world.EventShowAgent += OnAgentShow;
            world.EventHideAgent += OnAgentHide;
            world.EventChangeAgentLOD += OnAgentLODChanged;

            m_CommandQueue = new(this);
            m_RendererPool = IObjectPool<IAgentRenderer>.Create(createFunc: () => new ComponentBasedAgentRenderer());

            var agents = new List<IAgent>();    
            m_World.GetAllAgents(agents);
            foreach (var agent in agents)
            {
                OnAgentCreated(agent);
            }
        }

        public void OnDestroy()
        {
            m_World.EventCreateAgent -= OnAgentCreated;
            m_World.EventRemoveAgent -= OnAgentRemoved;
            m_World.EventUpdateAgent -= OnAgentUpdated;
            m_World.EventShowAgent -= OnAgentShow;
            m_World.EventHideAgent -= OnAgentHide;
            m_World.EventChangeAgentLOD -= OnAgentLODChanged;

            foreach (var kv in m_Renderers)
            {
                kv.Value.OnDestroy();
            }
            m_Renderers = null;

            m_World = null;

            m_CommandQueue.OnDestroy();
            m_CommandQueue = null;

            m_RendererPool?.OnDestroy();
            m_RendererPool = null;
        }

        public void Update(float dt)
        {
            m_CommandQueue.Update(dt);

            foreach (var renderer in m_Renderers.Values)
            {
                renderer.Update(dt);
            }
        }

        public T GetRenderer<T>(int agentID) where T : IAgentRenderer
        {
            m_Renderers.TryGetValue(agentID, out var renderer);
            if (renderer is T t)
            {
                return t;
            }
            return default;
        }

        private void OnAgentCreated(IAgent agent)
        {
            m_CommandQueue.CreateAgent(agent);
        }

        private void OnAgentShow(IAgent agent)
        {
            m_CommandQueue.ShowAgent(agent);
        }

        private void OnAgentHide(IAgent agent)
        {
            m_CommandQueue.HideAgent(agent);
        }

        private void OnAgentUpdated(IAgent agent)
        {
            m_CommandQueue.UpdateAgent(agent);
        }

        private void OnAgentLODChanged(IAgent agent, int oldLOD, int newLOD)
        {
            m_CommandQueue.ChangeAgentLOD(agent);
        }

        private void OnAgentRemoved(IAgent agent)
        {
            m_CommandQueue.DestroyAgent(agent.ID, agent.CurrentLOD);
        }

        internal void CreateRenderer(IAgent agent)
        {
            if (agent.Invalid)
            {
                return;
            }

            var agentRenderer = UpdateRenderer(agent);
            if (agentRenderer == null)
            {
                agentRenderer = m_RendererPool.Get();
                agentRenderer.Init(agent);
                m_Renderers.Add(agent.ID, agentRenderer);

                if (m_AgentCreateListeners.TryGetValue(agent.ID, out var list))
                {
                    m_AgentCreateListeners.Remove(agent.ID);
                    list.ForEach(action => action(agentRenderer));
                }
            }
        }

        internal void DestroyRenderer(int agentID)
        {
            if (m_Renderers.TryGetValue(agentID, out var agentRenderer))
            {
                if (agentRenderer != null)
                {
                    agentRenderer.OnDestroy();
                    m_RendererPool.Release(agentRenderer);
                }
                m_Renderers.Remove(agentID);
            }
        }

        internal void ShowRenderer(IAgent agent)
        {
            m_Renderers.TryGetValue(agent.ID, out var renderer);
            renderer?.SetActive(true);
        }

        internal void HideRenderer(int agentID)
        {
            m_Renderers.TryGetValue(agentID, out var renderer);
            renderer?.SetActive(false);
        }

        internal void ChangeRendererLOD(int agentID, int lod)
        {
            if (m_World.GetAgent(agentID) == null)
            {
                return;
            }

            if (m_Renderers.TryGetValue(agentID, out var agentRenderer))
            {
                agentRenderer.ChangeLOD(lod);
            }
        }

        internal IAgentRenderer UpdateRenderer(IAgent agent)
        {
            if (m_Renderers.TryGetValue(agent.ID, out var renderer))
            {
                renderer?.OnDataChange(renderer);
            }
            return renderer;
        }

        public void ExecuteAgentAction(int agentID, Action<IAgentRenderer> action)
        {
            if (m_Renderers.TryGetValue(agentID, out var agent))
            {
                action(agent);
            }
            else
            {
                if (!m_AgentCreateListeners.TryGetValue(agentID, out var list))
                {
                    list = new List<Action<IAgentRenderer>>();
                    m_AgentCreateListeners[agentID] = list;
                }

                list.Add(action);
            }
        }

        public void DrawGizmo()
        {
            foreach (var renderer in m_Renderers.Values)
            {
                renderer.DrawGizmo();
            }
        }

        private IWorld m_World;
        private AsyncAgentRendererCommandQueue m_CommandQueue;
        private Dictionary<int, IAgentRenderer> m_Renderers = new();
        private IObjectPool<IAgentRenderer> m_RendererPool;
        private readonly Dictionary<int, List<Action<IAgentRenderer>>> m_AgentCreateListeners = new();
    }
}