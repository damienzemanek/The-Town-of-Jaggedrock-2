using Extensions;
using UnityEngine;
using System.Linq;
using System.Collections;
using Sirenix.OdinInspector;

public class EvilInformationManager : MonoBehaviour
{

    #region Privates
    NPCs npcs;
    #endregion

    [SerializeField] bool covenSelected = false;

    [SerializeField, ReadOnly] Dialuage chosenCoven;
    [SerializeField, ReadOnly] IdentifiableInformationSystem iis;


    public string groupingTrait => chosenCoven.so_person.trait.ToString();
    public string activityHint => npcs.npcList.FirstOrDefault(n => n.Get<Dialuage>().so_person == chosenCoven).Get<LocationRandomizer>().activityC;

    public LocationRandomizer.Locations frequent => iis.frequentLocation;
    public bool isResident => iis.isResident;


    private void Awake()
    {
        covenSelected = false;
        if(npcs == null) npcs = FindFirstObjectByType<NPCs>();
    }

    private void Start()
    {
        SelectCoven();
    }

    public void SelectCoven()
    {
        Town potentialCoven = npcs.npcList.Rand().Get<Town>();

        while (potentialCoven != null && !potentialCoven.isResident)
            potentialCoven = npcs.npcList.Rand().Get<Town>();

        if (potentialCoven == null) return;

        potentialCoven.ConvertToCoven();
        chosenCoven = potentialCoven.Get<Dialuage>();
        covenSelected = true;
    }

    #region Methods

    #endregion

}
