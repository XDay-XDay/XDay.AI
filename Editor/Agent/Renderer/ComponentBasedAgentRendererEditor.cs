using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XDay.UtilityAPI;

namespace XDay.AI.Editor
{
    /// <summary>
    /// 编辑ComponentBasedAgentRenderer的配置
    /// </summary>
    internal class ComponentBasedAgentRendererEditor
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

            DrawRendererConfigs();

            EditorGUI.indentLevel++;
            DrawComponentTypes();

            EditorHelper.HorizontalLine();

            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);

            DrawExistedComponents();
            EditorGUI.indentLevel--;

            EditorGUILayout.EndScrollView();
        }

        private void Refresh()
        {
            GetRendererConfigNames();
            GetComponentTypeNames();
        }

        private void DrawExistedComponents()
        {
            var config = GetActiveConfig();
            if (config == null)
            {
                return;
            }

            for (var i = 0; i < config.Components.Count; ++i)
            {
                if (config.Components[i] == null)
                {
                    config.Components.RemoveAt(i);
                    break;
                }

                var deleted = config.Components[i].InspectorGUI(i);
                if (deleted)
                {
                    config.Components.RemoveAt(i);
                    break;
                }
            }
        }

        private void DrawRendererConfigs()
        {
            var config = GetActiveConfig();
            if (config == null)
            {
                return;
            }

            EditorGUIUtility.labelWidth = 110;
            EditorGUILayout.BeginHorizontal();
            var newIndex = EditorGUILayout.Popup("Renderer Configs", m_ActiveConfigIndex, m_RendererConfigNames);
            if (GUILayout.Button("=>", GUILayout.MaxWidth(30)))
            {
                EditorHelper.PingObject(config);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUIUtility.labelWidth = 0;
            SetActiveConfig(newIndex);
        }

        private void DrawComponentTypes()
        {
            var config = GetActiveConfig();
            if (config == null)
            {
                return;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 100;
            m_SelectedComponentIndex = EditorGUILayout.Popup("Components", m_SelectedComponentIndex, m_ComponentTypeNames);
            EditorGUIUtility.labelWidth = 0;
            if (GUILayout.Button("Add", GUILayout.MaxWidth(40)))
            {
                CreateComponentConfig(config);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void CreateComponentConfig(ComponentBasedAgentRendererConfig config)
        {
            if (m_SelectedComponentIndex >= 0 && m_SelectedComponentIndex < m_ComponentTypeNames.Length)
            {
                var componentConfigType = m_ComponentConfigTypes[m_SelectedComponentIndex];

                foreach (var sfc in config.Components)
                {
                    if (sfc.GetType() == componentConfigType)
                    {
                        return;
                    }
                }

                var componentConfig = Activator.CreateInstance(componentConfigType) as AgentRendererComponentConfig;
                config.Components.Add(componentConfig);
            }
        }

        private void SetActiveConfig(int index)
        {
            if (m_ActiveConfigIndex != index)
            {
                m_ActiveConfigIndex = index;
            }
        }

        private ComponentBasedAgentRendererConfig GetActiveConfig()
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

        private void RemoveInvalid()
        {
            var config = GetActiveConfig();
            for (var i = config.Components.Count - 1; i >= 0; i--)
            {
                if (config.Components[i] == null)
                {
                    config.Components.RemoveAt(i);
                }
            }
            Save();
        }

        private void GetComponentTypeNames()
        {
            m_ComponentConfigTypes = Common.QueryTypes<AgentRendererComponentConfig>(false);
            m_ComponentTypeNames = new string[m_ComponentConfigTypes.Length];
            for (var i = 0; i < m_ComponentConfigTypes.Length; i++)
            {
                m_ComponentTypeNames[i] = Helper.GetClassAttribute<AgentComponentLabel>(m_ComponentConfigTypes[i]).DisplayName;
            }

            if (m_ComponentTypeNames.Length == 0)
            {
                m_SelectedComponentIndex = -1;
            }
            else
            {
                if (m_SelectedComponentIndex < 0 || m_SelectedComponentIndex >= m_ComponentTypeNames.Length)
                {
                    m_SelectedComponentIndex = 0;
                }
            }
        }

        private void GetRendererConfigNames()
        {
            m_Configs = EditorHelper.QueryAssets<ComponentBasedAgentRendererConfig>();
            m_RendererConfigNames = new string[m_Configs.Count];
            for (var i = 0; i < m_Configs.Count; i++)
            {
                m_RendererConfigNames[i] = $"{m_Configs[i].name}-{m_Configs[i].GetInstanceID()}";
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
        private int m_SelectedComponentIndex = -1;
        private string[] m_ComponentTypeNames;
        private string[] m_RendererConfigNames;
        private Type[] m_ComponentConfigTypes;
        private List<ComponentBasedAgentRendererConfig> m_Configs = new();
        private Vector2 m_ScrollPos;
    }
}
