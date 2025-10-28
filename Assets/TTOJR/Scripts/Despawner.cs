using System.Collections.Generic;
using System.Linq;
using DependencyInjection;

using UnityEngine;
using Extensions;

[DefaultExecutionOrder(300)]
[RequireComponent(typeof(TimeCycle))]
public class Despawner : MonoBehaviour, IDependencyProvider
{
    [Provide] Despawner Provide() => this;
    [Inject] TimeCycle time;

    [SerializeField] List<GameObject> spawnedNPCs;
    [SerializeField] List<GameObject> disabledNPCs;

    private void Awake()
    {
        spawnedNPCs = new List<GameObject>();
        disabledNPCs = new List<GameObject>();
    }

    private void OnEnable()
    {
        AssignDespawnEvents();
    }

    private void OnDisable()
    {
        RemoveDespawnEvents();
    }

    public void SaveToBeDespawned(GameObject go)
    {
        if (!go) return;
        spawnedNPCs.Add(go);
        disabledNPCs.Remove(go);
    }

    public void DisableAllNPCS()
    {
        foreach (GameObject spawnedNPC in spawnedNPCs.ToArray())
        {
            if (!spawnedNPC) continue;
            spawnedNPC.SetActive(false);
            disabledNPCs.Add(spawnedNPC);
            spawnedNPCs.Remove(spawnedNPC);
        }

    }

    public void DisableNPC(GameObject npc)
    {
        if (!npc) return;

        GameObject match = spawnedNPCs.FirstOrDefault(x => x.gameObject == npc);
        if (!match) { spawnedNPCs.Add(npc); DisableNPC(npc);  return; }

        match.SetActive(false);
        disabledNPCs.Add(match);
        spawnedNPCs.Remove(match);
    }

    public bool TryGetFromPool(GameObject prefab, out GameObject match)
    {
        match = null;
        if (!prefab) return false;

        if (disabledNPCs.Count <= 0) return false;
        string lookingForName = prefab.TryGet<Dialuage>().personName;


        match = disabledNPCs.FirstOrDefault(npc => 
            npc != null && 
            npc.name.StartsWith(lookingForName));

        if (!match) return false;
        this.Log($"TryGetFromPool() match is {match} using name {lookingForName}");
        disabledNPCs.Remove(match);
        spawnedNPCs.Add(match);
        return true;
    }

    void AssignDespawnEvents()
    {
        time.OnDayStart.AddListener(StartNewDayOrNight);
        time.OnNightStart.AddListener(StartNewDayOrNight);
    }

    void RemoveDespawnEvents()
    {
        time.OnDayStart?.RemoveAllListeners();
        time.OnNightStart?.RemoveAllListeners();
    }

    void StartNewDayOrNight()
    {
        DisableAllNPCS();
    }
    
}
