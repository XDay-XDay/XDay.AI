

namespace XDay.AI
{
    public interface INavigator
    {
        void DrawGizmos();

        void SetAgent(IAgent agent);

        void Update(float dt);
    }

    public interface ISteeringForceNavigator : INavigator
    {
        static ISteeringForceNavigator Create()
        {
            return new SteeringForceNavigator();
        }

        void AddSteeringForce(ISteeringForce sf);

        void GetSteeringForce<Type>() where Type : ISteeringForce;
    }
}
