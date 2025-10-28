using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Sirenix.OdinInspector;
using DependencyInjection;
using Extensions;

public class NPC_Spawner : MonoBehaviour
{
    public bool onlySpawnFirstResident = false;
    [ShowIf("onlySpawnFirstResident")] public float spawnCount;
    public Transform spawnPoint;
    public NPC_Area spawnArea;
    public List<GameObject> spawnResidentPoolForCycle;
    public float delayBetweenSpawns;
    [ReadOnly] public int currentSpawnedResidents; //cant be bigger than the pool

    [Inject] Despawner despawner;
    [Inject] TimeCycle time;

    private void Start()
    {
        if (spawnResidentPoolForCycle == null || spawnResidentPoolForCycle.Count <= 0)
            Debug.LogError("NPC_Spawner: Residents not set, put residents to spawn");

        if (spawnArea == null) throw new System.Exception("NPC_Spawner: spawn area not set");
    }


    private void OnEnable()
    {
        AssignSpawnEvents();
    }

    private void OnDisable()
    {
        RemoveSpawnEvents();   
    }


    void StartSpawning()
    {
        currentSpawnedResidents = 0;
        SetSpawnAmount();
        StartCoroutine(C_SpawningCycle());
    }
    void SetSpawnAmount()
    {
        if (!onlySpawnFirstResident)
            spawnCount = spawnResidentPoolForCycle.Count;
    }
    IEnumerator C_SpawningCycle()
    {
        yield return new WaitForSeconds(delayBetweenSpawns);

        if (onlySpawnFirstResident)
            while (currentSpawnedResidents < spawnCount)
            {
                if (spawnResidentPoolForCycle[0] == null)
                    throw new System.Exception("NPC_Spawner: Set the first NPC_Movement in the pool");

                Spawn(spawnResidentPoolForCycle[0]);
                currentSpawnedResidents++;
                yield return new WaitForSeconds(delayBetweenSpawns);
            }
        else
            while (currentSpawnedResidents < spawnResidentPoolForCycle.Count)
            {
                if (spawnResidentPoolForCycle[currentSpawnedResidents] == null)
                    throw new System.IndexOutOfRangeException(tag);
                Spawn(spawnResidentPoolForCycle[currentSpawnedResidents]);
                yield return new WaitForSeconds(delayBetweenSpawns);
                currentSpawnedResidents++;
            }
    }

    void Spawn(GameObject prefab)
    {
        NPC_Movement newNPC;

        if (despawner.TryGetFromPool(prefab, out GameObject pooled))
            newNPC = PoolEnable(pooled);
        else
        {
            newNPC = InstantiateNPC(prefab);
            despawner.SaveToBeDespawned(newNPC.gameObject);
        }

        SpawnNPCAtSpawnPoint(newNPC.gameObject);
        if (newNPC.isActiveAndEnabled) newNPC.UseSpawnArea(spawnArea);
        else this.Log("New NPC was set false");
    }

    NPC_Movement InstantiateNPC(GameObject npcPrefab)
    {
        return Instantiate(npcPrefab,
                spawnPoint.transform.position,
                Quaternion.identity,
                null
                ).gameObject.GetComponent<NPC_Movement>();
    }

    NPC_Movement PoolEnable(GameObject pooledNPC)
    {
        this.Log($"Enabling {pooledNPC.TryGet<Dialuage>().personName}");
        return pooledNPC.SetActiveThen(true).TryGet<NPC_Movement>();
    }

    void SpawnNPCAtSpawnPoint(GameObject newNPC) => NavEX.Teleport(spawnPoint, newNPC, out _);

    void AssignSpawnEvents()
    {
        this.Log("Assigning spawn events");
        time.OnDayStart?.AddListener(StartSpawning);
        time.OnNightStart?.AddListener(StartSpawning);
    }

    void RemoveSpawnEvents()
    {
        time.OnDayStart?.RemoveAllListeners();
        time.OnNightStart?.RemoveAllListeners();
    }
}
