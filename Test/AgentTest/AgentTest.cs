using System.Collections.Generic;
using UnityEngine;
using XDay.AI;
using Cysharp.Threading.Tasks;
using XDay.Asset;
using Game.Asset;

public class AgentTest : MonoBehaviour
{
    public Transform Target;
    public Transform Me;
    public List<Vector3> Path;
    public Canvas Canvas;

    private async UniTaskVoid Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        var assteLoader = new RuntimeAssetLoader();
        await assteLoader.InitASync();

        m_World = IWorld.Create(new WorldCreateInfo()
        {
            ContainerCreateInfo = new SimpleAgentContainerCreateInfo(),
            RendererContainerCreateInfo = new AsyncAgentRendererContainerCreateInfo(),
            ObstacleManagerCreateInfo = new PhysicsObstacleManagerCreateInfo(),
            WorldCullerCreateInfo = new TopDownViewWorldCullerCreateInfo(),
            //WorldTicker = new RVOAgentManager(),
            WorldTicker = null,
            AssetLoader = assteLoader,
        });

        m_Target = new AgentTarget(Target);

        //for (var i = 0; i < 20; ++i)
        //{
        //    CreateSteeringForceAgent(Vector3.zero);
        //}

        CreateCharacterControllerAgent(Vector3.zero);

        //CreateRVOAgent(Vector3.zero);
        //CreateRVOAgent(new Vector3(0, 0, 3));

        //var obj = await AssetManager.LoadGameObjectAsync("Assets/Game/Res/UI/Window/Login/UILoginWindow.prefab");
        //obj.transform.SetParent(Canvas.transform, false);
    }

    private void CreateCharacterControllerAgent(Vector3 position)
    {
        AgentConfig config = AssetManager.Load<CharacterControllerAgentConfig>("Assets/Game/Packages/XDayUnity.AI/Test/AgentTest/Character Controller Agent.asset");

        var agent = m_World.CreateAgent(config, position);
        m_Agents.Add(agent);
    }

    private void CreateSteeringForceAgent(Vector3 position)
    {
        AgentConfig config = AssetManager.Load<RigidBodyAgentConfig>("Assets/Game/Packages/XDayUnity.AI/Test/AgentTest/SteeringForceAgent.asset");

        var agent = m_World.CreateAgent(config, position);
        m_Agents.Add(agent);

        agent.Target = m_Target;
    }

    private void CreateRVOAgent(Vector3 position)
    {
        AgentConfig config = AssetManager.Load<RigidBodyAgentConfig>("Assets/Game/Res/RVOAgent.asset");

        var agent = m_World.CreateAgent(config, position);
        m_Agents.Add(agent);
        agent.Target = m_Target;
    }

    private void OnDestroy()
    {
        m_World?.OnDestroy();
    }

    private void Update()
    {
        if (m_World == null)
        {
            return;
        }
        m_World.Update(Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.Return))
        {
            var obj = AssetManager.LoadGameObject("Assets/Game/Res/UI/Window/Login/UILoginWindow.prefab");
        }
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

