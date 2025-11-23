using System;
using System.Collections.Generic;
using UnityEngine;

namespace XDay.AI
{
    public interface ISteeringForce
    {
        bool Enable { get; set; }

        Vector3 Calculate(IAgent agent, float dt);
        void DrawGizmos();
    }

    public interface ISteeringForceSeek : ISteeringForce
    {
        static ISteeringForceSeek Create()
        {
            return new SteeringForceSeek();
        }

        void SetTarget(Vector3 position);
        void SetTarget(Transform target);
    }

    public interface ISteeringForceArrive : ISteeringForce
    {
        static ISteeringForceArrive Create()
        {
            return new SteeringForceArrive();
        }

        void SetDistanceOperator(Func<float, float> op);
        void SetSlowDistance(float distance);
        void SetTarget(Vector3 position);
        void SetTarget(Transform target);
    }

    public enum PathMode
    {
        Once,
        Loop,
        PingPong,
    }

    public interface ISteeringForceFollowPath : ISteeringForce
    {
        static ISteeringForceFollowPath Create()
        {
            return new SteeringForceFollowPath();
        }

        PathMode PathMode { get; set; }

        void SetPath(List<Vector3> path);
        void SetSlowDistance(float distance);
    }

    public interface ISteeringForceObstacleAvoidance : ISteeringForce
    {
        static ISteeringForceObstacleAvoidance Create()
        {
            return new SteeringForceObstacleAvoidance();
        }

        float AvoidStrength { get; set; }
    }
}
