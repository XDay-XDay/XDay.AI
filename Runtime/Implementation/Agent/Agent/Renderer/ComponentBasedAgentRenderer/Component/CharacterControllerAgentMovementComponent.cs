using UnityEngine;
using XDay.AI;
using XDay.UtilityAPI;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
[AgentComponentLabel(typeof(CharacterControllerAgentMovementComponent), "Character Controller Agent Movement")]
public class CharacterControllerAgentMovementComponentConfig : AgentRendererComponentConfig
{
    public float JumpSpeed = 5;
    public float Gravity = 10f;

#if UNITY_EDITOR
    protected override void OnInspectorGUI()
    {
        JumpSpeed = EditorGUILayout.FloatField("Jump Speed", JumpSpeed);
        Gravity = EditorGUILayout.FloatField("Gravity", Gravity);
    }
#endif
}

/// <summary>
/// 控制CharacterControllerAgent的移动,可通过继承修改其行为
/// </summary>
public class CharacterControllerAgentMovementComponent : AgentRendererComponent
{
    public CharacterControllerAgentMovementComponent(AgentRendererComponentConfig config)
    {
        var cfg = config as CharacterControllerAgentMovementComponentConfig;
        m_Gravity = cfg.Gravity;
        m_JumpSpeed = cfg.JumpSpeed;
    }

    protected override void OnInit()
    {
    }

    protected override void OnPostInit()
    {
    }

    protected override void OnDestroy()
    {
    }

    protected override void OnUpdate(float dt)
    {
        var agent = Renderer.Agent as ICharacterControllerAgent;
        if (agent == null)
        {
            Debug.LogError("Agent is not character controller agent");
            return;
        }

        if (agent.MoveByForce)
        {
            MoveByForce(agent);
        }
        else
        {
            MoveByVelocity(agent);
        }
    }

    private void MoveByVelocity(ICharacterControllerAgent agent)
    {
        var characterController = agent.Controller;

        Vector2 horizontalMoveDirection = GetHorizontalMoveDirection();
        Vector2 desiredHorizontalVelocity = horizontalMoveDirection * agent.MaxLinearHorizontalSpeed;

        float verticalVelocity = 0;
        var curY = agent.LinearVelocity.y;
        if (characterController.isGrounded)
        {
            var flags = characterController.collisionFlags;
            m_IsSliding = ShouldStartSliding(agent, characterController);

            if (m_IsSliding)
            {
                // 开始滑动逻辑
                verticalVelocity = HandleSliding(desiredHorizontalVelocity);
            }

            if (IsJump())
            {
                verticalVelocity += m_JumpSpeed;
                if (curY < 0)
                {
                    curY = 0;
                }
            }
        }
        else
        {
            m_IsSliding = false;
        }

        var curVerticalVelocity = curY + verticalVelocity - m_Gravity * Time.deltaTime;
        agent.LinearVelocity = new Vector3(desiredHorizontalVelocity.x, curVerticalVelocity, desiredHorizontalVelocity.y);
    }

    private bool ShouldStartSliding(IAgent agent, CharacterController controller)
    {
        float raycastDistance = controller.height / 2 + 0.1f;
        Vector3 raycastOrigin = agent.Position;
        //if (Physics.Raycast(raycastOrigin, Vector3.down, out RaycastHit hit, raycastDistance))
        if (Physics.SphereCast(raycastOrigin, controller.radius, Vector3.down, out RaycastHit hit, raycastDistance))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            if (angle >= m_SlideThresholdAngle)
            {
                m_SlideDirection = Vector3.ProjectOnPlane(Vector3.down, hit.normal).normalized;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 处理滑动时的移动
    /// </summary>
    private float HandleSliding(Vector2 horizontalMoveDirection)
    {
        // 设置移动方向和速度
        horizontalMoveDirection += Helper.ToVector2(m_SlideDirection);
        return 0;
    }

    private void MoveByForce(ICharacterControllerAgent agent)
    {
        var characterController = agent.Controller;
        var horizontalMoveDirection = GetHorizontalMoveDirection();
        if (characterController.isGrounded)
        {
            if (IsJump())
            {
                agent.AddForce(new Vector3(0, m_JumpSpeed, 0), ForceMode.VelocityChange);
            }
        }

        var desiredXZ = new Vector3(horizontalMoveDirection.x, 0, horizontalMoveDirection.y) * agent.MaxLinearHorizontalSpeed - agent.LinearVelocity;
        agent.AddForce(new Vector3(desiredXZ.x, -m_Gravity, desiredXZ.z), ForceMode.Acceleration);
    }

    protected virtual Vector2 GetHorizontalMoveDirection()
    {
        var moveDirection = new Vector2
        (
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );
        //Vector3 moveDirection = Vector3.zero;
        //if (Input.GetKey(KeyCode.W))
        //{
        //    moveDirection.z = 1;
        //}
        //if (Input.GetKey(KeyCode.S))
        //{
        //    moveDirection.z = -1;
        //}
        //if (Input.GetKey(KeyCode.A))
        //{
        //    moveDirection.x = -1;
        //}
        //if (Input.GetKey(KeyCode.D))
        //{
        //    moveDirection.x = 1;
        //}
        //moveDirection.Normalize();
        return moveDirection;
    }

    protected virtual bool IsJump()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    private float m_Gravity;
    private float m_JumpSpeed;
    private bool m_IsSliding = false;
    /// <summary>
    /// 超过这个angle就可以滑动
    /// </summary>
    private float m_SlideThresholdAngle = 45;
    private Vector3 m_SlideDirection;
    [Tooltip("滑动时沿斜坡向下的加速度")]
    public float m_SlideAcceleration = 15.0f;
    [Tooltip("滑动时的最大速度")]
    public float m_MaxSlideSpeed = 20.0f;
    [Tooltip("滑动时的摩擦系数（用于减速）")]
    public float m_SlideFriction = 2.0f; // 可以为0表示无摩擦
}

