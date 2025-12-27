using System.Collections.Generic;
using UnityEngine;
using XDay.UtilityAPI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace XDay.AI
{
    public class AgentConfig : ScriptableObject
    {
        public int ConfigID;
        public string Name;
        public float MaxLinearHorizontalSpeed = 5f;
        public float MaxLinearVerticalSpeed = 50;
        public float MaxAngularSpeed = 360;
        public float ReachDistance = 0.5f;
        public float ColliderRadius = 0.5f;
        public float MaxLinearAcceleration = 100f;

        public NavigatorConfig Navigator;
        public AgentRendererConfig Renderer;
        public List<LineDetectorConfig> LineDetectors = new();

        public bool ShowInInspector = true;

#if UNITY_EDITOR
        public void InspectorGUI(int index, out bool selectRenderer, out bool ping, out bool deleted, out bool copyPath)
        {
            selectRenderer = false;
            ping = false;
            deleted = false;
            copyPath = false;
            EditorGUILayout.BeginHorizontal();
            ShowInInspector = EditorGUILayout.Foldout(ShowInInspector, $"{index}. {ConfigID}-{Name}           [{GetType().Name}]");
            if (GUILayout.Button("=>", GUILayout.MaxWidth(30)))
            {
                ping = true;
            }
            if (GUILayout.Button(new GUIContent("#", "复制路径"), GUILayout.MaxWidth(20)))
            {
                copyPath = true;
            }
            if (GUILayout.Button("X", GUILayout.MaxWidth(20)))
            {
                if (EditorUtility.DisplayDialog("Warning", "Continue deletion?", "Yes", "No"))
                {
                    deleted = true;
                }
            }
            EditorGUILayout.EndHorizontal();
            if (ShowInInspector)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();

                EditorGUI.BeginChangeCheck();
                Name = EditorGUILayout.TextField("Name", Name);
                if (EditorGUI.EndChangeCheck())
                {
                    if (!string.IsNullOrEmpty(Name))
                    {
                        var path = AssetDatabase.GetAssetPath(this);
                        AssetDatabase.RenameAsset(path, Name);
                    }
                }
                EditorGUILayout.EndHorizontal();
                ConfigID = EditorGUILayout.IntField("Config ID", ConfigID);
                MaxLinearHorizontalSpeed = EditorGUILayout.FloatField("Max Linear Horizontal Speed", MaxLinearHorizontalSpeed);
                MaxLinearVerticalSpeed = EditorGUILayout.FloatField("Max Linear Vertical Speed", MaxLinearVerticalSpeed);
                MaxAngularSpeed = EditorGUILayout.FloatField("Max Angular Speed", MaxAngularSpeed);
                ReachDistance = EditorGUILayout.FloatField("Reach Distance", ReachDistance);
                ColliderRadius = EditorGUILayout.FloatField("Collider Radius", ColliderRadius);
                MaxLinearAcceleration = EditorGUILayout.FloatField("Max Linear Acceleration", MaxLinearAcceleration);
                EditorGUILayout.BeginHorizontal();
                Renderer = EditorGUILayout.ObjectField("Renderer", Renderer, typeof(AgentRendererConfig), false) as AgentRendererConfig;
                if (GUILayout.Button("=>", GUILayout.MaxWidth(30)))
                {
                    selectRenderer = true;
                }
                EditorGUILayout.EndHorizontal();
                Navigator = EditorGUILayout.ObjectField("Navigator", Navigator, typeof(NavigatorConfig), false) as NavigatorConfig;
                EditorHelper.DrawList("Line Detectors", LineDetectors, (lineDetector, index) =>
                {
                    lineDetector.EulerAngle = EditorGUILayout.Vector3Field($"{index}. Direction", lineDetector.EulerAngle);
                    lineDetector.Length = EditorGUILayout.FloatField("Length", lineDetector.Length);
                    lineDetector.CollisionLayerMask = EditorGUILayout.MaskField(new GUIContent("Collision Layer"), lineDetector.CollisionLayerMask, InternalEditorUtility.layers);
                });

                OnInspectorGUI();
                EditorGUI.indentLevel--;
            }
        }

        protected virtual void OnInspectorGUI() { }
#endif
    }

    [System.Serializable]
    public class LineDetectorConfig
    {
        public Vector3 EulerAngle = Vector3.zero;
        public float Length = 3;
        public int CollisionLayerMask = -1;
    }
}
