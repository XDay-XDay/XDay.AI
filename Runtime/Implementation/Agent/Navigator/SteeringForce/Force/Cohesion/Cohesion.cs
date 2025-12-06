using UnityEngine;

namespace XDay.AI
{
    internal class SteeringForceCohesion : SteeringForce, ISteeringForceCohesion
    {
        public SteeringForceCohesion(SteeringForceConfig config) : base(config)
        {
        }

        public override Vector3 Calculate(IAgent agent, float dt)
        {
            Debug.Assert(false, "todo Cohesion");
            return Vector3.zero;
        }
    }
}
