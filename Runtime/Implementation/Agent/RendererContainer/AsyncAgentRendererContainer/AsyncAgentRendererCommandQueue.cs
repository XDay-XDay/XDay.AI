using System;
using System.Collections.Generic;
using UnityEngine;
using XDay.AI;

namespace XDay
{
    internal class AsyncAgentRendererCommandQueue
    {
        public AsyncAgentRendererCommandQueue(AsyncAgentRendererContainer container)
        {
            m_RendererContainer = container;

            m_NodePool = IObjectPool<LinkedListNode<AgentQueueAction>>.Create(createFunc:
                () =>
                {
                    return new LinkedListNode<AgentQueueAction>(new AgentQueueAction());
                });
        }

        public void OnDestroy()
        {
            m_NodePool.OnDestroy();
        }

        public void Update(float dt)
        {
            var createN = 0;
            var showN = 0;
            var hideN = 0;
            var destroyN = 0;
            var updateN = 0;
            var changeLODN = 0;
            m_Timer.Begin();
            var cur = m_Actions.First;
            while (cur != null)
            {
                var action = cur.Value;
                if (action.Type == CommandType.Update)
                {
                    m_RendererContainer.UpdateRenderer(action.Agent);
                    ++updateN;
                }
                else if (action.Type == CommandType.Create)
                {
                    m_RendererContainer.CreateRenderer(action.Agent);
                    m_CreateActions.Remove(action.AgentID);
                    ++createN;
                }
                else if (action.Type == CommandType.Destroy)
                {
                    m_RendererContainer.DestroyRenderer(action.AgentID);
                    m_DestroyActions.Remove(action.AgentID);
                    ++destroyN;
                }
                else if (action.Type == CommandType.Show)
                {
                    m_RendererContainer.ShowRenderer(action.Agent);
                    m_ShowActions.Remove(action.AgentID);
                    ++showN;
                }
                else if (action.Type == CommandType.Hide)
                {
                    m_RendererContainer.HideRenderer(action.AgentID);
                    m_HideActions.Remove(action.AgentID);
                    ++hideN;
                }
                else if (action.Type == CommandType.ChangeLOD)
                {
                    m_RendererContainer.ChangeRendererLOD(action.AgentID, action.LOD);
                    ++changeLODN;
                }
                else
                {
                    Debug.LogError($"Invalid action type: {action.Type}");
                }
                var next = cur.Next;
                ReleaseNode(cur);
                cur = next;
                var time = m_Timer.ElapsedSeconds;
                if (time >= m_MaxUpdateSeconds)
                {
                    //Debug.LogError($"经过{time}秒后create个数{createN},destroy个数{destroyN},update个数{updateN}");
                    m_Timer.Stop();
                    break;
                }
            }
        }

        public void CreateAgent(IAgent agent)
        {
            var action = new AgentQueueAction()
            {
                Agent = agent,
                AgentID = agent.ID,
                Type = CommandType.Create,
                LOD = agent.CurrentLOD,
            };

            AddAction(action);
        }

        public void ShowAgent(IAgent agent)
        {
            var action = new AgentQueueAction()
            {
                Agent = agent,
                AgentID = agent.ID,
                Type = CommandType.Show,
                LOD = agent.CurrentLOD,
            };

            AddAction(action);
        }

        public void HideAgent(IAgent agent)
        {
            var action = new AgentQueueAction()
            {
                Agent = agent,
                AgentID = agent.ID,
                Type = CommandType.Hide,
                LOD = agent.CurrentLOD,
            };

            AddAction(action);
        }

        public void UpdateAgent(IAgent agent)
        {
            var action = new AgentQueueAction()
            {
                Agent = agent,
                AgentID = agent.ID,
                Type = CommandType.Update,
                LOD = agent.CurrentLOD,
            };
            AddAction(action);
        }

        public void ChangeAgentLOD(IAgent agent)
        {
            var action = new AgentQueueAction()
            {
                Agent = agent,
                AgentID = agent.ID,
                Type = CommandType.ChangeLOD,
                LOD = agent.CurrentLOD,
            };
            AddAction(action);
        }

        public void DestroyAgent(int agentID, int lod)
        {
            var action = new AgentQueueAction()
            {
                Agent = null,
                AgentID = agentID,
                Type = CommandType.Destroy,
                LOD = lod
            };
            AddAction(action);
        }

        private void AddAction(AgentQueueAction action)
        {
            if (action.Type == CommandType.Create)
            {
                if (m_DestroyActions.TryGetValue(action.AgentID, out var node))
                {
                    m_DestroyActions.Remove(action.AgentID);
                    ReleaseNode(node);
                    if (action.LOD != node.Value.LOD)
                    {
                        var updateAction = new AgentQueueAction()
                        {
                            Agent = action.Agent,
                            AgentID = action.AgentID,
                            Type = CommandType.ChangeLOD,
                            LOD = action.Agent.CurrentLOD,
                        };
                        var updateNode = m_NodePool.Get();
                        updateNode.Value = updateAction;
                        m_Actions.AddLast(updateNode);
                    }
                    return;
                }
            }
            else if (action.Type == CommandType.Destroy)
            {
                if (m_CreateActions.TryGetValue(action.AgentID, out var node))
                {
                    ReleaseNode(node);
                    m_CreateActions.Remove(action.AgentID);
                    return;
                }
            }
            else if (action.Type == CommandType.Show)
            {
                if (m_HideActions.TryGetValue(action.AgentID, out var node))
                {
                    m_HideActions.Remove(action.AgentID);
                    ReleaseNode(node);
                    return;
                }
            }
            else if (action.Type == CommandType.Hide)
            {
                if (m_ShowActions.TryGetValue(action.AgentID, out var node))
                {
                    m_ShowActions.Remove(action.AgentID);
                    ReleaseNode(node);
                    return;
                }
            }

            var newNode = m_NodePool.Get();
            newNode.Value = action;
            m_Actions.AddLast(newNode);

            if (action.Type == CommandType.Create)
            {
                m_CreateActions.Add(action.AgentID, newNode);
            }
            else if (action.Type == CommandType.Destroy)
            {
                m_DestroyActions.Add(action.AgentID, newNode);
            }
        }

        private void ReleaseNode(LinkedListNode<AgentQueueAction> node)
        {
            m_NodePool.Release(node);
            m_Actions.Remove(node);
        }

        private readonly LinkedList<AgentQueueAction> m_Actions = new();
        private readonly IObjectPool<LinkedListNode<AgentQueueAction>> m_NodePool;
        private readonly SimpleStopwatch m_Timer = new();
        private readonly AsyncAgentRendererContainer m_RendererContainer;
        private readonly Dictionary<int, LinkedListNode<AgentQueueAction>> m_CreateActions = new();
        private readonly Dictionary<int, LinkedListNode<AgentQueueAction>> m_DestroyActions = new();
        private readonly Dictionary<int, LinkedListNode<AgentQueueAction>> m_ShowActions = new();
        private readonly Dictionary<int, LinkedListNode<AgentQueueAction>> m_HideActions = new();
        private const double m_MaxUpdateSeconds = 5 / 1000.0f;

        internal struct AgentQueueAction
        {
            public int AgentID;
            public CommandType Type;
            public int LOD;
            public IAgent Agent;
        }

        internal enum CommandType
        {
            Create = 0,
            Destroy = 1,
            Update = 2,
            ChangeLOD = 3,
            Show = 4,
            Hide = 5,
        }
    }
}
