using System.Collections;
using System.ComponentModel;
using Extensions;
using UnityEngine;
using Sirenix.OdinInspector;
using ReadOnlyAttribute = Sirenix.OdinInspector.ReadOnlyAttribute;
using System.Linq;
using Sirenix.Utilities;

public class ComponentFlicker : MonoBehaviour
{

    #region Privates
    enum FlickerPattern
    {
        FastConsistentCreepy,
        MostlyOnSporaticOff,
    }


    [SerializeField] FlickerPattern flickerPattern;
    [SerializeField] bool childrenFlicker;
    [SerializeField] bool componentFlicker;
    [SerializeField] bool flickerWhenEnabled;
    [SerializeField] bool leaveOn;
    [SerializeField, ReadOnly] bool flickering;
    [SerializeField, ShowIf("componentFlicker")] Light light;
    [SerializeField] Vector2 flickerDelay;
    GameObject[] children;
    #endregion

    private void Awake()
    {
        if(componentFlicker) light = this.TryGet<Light>();
        children = new GameObject[transform.childCount];
        children.FillWithChildren(gameObject);
        if (componentFlicker) light.enabled = leaveOn;
        if (childrenFlicker) FlickerDeactivate();
    }

    private void OnEnable()
    {
        if (flickerWhenEnabled) FlickerActivate();
    }

    private void OnDisable()
    {
        if (flickerWhenEnabled) FlickerDeactivate();
    }

    public void FlickerActivate()
    {
        StartFlickering();
    }

    public void FlickerDeactivate()
    {
        StopFlickering();
    }


    void StartFlickering()
    {
        if (componentFlicker)  StartCoroutine(C_Flicker());
        if (childrenFlicker) StartCoroutine(C_ChildrenFlicker());
    }

    void StopFlickering()
    {
        StopAllCoroutines();
        if (componentFlicker) light.enabled = false;
        if (childrenFlicker) children.ForEach(c => c.SetActive(leaveOn));
        flickering = false;
    }

    #region Methods

    IEnumerator C_Flicker()
    {
        light.enabled = true;
        flickering = true;
        while (true)
        {
            switch (flickerPattern)
            {
                case FlickerPattern.FastConsistentCreepy:
                    light.enabled = !light.enabled;
                    yield return new WaitForSeconds(flickerDelay.Rand());
                    break;

                case FlickerPattern.MostlyOnSporaticOff:
                    if (Random.value < 0.2f)
                    {
                        light.enabled = false;
                        yield return new WaitForSeconds(flickerDelay.Rand());
                        light.enabled = true;
                    }
                    yield return new WaitForSeconds(flickerDelay.Rand() * 5f);
                    break;

            }
        }
    }

    IEnumerator C_ChildrenFlicker()
    {
        flickering = true;
        while (true)
        {
            switch (flickerPattern)
            {
                case FlickerPattern.FastConsistentCreepy:
                    children.ForEach(c => c.SetActive(!c.activeSelf));
                    yield return new WaitForSeconds(flickerDelay.Rand());
                    break;

                case FlickerPattern.MostlyOnSporaticOff:
                    if (Random.value < 0.2f)
                    {
                        children.ForEach(c => c.SetActive(false));
                        yield return new WaitForSeconds(flickerDelay.Rand());
                        children.ForEach(c => c.SetActive(true));
                    }
                    yield return new WaitForSeconds(flickerDelay.Rand() * 5f);
                    break;
            }
        }
    }

    #endregion

}



