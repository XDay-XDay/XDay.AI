using UnityEditor;
using UnityEngine;
using XDay.UtilityAPI;

namespace XDay.AI.Editor
{
    internal class NavigatorSteeringForceConfigEditor : EditorWindow
    {
        [MenuItem("XDay/AI/NavigatorSteeringForceConfigEditor")]
        private static void Open()
        {
            var dlg = GetWindow<NavigatorSteeringForceConfigEditor>("NavigatorSteeringForceConfigEditor");
            dlg.Show();
        }

        private void OnGUI()
        {
            if (m_Config == null)
            {
                m_Config = EditorHelper.QueryAsset<NavigatorSteeringForceConfig>();
            }

            if (GUILayout.Button("Start")) 
            { 
                if (m_Config != null)
                {
                    m_Config.ForceConfigs.Add(new ISteeringForceSeek.Config() { });
                    EditorUtility.SetDirty(m_Config);
                    AssetDatabase.SaveAssets();
                }
            }
        }

        private NavigatorSteeringForceConfig m_Config;
    }
}
