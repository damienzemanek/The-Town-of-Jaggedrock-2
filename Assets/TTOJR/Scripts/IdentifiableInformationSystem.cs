using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class IdentifiableInformationSystem : MonoBehaviour
{

    #region Privates

    #endregion


    [TitleGroup("Info")] public LocationRandomizer.Locations frequentLocation;
    [TitleGroup("Info"), ReadOnly] public SO_Person personITalkAbout;
    [TitleGroup("Info"), ShowInInspector] [field: SerializeField] public string personITalkAboutsGroupingTrait { get => (personITalkAbout != null) ? GetPersonITalkAboutsGroupingTrait() : string.Empty; }
    [TitleGroup("Info")] public bool isResident = false;
    [TitleGroup("Info")] public int roomNum = -1;



    public static int chosenFrequentLocIndex = 0;
    public string personITalkAboutsName { get => (personITalkAbout != null) ? GetPersonITalkAboutsName() : string.Empty; }

    public void SetPersonITalkAbout() => personITalkAbout = GetRandomPersonRemoveFromList();
    public string GetPersonITalkAboutsGroupingTrait() => personITalkAbout.groupingTrait.ToString();
    public string GetPersonITalkAboutsName() => personITalkAbout.personName;

    private void Awake()
    {
        DetermineFrequentLocationOnSpawn();
        SetPersonITalkAbout();
    }

    public void DetermineFrequentLocationOnSpawn()
    {
        frequentLocation = LocationRandomizer.frequentLocations[chosenFrequentLocIndex];
        chosenFrequentLocIndex++;
        if (chosenFrequentLocIndex >= LocationRandomizer.frequentLocations.Length)
            chosenFrequentLocIndex = 0;
    }

    public string GetRandomPersonName() => GetRandomPersonRemoveFromList().personName;

    public SO_Person GetRandomPersonRemoveFromList()
    {
        var excludeSelf = SO_Person.allPersons.Where(p => p != this).ToList();
        if (excludeSelf.Count == 0) return null;
        SO_Person chosen = excludeSelf[Random.Range(0, excludeSelf.Count)];
        SO_Person.allPersons.Remove(chosen);
        return chosen;
    }

    #region Methods

    #endregion

}
