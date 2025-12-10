using UnityEngine;
using Extensions;
using Sirenix.OdinInspector;
using System.Collections;

[DefaultExecutionOrder(1)]
public class GrowOverTime : MonoBehaviour
{
    public Vector3 initial;
    [ReadOnly] public Vector3 currentScale { get => transform.localScale; set => transform.localScale = value; }

    public Vector3 max; [ReadOnly] public Vector3 step;
    public float delay;

    private void Awake()
    {
        currentScale = initial;
    }

    private void OnEnable()
    {
        StartCoroutine(Grow());
    }

    IEnumerator Grow()
    {
        float maxTime = CorruptionManager.instance.maxTimeUntilCorrupted;
        step = (max - initial) / maxTime;

        while (currentScale.x <= max.x ||
               currentScale.y <= max.y ||
               currentScale.z <= max.z)
        {
            Vector3 next = currentScale + step;
            currentScale = new Vector3(
                Mathf.Min(next.x, max.x),
                Mathf.Min(next.y, max.y),
                Mathf.Min(next.z, max.z)
            );

            yield return new WaitForSecondsRealtime(delay);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        currentScale = initial;
    }
}
