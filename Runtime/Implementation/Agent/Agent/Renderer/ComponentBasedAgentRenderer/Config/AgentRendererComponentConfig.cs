#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using XDay.UtilityAPI;
#endif

namespace XDay.AI
{
    [System.Serializable]
    public abstract class AgentRendererComponentConfig
    {
#if UNITY_EDITOR
        public bool InspectorGUI(int index)
        {
            bool deleted = false;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{index + 1}. {Helper.GetClassAttribute<AgentComponentLabel>(GetType()).DisplayName}");
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
            OnInspectorGUI();
            EditorGUI.indentLevel--;
            return deleted;
        }

        protected virtual void OnInspectorGUI() { }
#endif
    }
}
