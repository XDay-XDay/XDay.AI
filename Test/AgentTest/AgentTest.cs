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
            ObstacleManagerCreateInfo = new PhysicsObstacleManagerCreateInfo()
        });

        var physicsAgentCreateInfo = new Physics3DAgentCreateInfo()
        {
            Rigidbody = Rigidbody,
            Position = Me.transform.position
        };
        m_Agent = m_World.CreateAgent(physicsAgentCreateInfo);

        var navigator = ISteeringForceNavigator.Create();
        navigator.SetAgent(m_Agent);

        var seek = ISteeringForceSeek.Create();
        seek.SetTarget(Target);
        seek.Enable = false;
        navigator.AddSteeringForce(seek);

        var arrive = ISteeringForceArrive.Create();
        arrive.SetTarget(Target);
        arrive.SetSlowDistance(3f);
        arrive.Enable = true;
        navigator.AddSteeringForce(arrive);

        var followPath = ISteeringForceFollowPath.Create();
        followPath.Enable = false;
        followPath.SetPath(Path);
        followPath.SetSlowDistance(3f);
        followPath.PathMode = PathMode.Loop;
        navigator.AddSteeringForce(followPath);

        var avoidance = ISteeringForceObstacleAvoidance.Create();
        avoidance.Enable = true;
        avoidance.AvoidStrength = 30f;
        navigator.AddSteeringForce(avoidance);

        m_Agent.Navigator = navigator;

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