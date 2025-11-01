using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class Room : MonoBehaviour
{

    #region Privates

    #endregion

    [SerializeField] Town resident;

    public void SetResident(Town _resident)
    {
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

    #region Methods
        
    #endregion

}
