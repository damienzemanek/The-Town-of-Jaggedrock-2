using Extensions;
using UnityEngine;
using System.Linq;
using System.Collections;
using NUnit.Framework;

public class EvilInformationManager : MonoBehaviour
{

    #region Privates
    NPCs npcs;
    #endregion

    [SerializeField] bool covenSelected;

    [SerializeField] SO_Person person;
    [SerializeField] IdentifiableInformationSystem iis;


    public string groupingTrait => person.groupingTrait.ToString();
    public LocationRandomizer.Locations frequent => iis.frequentLocation;
    public bool isResident => iis.isResident;


    private void Awake()
    {
        if(npcs == null) npcs = FindFirstObjectByType<NPCs>();
    }

    private void Start()
    {
        SelectCoven();
    }

    public void SelectCoven()
    {
        //if (covenSelected) return;

        //////GameObject randTown = npcs.npcs.Where(npc => npc.Has<Town>())
        //////    .ToList()
        //////    .Where(npc => !npc.Get<IdentifiableInformationSystem>().isResident)
        //////    .ToList()
        //////    .Rand();

        ////if (!randTown.TryGetComponent(out Dialuage dialauge)) return;

        //person = dialauge.so_person;
        //person.isCoven = true;

        //covenSelected = true;
    }

    #region Methods
        
    #endregion

}
