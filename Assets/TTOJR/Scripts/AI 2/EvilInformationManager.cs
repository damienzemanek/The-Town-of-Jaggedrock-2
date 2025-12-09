using Extensions;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using DesignPatterns.CreationalPatterns;
using System.Collections.Generic;
using static Extensions.FadeEX;
using System.Collections;

public class EvilInformationManager : Singleton<EvilInformationManager>
{
    public EntityControls player;
    #region Privates
    NPCs npcs;
    #endregion

    [SerializeField] bool covenSelected = false;

    [SerializeField, ReadOnly] Dialuage chosenCoven;
    [SerializeField, ReadOnly] IdentifiableInformationSystem iis;

    [TabGroup("Traits")] public GameObject[] traitPrefabs;
    [TabGroup("Frequents")] public GameObject[] frequentPrefabs;
    [TabGroup("Hints")] public List<GameObject> hintPrefabs;
    public List<bool> hasHint;
    public bool hasAllHints { get => hasHint.All(h => h == true); }

    public int previousNoteInt = 0;

    public string groupingTrait => chosenCoven.so_person.trait.ToString();
    public string activityHint => npcs.npcList.FirstOrDefault(n => n.Get<Dialuage>().so_person == chosenCoven).Get<LocationRandomizer>().activityC;

    [TabGroup("End game")] public FadeSettings fade;
    [TabGroup("End game")] public AudioSource source;
    [TabGroup("End game")] public AudioClip shootSFX;
    [TabGroup("End game")] public GameObject winDisplay;
    [TabGroup("End game")] public GameObject looseDisplay;
    [TabGroup("End game")] public GameObject corruptedDisplay;



    protected override void Awake()
    {
        base.Awake();
        covenSelected = false;
        if(npcs == null) npcs = FindFirstObjectByType<NPCs>();
        hasHint = new List<bool>(2);
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

    public void AttemptShoot(Dialuage shotPerson)
    {
        string shotPersonName = shotPerson.so_person.personName;

        StartCoroutine(C_FadeToOpaque(fade, () =>
        {
            StartCoroutine(CheckShot(shotPersonName));
        }));


    }

    IEnumerator CheckShot(string name)
    {
        yield return new WaitForSeconds(1);
        source.Play(shootSFX);
        yield return new WaitForSeconds(1);

        name.LogCompare(chosenCoven.so_person.personName);

        if (name == chosenCoven.so_person.personName)
            winDisplay.SetActive(true);
        else
            looseDisplay.SetActive(true);

    }

    public void Lose()
    {
        player.canMove = false;
    }

    #region Methods

    #endregion

}
