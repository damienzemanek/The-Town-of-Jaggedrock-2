using System;
using System.Collections;
using System.Collections.Generic;
using DependencyInjection;
using Extensions;
using NUnit.Framework;
using UnityEngine;

public class CorruptonLocation : MonoBehaviour, IResidentLocation
{
    [Inject] TimeCycle timeCy;

    #region Privates

    #endregion
    [SerializeField] bool corrupting;

    [field: SerializeField] public Town resident { get; set; }

    public Transform cursedAreaSpawnLoc;
    [SerializeReference] public List<GameObject> searchables;

    public void StartCorruption()
    {
        this.Log($"Starting Corruption");
        StartCoroutine(C_CorruptEvent());
    }


    IEnumerator C_CorruptEvent()
    {
        corrupting = true;
        yield return new WaitForSeconds((timeCy.nightLengthInMinutes * 60) - timeCy.currentTime);
        if (corrupting) CorruptComplete();
    }
    
    void CorruptComplete()
    {
        corrupting = false;
        if (!resident) return;
        resident.IncreaseCorruption();
    }

    #region Methods
        
    #endregion

}


public abstract class CorruptEvent
{
    public abstract void StartCorrupt(CorruptonLocation loc);
    public abstract void StopCorrupt(CorruptonLocation loc);
}

[Serializable]
public class CrowEffigyEvent : CorruptEvent
{
    public GameObject cursedAreaPrefab;
    public override void StartCorrupt(CorruptonLocation loc)
    {
        GameObject crowSpawnGO = loc.searchables.Rand();
        CrowEffigy crow = crowSpawnGO.AddComponent<CrowEffigy>();
        crow.room = GameObject.Instantiate(cursedAreaPrefab, loc.cursedAreaSpawnLoc).TryGet<CursedRoom>();
    }

    public override void StopCorrupt(CorruptonLocation loc)
    {

    }


}
