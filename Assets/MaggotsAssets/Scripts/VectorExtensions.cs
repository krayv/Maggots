using UnityEngine;
using System;
using System.Collections.Generic;

namespace Maggots
{
    public static class VectorExtensions
    {
        public static float VectorToAngle(this Vector2 vector)
        {
            return Mathf.Atan2(vector.x, vector.y);
        }

        public static Vector3 RemoveZ(this Vector3 vector)
        {
            return new Vector3(vector.x, vector.y, 0f);
        }


        public delegate bool CompareVectors(Vector2 vector1, Vector2 vector2);

        public static CompareVectors LeftVector()
        {
            return (vector1, vector2) =>
            {
                return vector1.x < vector2.x;
            };
        }

        public static CompareVectors RightVector()
        {
            return (vector1, vector2) =>
            {
                return vector1.x > vector2.x;
            };
        }

        public static CompareVectors DownVector()
        {
            return (vector1, vector2) =>
            {
                return vector1.y < vector2.y;
            };
        }

        public static CompareVectors UpVector()
        {
            return (vector1, vector2) =>
            {
                return vector1.y > vector2.y;
            };
        }

        public static Vector2 SelectFarthestVector(this Vector2[] vectors, CompareVectors compare)
        {
            Vector2 selectedVector = vectors[0];
            for (int i = 1; i < vectors.Length; i++)
            {
                var vector = vectors[i];
                selectedVector = compare(selectedVector, vector) ? selectedVector : vector;
            }
            return selectedVector;
        }

        public static Vector2 SelectFarthestVector(this List<Vector2> vectors, CompareVectors compare)
        {
            Vector2 selectedVector = vectors[0];
            for (int i = 0; i < vectors.Count; i++)
            {
                var vector = vectors[i];
                selectedVector = compare(selectedVector, vector) ? selectedVector : vector;
            }
            return selectedVector;
        }

        public static float InverseLerp(Vector2 a, Vector2 b, Vector2 value)
        {
            Vector2 AB = b - a;
            Vector2 AV = value - a;
            return Vector2.Dot(AV, AB) / Vector2.Dot(AB, AB);
        }

        public static bool EqualsValue(this Vector2Int a, Vector2Int b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool NeighborPixel(this Vector2Int a, Vector2Int b)
        {
            int xDiff = Mathf.Abs(a.x - b.x);
            int yDiff = Mathf.Abs(a.y - b.y);
            return xDiff <= 1 && yDiff <= 1;
        }

        public static Color GetPixel(this Texture2D texture, Vector2Int vector)
        {
            return texture.GetPixel(vector.x, vector.y);
        }

        public static bool IsOutOfBounds(this Vector2Int pixel, Texture2D texture)
        {
            return pixel.x < 0 || pixel.y < 0 || pixel.x >= texture.width || pixel.y >= texture.height;
        }

        public static Vector2Int Rotate90Degree(this Vector2Int point, bool positive)
        {
            return positive ? new Vector2Int(-point.y, point.x) : new Vector2Int(point.y, -point.x);
        }

        public static int Vector2IntToArrayIndex(this Vector2Int vector, int width)
        {
            return vector.y * width + vector.x;
        }
    }
}
