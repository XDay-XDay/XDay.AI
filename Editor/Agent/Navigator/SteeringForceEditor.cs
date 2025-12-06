using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XDay.UtilityAPI;

namespace XDay.AI.Editor
{
    /// <summary>
    /// 编辑NavigatorSteeringForce的配置
    /// </summary>
    internal class SteeringForceEditor
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
            if (GUILayout.Button("Remove Invalid"))
            {
                RemoveInvalid();
            }
            EditorGUILayout.EndHorizontal();

            DrawNavigatorConfigs();

            EditorGUI.indentLevel++;
            DrawForceTypes();

            EditorHelper.HorizontalLine();

            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);

            DrawExistedForces();
            EditorGUI.indentLevel--;

            EditorGUILayout.EndScrollView();
        }

        private void Refresh()
        {
            GetNavigatorConfigNames();
            GetForceTypeNames();
        }

        private void DrawExistedForces()
        {
            var navigatorConfig = GetActiveConfig();
            if (navigatorConfig == null)
            {
                return;
            }

            for (var i = 0; i < navigatorConfig.ForceConfigs.Count; ++i)
            {
                var deleted = navigatorConfig.ForceConfigs[i].InspectorGUI(i);
                if (deleted)
                {
                    navigatorConfig.ForceConfigs.RemoveAt(i);
                    break;
                }
            }
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

        private void DrawForceTypes()
        {
            var navigatorConfig = GetActiveConfig();
            if (navigatorConfig == null)
            {
                return;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 60;
            m_SelectedForceIndex = EditorGUILayout.Popup("Forces", m_SelectedForceIndex, m_ForceTypeNames);
            EditorGUIUtility.labelWidth = 0;
            if (GUILayout.Button("Add", GUILayout.MaxWidth(40)))
            {
                CreateForceConfig(navigatorConfig);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void CreateForceConfig(NavigatorSteeringForceConfig navigatorConfig)
        {
            if (m_SelectedForceIndex >= 0 && m_SelectedForceIndex < m_ForceTypeNames.Length)
            {
                var forceConfigType = m_ForceConfigTypes[m_SelectedForceIndex];

                foreach (var sfc in navigatorConfig.ForceConfigs)
                {
                    if (sfc.GetType() == forceConfigType)
                    {
                        return;
                    }
                }

                var forceConfig = Activator.CreateInstance(forceConfigType) as SteeringForceConfig;
                navigatorConfig.ForceConfigs.Add(forceConfig);
            }
        }

        private void SetActiveConfig(int index)
        {
            if (m_ActiveConfigIndex != index)
            {
                m_ActiveConfigIndex = index;
            }
        }

        private NavigatorSteeringForceConfig GetActiveConfig()
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

        private void GetForceTypeNames()
        {
            m_ForceConfigTypes = Common.QueryTypes<SteeringForceConfig>(false);
            m_ForceTypeNames = new string[m_ForceConfigTypes.Length];
            for (var i = 0; i < m_ForceConfigTypes.Length; i++)
            {
                m_ForceTypeNames[i] = Helper.GetClassAttribute<SteeringForceLabel>(m_ForceConfigTypes[i]).DisplayName;
            }

            if (m_ForceTypeNames.Length == 0)
            {
                m_SelectedForceIndex = -1;
            }
            else
            {
                if (m_SelectedForceIndex < 0 || m_SelectedForceIndex >= m_ForceTypeNames.Length)
                {
                    m_SelectedForceIndex = 0;
                }
            }
        }

        private void GetNavigatorConfigNames()
        {
            m_Configs = EditorHelper.QueryAssets<NavigatorSteeringForceConfig>();
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

        private void RemoveInvalid()
        {
            var config = GetActiveConfig();
            for (var i = config.ForceConfigs.Count - 1; i >= 0; i--)
            {
                if (config.ForceConfigs[i] == null)
                {
                    config.ForceConfigs.RemoveAt(i);
                }
            }
            Save();
        }

        private int m_ActiveConfigIndex = -1;
        private int m_SelectedForceIndex = -1;
        private string[] m_ForceTypeNames;
        private string[] m_NavigatorConfigNames;
        private Type[] m_ForceConfigTypes;
        private List<NavigatorSteeringForceConfig> m_Configs = new();
        private Vector2 m_ScrollPos;
    }
}
