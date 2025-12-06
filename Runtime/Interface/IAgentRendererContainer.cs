
namespace XDay.AI
{
    public interface IAgentRendererContainer
    {
        void Update(float dt);

        T GetRenderer<T>(int agentID) where T : IAgentRenderer;
    }

    public interface IAgentRendererContainerCreateInfo
    {
    }

    public class AsyncAgentRendererContainerCreateInfo : IAgentRendererContainerCreateInfo
    {
    }
}
