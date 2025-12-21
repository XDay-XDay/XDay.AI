using System;
using UnityEngine;

namespace XDay.AI
{
    public abstract class AgentRendererComponent
    {
        public IAgentRenderer Renderer => m_Renderer;

        internal void Init(IAgentRenderer renderer)
        {
            m_Renderer = renderer;

            OnInit();
        }

        internal void PostInit()
        {
            OnPostInit();
        }

        internal void Destroy()
        {
            OnDestroy();

            m_Renderer = null;
        }

        internal void Update(float dt)
        {
            OnUpdate(dt);
        }

        protected abstract void OnInit();
        protected virtual void OnPostInit() { }
        protected abstract void OnDestroy();
        protected virtual void OnUpdate(float dt) { }

        internal virtual void OnGameObjectChanged(GameObject oldGameObject, GameObject gameObject)
        {
        }

        private IAgentRenderer m_Renderer;
    }
}
