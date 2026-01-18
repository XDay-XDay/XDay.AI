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
        public bool InspectorGUI(int index, int n, out SwapType swapped)
        {
            bool deleted = false;
            swapped = SwapType.None;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{index + 1}. {Helper.GetClassAttribute<AgentComponentLabel>(GetType()).DisplayName}");
            EditorGUILayout.Space();

            if (index > 0)
            {
                if (GUILayout.Button(new GUIContent("<", "向上移动"), GUILayout.MaxWidth(20)))
                {
                    swapped = SwapType.DownUp;
                }
            }
            if (index < n - 1)
            {
                if (GUILayout.Button(new GUIContent(">", "向下移动"), GUILayout.MaxWidth(20)))
                {
                    swapped = SwapType.UpDown;
                }
            }
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
