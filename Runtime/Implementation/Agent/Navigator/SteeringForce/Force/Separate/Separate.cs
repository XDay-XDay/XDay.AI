using UnityEngine;

namespace XDay.AI
{
    internal class SteeringForceSeparate : SteeringForce, ISteeringForceSeparate
    {
        public override Vector3 Calculate(IAgent agent, float dt)
        {
            Debug.Assert(false, "todo Separate");
            return Vector3.zero;
        }
    }
}
