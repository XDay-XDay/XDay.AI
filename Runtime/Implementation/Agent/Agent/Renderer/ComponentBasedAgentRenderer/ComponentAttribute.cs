using System;

namespace XDay.AI
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AgentComponentLabel : Attribute
    {
        public Type ComponentType { get => m_ComponentType; set => m_ComponentType = value; }
        public string DisplayName { get => m_DisplayName; set => m_DisplayName = value; }

        public AgentComponentLabel(Type componentType, string displayName)
        {
            m_ComponentType = componentType;
            m_DisplayName = displayName;
        }

        private Type m_ComponentType;
        private string m_DisplayName;
    }
}
