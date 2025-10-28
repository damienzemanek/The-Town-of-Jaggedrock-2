using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class CantAccessMutator : MonoBehaviour
{
    public List<NPC_Area> areasICanAccess = new List<NPC_Area>();
    private void Start()
    {
        MakeAreasAccesibleToMe();
    }


    void MakeAreasAccesibleToMe()
    {
        if (!TryGetComponent<CantAccess>(out CantAccess cantAccess)) return;
        if (areasICanAccess == null || areasICanAccess.Count <= 0) return;

        areasICanAccess.ForEach(area => cantAccess.area.Remove(area));
    }
}
