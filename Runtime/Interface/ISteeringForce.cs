using System;
using System.Collections.Generic;
using UnityEngine;
using XDay.UtilityAPI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace XDay.AI
{
    public interface ISteeringForce
    {
        bool Enabled { get; set; }
        float Priority { get; set; }

        Vector3 Calculate(IAgent agent, float dt);
        void DrawGizmos(IAgent agent);
    }

    public interface ISteeringForceSeek : ISteeringForce
    {
        [Serializable]
        [SteeringForceLabel(typeof(SteeringForceSeek), "Seek")]
        public class Config : SteeringForceConfig
        {
#if UNITY_EDITOR
            protected override void OnInspectorGUI()
            {
            }
#endif
        }

        void SetOverridenTarget(IAgentTarget target);
    }

    public interface ISteeringForceArrive : ISteeringForce
    {
        [Serializable]
        [SteeringForceLabel(typeof(SteeringForceArrive), "Arrive")]
        public class Config : SteeringForceConfig
        {
            public float SlowDistance = 3;

#if UNITY_EDITOR
            protected override void OnInspectorGUI()
            {
                SlowDistance = EditorGUILayout.FloatField("Slow Distance", SlowDistance);
            }
#endif
        }

        void SetDistanceOperator(Func<float, float> op);
        void SetSlowDistance(float distance);
        void SetOverriddenTarget(IAgentTarget target);
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
        [SteeringForceLabel(typeof(SteeringForceFollowPath), "Follow Path")]
        public class Config : SteeringForceConfig
        {
            public PathMode PathMode = PathMode.Loop;
            public List<Vector3> Paths = new();
            public float SlowDistance = 3f;
            //路径点在障碍物情况下避免卡死
            public bool PassCheck = true;

#if UNITY_EDITOR
            protected override void OnInspectorGUI()
            {
                PathMode = (PathMode)EditorGUILayout.EnumPopup("Path Mode", PathMode);
                SlowDistance = EditorGUILayout.FloatField("Slow Distance", SlowDistance);
                PassCheck = EditorGUILayout.Toggle(new GUIContent("Pass Check", "路径点在障碍物情况下避免卡死"), PassCheck);
                EditorHelper.DrawVector3List("Paths", "Path", Paths);
            }
#endif
        }

        PathMode Mode { get; set; } 

        void SetPath(List<Vector3> path);
        void SetSlowDistance(float distance);
    }

    public interface ISteeringForceObstacleAvoidance : ISteeringForce
    {
        [Serializable]
        [SteeringForceLabel(typeof(SteeringForceObstacleAvoidance), "Obstacle Avoidance")]
        public class Config : SteeringForceConfig
        {
            public float AvoidStrength = 10f;

#if UNITY_EDITOR
            protected override void OnInspectorGUI()
            {
                AvoidStrength = EditorGUILayout.FloatField("Avoid Strength", AvoidStrength);
            }
#endif
        }

        float AvoidStrength { get; set; }
    }

    public interface ISteeringForceAlignment : ISteeringForce
    {
        [Serializable]
        [SteeringForceLabel(typeof(SteeringForceAlignment), "Alignment")]
        public class Config : SteeringForceConfig
        {
#if UNITY_EDITOR
            protected override void OnInspectorGUI()
            {
            }
#endif
        }
    }

    public interface ISteeringForceChase : ISteeringForce
    {
        [Serializable]
        [SteeringForceLabel(typeof(SteeringForceChase), "Chase")]
        public class Config : SteeringForceConfig
        {
#if UNITY_EDITOR
            protected override void OnInspectorGUI()
            {
            }
#endif
        }
    }

    public interface ISteeringForceCohesion : ISteeringForce
    {
        [Serializable]
        [SteeringForceLabel(typeof(SteeringForceCohesion), "Cohesion")]
        public class Config : SteeringForceConfig
        {
#if UNITY_EDITOR
            protected override void OnInspectorGUI()
            {
            }
#endif
        }
    }

    public interface ISteeringForceFlee : ISteeringForce
    {
        [Serializable]
        [SteeringForceLabel(typeof(SteeringForceFlee), "Flee")]
        public class Config : SteeringForceConfig
        {
            /// <summary>
            /// 进入该范围内才逃避
            /// </summary>
            public float ThreatDistance = 5f;
#if UNITY_EDITOR
            protected override void OnInspectorGUI()
            {
                ThreatDistance = EditorGUILayout.FloatField("Threat Distance", ThreatDistance);
            }
#endif
        }
    }

    public interface ISteeringForceSeparate : ISteeringForce
    {
        [Serializable]
        [SteeringForceLabel(typeof(SteeringForceSeparate), "Separate")]
        public class Config : SteeringForceConfig
        {
#if UNITY_EDITOR
            protected override void OnInspectorGUI()
            {
            }
#endif
        }
    }

    public interface ISteeringForceWander : ISteeringForce
    {
        [Serializable]
        [SteeringForceLabel(typeof(SteeringForceWander), "Wander")]
        public class Config : SteeringForceConfig
        {
            public float CircleRadius = 5f;
            public float CircleDistance = 10f;
            public float AngleChange = 5f;

#if UNITY_EDITOR
            protected override void OnInspectorGUI()
            {
                CircleRadius = EditorGUILayout.FloatField("Circle Radius", CircleRadius);
                CircleDistance = EditorGUILayout.FloatField("Circle Distance", CircleDistance);
                AngleChange = EditorGUILayout.FloatField("Angle Change", AngleChange);
            }
#endif
        }
    }
}
