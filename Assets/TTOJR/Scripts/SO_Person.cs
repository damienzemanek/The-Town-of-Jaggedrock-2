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

    public bool excludeSelfFromGetRandomPersonNamne;
    public LocationRandomizer.Trait trait;
    public string personName { get => _personName; }
    public static List<SO_Person> allPersons;


    private void OnEnable()
    {
        if(allPersons == null) allPersons = new List<SO_Person>();

        if (excludeSelfFromGetRandomPersonNamne) return;

        if(!allPersons.Contains(this))
            allPersons.Add(this);

    }



}
