using System.Collections.Generic;
using System.Linq;
using Extensions;
using NUnit.Framework;
using UnityEngine;

public class CorruptionManager : MonoBehaviour
{
    public static CorruptionManager instance;

    #region Privates

    #endregion

    public List<CorruptonLocation> corruptionLocations;
    [SerializeReference] public List<CorruptEvent> corruptEvents;


    private void Awake()
    {
        instance = this;
        if (corruptionLocations == null || corruptionLocations.Count == 0)
            corruptionLocations = gameObject.GetComponentsInChildren<CorruptonLocation>().ToList();
        if (corruptEvents == null || corruptEvents.Count == 0)
            this.Error("Corrupt Events not set, please assign");
    }


    public void CorruptRandom()
    {
        corruptionLocations.Rand().StartCorruption();
    }


    #region Methods

    #endregion

}
