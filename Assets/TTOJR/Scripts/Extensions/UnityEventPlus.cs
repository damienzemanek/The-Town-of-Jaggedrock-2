using System;
using Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class UnityEventPlus
{

    #region Privates

    #endregion

    public enum DelayType
    {
        oneVal,
        randomValBetween
    }
    public DelayType delayType;

    [ShowIf(condition: "oneVal")] public float delay = 0;
    [ShowIf(condition: "randomValBetween")] public Vector2 random = Vector2.zero;

    public UnityEvent get = new UnityEvent();

    public void InvokeWithDelay(MonoBehaviour mono)
    {
        if (!mono) return;

        if(oneVal)
            mono.DelayedCall(() => get?.Invoke(), delay);
        else if(randomValBetween)
            mono.DelayedCall(() => get?.Invoke(), random.Rand());

    }

    public UnityEventPlus()
    {
        delay = 0;
        get = new UnityEvent();
    }

    public bool oneVal => delayType == DelayType.oneVal;
    public bool randomValBetween => delayType == DelayType.randomValBetween;


    #region Methods

    #endregion

}
