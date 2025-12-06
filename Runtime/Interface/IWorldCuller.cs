using System.Collections.Generic;

namespace XDay.AI
{
    public class CullResult
    {
        public List<IAgent> VisibleAgents { get; } = new();
        public List<IAgent> InvisibleAgents { get; } = new();
    }

    public interface IWorldCullerCreateInfo
    {
    }

    public interface IWorldCuller
    {
        CullResult CullResult { get; }

        void Update(float dt);
    }

    public class TopDownViewWorldCullerCreateInfo : IWorldCullerCreateInfo
    {
    }
}
