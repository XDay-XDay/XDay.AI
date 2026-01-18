/*
 * Copyright (c) 2024-2026 XDay
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to
 * permit persons to whom the Software is furnished to do so, subject to
 * the following conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
 * CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace XDay.AI.Nav
{
    public partial class Map
    {
        public bool FindPath(Vector3 start, Vector3 end, List<Vector3> path)
        {
            path.Clear();

            ConvexPolygon startArea = FindNavArea(start);
            ConvexPolygon endArea = FindNavArea(end);
            if (startArea == endArea)
            {
                path.Add(start);
                path.Add(end);
                return true;
            }

            m_Start = start;
            m_End = end;

            FindRouteBorders();

            if (m_RouteBorders.Count == 0)
            {
                return false;
            }

            FindVertices(path);

            return true;
        }


        private void FindRouteBorders()
        {
            m_RouteBorders.Clear();
            m_RouteBorders.AddRange(m_Borders);
        }

        private ConvexPolygon FindNavArea(Vector3 start)
        {
            throw new NotImplementedException();
        }

        private Vector3 m_CurrentLeftBorder;
        private Vector3 m_CurrentRightBorder;
        private Vector3 m_LeftVertex;
        private Vector3 m_RightVertex;
        //左顶点所在的border序号
        private int m_LeftVertexBorderIndex;
        //右顶点所在的border序号
        private int m_RightVertexBorderIndex;
        private Vector3 m_Apex;
        private Vector3 m_Start;
        private Vector3 m_End;
        private readonly List<Border> m_RouteBorders = new();
        private enum BorderRelation
        {
            //外扩
            Expand,
            //内收
            Shrink,
            //左边超过右边或右边超过左边
            Pass,
        }
    }
}
