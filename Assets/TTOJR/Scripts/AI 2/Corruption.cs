using System.Collections.Generic;
using Extensions;
using UnityEngine;

public class Corruption : MonoBehaviour
{

    #region Privates

    #endregion

    public List<UnityEventPlus> corruptionEvents;
    public List<Vector2> delays;



    private void Awake()
    {
        if(this.Has(out TimeCycle timeCy))
        {
            timeCy.TakeOnNightEvents(corruptionEvents);
        }
    }

    #region Methods

    #endregion

}
