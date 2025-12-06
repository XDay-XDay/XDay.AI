using System.Collections.Generic;
using UnityEngine;
using XDay.UtilityAPI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace XDay.AI
{
    [System.Serializable]
    public abstract partial class SteeringForceConfig
    {
        public bool Enabled = true;
        public float Priority = 1f;

#if UNITY_EDITOR
        public bool InspectorGUI(int index)
        {
            bool deleted = false;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{index + 1}. {Helper.GetClassAttribute<SteeringForceLabel>(GetType()).DisplayName}");
            EditorGUILayout.Space();
            if (GUILayout.Button("X", GUILayout.MaxWidth(20)))
            {
                if (EditorUtility.DisplayDialog("Warning", "Are you sure?", "Yes", "No"))
                {
                    deleted = true;
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel++;
            Enabled = EditorGUILayout.Toggle("Enabled", Enabled);
            Priority = EditorGUILayout.FloatField("Priority", Priority);
            OnInspectorGUI();
            EditorGUI.indentLevel--;
            return deleted;
        }

        protected abstract void OnInspectorGUI();
#endif
    }

    [CreateAssetMenu(menuName = "XDay/AI/Agent/Navigator/Steering Force")]
    public class NavigatorSteeringForceConfig : NavigatorConfig
    {
        [SerializeReference]
        public List<SteeringForceConfig> ForceConfigs = new();
    }
}
