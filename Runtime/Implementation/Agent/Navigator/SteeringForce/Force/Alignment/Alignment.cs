using UnityEngine;

namespace XDay.AI
{
    internal class SteeringForceAlignment : SteeringForce, ISteeringForceAlignment
    {
        public SteeringForceAlignment(SteeringForceConfig config) : base(config)
        {
        }

        public override Vector3 Calculate(IAgent agent, float dt)
        {
            Debug.Assert(false, "todo Alignment");
            return Vector3.zero;
        }
    }
}
