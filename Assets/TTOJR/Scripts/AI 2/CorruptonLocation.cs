using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DependencyInjection;
using Extensions;
using NUnit.Framework;
using UnityEngine;

public class CorruptonLocation : MonoBehaviour, IResidentLocation
{

    #region Privates
    [Inject] TimeCycle timeCy;
    [SerializeField] bool _corrupting;
    #endregion

    public bool corrupting { get => _corrupting; set => _corrupting = value; }
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
        CorruptionManager.instance.corruptEvents.Rand().StartCorrupt(this);
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


[Serializable]
public abstract class CorruptEvent
{
    public abstract void StartCorrupt(CorruptonLocation loc);
    public abstract void StopCorrupt(CorruptonLocation loc);
}

[Serializable]
public class CrowEffigyEvent : CorruptEvent
{
    public float timeToSearch = 4f;
    public GameObject cursedAreaPrefab;
    public GameObject crowEffigyPrefab;
    public override void StartCorrupt(CorruptonLocation loc)
    {
        loc.searchables.ForEach(s => s.AddComponent<Searchable>().timeToComplete = timeToSearch);
        CursedRoom room = GameObject.Instantiate(original: cursedAreaPrefab, loc.cursedAreaSpawnLoc).TryGet<CursedRoom>();
        Searchable correctSearchable = loc.searchables.Rand().TryGet<Searchable>();
        correctSearchable.SetAsCorrect(() => SpawnEffigy(loc ,correctSearchable, room));
        this.Log($"Corrupted location {loc.name}, searchable {correctSearchable.name}");
    }

    void SpawnEffigy(CorruptonLocation loc ,Searchable correctSearchable, CursedRoom room)
    {
        CrowEffigy effigy = GameObject.Instantiate(crowEffigyPrefab, correctSearchable.foundLoc).TryGet<CrowEffigy>();
        effigy.room = room;
        effigy.DestroyedHook.AddListener(() => loc.corrupting = false);
    }

    public override void StopCorrupt(CorruptonLocation loc)
    {

    }


}
