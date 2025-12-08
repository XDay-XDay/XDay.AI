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
            AssetLoader = new XDay.WorldAPI.EditorWorldAssetLoader(),
        });

        m_Target = new AgentTarget(Target);

        CreateSteeringForceAgent(Vector3.zero);
        CreateRVOAgent(Vector3.zero);
        CreateRVOAgent(new Vector3(0, 0, 3));

        CreateFog();
    }

    private void CreateFog()
    {
        //m_FogSystem = new FogSystem();
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

        //m_FogSystem.Update(Time.deltaTime);

        //if (Input.GetKeyUp(KeyCode.Space))
        //{
        //    m_FogSystem.OpenRectangle(FogDataType.Client, 4, 4, 40, 40);
        //}

        //if (Input.GetKeyUp(KeyCode.Return))
        //{
        //    m_FogSystem.OpenRectangle(FogDataType.Client, 40, 40, 60, 60);
        //}

        //int sightRange = 16;
        //foreach (var agent in m_Agents)
        //{
        //    var centerCoord = m_FogSystem.PositionToCoord(agent.Position);
        //    var minX = centerCoord.x - sightRange / 2;
        //    var minY = centerCoord.y - sightRange / 2;
        //    var maxX = centerCoord.x + sightRange / 2;
        //    var maxY = centerCoord.y + sightRange / 2;
        //    m_FogSystem.OpenCircle(FogDataType.Client, minX, minY, maxX, maxY, false);
        //}
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
    //private FogSystem m_FogSystem;
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