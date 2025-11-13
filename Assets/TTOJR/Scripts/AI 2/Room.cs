using System.Collections.Generic;
using System.Linq;
using Extensions;
using NUnit.Framework;
using UnityEngine;

public class Room : MonoBehaviour
{

    #region Privates
    Teleport tp;
    #endregion

    [SerializeField] public int roomNum;
    [SerializeField] Town resident;

    private void Awake()
    {
        tp = this.Get<Teleport>();
    }

    public void SetResident(Town _resident)
    {
        tp.objToTeleport = _resident.gameObject;
        resident = _resident;
        AssignResidentToLocations();
    }

    void AssignResidentToLocations()
    {
        List<IResidentLocation> locations = gameObject.GetComponentsInChildren<MonoBehaviour>()
            .OfType<IResidentLocation>()
            .ToList();

        if(locations != null && locations.Count > 0) 
            locations.ForEach(l => l.resident = resident);

    }

    public void TelportBack() => tp.DoTeleport();

    #region Methods
        
    #endregion

}
