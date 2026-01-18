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
            curY = 0;
            var flags = characterController.collisionFlags;
            if (flags.HasFlag(CollisionFlags.Below))
            {
                // 开始滑动逻辑
                verticalVelocity = HandleSliding(agent, characterController);
            }

            if (IsJump())
            {
                verticalVelocity = m_JumpSpeed;
                if (curY < 0)
                {
                    curY = 0;
                }
            }
        }
        else
        {
            m_IsSliding = false;
            m_SlidingHorizontalVelocity = Vector2.zero;
        }

        var curVerticalVelocity = curY + verticalVelocity - m_Gravity * Time.deltaTime;

        SetLinearVelocity(agent, new Vector3(desiredHorizontalVelocity.x + m_SlidingHorizontalVelocity.x, curVerticalVelocity, desiredHorizontalVelocity.y + m_SlidingHorizontalVelocity.y));
    }

    protected virtual void SetLinearVelocity(IAgent agent, Vector3 vector3)
    {
        agent.LinearVelocity = vector3;
    }

    private bool ShouldStartSliding(IAgent agent, CharacterController controller)
    {
        m_SlideRayDistance = controller.height + 0.1f;
        m_SlideRayOrigin = agent.Position + new Vector3(0, controller.height, 0);

        if (Physics.SphereCast(m_SlideRayOrigin, controller.radius, Vector3.down, out RaycastHit hit, m_SlideRayDistance))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            if (angle >= m_SlideThresholdAngle)
            {
                m_SlidingVerticalSpeed = -Vector3.Project(hit.normal, Vector3.up).y * m_Gravity;
                m_SlidingHorizontalSpeed = m_Gravity + m_SlidingVerticalSpeed;
                m_SlidePosition = hit.point;
                m_SlideDirection = Vector3.ProjectOnPlane(Vector3.down, hit.normal).normalized;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 处理滑动时的移动
    /// </summary>
    private float HandleSliding(IAgent agent, CharacterController controller)
    {
        var isSliding = ShouldStartSliding(agent, controller);
        if (!isSliding && m_IsSliding)
        {
            m_SlidingVerticalSpeed = 0;
        }
        m_IsSliding = isSliding;
        if (isSliding)
        {
            // 设置移动方向和速度
            m_SlidingHorizontalVelocity = Vector2.MoveTowards(m_SlidingHorizontalVelocity, Helper.ToVector2(m_SlideDirection).normalized * m_SlidingHorizontalSpeed, 10 * Time.deltaTime);
            //Debug.LogError($"issliding: {m_SlidingVerticalSpeed}, {m_SlidingHorizontalVelocity}");
            return m_SlidingVerticalSpeed;
        }
        else
        {
            m_SlidingHorizontalVelocity = Vector2.zero;
        }
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
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );
        return moveDirection;
    }

    protected virtual bool IsJump()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    internal override void DrawGizmo()
    {
        var agent = Renderer.Agent as CharacterControllerAgent;
        Gizmos.DrawSphere(m_SlideRayOrigin, agent.Controller.radius);
        Gizmos.DrawSphere(m_SlideRayOrigin + Vector3.down * m_SlideRayDistance, agent.Controller.radius);
        Gizmos.DrawLine(m_SlidePosition, m_SlidePosition + m_SlideDirection * 2);
    }

    private float m_Gravity;
    private float m_JumpSpeed;
    private float m_SlidingVerticalSpeed;
    private float m_SlidingHorizontalSpeed;
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

    private Vector3 m_SlideRayOrigin;
    private Vector3 m_SlidePosition;
    private Vector2 m_SlidingHorizontalVelocity;
    private float m_SlideRayDistance;
}

