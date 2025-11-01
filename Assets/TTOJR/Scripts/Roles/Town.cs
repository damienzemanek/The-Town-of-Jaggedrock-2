using System.Collections.Generic;
using UnityEngine;

public class Town : MonoBehaviour
{
    [SerializeField] bool corrupted;
    #region Privates

    #endregion

    public List<Quest> activityQuests;
    public int currentCorruptionLevel = 0;

    public void IncreaseCorruption()
    {
        currentCorruptionLevel++;
    }

    public void BecomeResident()
    {

    }


    #region Methods
        
    #endregion

}
