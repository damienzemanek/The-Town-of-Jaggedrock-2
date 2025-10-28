using UnityEngine;

namespace Extensions
{
    public static class TransformEX
    {
        public static Transform LookAtPosThenMyTransform(this Transform transform, Vector3 pos)
        {
            transform.LookAt(pos);
            return transform;
        }

        public static Transform WithEuler(this Transform transform, float? x = null, float? y = null, float? z = null)
        {
            transform.eulerAngles = new Vector3(
                x ?? transform.eulerAngles.x,
                y ?? transform.eulerAngles.y,
                z ?? transform.eulerAngles.z);
            return transform;
        }
    }
}