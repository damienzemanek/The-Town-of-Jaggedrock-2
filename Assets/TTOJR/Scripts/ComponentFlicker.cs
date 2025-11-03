using System.Collections;
using Extensions;
using UnityEngine;

public class ComponentFlicker : MonoBehaviour
{

    #region Privates
    [SerializeField] bool flickerWhenEnabled;
    [SerializeField] bool flickering;
    [SerializeField] Light light;
    [SerializeField] Vector2 flickerDelay;
    #endregion

    private void Awake()
    {
        light = this.TryGet<Light>();
    }

    private void OnEnable()
    {
        if (flickerWhenEnabled) StartFlickering();
    }

    private void OnDisable() => StopFlickering();

    public void StartFlickering() => StartCoroutine(C_Flicker());

    public void StopFlickering()
    {
        StopAllCoroutines();
        flickering = false;
    }

    #region Methods

    IEnumerator C_Flicker()
    {
        flickering = true;
        while (true)
        {
            yield return new WaitForSeconds(flickerDelay.Rand());
            light.enabled = false;
            yield return new WaitForSeconds(flickerDelay.Rand());
            light.enabled = true;
        }
    }

    #endregion

}
