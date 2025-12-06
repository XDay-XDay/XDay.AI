using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XDay.UtilityAPI;

namespace XDay.AI.Editor
{
    /// <summary>
    /// 编辑Agent的配置
    /// </summary>
    internal class AgentEditor
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

            DrawAgentConfigs();

            EditorGUI.indentLevel++;

            EditorHelper.HorizontalLine();

            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);

            DrawActiveConfig();

            EditorGUI.indentLevel--;

            EditorGUILayout.EndScrollView();
        }

        private void DrawActiveConfig()
        {
            var config = GetActiveConfig();
            if (config == null)
            {
                return;
            }
            
            config.Name = EditorGUILayout.TextField("Name", config.Name);
            config.MaxLinearSpeed = EditorGUILayout.FloatField("Max Linear Speed", config.MaxLinearSpeed);
            config.MaxAngularSpeed = EditorGUILayout.FloatField("Max Angular Speed", config.MaxAngularSpeed);
            config.ReachDistance = EditorGUILayout.FloatField("Reach Distance", config.ReachDistance);
            config.ColliderRadius = EditorGUILayout.FloatField("Reach Distance", config.ColliderRadius);
            config.Renderer = EditorGUILayout.ObjectField("Renderer", config.Renderer, typeof(AgentRendererConfig), false) as AgentRendererConfig;
            config.Navigator = EditorGUILayout.ObjectField("Navigator", config.Navigator, typeof(NavigatorConfig), false) as NavigatorConfig;
            EditorHelper.DrawList("Line Detectors", config.LineDetectors, (lineDetector, index) =>
            {
                lineDetector.EulerAngle = EditorGUILayout.Vector3Field($"{index}. Direction", lineDetector.EulerAngle);
                lineDetector.Length = EditorGUILayout.FloatField("Length", lineDetector.Length);
                lineDetector.CollisionLayerMask = EditorGUILayout.MaskField(new GUIContent("Collision Layer"), lineDetector.CollisionLayerMask, InternalEditorUtility.layers);
            });

            if (config.GetType() == typeof(Physics3DAgentConfig))
            {
                var c = config as Physics3DAgentConfig;
                c.EnableCollision = EditorGUILayout.Toggle("Enable Collision", c.EnableCollision);
            }
            else
            {
                Debug.Assert(false, $"todo: {config.GetType()}");
            }
        }

        private void Refresh()
        {
            GetAgentConfigNames();
        }

        private void DrawAgentConfigs()
        {
            var agentConfig = GetActiveConfig();
            if (agentConfig == null)
            {
                return;
            }

            EditorGUIUtility.labelWidth = 110;
            EditorGUILayout.BeginHorizontal();
            var newIndex = EditorGUILayout.Popup("Agent Configs", m_ActiveConfigIndex, m_AgentConfigNames);
            if (GUILayout.Button("=>", GUILayout.MaxWidth(30)))
            {
                EditorHelper.PingObject(agentConfig);
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

        private AgentConfig GetActiveConfig()
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

        private void GetAgentConfigNames()
        {
            m_Configs = EditorHelper.QueryAssets<AgentConfig>();
            m_AgentConfigNames = new string[m_Configs.Count];
            for (var i = 0; i < m_Configs.Count; i++)
            {
                m_AgentConfigNames[i] = $"{m_Configs[i].name}-{m_Configs[i].GetInstanceID()}";
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
        private string[] m_AgentConfigNames;
        private List<AgentConfig> m_Configs = new();
        private Vector2 m_ScrollPos;
    }
}
