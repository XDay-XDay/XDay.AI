using System.Collections.Generic;
using UnityEngine;
using XDay.UtilityAPI;

namespace XDay.AI
{
    internal class SteeringForceFollowPath : SteeringForce, ISteeringForceFollowPath
    {
        public PathMode PathMode { get => m_PathMode; set => m_PathMode = value; }

        public SteeringForceFollowPath()
        {
            SetSlowDistance(3);
        }

        public override Vector3 Calculate(IAgent agent, float dt)
        {
            if (m_Path.Count == 0)
            {
                return Vector3.zero;
            }

            var force = m_Arrive.Calculate(agent, dt);

            if (m_Arrive.ReachTarget(agent))
            {
                ++m_CurrentPathIndex;
                if (m_CurrentPathIndex < m_Path.Count)
                {
                    m_Arrive.SetTarget(m_Path[m_CurrentPathIndex]);
                }
                else
                {
                    if (m_PathMode == PathMode.Once)
                    {
                        m_CurrentPathIndex = m_Path.Count - 1;
                        m_Arrive.SetTarget(m_Path[^1]);
                    }
                    else if (m_PathMode == PathMode.Loop)
                    {
                        m_CurrentPathIndex = 0;
                        m_Arrive.SetTarget(m_Path[0]);
                    }
                    else if (m_PathMode == PathMode.PingPong)
                    {
                        m_CurrentPathIndex = 0;
                        Helper.ReverseList(m_Path);
                        m_Arrive.SetTarget(m_Path[0]);
                    }
                }
            }
            return force;
        }

        public override void DrawGizmos()
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
                m_Arrive.SetTarget(path[0]);
            }
        }

        public void SetSlowDistance(float distance)
        {
            m_Arrive.SetSlowDistance(distance);
        }

        private int m_CurrentPathIndex;
        private readonly List<Vector3> m_Path = new();
        private readonly SteeringForceArrive m_Arrive = new();
        private PathMode m_PathMode = PathMode.Once;
    }
}
