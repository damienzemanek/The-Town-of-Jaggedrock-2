using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Extensions
{
    public static class EnumerateEX
    {

        public static float Rand(this Vector2 v)
        {
            return UnityEngine.Random.Range(v.x, v.y);
        }

        public static float Rand(float min, float max)
        {
            return UnityEngine.Random.Range(min, max);
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
        public static T RandAndRemove<T>(this List<T> ts)
        {
            T get = ts[UnityEngine.Random.Range(0, ts.Count)];
            ts.Remove(get);
            return get;
        }

        public static T Rand<T>(this List<T> ts, List<T> exclude)
        {
            var include = ts.Where(t => !exclude.Contains(t)).ToList();
            if (include.Count <= 0) return default;
            return Rand(include);
        }

        public static T RandAndRemove<T>(this List<T> ts, List<T> exclude)
        {
            var include = ts.Where(t => !exclude.Contains(t)).ToList();
            if (include.Count <= 0) return default;
            T get = Rand(include);
            ts.Remove(get);
            return get;
        }

        public static T Rand<T>(this List<T> ts, int min, int max)
        {
            return ts[UnityEngine.Random.Range(min, max)];
        }


        public static void MakeShallowCopyOf<T>(this List<T> ts, List<T> copyFrom)
        {
            ts.Clear();

            for (int i = 0; i < copyFrom.Count; i++)
                ts.Add(item: copyFrom[i]);
        }

        public static void RemoveNulls<T>(this List<T> ts) =>
            ts.RemoveAll(t => t == null);


        public static void Ensure<T>(this List<T> ts)
        {
            if (ts == null || ts.Count == 0) { ts.Error("list null, or has no values"); return; }
            ts.RemoveNulls();
        }

        public static List<T> Combine<T>(List<T> ts1, List<T> ts2)
        {
            List<T> combined = new List<T>(ts1.Count + ts2.Count);
            combined.AddRange(ts1);
            combined.AddRange(ts2);
            return combined;
        }




        #region Methods

        #endregion

    }

}