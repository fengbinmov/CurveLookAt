using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace MOV.Curve {

    public class MCurve
    {

        #region 2D

        public static Vector2[] Subsection(Vector2[] points, uint iStep)
        {

            if (points.Length < iStep)
                return points;

            uint iNewLength = (uint)points.Length / iStep;
            Vector2[] result = new Vector2[iNewLength + 1];

            for (uint i = 0, j = 0; i < iNewLength; i++)
            {
                result[i] = points[j];
                j += iStep;
            }
            result[result.Length - 1] = points[points.Length - 1];
            return result;
        }

        public static void DrawPathHelper(Vector2[] path, Color color, int smoothLevel = 5)
        {

            Vector2[] vector3s = PathControlPointGenerator(path);
            Vector2 prevPt = Interp(vector3s, 0);
            Gizmos.color = color;
            int SmoothAmount = path.Length * smoothLevel;
            for (int i = 1; i <= SmoothAmount; i++)
            {
                float pm = (float)i / SmoothAmount;
                Vector2 currPt = Interp(vector3s, pm);
                Gizmos.DrawLine(currPt, prevPt);
                prevPt = currPt;
            }
        }

        public static Vector2[] GetSmoothPath(Vector2[] path, int smoothLevel = 5)
        {
            if (smoothLevel == 1 || path.Length < 3) return path;

            int SmoothAmount = path.Length * smoothLevel;

            Vector2[] vector3s = PathControlPointGenerator(path);
            Vector2[] result = new Vector2[SmoothAmount];

            for (int i = 0; i < SmoothAmount; i++)
            {
                float pm = (float)i / SmoothAmount;
                result[i] = Interp(vector3s, pm);
            }
            result[0] = path[0];
            result[result.Length - 1] = path[path.Length - 1];

            result = SubStraigPoints(result);
            return result;
        }

        private static Vector2[] PathControlPointGenerator(Vector2[] path)
        {
            Vector2[] suppliedPath;
            Vector2[] vector3s;

            suppliedPath = path;

            int offset = 2;
            vector3s = new Vector2[suppliedPath.Length + offset];
            Array.Copy(suppliedPath, 0, vector3s, 1, suppliedPath.Length);

            vector3s[0] = vector3s[1] + (vector3s[1] - vector3s[2]);
            vector3s[vector3s.Length - 1] = vector3s[vector3s.Length - 2] + (vector3s[vector3s.Length - 2] - vector3s[vector3s.Length - 3]);

            if (vector3s[1] == vector3s[vector3s.Length - 2])
            {
                Vector2[] tmpLoopSpline = new Vector2[vector3s.Length];
                Array.Copy(vector3s, tmpLoopSpline, vector3s.Length);
                tmpLoopSpline[0] = tmpLoopSpline[tmpLoopSpline.Length - 3];
                tmpLoopSpline[tmpLoopSpline.Length - 1] = tmpLoopSpline[2];
                vector3s = new Vector2[tmpLoopSpline.Length];
                Array.Copy(tmpLoopSpline, vector3s, tmpLoopSpline.Length);
            }

            return vector3s;
        }
        
        private static Vector3 Interp(Vector2[] pts, float t)
        {
            int numSections = pts.Length - 3;
            int currPt = Mathf.Min(Mathf.FloorToInt(t * (float)numSections), numSections - 1);
            float u = t * (float)numSections - (float)currPt;

            Vector3 a = pts[currPt];
            Vector3 b = pts[currPt + 1];
            Vector3 c = pts[currPt + 2];
            Vector3 d = pts[currPt + 3];

            return .5f * ((-a + 3f * b - 3f * c + d) * (u * u * u) + (2f * a - 5f * b + 4f * c - d) * (u * u) + (-a + c) * u + 2f * b);
        }

        private static Vector2[] SubStraigPoints(Vector2[] points,float dotV = 0.99999f) {

            if (points.Length < 3 || dotV == -1f) return points;

            List<Vector2> result = new List<Vector2>();

            result.Add(points[0]);
            result.Add(points[1]);
            int iCount = points.Length;
            Vector2 dir1;
            Vector2 dir2;
            for (int i = 2; i < points.Length; i++)
            {
                dir1 = (points[i] - points[i - 1]).normalized;
                dir2 = (points[i - 1] - points[i - 2]).normalized;

                float fDotValue = Vector2.Dot(dir1, dir2);

                //Debug.Log(Time.realtimeSinceStartup + " " + fDotValue);
                if (fDotValue >= dotV)
                    result[result.Count - 1] = points[i];
                else
                    result.Add(points[i]);
            }
            return result.ToArray();
        }
        #endregion

        #region 3D

        public static Vector3[] Subsection(Vector3[] points, uint iStep)
        {

            if (points.Length < iStep)
                return points;

            uint iNewLength = (uint)points.Length / iStep;
            Vector3[] result = new Vector3[iNewLength + 1];

            for (uint i = 0, j = 0; i < iNewLength; i++)
            {
                result[i] = points[j];
                j += iStep;
            }
            result[result.Length - 1] = points[points.Length - 1];
            return result;
        }

        public static void DrawPathHelper(Vector3[] path, Color color, int smoothLevel = 5)
        {

            Vector3[] vector3s = PathControlPointGenerator(path);
            Vector3 prevPt = Interp(vector3s, 0);
            Gizmos.color = color;
            int SmoothAmount = path.Length * smoothLevel;
            for (int i = 1; i <= SmoothAmount; i++)
            {
                float pm = (float)i / SmoothAmount;
                Vector2 currPt = Interp(vector3s, pm);
                Gizmos.DrawLine(currPt, prevPt);
                prevPt = currPt;
            }
        }

        public static Vector3[] GetSmoothPath(Vector3[] path, int smoothLevel = 5)
        {
            if (smoothLevel == 1 || path.Length < 3) {

                Vector3[] p = new Vector3[path.Length];
                Array.Copy(path, 0, p, 0, p.Length);
                return p;
            }

            int SmoothAmount = path.Length * smoothLevel;

            Vector3[] vector3s = PathControlPointGenerator(path);
            Vector3[] result = new Vector3[SmoothAmount];

            for (int i = 0; i < SmoothAmount; i++)
            {
                float pm = (float)i / SmoothAmount;
                result[i] = Interp(vector3s, pm);
            }
            result[0] = path[0];
            result[result.Length - 1] = path[path.Length - 1];
            return result;
        }

        private static Vector3[] PathControlPointGenerator(Vector3[] path)
        {
            Vector3[] suppliedPath;
            Vector3[] vector3s;

            suppliedPath = path;

            int offset = 2;
            vector3s = new Vector3[suppliedPath.Length + offset];
            Array.Copy(suppliedPath, 0, vector3s, 1, suppliedPath.Length);

            vector3s[0] = vector3s[1] + (vector3s[1] - vector3s[2]);
            vector3s[vector3s.Length - 1] = vector3s[vector3s.Length - 2] + (vector3s[vector3s.Length - 2] - vector3s[vector3s.Length - 3]);

            if (vector3s[1] == vector3s[vector3s.Length - 2])
            {
                Vector3[] tmpLoopSpline = new Vector3[vector3s.Length];
                Array.Copy(vector3s, tmpLoopSpline, vector3s.Length);
                tmpLoopSpline[0] = tmpLoopSpline[tmpLoopSpline.Length - 3];
                tmpLoopSpline[tmpLoopSpline.Length - 1] = tmpLoopSpline[2];
                vector3s = new Vector3[tmpLoopSpline.Length];
                Array.Copy(tmpLoopSpline, vector3s, tmpLoopSpline.Length);
            }

            return vector3s;
        }

        private static Vector3 Interp(Vector3[] pts, float t)
        {
            int numSections = pts.Length - 3;
            int currPt = Mathf.Min(Mathf.FloorToInt(t * (float)numSections), numSections - 1);
            float u = t * (float)numSections - (float)currPt;

            Vector3 a = pts[currPt];
            Vector3 b = pts[currPt + 1];
            Vector3 c = pts[currPt + 2];
            Vector3 d = pts[currPt + 3];

            return .5f * ((-a + 3f * b - 3f * c + d) * (u * u * u) + (2f * a - 5f * b + 4f * c - d) * (u * u) + (-a + c) * u + 2f * b);
        }
        #endregion
    }

}