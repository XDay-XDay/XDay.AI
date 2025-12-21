using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XDay.UtilityAPI;

namespace XDay.AI.Editor
{
    /// <summary>
    /// 编辑Agent的配置
    /// </summary>
    internal class AgentEditor
    {
        public AgentEditor(AIEditor editor)
        {
            m_Editor = editor;
        }

        public void OnEnable()
        {
            Refresh();
        }

        public void OnGUI()
        {
            DrawGroups();

            DrawActiveGroup();
        }

        private void DrawActiveGroup()
        {
            var group = GetActiveGroup();
            if (group == null)
            {
                return;
            }

            EditorGUI.indentLevel++;

            EditorHelper.HorizontalLine(Color.white);

            group.ConfigCreateFolder = EditorHelper.ObjectField<DefaultAsset>("Agent Config Folder", group.ConfigCreateFolder, 0, null, "创建Config时默认输出目录");
            group.RendererCreateFolder = EditorHelper.ObjectField<DefaultAsset>("Agent Renderer Folder", group.RendererCreateFolder, 0, null, "创建Renderer时默认输出目录");
            EditorGUILayout.BeginHorizontal();
            group.CreateRendererConfig = EditorGUILayout.Toggle(new GUIContent("Create Render Config", "创建Config时同时也创建一个RendererConfig"), group.CreateRendererConfig);
            group.DeleteRendererConfig = EditorGUILayout.Toggle(new GUIContent("Delete Render Config", "删除Config时同时也删除RendererConfig"), group.DeleteRendererConfig);
            EditorGUILayout.EndHorizontal();

            EditorHelper.HorizontalLine(Color.yellow);

            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);

            var idx = 0;
            var breakLoop = false;
            foreach (var config in group.Configs)
            {
                if (config == null)
                {
                    var old = GUI.color;
                    GUI.color = Color.red;
                    EditorGUILayout.LabelField($"{idx}. Invalid Agent Config");
                    GUI.color = old;
                    continue;
                }
                EditorGUI.BeginChangeCheck();

                config.InspectorGUI(idx, out var selectRenderer, out var ping, out var deleted, out var copyPath);

                if (copyPath)
                {
                    EditorGUIUtility.systemCopyBuffer = AssetDatabase.GetAssetPath(config);
                }

                if (deleted)
                {
                    if (group.DeleteRendererConfig && config.Renderer != null)
                    {
                        FileUtil.DeleteFileOrDirectory(AssetDatabase.GetAssetPath(config.Renderer));
                    }
                    FileUtil.DeleteFileOrDirectory(AssetDatabase.GetAssetPath(config));
                    group.Configs.Remove(config);
                    breakLoop = true;
                    AssetDatabase.Refresh();
                }

                if (selectRenderer)
                {
                    if (config.Renderer is ComponentBasedAgentRendererConfig r)
                    {
                        m_Editor.SetAgentTab(AIEditor.AgentComponentBasedRendererTab);
                        m_Editor.SetActiveAgentRendererConfig(config.Renderer);
                    }
                    else
                    {
                        Debug.Assert(false, "todo");
                    }
                }

                if (ping)
                {
                    EditorHelper.PingObject(config);
                }

                EditorHelper.HorizontalLine(Color.green);

                if (EditorGUI.EndChangeCheck())
                {
                    if (config != null)
                    {
                        EditorUtility.SetDirty(config);
                    }
                    EditorUtility.SetDirty(group);
                }

                ++idx;

                if (breakLoop)
                {
                    break;
                }
            }

            EditorGUI.indentLevel--;

            EditorGUILayout.EndScrollView();
        }

        public void Refresh()
        {
            GetAgentGroupNames();
            GetAgentConfigTypes();
        }

        private void DrawGroups()
        {
            var group = GetActiveGroup();
            if (group == null)
            {
                return;
            }

            EditorGUIUtility.labelWidth = 110;
            EditorGUILayout.BeginHorizontal();
            var newIndex = EditorGUILayout.Popup("Groups", m_ActiveGroupIndex, m_AgentConfigGroupNames);

            m_ActiveRendererTypeIndex = EditorGUILayout.Popup("Renderer", m_ActiveRendererTypeIndex, m_AgentRendererConfigNames);

            EditorGUILayout.Space();

            if (GUILayout.Button("+", GUILayout.MaxWidth(20)))
            {
                CreateAgentConfigContextMenu();
            }

            if (GUILayout.Button("=>", GUILayout.MaxWidth(30)))
            {
                EditorHelper.PingObject(group);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUIUtility.labelWidth = 0;
            SetActiveGroup(newIndex);
        }

        private void CreateAgentConfigContextMenu()
        {
            var group = GetActiveGroup();
            if (group == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(group.ConfigCreateFolder))
            {
                Debug.LogError($"{group.GroupName}没有设置config创建目录");
                return;
            }
            if (group.CreateRendererConfig)
            {
                if (string.IsNullOrEmpty(group.RendererCreateFolder))
                {
                    Debug.LogError($"{group.GroupName}没有设置renderer创建目录");
                    return;
                }

                if (m_ActiveRendererTypeIndex < 0 || m_ActiveRendererTypeIndex >= m_AgentRendererConfigTypes.Count)
                {
                    return;
                }
            }

            var contextMenu = new GenericMenu();

            foreach (var type in m_AgentConfigTypes)
            {
                contextMenu.AddItem(new GUIContent(type.Name), false, () =>
                {
                    var group = GetActiveGroup();
                    if (group != null)
                    {
                        var agentConfig = ScriptableObject.CreateInstance(type) as AgentConfig;
                        var name = $"{type.Name}-{UnityEngine.Random.Range(0, int.MaxValue)}";
                        agentConfig.Name = name;

                        if (group.CreateRendererConfig)
                        {
                            var rendererConfig = ScriptableObject.CreateInstance(m_AgentRendererConfigTypes[m_ActiveRendererTypeIndex]) as AgentRendererConfig;
                            AssetDatabase.CreateAsset(rendererConfig, $"{group.RendererCreateFolder}/{name}_Renderer.asset");
                            agentConfig.Renderer = rendererConfig;
                        }

                        AssetDatabase.CreateAsset(agentConfig, $"{group.ConfigCreateFolder}/{name}.asset");
                        group.Configs.Add(agentConfig);

                        EditorUtility.SetDirty(group);
                        Save();
                    }
                });
            }
            contextMenu.ShowAsContext();
        }

        private void SetActiveGroup(int index)
        {
            if (m_ActiveGroupIndex != index)
            {
                m_ActiveGroupIndex = index;
            }
        }

        private AgentConfigGroup GetActiveGroup()
        {
            if (m_ActiveGroupIndex >= 0 && m_ActiveGroupIndex <= m_Groups.Count)
            {
                return m_Groups[m_ActiveGroupIndex];
            }
            return null;
        }

        public void RemoveInvalid()
        {
            var config = GetActiveGroup();
            for (var i = config.Configs.Count - 1; i >= 0; i--)
            {
                if (config.Configs[i] == null)
                {
                    config.Configs.RemoveAt(i);
                }
            }
            Save();
        }

        public void Save()
        {
            var group = GetActiveGroup();
            if (group != null)
            {
                EditorUtility.SetDirty(group);
            }
            AssetDatabase.SaveAssets();
        }

        private void GetAgentConfigTypes()
        {
            m_AgentConfigTypes = Helper.GetAllSubclasses(typeof(AgentConfig));
            m_AgentRendererConfigTypes = Helper.GetAllSubclasses(typeof(AgentRendererConfig));

            m_AgentRendererConfigNames = new string[m_AgentRendererConfigTypes.Count];
            for (var i = 0; i < m_AgentRendererConfigNames.Length; ++i)
            {
                m_AgentRendererConfigNames[i] = m_AgentRendererConfigTypes[i].Name;
            }

            if (m_AgentRendererConfigTypes.Count > 0)
            {
                m_ActiveRendererTypeIndex = 0;
            }
        }

        private void GetAgentGroupNames()
        {
            m_Groups = EditorHelper.QueryAssets<AgentConfigGroup>();
            m_AgentConfigGroupNames = new string[m_Groups.Count];
            for (var i = 0; i < m_Groups.Count; i++)
            {
                m_AgentConfigGroupNames[i] = $"{i}.{m_Groups[i].name}";
            }

            if (m_Groups.Count == 0)
            {
                SetActiveGroup(-1);
            }
            else if (m_ActiveGroupIndex < 0 || m_ActiveGroupIndex >= m_Groups.Count)
            {
                SetActiveGroup(0);
            }
        }

        internal void SelectFirstUseAgentConfig(ComponentBasedAgentRendererConfig rendererConfig)
        {
            var groupIdx = 0;
            foreach (var group in m_Groups)
            {
                for (var i = 0; i < group.Configs.Count; ++i)
                {
                    var config = group.Configs[i];
                    if (config.Renderer is ComponentBasedAgentRendererConfig c && c == rendererConfig)
                    {
                        Debug.Log($"使用者:{i}. {config.ConfigID}-{config.Name}");
                        SetActiveGroup(groupIdx);
                        break;
                    }
                }
                ++groupIdx;
            }
        }

        private int m_ActiveGroupIndex = -1;
        private int m_ActiveRendererTypeIndex = -1;
        private string[] m_AgentConfigGroupNames;
        private string[] m_AgentRendererConfigNames;
        private List<AgentConfigGroup> m_Groups = new();
        private Vector2 m_ScrollPos;
        private List<Type> m_AgentConfigTypes = new();
        private List<Type> m_AgentRendererConfigTypes = new();
        private AIEditor m_Editor;
    }
}