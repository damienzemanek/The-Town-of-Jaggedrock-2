using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "New Person", menuName = "ScriptableObjects/Person")]
[Serializable]
public class SO_Person : ScriptableObject
{
    [SerializeField] string _personName;
    public bool excludeSelfFromGetRandomPersonNamne;
    public enum CharacterRole
    {
        Town,
        Coven,
        Sherrif,
        LadyInBlack,
        Photographer,
        AssylumEscapee
    }

    public enum GroupingTrait
    {
        None,
        Anxious,
        Hopefull,
        Nervous,
        Morbid
    }
    public CharacterRole role;
    public GroupingTrait groupingTrait;

    public string personName { get => _personName; }

    public static List<SO_Person> allPersons;
    public SO_Person myPersonITalkAbout;

    public void SetMyPersonITalkAbout() => myPersonITalkAbout = GetRandomPerson();
    public string GetMyPersonITalkAboutsGroupingTrait() => myPersonITalkAbout.groupingTrait.ToString();
    public string GetMyPersonITalkAboutsName() => myPersonITalkAbout.personName;


    private void OnEnable()
    {
        if(allPersons == null) allPersons = new List<SO_Person>();

        if (excludeSelfFromGetRandomPersonNamne) return;

        if(!allPersons.Contains(this))
            allPersons.Add(this);

        SetMyPersonITalkAbout();
    }

    public string GetRandomPersonName() => GetRandomPerson().personName;

    public SO_Person GetRandomPerson()
    {
        var excludeSelf = allPersons.Where(p => p != this).ToList();
        if (excludeSelf.Count == 0) return null;

        return excludeSelf[Random.Range(0, excludeSelf.Count)];
    }

}
