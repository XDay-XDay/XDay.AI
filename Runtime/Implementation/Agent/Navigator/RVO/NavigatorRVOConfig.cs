using UnityEngine;

namespace XDay.AI
{
    [CreateAssetMenu(menuName = "XDay/AI/Agent/Navigator/RVO")]
    public class NavigatorRVOConfig : NavigatorConfig
    {
        public float SlowDistance = 5f;
    }
}
