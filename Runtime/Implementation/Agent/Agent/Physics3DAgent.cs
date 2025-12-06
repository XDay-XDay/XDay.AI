using UnityEngine;
using XDay.UtilityAPI;

namespace XDay.AI
{
    [AgentLabel(typeof(Physics3DAgentConfig))]
    internal class Physics3DAgent : Agent
    {
        public override Vector3 Position 
        { 
            get => m_Rigidbody.position; 
            set => m_Rigidbody.position = value; 
        }
        public override Quaternion Rotation { get => m_Rigidbody.rotation; set => m_Rigidbody.rotation = value; }
#if ENABLE_TUANJIE
        public override Vector3 LinearVelocity { get => m_Rigidbody.velocity; set => m_Rigidbody.velocity = value; }
#else
        public override Vector3 LinearVelocity { get => m_Rigidbody.linearVelocity; set => m_Rigidbody.linearVelocity = value; }
#endif
        public override float MaxLinearSpeed { get => m_Rigidbody.maxLinearVelocity; set => m_Rigidbody.maxLinearVelocity = value; }
        public override float MaxAngularSpeed { get => m_Rigidbody.maxAngularVelocity; set => m_Rigidbody.maxAngularVelocity = value; }
        public override Transform Root => m_Rigidbody.transform;

        public Physics3DAgent(int id, AgentConfig config, IWorld world, Vector3 position)
            : base(id, config, world, position)
        {
            var cfg = config as Physics3DAgentConfig;

            CreateRigidbody();
            Position = position;
            m_Rigidbody.detectCollisions = cfg.EnableCollision;
            MaxLinearSpeed = cfg.MaxLinearSpeed;
            MaxAngularSpeed = cfg.MaxAngularSpeed;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            Helper.DestroyUnityObject(m_Rigidbody.gameObject);
            m_Rigidbody = null;
        }

        public override void AddForce(Vector3 force, ForceMode mode)
        {
            m_Rigidbody.AddForce(force, mode);
        }

        private void CreateRigidbody()
        {
            var gameObject = new GameObject($"{Name} Rigidbody");
            var collider = gameObject.AddComponent<UnityEngine.SphereCollider>();
            collider.radius = ColliderRadius;
            m_Rigidbody = gameObject.AddComponent<UnityEngine.Rigidbody>();
            m_Rigidbody.useGravity = false;
            m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }

        private UnityEngine.Rigidbody m_Rigidbody;
    }
}