using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace XDay.AI
{
    [CreateAssetMenu(menuName = "XDay/AI/Agent/Agent/CharacterController")]
    public class CharacterControllerAgentConfig : AgentConfig
    {
        public float Height = 2;
        public Vector3 Center = new(0, 1, 0);
        public float StepOffset = 0.3f;
        public float SlopeLimit = 45f;
        /// <summary>
        /// true表示用Force来移动,false表示直接使用Velocity移动
        /// </summary>
        public bool MoveByForce = true;

#if UNITY_EDITOR
        protected override void OnInspectorGUI() 
        {
            Height = EditorGUILayout.FloatField("Height", Height);
            Center = EditorGUILayout.Vector3Field("Center", Center);
            SlopeLimit = EditorGUILayout.FloatField("Slope Limit", SlopeLimit);
            StepOffset = EditorGUILayout.FloatField("Step Height", StepOffset);
            MoveByForce = EditorGUILayout.Toggle("Move By Force", MoveByForce);
        }
#endif
    }
}
