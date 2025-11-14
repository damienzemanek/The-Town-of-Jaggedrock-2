using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Sirenix.OdinInspector;
using DependencyInjection;
using Extensions;
using UnityEngine.Events;
using System.Linq;

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

    [SerializeField] bool initialSpawnComplete = false;
    [SerializeField] float delayBetweenSpawns;
    [SerializeField] float delayToStartSpawning;
    [SerializeField] int townPopulationWithCov;
    [SerializeField] int maxPeoplePerPeriod = 8;
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
        //need to wait until the time cycle sets it to day
        this.DelayedCall(() =>
        {
            InitialSpawn();
            spawningCompleteHook?.Invoke();
        }, 0.1f);
    }

    void InitialSpawn()
    {
        if (initialSpawnComplete) return;
        SpawnTownies();
        SpawnRoles();
        initialSpawnComplete = true;
    }


    void SpawnTownies()
    {
        for (int i = 0; i < townPopulationWithCov; i++)
        {
            GameObject spawnedTownie = InstantiateNPC(townPrefab).gameObject.SetActiveThen(false);
            spawnedTownie.Get<Dialuage>().so_person = usedTownieData.RandAndRemove();
            spawnedTownie.Get<Dialuage>().Init();

            despawner.SaveAsDespawned(spawnedTownie);
        }
    }

    void SpawnRoles()
    {
        for (int i = 0; i < rolePrefabs.Count; i++)
        {
            GameObject spawnedRole = InstantiateNPC(rolePrefabs[i]).gameObject.SetActiveThen(false);
            spawnedRole.Get<Dialuage>().Init();
            despawner.SaveAsDespawned(spawnedRole);
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

        StartCoroutine(C_SpawningCycle());

    }

    IEnumerator C_SpawningCycle()
    {
        yield return new WaitForSeconds(delayToStartSpawning);


        int amountToSpawn = maxPeoplePerPeriod;

        //Spawn half the people at night
        if (time.IsNight())
        {
            this.Log("Spawning half, it is night");
            amountToSpawn = (maxPeoplePerPeriod / 2) - 1;

        }
        currentSpawnedResidents = 0;

        //Spawn roles first
        List<GameObject> roles = despawner.disabledNPCs.Where(n => !n.Has<Town>()).ToList();
        this.Log($"Spawning roles: we have {roles.Count} many");
        roles.ForEach(r =>
        {
            Spawn(r);
            currentSpawnedResidents++;
            this.Log($"Spawning {r.name}, this is number {currentSpawnedResidents}");
        });

        yield return null;


        while (currentSpawnedResidents < amountToSpawn)
        {
            GameObject npcToSpawn = despawner.disabledNPCs.Rand();

            //Then spawn townies
            Spawn(npcToSpawn);
            this.Log($"Spawning : {npcToSpawn.name}, this is number {currentSpawnedResidents}");
            currentSpawnedResidents++;
            yield return new WaitForSeconds(delayBetweenSpawns);

        }


        this.Log($"Spawning complete, spawned {currentSpawnedResidents}, this is number {currentSpawnedResidents}");

    }

    void Spawn(GameObject prefab)
    {
        NPC_Movement newNPC;

        if (!despawner.TryGetFromPool(prefab, out GameObject pooled)) return;
       
        newNPC = PoolEnable(pooled);

        //Role characters dont have iis
        if(!newNPC.Has(out IdentifiableInformationSystem iis)) SpawnNPCAtSpawnPoint(newNPC.gameObject);
        
        //townies that are not residents
        if(newNPC.Has(out Town town))
            if (town.Get<IdentifiableInformationSystem>().isResident == false)
                SpawnNPCAtSpawnPoint(newNPC.gameObject);


        if (newNPC.isActiveAndEnabled)
        {
            this.Log("Npc spawned");
            newNPC.area = spawnArea;
            newNPC.UseSpawnArea(spawnArea);
        }
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
        this.Log($"Enabling {pooledNPC.Get<Dialuage>().personName}");
        return pooledNPC.SetActiveThen(true).Get<NPC_Movement>();
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
