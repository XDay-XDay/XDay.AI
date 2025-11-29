using System;
using UnityEngine;

namespace XDay.AI
{
    public interface INavigator
    {
        void OnDestroy();

        void DrawGizmos();

        void SetAgent(IAgent agent);

        void Update(float dt);
    }

    public interface INavigatorSteeringForce : INavigator
    {
        static INavigatorSteeringForce Create()
        {
            return new NavigatorSteeringForce();
        }

        void AddSteeringForce(ISteeringForce sf);

        void GetSteeringForce<Type>() where Type : ISteeringForce;
    }

    public interface INavigatorRVO : INavigator
    {
        static INavigatorRVO Create()
        {
            return new NavigatorRVO();
        }

        void SetDistanceOperator(Func<float, float> op);
        void SetSlowDistance(float distance);
        void SetTarget(Vector3 position);
        void SetTarget(Transform target);
    }
}
