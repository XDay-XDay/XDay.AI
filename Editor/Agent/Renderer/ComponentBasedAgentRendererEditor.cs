using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XDay.UtilityAPI;
using XDay.UtilityAPI.Editor;

namespace XDay.AI.Editor
{
    /// <summary>
    /// 编辑ComponentBasedAgentRenderer的配置
    /// </summary>
    internal class ComponentBasedAgentRendererEditor
    {
        public ComponentBasedAgentRendererEditor(AIEditor editor)
        {
            m_Editor = editor;
        }

        public void OnEnable()
        {
            Refresh();
        }

        public void OnGUI()
        {
            DrawRendererConfigs();

            EditorGUI.indentLevel++;
            DrawComponentTypes();

            EditorHelper.HorizontalLine();

            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);

            DrawExistedComponents();
            EditorGUI.indentLevel--;

            EditorGUILayout.EndScrollView();
        }

        public void Refresh()
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
                    var old = GUI.color;
                    GUI.color = Color.red;
                    EditorGUILayout.LabelField($"{i}. Invalid Component");
                    GUI.color = old;
                    continue;
                }

                var deleted = config.Components[i].InspectorGUI(i, config.Components.Count, out var swapped);

                if (swapped == SwapType.UpDown)
                {
                    (config.Components[i], config.Components[i + 1]) = (config.Components[i + 1], config.Components[i]);
                    Save();
                    break;
                }
                else if (swapped == SwapType.DownUp)
                {
                    (config.Components[i], config.Components[i - 1]) = (config.Components[i - 1], config.Components[i]);
                    Save();
                    break;
                }

                EditorHelper.HorizontalLine(Color.green);
                if (deleted)
                {
                    config.Components.RemoveAt(i);
                    Save();
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

            if (GUILayout.Button("选中使用者", GUILayout.MaxWidth(80)))
            {
                m_Editor.SelectFirstUseAgentConfig(config);
            }

            if (GUILayout.Button("重命名", GUILayout.MaxWidth(60)))
            {
                Rename(config);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUIUtility.labelWidth = 0;
            SetActiveConfig(newIndex);
        }

        private void Rename(ComponentBasedAgentRendererConfig config)
        {
            var path = AssetDatabase.GetAssetPath(config);
            var parameters = new List<ParameterWindow.Parameter>()
            {
                new ParameterWindow.StringParameter("Name", "", $"{Helper.GetPathName(path, false)}"),
            };
            ParameterWindow.Open("重命名", parameters, (p) =>
            {
                var ok = ParameterWindow.GetString(p[0], out var name);
                if (ok)
                {
                    var error = AssetDatabase.RenameAsset(path, name);
                    if (!string.IsNullOrEmpty(error))
                    {
                        Debug.LogError(error);
                    }
                    else
                    {
                        Refresh();
                    }
                }
                return ok;
            });
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

        public void Save()
        {
            if (m_ActiveConfigIndex >= 0 && m_ActiveConfigIndex < m_Configs.Count)
            {
                EditorUtility.SetDirty(m_Configs[m_ActiveConfigIndex]);
                AssetDatabase.SaveAssets();
            }
        }

        public void RemoveInvalid()
        {
            var removed = false;
            var config = GetActiveConfig();
            for (var i = config.Components.Count - 1; i >= 0; i--)
            {
                if (config.Components[i] == null)
                {
                    config.Components.RemoveAt(i);
                    removed = true;
                }
            }
            if (removed)
            {
                Save();
            }
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
                m_RendererConfigNames[i] = $"{i}. {m_Configs[i].name}";
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

        internal void SetActiveAgentRendererConfig(ComponentBasedAgentRendererConfig config)
        {
            Refresh();
            SetActiveConfig(m_Configs.IndexOf(config));
        }

        private int m_ActiveConfigIndex = -1;
        private int m_SelectedComponentIndex = -1;
        private string[] m_ComponentTypeNames;
        private string[] m_RendererConfigNames;
        private Type[] m_ComponentConfigTypes;
        private List<ComponentBasedAgentRendererConfig> m_Configs = new();
        private Vector2 m_ScrollPos;
        private AIEditor m_Editor;
    }
}
