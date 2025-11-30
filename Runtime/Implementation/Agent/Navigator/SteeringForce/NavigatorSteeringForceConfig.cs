using System.Collections.Generic;
using UnityEngine;

namespace XDay.AI
{
    [System.Serializable]
    public abstract class SteeringForceConfig
    {
        public bool Enabled;
        public float Priority;
    }

    [CreateAssetMenu(menuName = "XDay/AI/NavigatorSteeringForceConfig")]
    public class NavigatorSteeringForceConfig : ScriptableObject
    {
        [SerializeReference]
        public List<SteeringForceConfig> ForceConfigs = new();
    }
}
