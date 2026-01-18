using UnityEditor;
using UnityEngine;
using XDay.UtilityAPI;

namespace XDay.AI.Editor
{
    public partial class AIEditor : EditorWindow
    {
        [MenuItem("XDay/AI/Editor")]
        private static void Open()
        {
            var dlg = GetWindow<AIEditor>("AI Editor");
            dlg.Show();
        }

        private void OnEnable()
        {
            OnAgentTabEnable();
            OnNavigatorTabEnable();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();

            m_SelectedTabIndex = GUILayout.Toolbar(m_SelectedTabIndex, m_TabNames);

            EditorGUILayout.Space();

            if (GUILayout.Button("Save", GUILayout.MaxWidth(40)))
            {
                SaveAll();
            }

            if (GUILayout.Button("Refresh", GUILayout.MaxWidth(60)))
            {
                RefreshAll();
            }

            if (GUILayout.Button("Remove Invalid", GUILayout.MaxWidth(100)))
            {
                RemoveInvalidAll();
            }

            EditorGUILayout.EndHorizontal();

            EditorHelper.HorizontalLine();

            switch (m_SelectedTabIndex)
            {
                case AgentTab:
                    {
                        DrawAgentTab();
                        break;
                    }
                case NavigatorTab:
                    {
                        DrawNavigatorTab();
                        break;
                    }
                case WorldTab:
                    {
                        DrawWorldTab();
                        break;
                    }
                default: GUILayout.Label("Tab index is out of bounds"); break;
            }
        }
        
        public void SelectAgentConfig(AgentConfig config)
        {
            if (config == null)
            {
                return;
            }

            m_SelectedTabIndex = AgentTab;
            SetAgentTab(AgentInfoTab);
            m_AgentEditor.Select(config);
        }

        private void RemoveInvalidAll()
        {
            RemoveInvalidAgentTab();
            RemoveInvalidNavigatorTab();
        }

        private void RefreshAll()
        {
            RefreshAgentTab();
            RefreshNavigatorTab();

            RemoveInvalidAll();
        }

        private void SaveAll()
        {
            RemoveInvalidAll();

            SaveAgentTab();
            SaveNavigatorTab();
        }

        private int m_SelectedTabIndex = 0;
        private readonly string[] m_TabNames = new string[]
        {
            "Agent",
            "Navigator",
            "World",
        };
        public const int AgentTab = 0;
        public const int NavigatorTab = 1;
        public const int WorldTab = 2;
    }
}
