using UnityEngine;

namespace XDay.AI
{
    internal class SteeringForceChase : SteeringForce, ISteeringForceChase
    {
        public override Vector3 Calculate(IAgent agent, float dt)
        {
            Debug.Assert(false, "todo Chase");
            return Vector3.zero;
        }
    }
}
