using System.Collections.Generic;
using System.Linq;
using Extensions;
using NUnit.Framework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class CorruptionManager : MonoBehaviour
{
    public static CorruptionManager instance;


    #region Privates
    [SerializeField] FadeScreen afflictBg;
    [SerializeField] FadeScreen afflictFadeInto;
    [SerializeField] GameObject afflictVisual;
    #endregion

    public List<CorruptonLocation> corruptionLocations;
    [SerializeReference] public List<CorruptEvent> corruptEvents;

    public UnityEvent onCorrupted;


    private void Awake()
    {
        instance = this;
        if(onCorrupted == null) onCorrupted = new UnityEvent();
        if (corruptionLocations == null || corruptionLocations.Count == 0)
            corruptionLocations = gameObject.GetComponentsInChildren<CorruptonLocation>().ToList();
        if (corruptEvents == null || corruptEvents.Count == 0)
            this.Error("Corrupt Events not set, please assign");
    }



    public void CorruptRandom()
    {
        corruptionLocations.Rand().StartCorruption();
    }

    [Button]
    public void AfflictResidentDisplay()
    {
        afflictBg.gameObject.SetActive(true);
        afflictFadeInto.gameObject.SetActive(true);
        afflictVisual.SetActive(true);
        afflictFadeInto.FadeToVisible(() => this.DelayedCall(AfflictStop, 2));
    }

    void AfflictStop()
    {
        afflictFadeInto.FadeToBlack(() =>
        {
            afflictFadeInto.gameObject.SetActive(false);
            afflictVisual.SetActive(false);
            afflictBg.FadeToVisible(() => afflictBg.gameObject.SetActive(false));

        });
    }


    #region Methods

    #endregion

}
