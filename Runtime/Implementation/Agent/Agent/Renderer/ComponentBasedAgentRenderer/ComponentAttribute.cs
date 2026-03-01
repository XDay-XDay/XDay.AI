using System;

namespace XDay.AI
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AgentComponentLabel : Attribute
    {
        public Type ComponentType => m_ComponentType;
        public string DisplayName => m_DisplayName;
        public string Tooltips => m_Tooltips;
        public bool Show => m_Show;

        public AgentComponentLabel(Type componentType, string displayName, string tooltips, bool show)
        {
            m_ComponentType = componentType;
            m_DisplayName = displayName;
            m_Tooltips = tooltips;
            m_Show = show;
        }

        private Type m_ComponentType;
        private string m_DisplayName;
        private string m_Tooltips;
        private readonly bool m_Show;
    }
}
