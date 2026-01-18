using UnityEngine;
using XDay.UtilityAPI;

namespace XDay.AI.Editor
{
    public partial class AIEditor
    {
        public void SetAgentTab(int tab)
        {
            m_SelectedAgentTabIndex = tab;
        }

        public void SetActiveAgentRendererConfig(AgentRendererConfig renderer)
        {
            if (renderer is ComponentBasedAgentRendererConfig r)
            {
                m_ComponentBasedAgentRendererEditor.SetActiveAgentRendererConfig(r);
            }
            else
            {
                Debug.Assert(false, "todo");
            }
        }

        internal void SelectFirstUseAgentConfig(ComponentBasedAgentRendererConfig config)
        {
            SetAgentTab(AIEditor.AgentInfoTab);
            m_AgentEditor.SelectFirstUseAgentConfig(config);
        }

        private void OnAgentTabEnable()
        {
            m_AgentEditor = new(this);
            m_AgentEditor.OnEnable();

            m_ComponentBasedAgentRendererEditor = new(this);
            m_ComponentBasedAgentRendererEditor.OnEnable();
        }

        private void SaveAgentTab()
        {
            m_AgentEditor.Save();
            m_ComponentBasedAgentRendererEditor.Save();
        }

        private void RefreshAgentTab()
        {
            m_AgentEditor.Refresh();
            m_ComponentBasedAgentRendererEditor.Refresh();
        }

        private void RemoveInvalidAgentTab()
        {
            m_AgentEditor.RemoveInvalid();
            m_ComponentBasedAgentRendererEditor.RemoveInvalid();
        }

        private void DrawAgentTab()
        {
            m_SelectedAgentTabIndex = GUILayout.Toolbar(m_SelectedAgentTabIndex, m_AgentTabNames);
            EditorHelper.HorizontalLine();

            switch (m_SelectedAgentTabIndex)
            {
                case AgentInfoTab:
                    {
                        m_AgentEditor.OnGUI();
                        break;
                    }
                case AgentComponentBasedRendererTab:
                    {
                        m_ComponentBasedAgentRendererEditor.OnGUI();
                        break;
                    }
                default: GUILayout.Label("Tab index is out of bounds"); break;
            }
        }

        private AgentEditor m_AgentEditor;
        private ComponentBasedAgentRendererEditor m_ComponentBasedAgentRendererEditor;
        private int m_SelectedAgentTabIndex = 0;
        private readonly string[] m_AgentTabNames = new string[]
        {
            "Agent",
            "Component Based Renderer",
        };
        public const int AgentInfoTab = 0;
        public const int AgentComponentBasedRendererTab = 1;
    }
}
