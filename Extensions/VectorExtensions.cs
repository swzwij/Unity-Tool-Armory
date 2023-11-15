using UnityEngine;

namespace swzwij.Extensions
{
    public static class VectorExtensions
    {
        /// <summary>
        /// Calculates the square distance between two points in 3D space.
        /// </summary>
        /// <param name="a">The first point in 3D space.</param>
        /// <param name="b">The second point in 3D space.</param>
        /// <returns>The square distance between points 'a' and 'b'.</returns>
        public static float SqrDistance(Vector3 a, Vector3 b)
        {
            Vector3 c = new(b.x - a.x, b.y - a.y, b.z - a.z);
            return c.x * c.x + c.y * c.y + c.z * c.z;
        }

        /// <summary>
        /// Sets the X axis of a Vector3 to the specified value.
        /// </summary>
        /// <param name="a">The original Vector3.</param>
        /// <param name="x">The new value for the X axis.</param>
        /// <returns>A new Vector3 with the X axis set to the specified value and the Y and Z axes unchanged.</returns>
        public static Vector3 SetX(this Vector3 a, float x) => new(x, a.y, a.z);

        /// <summary>
        /// Sets the Y axis of a Vector3 to the specified value.
        /// </summary>
        /// <param name="a">The original Vector3.</param>
        /// <param name="y">The new value for the Y axis.</param>
        /// <returns>A new Vector3 with the Y axis set to the specified value and the X and Z axes unchanged.</returns>
        public static Vector3 SetY(this Vector3 a, float y) => new(a.x, y, a.z);

        /// <summary>
        /// Sets the Z axis of a Vector3 to the specified value.
        /// </summary>
        /// <param name="a">The original Vector3.</param>
        /// <param name="z">The new value for the Z axis.</param>
        /// <returns>A new Vector3 with the Z axis set to the specified value and the X and Y axes unchanged.</returns>
        public static Vector3 SetZ(this Vector3 a, float z) => new(a.x, a.y, z);

        /// <summary>
        /// Sets the X axis of a Vector2 to the specified value.
        /// </summary>
        /// <param name="a">The original Vector2.</param>
        /// <param name="x">The new value for the X axis.</param>
        /// <returns>A new Vector2 with the X axis set to the specified value and the Y axis unchanged.</returns>
        public static Vector2 SetX(this Vector2 a, float x) => new(x, a.y);

        /// <summary>
        /// Sets the Y axis of a Vector2 to the specified value.
        /// </summary>
        /// <param name="a">The original Vector2.</param>
        /// <param name="y">The new value for the Y axis.</param>
        /// <returns>A new Vector2 with the Y axis set to the specified value and the X axis unchanged.</returns>
        public static Vector2 SetY(this Vector2 a, float y) => new(a.x, y);
    }
}