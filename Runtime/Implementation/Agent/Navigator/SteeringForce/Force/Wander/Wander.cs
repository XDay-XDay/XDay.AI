using UnityEngine;

namespace XDay.AI
{
    internal class SteeringForceWander : SteeringForce, ISteeringForceWander
    {
        public override Vector3 Calculate(IAgent agent, float dt)
        {
            Debug.Assert(false, "todo Wander");
            return Vector3.zero;
        }
    }
}
