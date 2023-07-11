using UnityEngine;
using System;

namespace Maggots
{
    public static class VectorExtensions
    {
        public static float VectorToAngle(this Vector2 vector)
        {
            return Mathf.Atan2(vector.x, vector.y);
        }
    }
}
