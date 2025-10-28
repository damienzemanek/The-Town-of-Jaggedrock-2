using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[Serializable]
public class CantAccess : MonoBehaviour
{
    public ResidentialStatus.Status status;
    [SerializeReference] public List<NPC_Area> area = new List<NPC_Area>();

    public bool CanIAccessThisArea(NPC_Area _area, ResidentialStatus.Status _status)
    {
        if (_status != status)
            return false;
        return area.Contains(_area);
    }
}
