using System;
using System.Collections.Generic;
using UnityEngine;

namespace XDay.AI
{
    public interface ISteeringForce
    {
        bool Enabled { get; set; }
        float Priority { get; set; }

        Vector3 Calculate(IAgent agent, float dt);
        void DrawGizmos();
    }

    public interface ISteeringForceSeek : ISteeringForce
    {
        [Serializable]
        public class Config : SteeringForceConfig
        {
        }

        static ISteeringForceSeek Create()
        {
            return new SteeringForceSeek();
        }

        void SetTarget(Vector3 position);
        void SetTarget(Transform target);
    }

    public interface ISteeringForceArrive : ISteeringForce
    {
        [Serializable]
        public class Config : SteeringForceConfig
        {
            public float SlowDistance = 3;
        }

        static ISteeringForceArrive Create()
        {
            return new SteeringForceArrive();
        }

        void SetDistanceOperator(Func<float, float> op);
        void SetSlowDistance(float distance);
        void SetTarget(Vector3 position);
        void SetTarget(Transform target);
    }

    public interface ISteeringForceFollowPath : ISteeringForce
    {
        public enum PathMode
        {
            Once,
            Loop,
            PingPong,
        }

        [Serializable]
        public class Config : SteeringForceConfig
        {
            public PathMode PathMode;
        }

        static ISteeringForceFollowPath Create()
        {
            return new SteeringForceFollowPath();
        }

        PathMode Mode { get; set; } 

        void SetPath(List<Vector3> path);
        void SetSlowDistance(float distance);
    }

    public interface ISteeringForceObstacleAvoidance : ISteeringForce
    {
        [Serializable]
        public class Config : SteeringForceConfig
        {
            public float AvoidStrength;
        }

        static ISteeringForceObstacleAvoidance Create()
        {
            return new SteeringForceObstacleAvoidance();
        }

        float AvoidStrength { get; set; }
    }

    public interface ISteeringForceAlignment : ISteeringForce
    {
        [Serializable]
        public class Config : SteeringForceConfig
        {
        }

        static ISteeringForceAlignment Create()
        {
            return new SteeringForceAlignment();
        }
    }

    public interface ISteeringForceChase : ISteeringForce
    {
        [Serializable]
        public class Config : SteeringForceConfig
        {
        }

        static ISteeringForceChase Create()
        {
            return new SteeringForceChase();
        }
    }

    public interface ISteeringForceCohesion : ISteeringForce
    {
        [Serializable]
        public class Config : SteeringForceConfig
        {
        }

        static ISteeringForceCohesion Create()
        {
            return new SteeringForceCohesion();
        }
    }

    public interface ISteeringForceFlee : ISteeringForce
    {
        [Serializable]
        public class Config : SteeringForceConfig
        {
        }

        static ISteeringForceFlee Create()
        {
            return new SteeringForceFlee();
        }
    }

    public interface ISteeringForceSeparate : ISteeringForce
    {
        [Serializable]
        public class Config : SteeringForceConfig
        {
        }

        static ISteeringForceSeparate Create()
        {
            return new SteeringForceSeparate();
        }
    }

    public interface ISteeringForceWander : ISteeringForce
    {
        [Serializable]
        public class Config : SteeringForceConfig
        {
        }

        static ISteeringForceWander Create()
        {
            return new SteeringForceWander();
        }
    }
}
