using RVO;

namespace XDay.AI
{
    public class RVOAgentManager : IWorldTicker
    {
        public RVOAgentManager()
        {
            Simulator.Instance.setTimeStep((1 / 60.0f));

            Simulator.Instance.processObstacles();
        }

        public void Update(float dt)
        {
            Simulator.Instance.doStep();
        }
    }
}
