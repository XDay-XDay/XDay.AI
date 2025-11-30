using UnityEngine;

namespace XDay.AI
{
    internal class SteeringForceFlee : SteeringForce, ISteeringForceFlee
    {
        public override Vector3 Calculate(IAgent agent, float dt)
        {
            Debug.Assert(false, "todo Flee");
            return Vector3.zero;
        }
    }
}
