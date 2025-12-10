using UnityEngine;
using Extensions;
using System.Collections;
using Sirenix.OdinInspector;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

[DefaultExecutionOrder(1)]
public class OvertimeGetBrighter : MonoBehaviour
{
    TimeCycle time;
    public float initial;
    [ReadOnly]
    public float intensity
    {
        get => light != null ? light.intensity : 0;
        set
        {
            if (light != null)
                light.intensity = value;
        }
    }
    public float max;
    [ReadOnly] public float step;
    public float delay;
    Light light;

    private void Awake()
    {
        time = TimeCycle.Instance;
        light = this.Get<Light>();
        intensity = initial;
    }

    private void OnEnable()
    {
        StartCoroutine(GainBrightnessOverTime());
    }

    IEnumerator GainBrightnessOverTime()
    {
        step = (max - initial) / CorruptionManager.instance.maxTimeUntilCorrupted;

        while(intensity <= max)
        {
            intensity = Mathf.Min(intensity + step, max);
            yield return new WaitForSecondsRealtime(delay);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        intensity = initial;
    }

}
