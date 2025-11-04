using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DependencyInjection;
using Extensions;
using SingularityGroup.HotReload;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using ReadOnlyAttribute = Sirenix.OdinInspector.ReadOnlyAttribute;

public class CorruptonLocation : MonoBehaviour, IResidentLocation
{

    #region Privates
    [Inject] TimeCycle timeCy;
    [SerializeField] bool _corrupting;
    [SerializeReference, ReadOnly] CorruptEvent currentEvent;
    #endregion

    public bool corrupting { get => _corrupting; set => _corrupting = value; }
    [field: SerializeField] public Town resident { get; set; }
    public Transform cursedAreaSpawnLoc;

    [TabGroup("Crow")] [SerializeReference] public List<GameObject> searchables;
    [TabGroup("Crow")] public GameObject[] flickerObjs;

    [TabGroup("Sacrifice")] public Transform sacrificeSpawnLoc;
    [TabGroup("Sacrifice")] public Transform doorBloodSpawnLoc;


    private void Start()
    {
        ResetAll();
    }

    void ResetAll()
    {
        flickerObjs.ToList().ForEach(o => o.TryGet<ComponentFlicker>().FlickerDeactivate());
        currentEvent = null;
    }

    public void StartCorruption()
    {
        this.Log($"Starting Corruption");
        StartCoroutine(C_CorruptEvent());
    }

    IEnumerator C_CorruptEvent()
    {
        corrupting = true;
        currentEvent = CorruptionManager.instance.corruptEvents.Rand().StartCorrupt(this);
        yield return new WaitForSeconds((timeCy.nightLengthInMinutes * 60) - timeCy.currentTime);
        if (corrupting) CorruptComplete();
    }

    public void StopCorruption()
    {
        corrupting = false;
        currentEvent.StopCorrupt(this);
        currentEvent = null;
    }
    
    void CorruptComplete()
    {
        if (!resident) this.Error("No resident has been set");
        resident.IncreaseCorruption();
        StopCorruption();
    }

    #region Methods

    #endregion

}


[Serializable]
public abstract class CorruptEvent
{
    public abstract CorruptEvent StartCorrupt(CorruptonLocation loc);
    public abstract void StopCorrupt(CorruptonLocation loc);
}

[Serializable]
public class CrowEffigyEvent : CorruptEvent
{
    public float timeToSearch = 4f;
    public GameObject cursedAreaPrefab;
    public GameObject crowEffigyPrefab;

    CursedRoom room;

    public override CorruptEvent StartCorrupt(CorruptonLocation loc)
    {
        room = null;
        loc.searchables.ForEach(s => s.AddComponent<Searchable>().timeToComplete = timeToSearch);
        room = GameObject.Instantiate(original: cursedAreaPrefab, loc.cursedAreaSpawnLoc).TryGet<CursedRoom>();
        Searchable correctSearchable = loc.searchables.Rand().TryGet<Searchable>();
        correctSearchable.SetAsCorrect(() => SpawnEffigy(loc ,correctSearchable, room));
        this.Log($"Corrupted location {loc.name}, searchable {correctSearchable.name}");

        loc.flickerObjs.ToList().ForEach(o => o.TryGet<ComponentFlicker>().FlickerActivate());

        return this;
    }

    void SpawnEffigy(CorruptonLocation loc ,Searchable correctSearchable, CursedRoom room)
    {
        CrowEffigy effigy = GameObject.Instantiate(crowEffigyPrefab, correctSearchable.foundLoc).TryGet<CrowEffigy>();
        effigy.room = room;
        effigy.DestroyedHook.AddListener(loc.StopCorruption);
    }

    public override void StopCorrupt(CorruptonLocation loc)
    {
        loc.searchables.ForEach(s => s.TryGet<Searchable>().ComponentReset());
        loc.flickerObjs.ToList().ForEach(o => o.TryGet<ComponentFlicker>().FlickerDeactivate());
        GameObject.Destroy(room.gameObject);
    }


}

[Serializable]
public class SacrificeEvent : CorruptEvent
{
    public GameObject sacrificePrefab;
    public GameObject bloodDoorPrefab;

    public override CorruptEvent StartCorrupt(CorruptonLocation loc)
    {
        loc.flickerObjs.ToList().ForEach(o => o.TryGet<ComponentFlicker>().FlickerActivate());
        Spawn(loc);

        return this;
    }

    void Spawn(CorruptonLocation loc)
    {
        Sacrifice sacrifice = GameObject.Instantiate(sacrificePrefab, loc.sacrificeSpawnLoc).TryGet<Sacrifice>();
        sacrifice.stoppedHook.AddListener(loc.StopCorruption);
    }

    public override void StopCorrupt(CorruptonLocation loc)
    {
        loc.flickerObjs.ToList().ForEach(o => o.TryGet<ComponentFlicker>().FlickerDeactivate());
    }


}

