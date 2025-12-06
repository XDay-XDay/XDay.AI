using UnityEngine;

namespace XDay.AI
{
    [CreateAssetMenu(menuName = "XDay/AI/Agent/Physics 3D")]
    public class Physics3DAgentConfig : AgentConfig
    {
        public bool EnableCollision = false;
    }
}
