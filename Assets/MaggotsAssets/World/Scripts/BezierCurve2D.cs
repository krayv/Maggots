using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maggots
{
    public struct BezierCurve2D
    {
        public Vector2 point1;
        public Vector2 point2;
        public Vector2 point3;
        public Vector2 point4;
        public Vector2 point1UV;
        public Vector2 point2UV;
        public Vector2 point3UV;
        public Vector2 point4UV;

        public Vector2 Start { get { return point1; } }
        public Vector2[] Points
        {
            get
            {
                Vector2[] points = new Vector2[4];
                points[0] = point1;
                points[1] = point2;
                points[2] = point3;
                points[3] = point4;
                return points;
            }
        }

        public Vector2 LeftPoint
        {
            get
            {
                return Points.SelectFarthestVector(VectorExtensions.LeftVector());
            }
        }

        public Vector2 RightPoint
        {
            get
            {
                return Points.SelectFarthestVector(VectorExtensions.RightVector());
            }
        }

        public Vector2 UpPoint
        {
            get
            {
                return Points.SelectFarthestVector(VectorExtensions.UpVector());
            }
        }

        public Vector2 DownPoint
        {
            get
            {
                return Points.SelectFarthestVector(VectorExtensions.DownVector());
            }
        }

        public float Length
        {
            get
            {
                float chord = (point1 - point4).magnitude;
                float length = (point1 - point2).magnitude + (point3 - point2).magnitude + (point4 - point3).magnitude;
                return (length + chord) / 2;
            }
        }
        public BezierCurve2D(Vector2 point1, Vector2 point2, Vector2 point3, Vector2 point4, Vector2 xBorderRange, Vector2 yBorderRange)
        {
            this.point1 = point1;
            this.point2 = point2;
            this.point3 = point3;
            this.point4 = point4;
            point1UV = new Vector2(Mathf.InverseLerp(xBorderRange.x, xBorderRange.y, point1.x), Mathf.InverseLerp(yBorderRange.x, yBorderRange.y, point1.y));
            point2UV = new Vector2(Mathf.InverseLerp(xBorderRange.x, xBorderRange.y, point2.x), Mathf.InverseLerp(yBorderRange.x, yBorderRange.y, point2.y));
            point3UV = new Vector2(Mathf.InverseLerp(xBorderRange.x, xBorderRange.y, point3.x), Mathf.InverseLerp(yBorderRange.x, yBorderRange.y, point3.y));
            point4UV = new Vector2(Mathf.InverseLerp(xBorderRange.x, xBorderRange.y, point4.x), Mathf.InverseLerp(yBorderRange.x, yBorderRange.y, point4.y));
        }

        public void SetBorders(Vector2 xBorderRange, Vector2 yBorderRange)
        {
            point1UV = new Vector2(Mathf.InverseLerp(xBorderRange.x, xBorderRange.y, point1.x), Mathf.InverseLerp(yBorderRange.x, yBorderRange.y, point1.y));
            point2UV = new Vector2(Mathf.InverseLerp(xBorderRange.x, xBorderRange.y, point2.x), Mathf.InverseLerp(yBorderRange.x, yBorderRange.y, point2.y));
            point3UV = new Vector2(Mathf.InverseLerp(xBorderRange.x, xBorderRange.y, point3.x), Mathf.InverseLerp(yBorderRange.x, yBorderRange.y, point3.y));
            point4UV = new Vector2(Mathf.InverseLerp(xBorderRange.x, xBorderRange.y, point4.x), Mathf.InverseLerp(yBorderRange.x, yBorderRange.y, point4.y));
        }


        public Vector2 GetPoint(float progress)
        {
            Vector2 startToSecond = Vector2.Lerp(point1, point2, progress);
            Vector2 secondToThird = Vector2.Lerp(point2, point3, progress);
            Vector2 thirdToEnd = Vector2.Lerp(point3, point4, progress);
            Vector2 startToThird = Vector2.Lerp(startToSecond, secondToThird, progress);
            Vector2 secondToEnd = Vector2.Lerp(secondToThird, thirdToEnd, progress);
            return Vector2.Lerp(startToThird, secondToEnd, progress);
        }

        public Vector2 GetPointUV(float progress)
        {
            Vector2 startToSecond = Vector2.Lerp(point1UV, point2UV, progress);
            Vector2 secondToThird = Vector2.Lerp(point2UV, point3UV, progress);
            Vector2 thirdToEnd = Vector2.Lerp(point3UV, point4UV, progress);
            Vector2 startToThird = Vector2.Lerp(startToSecond, secondToThird, progress);
            Vector2 secondToEnd = Vector2.Lerp(secondToThird, thirdToEnd, progress);
            return Vector2.Lerp(startToThird, secondToEnd, progress);
        }

        public Vector2 GetNormal(float progress)
        {
            Vector2 startToSecond = Vector2.Lerp(point1UV, point2UV, progress);
            Vector2 secondToThird = Vector2.Lerp(point2UV, point3UV, progress);
            Vector2 thirdToEnd = Vector2.Lerp(point3UV, point4UV, progress);
            Vector2 startToThird = Vector2.Lerp(startToSecond, secondToThird, progress);
            Vector2 secondToEnd = Vector2.Lerp(secondToThird, thirdToEnd, progress);

            Vector3 tangent = (startToThird - secondToEnd).normalized;
            Vector3 normal = Vector3.zero;
            normal.x = tangent.y;
            normal.y = -tangent.x;

            return normal;
        }
    }
}

