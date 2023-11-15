using UnityEngine;

namespace swzwij.Extensions
{
    public static class TransformExtensions
    {
        /// <summary>
        /// Destroys all child GameObjects of the specified Transform.
        /// </summary>
        /// <param name="transform">The parent Transform whose children will be destroyed.</param>
        public static void DestroyChildren(this Transform transform)
        {
            for (int i = 0; i < transform.childCount; i++)
                Object.Destroy(transform.GetChild(i).gameObject);
        }

        /// <summary>
        /// Resets the position, rotation, and scale of the specified Transform to their default values.
        /// </summary>
        /// <param name="transform">The Transform to be reset.</param>
        public static void ResetTransform(this Transform transform)
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        /// <summary>
        /// Sets the X position of a Transform.
        /// </summary>
        /// <param name="transform">The Transform to modify.</param>
        /// <param name="x">The new X position value.</param>
        public static void SetXPosition(this Transform transform, float x)
            => transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);

        /// <summary>
        /// Sets the Y position of a Transform.
        /// </summary>
        /// <param name="transform">The Transform to modify.</param>
        /// <param name="y">The new Y position value.</param>
        public static void SetYPosition(this Transform transform, float y)
            => transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);

        /// <summary>
        /// Sets the Z position of a Transform.
        /// </summary>
        /// <param name="transform">The Transform to modify.</param>
        /// <param name="z">The new Z position value.</param>
        public static void SetZPosition(this Transform transform, float z)
            => transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, z);

        /// <summary>
        /// Sets the X rotation of a Transform in degrees.
        /// </summary>
        /// <param name="transform">The Transform to modify.</param>
        /// <param name="x">The new X rotation value in degrees.</param>
        public static void SetXRotation(this Transform transform, float x)
            => transform.localRotation = Quaternion.Euler(x, transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z);

        /// <summary>
        /// Sets the Y rotation of a Transform in degrees.
        /// </summary>
        /// <param name="transform">The Transform to modify.</param>
        /// <param name="y">The new Y rotation value in degrees.</param>
        public static void SetYRotation(this Transform transform, float y)
            => transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, y, transform.localRotation.eulerAngles.z);

        /// <summary>
        /// Sets the Z rotation of a Transform in degrees.
        /// </summary>
        /// <param name="transform">The Transform to modify.</param>
        /// <param name="z">The new Z rotation value in degrees.</param>
        public static void SetZRotation(this Transform transform, float z)
            => transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, z);

        /// <summary>
        /// Sets the X scale of a Transform.
        /// </summary>
        /// <param name="transform">The Transform to modify.</param>
        /// <param name="x">The new X scale value.</param>
        public static void SetXScale(this Transform transform, float x)
            => transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);

        /// <summary>
        /// Sets the Y scale of a Transform.
        /// </summary>
        /// <param name="transform">The Transform to modify.</param>
        /// <param name="y">The new Y scale value.</param>
        public static void SetYScale(this Transform transform, float y)
            => transform.localScale = new Vector3(transform.localScale.x, y, transform.localScale.z);

        /// <summary>
        /// Sets the Z scale of a Transform.
        /// </summary>
        /// <param name="transform">The Transform to modify.</param>
        /// <param name="z">The new Z scale value.</param>
        public static void SetZScale(this Transform transform, float z)
            => transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, z);
    }
}