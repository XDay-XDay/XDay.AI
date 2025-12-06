using System;

namespace XDay.AI
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AgentContainerLabel : Attribute
    {
        public Type CreateInfoType { get => m_AgentContainerCreateInfoType; set => m_AgentContainerCreateInfoType = value; }

        public AgentContainerLabel(Type agentContainerCreateInfoType)
        {
            m_AgentContainerCreateInfoType = agentContainerCreateInfoType;
        }

        private Type m_AgentContainerCreateInfoType;
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class AgentRendererContainerLabel : Attribute
    {
        public Type CreateInfoType { get => m_AgentRendererContainerCreateInfoType; set => m_AgentRendererContainerCreateInfoType = value; }

        public AgentRendererContainerLabel(Type agentRendererContainerCreateInfoType)
        {
            m_AgentRendererContainerCreateInfoType = agentRendererContainerCreateInfoType;
        }

        private Type m_AgentRendererContainerCreateInfoType;
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class AgentNavigatorLabel : Attribute
    {
        public Type ConfigType { get => m_ConfigType; set => m_ConfigType = value; }

        public AgentNavigatorLabel(Type configType)
        {
            m_ConfigType = configType;
        }

        private Type m_ConfigType;
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class AgentLabel : Attribute
    {
        public Type ConfigType { get => m_AgentConfigType; set => m_AgentConfigType = value; }

        public AgentLabel(Type agentConfigType)
        {
            m_AgentConfigType = agentConfigType;
        }

        private Type m_AgentConfigType;
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ObstacleManagerLabel : Attribute
    {
        public Type CreateInfoType { get => m_ObstacleManagerCreateInfoType; set => m_ObstacleManagerCreateInfoType = value; }

        public ObstacleManagerLabel(Type obstacleManagerCreateInfoType)
        {
            m_ObstacleManagerCreateInfoType = obstacleManagerCreateInfoType;
        }

        private Type m_ObstacleManagerCreateInfoType;
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class SteeringForceLabel : Attribute
    {
        public Type ForceType { get => m_SteeringForceType; set => m_SteeringForceType = value; }
        public string DisplayName { get => m_DisplayName; set => m_DisplayName = value; }

        public SteeringForceLabel(Type configType, string displayName)
        {
            m_SteeringForceType = configType;
            m_DisplayName = displayName;
        }

        private Type m_SteeringForceType;
        private string m_DisplayName;
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class WorldCullerLabel : Attribute
    {
        public Type CreateInfoType { get => m_WorldCullerCreateInfoType; set => m_WorldCullerCreateInfoType = value; }

        public WorldCullerLabel(Type worldCullerCreateInfoType)
        {
            m_WorldCullerCreateInfoType = worldCullerCreateInfoType;
        }

        private Type m_WorldCullerCreateInfoType;
    }
}
