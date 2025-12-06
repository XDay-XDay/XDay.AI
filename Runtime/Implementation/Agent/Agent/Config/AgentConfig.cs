using System.Collections.Generic;
using UnityEngine;

namespace XDay.AI
{
    public class AgentConfig : ScriptableObject
    {
        public string Name;
        public float MaxLinearSpeed = 5f;
        public float MaxAngularSpeed = 360;
        public float ReachDistance = 0.5f;
        public float ColliderRadius = 0.5f;

        public NavigatorConfig Navigator;
        public AgentRendererConfig Renderer;
        public List<LineDetectorConfig> LineDetectors = new();
    }

    [System.Serializable]
    public class LineDetectorConfig
    {
        public Vector3 EulerAngle = Vector3.zero;
        public float Length = 3;
        public int CollisionLayerMask = -1;
    }
}
