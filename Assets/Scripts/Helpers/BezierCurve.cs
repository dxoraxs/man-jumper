using UnityEngine;

namespace Helpers
{
    public static class BezierCurve
    {
        public static Vector3 CalculateBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            var x = Mathf.Pow(1 - t, 2) * p0.x + 2 * (1 - t) * t * p1.x + Mathf.Pow(t, 2) * p2.x;
            var y = Mathf.Pow(1 - t, 2) * p0.y + 2 * (1 - t) * t * p1.y + Mathf.Pow(t, 2) * p2.y;
            var z = Mathf.Pow(1 - t, 2) * p0.z + 2 * (1 - t) * t * p1.z + Mathf.Pow(t, 2) * p2.z;

            return new Vector3(x, y, z);
        }
    }
}