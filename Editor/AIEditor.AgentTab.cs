using UnityEngine;
using XDay.UtilityAPI;

namespace XDay.AI.Editor
{
    internal partial class AIEditor
    {
        private void OnAgentTabEnable()
        {
            m_AgentEditor.OnEnable();
            m_ComponentBasedAgentRendererEditor.OnEnable();
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

        private readonly AgentEditor m_AgentEditor = new();
        private readonly ComponentBasedAgentRendererEditor m_ComponentBasedAgentRendererEditor = new();
        private int m_SelectedAgentTabIndex = 0;
        private readonly string[] m_AgentTabNames = new string[]
        {
            "Agent",
            "Component Based Renderer",
        };
        private const int AgentInfoTab = 0;
        private const int AgentComponentBasedRendererTab = 1;
    }
}
