using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Extensions
{
    public static class EnumerateEX
    {

        public static float Rand(this Vector2 v)
        {
            return UnityEngine.Random.Range(v.x, v.y);
        }

        #region Privates

        #endregion
        public static T Rand<T>(this T[] ts)
        {
            return ts[UnityEngine.Random.Range(0, ts.Length)];
        }
        public static T Rand<T>(this T[] ts, T[] exclude)
        {
            var include = ts.Where(t => !exclude.Contains(t)).ToArray();
            if (include.Length <= 0) return default;
            return Rand(include);
        }
        public static T Rand<T>(this T[] ts, int min, int max)
        {
            return ts[UnityEngine.Random.Range(min, max)];
        }


        public static T Rand<T>(this List<T> ts)
        {
            return ts[UnityEngine.Random.Range(0, ts.Count)];
        }

        public static T Rand<T>(this List<T> ts, List<T> exclude)
        {
            var include = ts.Where(t => !exclude.Contains(t)).ToList();
            if (include.Count <= 0) return default;
            return Rand(include);
        }

        public static T Rand<T>(this List<T> ts, int min, int max)
        {
            return ts[UnityEngine.Random.Range(min, max)];
        }



        #region Methods

        #endregion

    }

}