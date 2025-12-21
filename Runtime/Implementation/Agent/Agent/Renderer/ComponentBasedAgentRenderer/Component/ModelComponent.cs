using XDay.UtilityAPI;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace XDay.AI
{
    [System.Serializable]
    [AgentComponentLabel(typeof(ModelComponent), "Model")]
    public class ModelComponentConfig : AgentRendererComponentConfig
    {
        public GameObject Prefab;
#if UNITY_EDITOR
        protected override void OnInspectorGUI()
        {
            Prefab = EditorGUILayout.ObjectField("Prefab", Prefab, typeof(GameObject), false) as GameObject;
        }
#endif
    }

    public class ModelComponent : AgentRendererComponent
    {
        public ModelComponent(AgentRendererComponentConfig config)
        {
            var cfg = config as ModelComponentConfig;
            m_Prefab = cfg.Prefab;
        }

        protected override void OnInit()
        {
            if (m_Prefab != null)
            {
                var gameObject = Object.Instantiate(m_Prefab);
                var root = Renderer.Agent.Root;
                gameObject.transform.SetParent(root, false);
                if (root == null)
                {
                    gameObject.transform.position = Renderer.Agent.Position;
                }
                Renderer.SetGameObject(gameObject);
            }
        }

        protected override void OnDestroy()
        {
            var gameObject = Renderer.GameObject;
            Helper.DestroyUnityObject(gameObject);
            Renderer.SetGameObject(null);
        }

        private readonly GameObject m_Prefab;
    }
}
