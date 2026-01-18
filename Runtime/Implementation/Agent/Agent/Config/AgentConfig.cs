using System.Collections.Generic;
using UnityEngine;
using XDay.UtilityAPI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace XDay.AI
{
    public enum SwapType
    {
        None,
        UpDown,
        DownUp,
    }

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
        public Vector2 InspectorGUI(int index, int n, out bool selectRenderer, out bool ping, out bool deleted, out bool copyPath, out bool moveToGroup, out SwapType swapped)
        {
            selectRenderer = false;
            ping = false;
            deleted = false;
            copyPath = false;
            moveToGroup = false;
            swapped = SwapType.None;
            EditorGUILayout.BeginHorizontal();
            ShowInInspector = EditorGUILayout.Foldout(ShowInInspector, $"{index}. {ConfigID}-{Name}           [{GetType().Name}]");

            var pos = new Vector2(-1, -1);
            if (Event.current.type == EventType.Repaint)
            {
                pos = GUILayoutUtility.GetLastRect().min;
            }

            if (index > 0)
            {
                if (GUILayout.Button(new GUIContent("<", "向上移动"), GUILayout.MaxWidth(20)))
                {
                    swapped = SwapType.DownUp;
                }
            }
            if (index < n - 1)
            {
                if (GUILayout.Button(new GUIContent(">", "向下移动"), GUILayout.MaxWidth(20)))
                {
                    swapped = SwapType.UpDown;
                }
            }
            if (GUILayout.Button(new GUIContent("=>", "选中资源"), GUILayout.MaxWidth(25)))
            {
                ping = true;
            }
            if (GUILayout.Button(new GUIContent("#", "复制路径"), GUILayout.MaxWidth(20)))
            {
                copyPath = true;
            }
            if (GUILayout.Button(new GUIContent("^", "移动到其他Group"), GUILayout.MaxWidth(20)))
            {
                moveToGroup = true;
            }
            if (GUILayout.Button(new GUIContent("X", "删除"), GUILayout.MaxWidth(20)))
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

            return pos;
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
