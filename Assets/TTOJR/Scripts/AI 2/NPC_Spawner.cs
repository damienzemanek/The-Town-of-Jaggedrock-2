using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Sirenix.OdinInspector;
using DependencyInjection;
using Extensions;
using UnityEngine.Events;

public class NPC_Spawner : MonoBehaviour, IDependencyProvider
{
    [Provide] public NPC_Spawner Provide() => this;

    public bool onlySpawnFirstResident = false;
    [ShowIf("onlySpawnFirstResident")] public float spawnCount;
    public Transform spawnPoint;
    public NPC_Area spawnArea;
    [TabGroup("Prefabs")] public List<GameObject> rolePrefabs;
    [TabGroup("Prefabs")] public GameObject townPrefab;


    [TabGroup("Data")] public List<SO_Person> townieData;
    [TabGroup("Data"), ReadOnly] public List<SO_Person> usedTownieData;

    [SerializeField] float delayBetweenSpawns;
    [SerializeField] float delayToStartSpawning;
    [SerializeField] int townPopulationWithCov;
    [SerializeField] int roomsCount;
    [SerializeField, ReadOnly] int spawnedAtRoom;

    [ReadOnly] public int currentSpawnedResidents; //cant be bigger than the pool
    public UnityEvent spawningCompleteHook;

    [Inject] Despawner despawner;
    [Inject] TimeCycle time;

    private void Awake()
    {
        usedTownieData.MakeShallowCopyOf(townieData);
        rolePrefabs.Ensure();
        townieData.Ensure();
        if (spawningCompleteHook == null) spawningCompleteHook = new();
    }

    private void Start()
    {
        if (spawnArea == null) throw new System.Exception("NPC_Spawner: spawn area not set");
    }


    void SpawnTownies()
    {
        for (int i = 0; i < townPopulationWithCov; i++)
        {
            GameObject spawnedTownie = InstantiateNPC(townPrefab).gameObject.SetActiveThen(false);
            spawnedTownie.TryGet<Dialuage>().so_person = usedTownieData.RandAndRemove();
            despawner.SaveToBeDespawned(spawnedTownie);
        }
    }

    void SpawnRoles()
    {
        for (int i = 0; i < rolePrefabs.Count; i++)
        {
            GameObject spawnedRole = InstantiateNPC(rolePrefabs[i]).gameObject.SetActiveThen(false);
            despawner.SaveToBeDespawned(spawnedRole);
        }
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

        SpawnTownies();
        SpawnRoles();

        for (int i = 0; i < townPopulationWithCov; i++)
            despawner.SaveToBeDespawned(InstantiateNPC(townPrefab).gameObject.SetActiveThen(false));

        spawningCompleteHook?.Invoke();


        StartCoroutine(C_SpawningCycle());

    }

    IEnumerator C_SpawningCycle()
    {
        currentSpawnedResidents = 0;
        yield return new WaitForSeconds(delayToStartSpawning);

        while (currentSpawnedResidents < despawner.spawnedNPCs.Count)
        {
            Spawn(despawner.spawnedNPCs[currentSpawnedResidents]);
            yield return new WaitForSeconds(delayBetweenSpawns);
            currentSpawnedResidents++;
        }

        this.Log("Spawning Hook");
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
        NPC_Movement spawned = Instantiate(npcPrefab,
                spawnPoint.transform.position,
                Quaternion.identity,
                null
                ).gameObject.GetComponent<NPC_Movement>();


        return spawned;
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
