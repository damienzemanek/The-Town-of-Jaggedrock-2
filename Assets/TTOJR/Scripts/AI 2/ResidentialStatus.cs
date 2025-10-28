using System;
using System.Collections.Generic;
using UnityEngine;


public class ResidentialStatus : MonoBehaviour
{
    [field: SerializeReference] public CantAccess cantAccess;

    [Serializable]
    public enum Status
    {
      Visitor,
      Resident
    }

    public Status residentialStatus;


}
