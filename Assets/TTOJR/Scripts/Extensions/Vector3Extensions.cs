using UnityEngine;

namespace Extensions
{
    public static class Vector3EX
    {
        public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null)
            => new Vector3(x ?? vector.x, y ?? vector.y, z ?? vector.z);

    }
}