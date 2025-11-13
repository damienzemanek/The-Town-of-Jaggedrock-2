using Extensions;
using UnityEngine;
using System.Linq;
using System.Collections;
using NUnit.Framework;

public class EvilInformationManager : MonoBehaviour
{

    #region Privates
    NPC_Spawner spawner;
    Despawner despawner;
    #endregion

    [SerializeField] bool covenSelected;

    [SerializeField] SO_Person person;
    [SerializeField] IdentifiableInformationSystem iis;


    public string groupingTrait => person.groupingTrait.ToString();
    public LocationRandomizer.Locations frequent => iis.frequentLocation;
    public bool isResident => iis.isResident;


    private void Awake()
    {
        if(spawner == null) spawner = FindFirstObjectByType<NPC_Spawner>();
        if(despawner == null) despawner = FindFirstObjectByType<Despawner>();
    }

    private void OnEnable()
    {
        spawner.spawningCompleteHook?.AddListener(call: SelectCoven);
    }

    private void OnDisable()
    {
        spawner.spawningCompleteHook?.RemoveListener(call: SelectCoven);

    }

    public void SelectCoven()
    {
        if (covenSelected) return;

        this.Log("" + despawner.disabledNPCs.Count);

        GameObject randTown = despawner.disabledNPCs.Where(npc => npc.Has<Town>())
            .ToList()
            .Where(npc => !npc.Get<IdentifiableInformationSystem>().isResident)
            .ToList()
            .Rand();

        if (!randTown.TryGetComponent(out Dialuage dialauge)) return;

        person = dialauge.so_person;
        person.isCoven = true;

        covenSelected = true;
    }

    #region Methods
        
    #endregion

}
