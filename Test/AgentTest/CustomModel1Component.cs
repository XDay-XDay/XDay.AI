using XDay.UtilityAPI;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace XDay.AI
{
    [System.Serializable]
    [AgentComponentLabel(typeof(CustomModel1Component), "Custom Model1")]
    public class CustomModel1ComponentConfig : AgentRendererComponentConfig
    {
        public GameObject Prefab;
#if UNITY_EDITOR
        protected override void OnInspectorGUI()
        {
            Prefab = EditorGUILayout.ObjectField("Prefab", Prefab, typeof(GameObject), false) as GameObject;
        }
#endif
    }

    public class CustomModel1Component : AgentRendererComponent
    {
        public CustomModel1Component(AgentRendererComponentConfig config)
        {
            var cfg = config as CustomModel1ComponentConfig;
            m_Prefab = cfg.Prefab;
        }

        protected override void OnInit()
        {
            if (m_Prefab != null)
            {
                var gameObject = Object.Instantiate(m_Prefab);
                gameObject.transform.SetParent(Renderer.Agent.Root, false);
                gameObject.transform.localScale = Vector3.one;
                Renderer.SetGameObject(gameObject);
            }
        }

        protected override void OnDestroy()
        {
            var gameObject = Renderer.GameObject;
            Helper.DestroyUnityObject(gameObject);
            Renderer.SetGameObject(null);
        }

        private GameObject m_Prefab;
    }
}
