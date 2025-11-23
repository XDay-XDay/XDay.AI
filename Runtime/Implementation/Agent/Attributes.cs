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
    public class AgentLabel : Attribute
    {
        public Type CreateInfoType { get => m_AgentCreateInfoType; set => m_AgentCreateInfoType = value; }

        public AgentLabel(Type agentCreateInfoType)
        {
            m_AgentCreateInfoType = agentCreateInfoType;
        }

        private Type m_AgentCreateInfoType;
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
}
