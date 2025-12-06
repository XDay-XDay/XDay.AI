using UnityEngine;
using System.Collections.Generic;

namespace XDay.AI
{
    [CreateAssetMenu(menuName = "XDay/AI/Agent/Renderer/Component Based Agent")]
    public class ComponentBasedAgentRendererConfig : AgentRendererConfig
    {
        [SerializeReference]
        public List<AgentRendererComponentConfig> Components = new();
    }
}
