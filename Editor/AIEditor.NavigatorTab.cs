using UnityEngine;
using XDay.UtilityAPI;

namespace XDay.AI.Editor
{
    internal partial class AIEditor
    {
        private void OnNavigatorTabEnable()
        {
            m_SteeringForceEditor.OnEnable();
            m_RVOEditor.OnEnable();
        }

        private void DrawNavigatorTab()
        {
            m_SelectedNavigatorTabIndex = GUILayout.Toolbar(m_SelectedNavigatorTabIndex, m_NavigatorTabNames);
            EditorHelper.HorizontalLine();

            switch (m_SelectedNavigatorTabIndex)
            {
                case SteeringForceTab:
                    {
                        m_SteeringForceEditor.OnGUI();
                        break;
                    }
                case RVOTab:
                    {
                        m_RVOEditor.OnGUI();
                        break;
                    }
                default: GUILayout.Label("Tab index is out of bounds"); break;
            }
        }

        private int m_SelectedNavigatorTabIndex = 0;
        private readonly SteeringForceEditor m_SteeringForceEditor = new();
        private readonly RVOEditor m_RVOEditor = new();
        private readonly string[] m_NavigatorTabNames = new string[]
        {
            "Steering Force",
            "RVO",
        };
        private const int SteeringForceTab = 0;
        private const int RVOTab = 1;
    }
}
