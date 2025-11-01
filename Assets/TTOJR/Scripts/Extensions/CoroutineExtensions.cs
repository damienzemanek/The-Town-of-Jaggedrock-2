using UnityEngine;
using System;
using System.Collections;

namespace Extensions
{
    public static class CoroutineExtensions
    {

        #region Privates

        #endregion

        public static void DelayedCall(this MonoBehaviour mono, Action method, float delay)
        {
            if(!mono || method == null) return;
            mono.StartCoroutine(C_CallAfterDelay(method, delay));
        }

        static IEnumerator C_CallAfterDelay(Action method, float delay)
        {
            yield return new WaitForSeconds(delay);
            method?.Invoke();
        }

        #region Methods

        #endregion

    }
}
