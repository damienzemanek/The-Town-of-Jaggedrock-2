using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DependencyInjection;
using Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;
using ReadOnlyAttribute = Sirenix.OdinInspector.ReadOnlyAttribute;

public class CorruptonLocation : MonoBehaviour, IResidentLocation
{

    #region Privates
    [SerializeField] Transform _cursedAreaSpawnLoc;
    [SerializeField, ReadOnly] bool _corrupting;
    [SerializeField] Town _resident;
    [SerializeReference, ReadOnly]  public CorruptEventType currentEvent;
    #endregion

    public bool corrupting { get => _corrupting; set => _corrupting = value; }
    public Town resident { get => _resident; set => _resident = value; }
    public Transform noteSpawnLoc;
    public Transform cursedAreaSpawnLoc { get => _cursedAreaSpawnLoc; set => _cursedAreaSpawnLoc = value; }

    public UnityEvent haltedHook;

    [TabGroup("Crow")] [SerializeReference] public List<GameObject> searchables;
    [TabGroup("Crow")] public Transform roomEffectsSpawnLoc;
    [TabGroup("Crow")] public GameObject[] flickerObjs;

    [TabGroup("Sacrifice")] public Transform sacrificeSpawnLoc;
    [TabGroup("Sacrifice")] public Transform doorBloodSpawnLoc;

    [TabGroup("Readonly"), ReadOnly] public CursedRoom room;
    [TabGroup("Readonly"), ReadOnly] public Sacrifice sacrifice;



    private void Awake()
    {
        if (resident == null) this.Error("Resident not assigned");
        resident.isResident = true;
    }

    private void Start()
    {
        haltedHook = new UnityEvent();
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


    public void HaltCorruption()
    {
        currentEvent.StopCorrupt(this, true);


        TimeCycle.Instance.ambience.PlayGeneralAmbience();
        CorruptionManager.instance.CorruptHalted();

        TransitionOutOfCorruption();
    }

    [Button]
    void CorruptComplete()
    {
        if (currentEvent != null) currentEvent.currentTime = 0;
        if (!resident) this.Error("No resident has been set");
        currentEvent.StopCorrupt(this, false);

        resident.IncreaseCorruption();
        TimeCycle.Instance.ambience.PlayGeneralAmbience();
        CorruptionManager.instance.CorruptCompelte();


        TransitionOutOfCorruption();
    }

    void TransitionOutOfCorruption()
    {
        TimeCycle.Instance.ambience.PlayGeneralAmbience();
        corrupting = false;
        currentEvent = null;
    }

    public void SpawnNote()
    {
        this.Log("Spawning note");
        int rand = Random.Range(1, 4); 
        if(rand == EvilInformationManager.Instance.previousNoteInt) { SpawnNote(); return; }
        GameObject spawnedNote = null;
        if(rand == 1)
        {
            spawnedNote = Instantiate(EvilInformationManager.Instance.GetTraitNote(), noteSpawnLoc);
        }
        else if(rand == 2)
        {
            spawnedNote =Instantiate(EvilInformationManager.Instance.GetFrequentNote(), noteSpawnLoc);
        }
        else if(rand == 3)
        {
            if (EvilInformationManager.Instance.hasAllHints)
            {
                SpawnNote();
                return;
            }
            spawnedNote =Instantiate(EvilInformationManager.Instance.GetHintPrefab(), noteSpawnLoc);
        }

        EvilInformationManager.Instance.previousNoteInt = rand;
        this.Log($"Spawned note {spawnedNote.name}");

    }

    #region Methods

    #endregion

}


[Serializable]
public abstract class CorruptEventType
{
    public float duration;
    [ReadOnly] public float currentTime;
    public Transform noteSpawnLocation;

    public CorruptEventType StartCorrupt(CorruptonLocation loc)
    {
        currentTime = duration;
        return StartCorruptImplementation(loc);
    }

    public abstract CorruptEventType StartCorruptImplementation(CorruptonLocation loc);
    public void StopCorrupt(CorruptonLocation loc, bool spawnNote)
    {
        this.Log("Player stopped corrupt");
        if(spawnNote)
            loc.SpawnNote();
        StopCorruptImplementation(loc);
    }
    public abstract void StopCorruptImplementation(CorruptonLocation loc);

}

[Serializable]
public class CrowEffigyEvent : CorruptEventType
{
    public float timeToSearch = 4f;
    public GameObject cursedAreaPrefab;
    public GameObject crowEffigyPrefab;
    public GameObject roomEffectsPrefabs;

    public override CorruptEventType StartCorruptImplementation(CorruptonLocation loc)
    {
        loc.searchables.ForEach(s =>
        {
            Searchable searchable = s.AddComponent<Searchable>();
            searchable.timeToComplete = timeToSearch;
        });
        loc.room = GameObject.Instantiate(original: cursedAreaPrefab, loc.cursedAreaSpawnLoc).Get<CursedRoom>();
        GameObject.Instantiate(roomEffectsPrefabs, loc.roomEffectsSpawnLoc);
        Searchable correctSearchable = loc.searchables.Rand().Get<Searchable>();
        correctSearchable.SetAsCorrect(() => SpawnEffigy(loc ,correctSearchable, loc.room));
        this.Log($"Corrupted location {loc.name}, searchable {correctSearchable.name}");

        loc.flickerObjs.ToList().ForEach(o => o.Get<ComponentFlicker>().FlickerActivate());

        return this;
    }

    void SpawnEffigy(CorruptonLocation loc ,Searchable correctSearchable, CursedRoom room)
    {
        CrowEffigy effigy = GameObject.Instantiate(crowEffigyPrefab, correctSearchable.foundLoc).Get<CrowEffigy>().SetLoc(loc);
        effigy.room = room;
        loc.haltedHook.RemoveAllListeners();
        loc.haltedHook.AddListener(loc.HaltCorruption);
    }

    public override void StopCorruptImplementation(CorruptonLocation loc)
    {
        loc.searchables.ForEach(s =>
        {
            var comp = s.GetComponent<Searchable>();
            if (comp != null) GameObject.Destroy(comp);

            var comp2 = s.GetComponent<EffigySearchable>();
            if(comp != null) GameObject.Destroy(comp2);
        });
        loc.flickerObjs.ToList().ForEach(o => o.Get<ComponentFlicker>().FlickerDeactivate());
        loc.room.SelfDestroy();
        loc.searchables.ForEach(s => s.Get<Searchable>().SelfDestroy());
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
        Sacrifice sacrifice = GameObject.Instantiate(sacrificePrefab, loc.sacrificeSpawnLoc).Get<Sacrifice>().SetLoc(loc);
        loc.sacrifice = sacrifice;
        loc.haltedHook.RemoveAllListeners();
        loc.haltedHook.AddListener(loc.HaltCorruption);
    }

    public override void StopCorruptImplementation(CorruptonLocation loc)
    {
        loc.flickerObjs.ToList().ForEach(o => o.Get<ComponentFlicker>().FlickerDeactivate());
        loc.sacrifice.SelfDestroy();
    }


}

