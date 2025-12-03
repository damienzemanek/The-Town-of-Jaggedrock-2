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
using Unity.Collections;
using UnityEngine;
using ReadOnlyAttribute = Sirenix.OdinInspector.ReadOnlyAttribute;

public class CorruptonLocation : MonoBehaviour, IResidentLocation
{

    #region Privates
    [SerializeField] Transform _cursedAreaSpawnLoc;
    [SerializeField, ReadOnly] bool _corrupting;
    [SerializeField] Town _resident;
    [SerializeReference, ReadOnly] CorruptEventType currentEvent;
    #endregion

    public bool corrupting { get => _corrupting; set => _corrupting = value; }
    public Town resident { get => _resident; set => _resident = value; }  
    public Transform cursedAreaSpawnLoc { get => _cursedAreaSpawnLoc; set => _cursedAreaSpawnLoc = value; }

    [TabGroup("Crow")] [SerializeReference] public List<GameObject> searchables;
    [TabGroup("Crow")] public GameObject[] flickerObjs;

    [TabGroup("Sacrifice")] public Transform sacrificeSpawnLoc;
    [TabGroup("Sacrifice")] public Transform doorBloodSpawnLoc;

    private void OnEnable()
    {
        if (resident == null) this.Error("Resident not assigned");
        resident.isResident = true;
    }

    private void Start()
    {
        ResetAll();
    }

    void ResetAll()
    {
        flickerObjs.ToList().ForEach(o => o.Get<ComponentFlicker>().FlickerDeactivate());
        currentEvent = null;
    }

    private void FixedUpdate()
    {
        if (!corrupting) return;
        if (currentEvent == null) return;

        currentEvent.currentTime -= Time.deltaTime;

        if (currentEvent.currentTime < 0) CorruptComplete();
    }

    public void StartCorruption()
    {
        this.Log($"Starting Corruption");
        StartCoroutine(C_CorruptEvent());
    }

    IEnumerator C_CorruptEvent()
    {
        corrupting = true;
        currentEvent = CorruptionManager.instance.corruptEventTypes.Rand().StartCorrupt(this);
        yield return new WaitForSeconds(60);
        if (corrupting) CorruptComplete();
    }

    public void StopCorruption()
    {
        corrupting = false;
        currentEvent.StopCorrupt(this);
        currentEvent = null;
    }

    [Button]
    void CorruptComplete()
    {
        if (currentEvent != null) currentEvent.currentTime = 0;
        if (!resident) this.Error("No resident has been set");
        resident.IncreaseCorruption();
        StopCorruption();
        CorruptionManager.instance.CorruptCompelte();
    }

    #region Methods

    #endregion

}


[Serializable]
public abstract class CorruptEventType
{
    public float duration;
    [ReadOnly] public float currentTime;

    public CorruptEventType StartCorrupt(CorruptonLocation loc)
    {
        currentTime = duration;
        return StartCorruptImplementation(loc);
    }

    public abstract CorruptEventType StartCorruptImplementation(CorruptonLocation loc);
    public abstract void StopCorrupt(CorruptonLocation loc);
}

[Serializable]
public class CrowEffigyEvent : CorruptEventType
{
    public float timeToSearch = 4f;
    public GameObject cursedAreaPrefab;
    public GameObject crowEffigyPrefab;

    CursedRoom room;

    public override CorruptEventType StartCorruptImplementation(CorruptonLocation loc)
    {
        room = null;
        loc.searchables.ForEach(s => s.AddComponent<Searchable>().timeToComplete = timeToSearch);
        room = GameObject.Instantiate(original: cursedAreaPrefab, loc.cursedAreaSpawnLoc).Get<CursedRoom>();
        Searchable correctSearchable = loc.searchables.Rand().Get<Searchable>();
        correctSearchable.SetAsCorrect(() => SpawnEffigy(loc ,correctSearchable, room));
        this.Log($"Corrupted location {loc.name}, searchable {correctSearchable.name}");

        loc.flickerObjs.ToList().ForEach(o => o.Get<ComponentFlicker>().FlickerActivate());

        return this;
    }

    void SpawnEffigy(CorruptonLocation loc ,Searchable correctSearchable, CursedRoom room)
    {
        CrowEffigy effigy = GameObject.Instantiate(crowEffigyPrefab, correctSearchable.foundLoc).Get<CrowEffigy>();
        effigy.room = room;
        effigy.DestroyedHook.AddListener(loc.StopCorruption);
    }

    public override void StopCorrupt(CorruptonLocation loc)
    {
        loc.searchables.ForEach(s => s.Get<Searchable>().ComponentReset());
        loc.flickerObjs.ToList().ForEach(o => o.Get<ComponentFlicker>().FlickerDeactivate());
        GameObject.Destroy(room.gameObject);
    }


}

[Serializable]
public class SacrificeEvent : CorruptEventType
{
    public GameObject sacrificePrefab;
    public GameObject bloodDoorPrefab;

    public override CorruptEventType StartCorruptImplementation(CorruptonLocation loc)
    {
        loc.flickerObjs.ToList().ForEach(o => o.Get<ComponentFlicker>().FlickerActivate());
        Spawn(loc);

        return this;
    }

    void Spawn(CorruptonLocation loc)
    {
        Sacrifice sacrifice = GameObject.Instantiate(sacrificePrefab, loc.sacrificeSpawnLoc).Get<Sacrifice>();
        sacrifice.stoppedHook.AddListener(loc.StopCorruption);
    }

    public override void StopCorrupt(CorruptonLocation loc)
    {
        loc.flickerObjs.ToList().ForEach(o => o.Get<ComponentFlicker>().FlickerDeactivate());

    }


}

