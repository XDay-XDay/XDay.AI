using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XDay.UtilityAPI;

namespace XDay.AI.Editor
{
    /// <summary>
    /// 编辑NavigatorRVO的配置
    /// </summary>
    internal class RVOEditor
    {
        public void OnEnable()
        {
            Refresh();
        }

        public void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Refresh"))
            {
                Refresh();
            }
            if (GUILayout.Button("Save"))
            {
                Save();
            }
            EditorGUILayout.EndHorizontal();

            DrawNavigatorConfigs();

            EditorGUI.indentLevel++;

            EditorHelper.HorizontalLine();

            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);

            DrawActiveConfig();

            EditorGUI.indentLevel--;

            EditorGUILayout.EndScrollView();
        }

        private void DrawActiveConfig()
        {
            var navigatorConfig = GetActiveConfig();
            if (navigatorConfig == null)
            {
                return;
            }

            navigatorConfig.SlowDistance = EditorGUILayout.FloatField("Slow Distance", navigatorConfig.SlowDistance);
        }

        private void Refresh()
        {
            GetNavigatorConfigNames();
        }

        private void DrawNavigatorConfigs()
        {
            var navigatorConfig = GetActiveConfig();
            if (navigatorConfig == null)
            {
                return;
            }

            EditorGUIUtility.labelWidth = 110;
            EditorGUILayout.BeginHorizontal();
            var newIndex = EditorGUILayout.Popup("Navigator Configs", m_ActiveConfigIndex, m_NavigatorConfigNames);
            if (GUILayout.Button("=>", GUILayout.MaxWidth(30)))
            {
                EditorHelper.PingObject(navigatorConfig);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUIUtility.labelWidth = 0;
            SetActiveConfig(newIndex);
        }

        private void SetActiveConfig(int index)
        {
            if (m_ActiveConfigIndex != index)
            {
                m_ActiveConfigIndex = index;
            }
        }

        private NavigatorRVOConfig GetActiveConfig()
        {
            if (m_ActiveConfigIndex >= 0 && m_ActiveConfigIndex <= m_Configs.Count)
            {
                return m_Configs[m_ActiveConfigIndex];
            }
            return null;
        }

        private void Save()
        {
            if (m_ActiveConfigIndex >= 0 && m_ActiveConfigIndex < m_Configs.Count)
            {
                EditorUtility.SetDirty(m_Configs[m_ActiveConfigIndex]);
                AssetDatabase.SaveAssets();
            }
        }

        private void GetNavigatorConfigNames()
        {
            m_Configs = EditorHelper.QueryAssets<NavigatorRVOConfig>();
            m_NavigatorConfigNames = new string[m_Configs.Count];
            for (var i = 0; i < m_Configs.Count; i++)
            {
                m_NavigatorConfigNames[i] = $"{m_Configs[i].name}-{m_Configs[i].GetInstanceID()}";
            }

            if (m_Configs.Count == 0)
            {
                SetActiveConfig(-1);
            }
            else if (m_ActiveConfigIndex < 0 || m_ActiveConfigIndex >= m_Configs.Count)
            {
                SetActiveConfig(0);
            }
        }

        private int m_ActiveConfigIndex = -1;
        private string[] m_NavigatorConfigNames;
        private List<NavigatorRVOConfig> m_Configs = new();
        private Vector2 m_ScrollPos;
    }
}
