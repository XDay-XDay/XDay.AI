using UnityEditor;
using UnityEngine;
using XDay.UtilityAPI;

namespace XDay.AI.Editor
{
    internal partial class AIEditor : EditorWindow
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
            m_SelectedTabIndex = GUILayout.Toolbar(m_SelectedTabIndex, m_TabNames);
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

        private int m_SelectedTabIndex = 0;
        private string[] m_TabNames = new string[]
        {
            "Agent",
            "Navigator",
            "World",
        };
        private const int AgentTab = 0;
        private const int NavigatorTab = 1;
        private const int WorldTab = 2;
    }
}
