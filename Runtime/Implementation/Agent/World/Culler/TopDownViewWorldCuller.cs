

namespace XDay.AI
{
    [WorldCullerLabel(typeof(TopDownViewWorldCullerCreateInfo))]
    internal class TopDownViewWorldCuller : IWorldCuller
    {
        public CullResult CullResult => m_CullResult;

        public TopDownViewWorldCuller(IWorldCullerCreateInfo createInfo)
        {
        }

        public void Update(float dt)
        {
        }

        private readonly CullResult m_CullResult = new();
    }
}
