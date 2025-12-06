using System.Collections.Generic;
using UnityEngine;
using XDay.UtilityAPI;
using static XDay.AI.ISteeringForceFollowPath;

namespace XDay.AI
{
    internal class SteeringForceFollowPath : SteeringForce, ISteeringForceFollowPath
    {
        public PathMode Mode { get => m_PathMode; set => m_PathMode = value; }

        public SteeringForceFollowPath(SteeringForceConfig config) 
            : base(config)
        {
            var cfg = config as Config;

            m_OVerriddenTarget = new();

            m_CurrentPathIndex = 0;
            var arriveConfig = new ISteeringForceArrive.Config()
            {
                Enabled = true,
                Priority = 1f,
                SlowDistance = cfg.SlowDistance,
            };
            m_PassCheck = cfg.PassCheck;
            m_Arrive = new SteeringForceArrive(arriveConfig);
            SetSlowDistance(cfg.SlowDistance);
            Mode = cfg.PathMode;
            SetPath(cfg.Paths);
        }

        public override Vector3 Calculate(IAgent agent, float dt)
        {
            if (m_Path.Count == 0)
            {
                return Vector3.zero;
            }

            var force = m_Arrive.Calculate(agent, dt);

            if (m_Arrive.ReachTarget(agent) || PassCheck(agent))
            {
                ++m_CurrentPathIndex;
                if (m_CurrentPathIndex < m_Path.Count)
                {
                    SetArriveTarget(m_Path[m_CurrentPathIndex]);
                }
                else
                {
                    if (m_PathMode == PathMode.Once)
                    {
                        m_CurrentPathIndex = m_Path.Count - 1;
                        SetArriveTarget(m_Path[^1]);
                    }
                    else if (m_PathMode == PathMode.Loop)
                    {
                        m_CurrentPathIndex = 0;
                        SetArriveTarget(m_Path[0]);
                    }
                    else if (m_PathMode == PathMode.PingPong)
                    {
                        m_CurrentPathIndex = 0;
                        Helper.ReverseList(m_Path);
                        SetArriveTarget(m_Path[0]);
                    }
                }
            }
            return force;
        }


        private bool PassCheck(IAgent agent)
        {
            if (m_PassCheck && m_CurrentPathIndex > 0)
            {
                var dir = m_Path[m_CurrentPathIndex] - m_Path[m_CurrentPathIndex - 1];
                var delta = agent.Position - m_Path[m_CurrentPathIndex - 1];
                var project = Vector3.Project(delta, dir);
                if (project.sqrMagnitude > dir.sqrMagnitude)
                {
                    return true;
                }
            }
            return false;
        }

        protected override void OnDrawGizmos(IAgent agent)
        {
            for (var i = 0; i < m_Path.Count - 1; ++i)
            {
                Gizmos.DrawLine(m_Path[i], m_Path[i + 1]);
            }
        }

        public void SetPath(List<Vector3> path)
        {
            m_Path.Clear();
            m_Path.AddRange(path);

            if (path.Count > 0)
            {
                SetArriveTarget(path[0]);
            }
        }

        public void SetSlowDistance(float distance)
        {
            m_Arrive.SetSlowDistance(distance);
        }

        private void SetArriveTarget(Vector3 position)
        {
            m_OVerriddenTarget.TargetPosition = position;
            m_Arrive.SetOverriddenTarget(m_OVerriddenTarget);
        }

        private int m_CurrentPathIndex;
        private readonly List<Vector3> m_Path = new();
        private readonly SteeringForceArrive m_Arrive;
        private PathMode m_PathMode = PathMode.Once;
        private readonly Target m_OVerriddenTarget;
        private bool m_PassCheck;

        private class Target : IAgentTarget
        {
            public Vector3 TargetPosition { get; set; }
            public Transform TargetTransform { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
            public Vector3 TargetVelocity => Vector3.zero;
        }
    }
}
