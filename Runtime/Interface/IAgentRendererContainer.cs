
namespace XDay.AI
{
    public interface IAgentRendererContainer
    {
        void Update(float dt);

        T GetRenderer<T>(int agentID) where T : IAgentRenderer;
        void OnDestroy();
    }

    public interface IAgentRendererContainerCreateInfo
    {
    }

    public class AsyncAgentRendererContainerCreateInfo : IAgentRendererContainerCreateInfo
    {
    }
}
