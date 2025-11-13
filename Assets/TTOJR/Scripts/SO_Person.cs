using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "New Person", menuName = "ScriptableObjects/Person")]
[Serializable]
public class SO_Person : ScriptableObject
{
    [SerializeField] string _personName;
    [SerializeField] bool _isCoven = false;

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
        Loud,
        Nervous,
        Clumsy
    }
    public CharacterRole role;
    public GroupingTrait groupingTrait;

    public string personName { get => _personName; }
    public bool isCoven { get => _isCoven; set => _isCoven = value; }



    public static List<SO_Person> allPersons;


    private void OnEnable()
    {
        if(allPersons == null) allPersons = new List<SO_Person>();

        if (excludeSelfFromGetRandomPersonNamne) return;

        if(!allPersons.Contains(this))
            allPersons.Add(this);

    }


}
