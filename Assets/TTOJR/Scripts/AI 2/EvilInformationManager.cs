using Extensions;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using DesignPatterns.CreationalPatterns;
using System.Collections.Generic;
using Sirenix.Utilities;

public class EvilInformationManager : Singleton<EvilInformationManager>
{

    #region Privates
    NPCs npcs;
    #endregion

    [SerializeField] bool covenSelected = false;

    [SerializeField, ReadOnly] Dialuage chosenCoven;
    [SerializeField, ReadOnly] IdentifiableInformationSystem iis;

    [TabGroup("Traits")] public GameObject[] traitPrefabs;
    [TabGroup("Frequents")] public GameObject[] frequentPrefabs;
    [TabGroup("Hints")] public List<GameObject> hintPrefabs;
    public bool[] hasHint;
    public bool hasAllHints { get => hasHint.All(h => h == true); }



    public string groupingTrait => chosenCoven.so_person.trait.ToString();
    public string activityHint => npcs.npcList.FirstOrDefault(n => n.Get<Dialuage>().so_person == chosenCoven).Get<LocationRandomizer>().activityC;

    protected override void Awake()
    {
        base.Awake();
        covenSelected = false;
        if(npcs == null) npcs = FindFirstObjectByType<NPCs>();
        hasHint = new bool[2];
        hasHint.ForEach(b => b = false);
    }

    private void Start()
    {
        SelectCoven();
    }

    public void SelectCoven()
    {
        Town potentialCoven = npcs.npcList.Rand().Get<Town>();
        potentialCoven.ConvertToCoven();
        chosenCoven = potentialCoven.Get<Dialuage>();
        covenSelected = true;
    }


    public GameObject GetTraitNote() => traitPrefabs[(int)chosenCoven.so_person.trait];
    public GameObject GetFrequentNote() => frequentPrefabs[(int)chosenCoven.so_person.frequent];
    public GameObject GetHintPrefab()
    {
        GameObject potential = hintPrefabs.Rand();
        int indx = hintPrefabs.IndexOf(potential);
        if (!hasAllHints)
        {
            if (!hasHint[indx]) return potential;
            else return GetHintPrefab();
        }
        else
        {
            return potential;
        }
    }

    #region Methods

    #endregion

}
