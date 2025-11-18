using NUnit.Framework;
using UnityEngine;
using Extensions;
using System.Collections.Generic;

public class Talk : MonoBehaviour
{
    [SerializeField] List<string> intros;
    public string intro { get => intros.Rand(); }




}
