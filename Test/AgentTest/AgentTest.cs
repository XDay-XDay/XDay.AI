using RVO;
using System.Collections.Generic;
using UnityEngine;
using XDay.AI;

#if UNITY_EDITOR

public class AgentTest : MonoBehaviour
{
    public Transform Target;
    public Transform Me;
    public Rigidbody Rigidbody;
    public List<Vector3> Path;

    private void Start()
    {
        m_World = IWorld.Create(new WorldCreateInfo()
        {
            ContainerCreateInfo = new SimpleAgentContainerCreateInfo(),
            ObstacleManagerCreateInfo = new PhysicsObstacleManagerCreateInfo(),
            WorldTicker = new RVOAgentManager(),
        });

        var physicsAgentCreateInfo = new Physics3DAgentCreateInfo()
        {
            Rigidbody = Rigidbody,
            Position = Me.transform.position,
            MaxLinearSpeed = 3f,
            ColliderRadius = 0.5f,
            EnableCollision = false,
            MaxAngularSpeed = 360f,
            ReachDistance = 0.5f,
        };
        m_Agent = m_World.CreateAgent(physicsAgentCreateInfo);

        {
            var navigatorSteeringForce = INavigatorSteeringForce.Create();
            navigatorSteeringForce.SetAgent(m_Agent);

            var seek = ISteeringForceSeek.Create();
            seek.SetTarget(Target);
            seek.Enabled = false;
            navigatorSteeringForce.AddSteeringForce(seek);

            var arrive = ISteeringForceArrive.Create();
            arrive.SetTarget(Target);
            arrive.SetSlowDistance(3f);
            arrive.Enabled = false;
            navigatorSteeringForce.AddSteeringForce(arrive);

            var followPath = ISteeringForceFollowPath.Create();
            followPath.Enabled = true;
            followPath.SetPath(Path);
            followPath.SetSlowDistance(3f);
            followPath.Mode = ISteeringForceFollowPath.PathMode.Loop;
            navigatorSteeringForce.AddSteeringForce(followPath);

            var avoidance = ISteeringForceObstacleAvoidance.Create();
            avoidance.Enabled = false;
            avoidance.AvoidStrength = 30f;
            navigatorSteeringForce.AddSteeringForce(avoidance);

            m_Agent.Navigator = navigatorSteeringForce;
        }

        //{
        //    var navigatorRVO = INavigatorRVO.Create();
        //    navigatorRVO.SetAgent(m_Agent);
        //    navigatorRVO.SetTarget(Target);
        //    navigatorRVO.SetSlowDistance(6);

        //    m_Agent.Navigator = navigatorRVO;
        //}

        m_Agent.AddLineDetector(new LineDetector()
        {
            ColliderLayer = 0,
            Length = 5,
            LocalDirection = Vector3.forward,
        });

        m_Agent.AddLineDetector(new LineDetector()
        {
            ColliderLayer = 0,
            Length = 2,
            LocalDirection = Quaternion.Euler(0, 45, 0) * Vector3.forward,
        });

        m_Agent.AddLineDetector(new LineDetector()
        {
            ColliderLayer = 0,
            Length = 2,
            LocalDirection = Quaternion.Euler(0, -45, 0) * Vector3.forward,
        });
    }

    private void OnDestroy()
    {
        m_World.OnDestroy();
    }

    private void Update()
    {
        m_World.Update(Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        m_Agent?.Navigator?.DrawGizmos();
        m_Agent?.DrawGizmos();
    }

    private IWorld m_World;
    private IAgent m_Agent;
}

#endif