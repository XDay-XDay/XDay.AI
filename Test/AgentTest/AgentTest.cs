using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XDay.AI;

#if UNITY_EDITOR

public class AgentTest : MonoBehaviour
{
    public Transform Target;
    public Transform Me;
    public List<Vector3> Path;

    private void Start()
    {
        m_World = IWorld.Create(new WorldCreateInfo()
        {
            ContainerCreateInfo = new SimpleAgentContainerCreateInfo(),
            RendererContainerCreateInfo = new AsyncAgentRendererContainerCreateInfo(),
            ObstacleManagerCreateInfo = new PhysicsObstacleManagerCreateInfo(),
            WorldCullerCreateInfo = new TopDownViewWorldCullerCreateInfo(),
            WorldTicker = new RVOAgentManager(),
            AssetLoader = new AssetLoader(),
        });

        m_Target = new AgentTarget(Target);

        CreateSteeringForceAgent(Vector3.zero);
        CreateRVOAgent(Vector3.zero);
        CreateRVOAgent(new Vector3(0, 0, 3));
    }

    private void CreateSteeringForceAgent(Vector3 position)
    {
        AgentConfig config = AssetDatabase.LoadAssetAtPath<Physics3DAgentConfig>("Assets/Game/Packages/XDayUnity.AI/Test/AgentTest/SteeringForceAgent.asset");

        var agent = m_World.CreateAgent(config, position);
        m_Agents.Add(agent);

        agent.Target = m_Target;
    }

    private void CreateRVOAgent(Vector3 position)
    {
        AgentConfig config = AssetDatabase.LoadAssetAtPath<Physics3DAgentConfig>("Assets/Game/Packages/XDayUnity.AI/Test/AgentTest/RVOAgent.asset");

        var agent = m_World.CreateAgent(config, position);
        m_Agents.Add(agent);
        agent.Target = m_Target;
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
        foreach (var agent in m_Agents)
        {
            agent?.Navigator?.DrawGizmos();
            agent?.DrawGizmos();
        }
    }

    private IAgentTarget m_Target;
    private IWorld m_World;
    private readonly List<IAgent> m_Agents = new();
}

class AgentTarget : IAgentTarget
{
    public Vector3 TargetPosition { get => m_Transform.position; set => m_Transform.position = value; }
    public Transform TargetTransform { get => m_Transform; set => m_Transform = value; }
    public Vector3 TargetVelocity => Vector3.zero;

    public AgentTarget(Transform transform)
    {
        m_Transform = transform;
    }

    private Transform m_Transform;
}

#endif