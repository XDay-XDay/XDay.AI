using System;
using System.Collections.Generic;
using UnityEngine;
using XDay.UtilityAPI;

namespace XDay.AI
{
    public class ComponentBasedAgentRenderer : IAgentRenderer
    {
        public GameObject GameObject => m_GameObject;
        public IAgent Agent => m_Agent;

        public void Init(IAgent agent)
        {
            m_Agent = agent;

            CreateComponents();

            foreach (var component in m_Components)
            {
                component.Init(this);
            }

            foreach (var component in m_Components)
            {
                component.PostInit();
            }
        }

        private void CreateComponents()
        {
            var config = m_Agent.Config.Renderer as ComponentBasedAgentRendererConfig;
            if (config != null)
            {
                foreach (var comp in config.Components)
                {
                    if (comp != null)
                    {
                        var componentType = Helper.GetClassAttribute<AgentComponentLabel>(comp.GetType()).ComponentType;
                        var args = new object[] { comp };
                        var component = Activator.CreateInstance(componentType, args) as AgentRendererComponent;
                        m_Components.Add(component);
                    }
                    else
                    {
                        Debug.LogError("component config is null");
                    }
                }
            }
            else
            {
                Debug.LogError($"{m_Agent.Name} no rendere config");
            }
        }

        public void OnDestroy()
        {
            for (var i = m_Components.Count - 1; i >= 0; --i)
            {
                m_Components[i].Destroy();
            }
        }

        public void ChangeLOD(int lod)
        {
            throw new System.NotImplementedException();
        }

        public void OnDataChange(IAgentRenderer renderer)
        {
            throw new System.NotImplementedException();
        }

        public void SetActive(bool active)
        {
            if (m_GameObject != null)
            {
                m_GameObject.SetActive(active);
            }
        }

        public void SetGameObject(GameObject gameObject)
        {
            var oldGameObject = m_GameObject;
            m_GameObject = gameObject;

            OnGameObjectChanged();

            foreach (var component in m_Components)
            {
                component.OnGameObjectChanged(oldGameObject, gameObject);
            }
        }

        public void Update(float dt)
        {
            foreach (var component in m_Components)
            {
                component.Update(dt);
            }
        }

        protected virtual void OnGameObjectChanged() { }

        private IAgent m_Agent;
        private GameObject m_GameObject;
        private readonly List<AgentRendererComponent> m_Components = new();
    }
}
