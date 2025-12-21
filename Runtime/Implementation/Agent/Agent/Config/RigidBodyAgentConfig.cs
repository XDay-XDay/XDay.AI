using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace XDay.AI
{
    [CreateAssetMenu(menuName = "XDay/AI/Agent/Agent/RigidBody")]
    public class RigidBodyAgentConfig : AgentConfig
    {
        public bool EnableCollision = false;
        public float Height = 2;
        public Vector3 Center = new(0, 1, 0);

#if UNITY_EDITOR
        protected override void OnInspectorGUI()
        {
            EnableCollision = EditorGUILayout.Toggle("Enable Collision", EnableCollision);
            Height = EditorGUILayout.FloatField("Height", Height);
            Center = EditorGUILayout.Vector3Field("Center", Center);
        }
#endif
    }
}
