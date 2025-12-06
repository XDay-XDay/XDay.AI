/*
 * Copyright (c) 2024-2025 XDay
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

using System.Collections.Generic;
using UnityEngine;
using XDay.UtilityAPI;

namespace XDay.AI.Nav
{
    public class Border
    {
        public int PolygonIndex0 = -1;
        public int PolygonIndex1 = -1;
        public int VertexIndex0 = -1;
        public int VertexIndex1 = -1;
    }

    public class ConvexPolygon
    {
        public int PolygonType => m_PolygonType;
        public List<Border> Borders => m_Borders;
#if UNITY_EDITOR
        public List<Vector3> Vertices { get; set; }
        public Vector3[] LoopVertices { get; set; }
        public List<int> VertexIndices { get; set; }
        public Vector3 Center { get; set; }
        public int Index { get; set; }
#endif

        public ConvexPolygon(int type)
        {
            m_PolygonType = type;
        }

        public void AddBorder(Border border)
        {
            m_Borders.Add(border);
        }

        private readonly int m_PolygonType;
        private readonly List<Border> m_Borders = new();
    }

    public partial class Map
    {
        public List<ConvexPolygon> Polygons => m_ConvexPolygons;

        public void Create(List<Vector3> vertices, List<List<int>> convexPolygons, List<int> polygonTypes)
        {
            Debug.Assert(convexPolygons.Count == polygonTypes.Count);
            m_Vertices = vertices;
            for (var i = 0; i < convexPolygons.Count; ++i)
            {
                var polygon = new ConvexPolygon(polygonTypes[i]);
                SetDebugData(polygon, i, convexPolygons[i], vertices);
                m_ConvexPolygons.Add(polygon);
            }

            var navBorders = CreateBorders(convexPolygons);
#if UNITY_EDITOR
            m_Borders = navBorders;
#endif
            foreach (var border in navBorders)
            {
                m_ConvexPolygons[border.PolygonIndex0].AddBorder(border);
                m_ConvexPolygons[border.PolygonIndex1].AddBorder(border);
            }

            m_Renderer = new NavMapRenderer(this);
        }

        private void SetDebugData(ConvexPolygon polygon, int polygonIndex, List<int> indices, List<Vector3> vertices)
        {
            var verts = new List<Vector3>();
            var loopVerts = new List<Vector3>();
            foreach (var index in indices)
            {
                verts.Add(vertices[index]);
            }
            loopVerts.AddRange(verts);
            loopVerts.Add(verts[0]);
            polygon.Vertices = vertices;
            polygon.LoopVertices = loopVerts.ToArray();
            polygon.VertexIndices = new(indices);
            polygon.Index = polygonIndex;
            polygon.Center = Helper.CalculateCenter(verts);
        }

        public void OnDestroy()
        {
            m_Renderer?.OnDestroy();
        }

        public void Draw()
        {
            m_Renderer?.Draw();
        }

        private List<Border> CreateBorders(List<List<int>> polygons)
        {
            Dictionary<Vector2Int, Border> borders = new();
            for (var i = 0; i < polygons.Count; ++i)
            {
                var curPolygon = polygons[i];
                for (var cur = 0; cur < curPolygon.Count; ++cur)
                {
                    var next = (cur + 1) % curPolygon.Count;
                    var start = curPolygon[cur];
                    var end = curPolygon[next];
                    var key = new Vector2Int(Mathf.Min(start, end), Mathf.Max(start, end));
                    if (borders.TryGetValue(key, out var border))
                    {
                        border.PolygonIndex1 = i;
                    }
                    else
                    {
                        border = new Border()
                        {
                            PolygonIndex0 = i,
                            VertexIndex0 = key.x,
                            VertexIndex1 = key.y,
                        };
                        borders[key] = border;
                    }
                }
            }

            List<Border> result = new();
            foreach (var border in borders.Values)
            {
                if (border.PolygonIndex0 >= 0 &&
                    border.PolygonIndex1 >= 0)
                {
                    result.Add(border);
                }
            }
            return result;
        }

        private List<Vector3> m_Vertices;
        private readonly List<ConvexPolygon> m_ConvexPolygons = new();
        private NavMapRenderer m_Renderer;
#if UNITY_EDITOR
        private List<Border> m_Borders;
#endif
    }
}
