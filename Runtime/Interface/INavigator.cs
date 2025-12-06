using System;

namespace XDay.AI
{
    public interface INavigator
    {
        void OnDestroy();

        void DrawGizmos();

        void Update(float dt);
    }

    public interface INavigatorSteeringForce : INavigator
    {
        void AddSteeringForce(ISteeringForce sf);

        void GetSteeringForce<Type>() where Type : ISteeringForce;
    }

    public interface INavigatorRVO : INavigator
    {
        void SetDistanceOperator(Func<float, float> op);
        void SetSlowDistance(float distance);
    }
}
