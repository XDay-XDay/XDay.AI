using UnityEngine;
using XDay.UtilityAPI;

namespace XDay.AI
{
    /// <summary>
    /// 使用Unity内置的CharacterController实现的Agent
    /// </summary>
    [AgentLabel(typeof(CharacterControllerAgentConfig))]
    internal class CharacterControllerAgent : Agent, ICharacterControllerAgent
    {
        public override Vector3 Position
        {
            get => m_Controller.transform.position;
            set => m_Controller.transform.position = value;
        }
        public override Quaternion Rotation { get => m_Controller.transform.rotation; set => m_Controller.transform.rotation = value; }
        public override Vector3 LinearVelocity { get => m_LinearVelocity; set => m_LinearVelocity = value; }
        public override float MaxLinearHorizontalSpeed { get => m_MaxLinearHorizontalSpeed; set => m_MaxLinearHorizontalSpeed = value; }
        public override float MaxLinearVerticalSpeed { get => m_MaxLinearVerticalSpeed; set => m_MaxLinearVerticalSpeed = value; }
        public override float MaxAngularSpeed { get => m_MaxAngularSpeed; set => m_MaxAngularSpeed = value; }
        public override Transform Root => m_Controller.transform;
        public bool IsGrounded => m_Controller.isGrounded;
        public CharacterController Controller => m_Controller;
        public bool MoveByForce => m_MoveByForce;

        public CharacterControllerAgent(int id, AgentConfig config, IWorld world, Vector3 position)
            : base(id, config, world, position)
        {
            var cfg = config as CharacterControllerAgentConfig;

            CreateCharacterController(cfg);
            Position = position;
            MaxLinearHorizontalSpeed = cfg.MaxLinearHorizontalSpeed;
            MaxLinearVerticalSpeed = cfg.MaxLinearVerticalSpeed;
            MaxAngularSpeed = cfg.MaxAngularSpeed;
            m_MoveByForce = cfg.MoveByForce;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            Helper.DestroyUnityObject(m_Controller.gameObject);
            m_Controller = null;
        }

        public override void AddForce(Vector3 force, ForceMode mode)
        {
            if (m_MoveByForce)
            {
                if (mode == ForceMode.VelocityChange ||
                mode == ForceMode.Impulse)
                {
                    m_LinearVelocity += force;
                }
                else
                {
                    m_TotalForce += force;
                    Helper.Clamp(ref m_TotalForce, MaxLinearAcceleration);
                }
            }
        }

        public override void Stop()
        {
            LinearVelocity = Vector3.zero;
            m_TotalForce = Vector3.zero;
        }

        public override void Update(float dt)
        {
            base.Update(dt);

            if (m_MoveByForce)
            {
                m_LinearVelocity += m_TotalForce * Time.deltaTime;
            }

            Helper.ClampXZ(ref m_LinearVelocity, MaxLinearHorizontalSpeed);
            Helper.ClampY(ref m_LinearVelocity, MaxLinearVerticalSpeed);

            if (m_LinearVelocity != Vector3.zero)
            {
                m_Controller.Move(m_LinearVelocity * Time.deltaTime);
            }

            m_TotalForce = Vector3.zero;

            if (m_Controller.isGrounded)
            {
                //m_LinearVelocity.y = 0;
            }

            //Debug.LogError($"isGrounded: {m_Controller.isGrounded}");
        }

        private void CreateCharacterController(CharacterControllerAgentConfig cfg)
        {
            var gameObject = new GameObject($"{Name} CharacterController");
            m_Controller = gameObject.AddComponent<CharacterController>();
            m_Controller.radius = ColliderRadius;
            m_Controller.center = cfg.Center;
            m_Controller.height = cfg.Height;
            m_Controller.stepOffset = cfg.StepOffset;
            m_Controller.slopeLimit = cfg.SlopeLimit;
        }

        private CharacterController m_Controller;
        private Vector3 m_LinearVelocity;
        private float m_MaxLinearHorizontalSpeed;
        private float m_MaxLinearVerticalSpeed;
        private float m_MaxAngularSpeed;
        private Vector3 m_TotalForce;
        private bool m_MoveByForce;
    }
}