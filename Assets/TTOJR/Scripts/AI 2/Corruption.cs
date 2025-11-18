using System.Collections.Generic;
using DependencyInjection;
using Extensions;
using UnityEngine;

public class Corruption : MonoBehaviour
{
    #region Privates
    [Inject] TimeCycle time;
    #endregion

    public List<UnityEventPlus> corruptionEvents;
    public List<Vector2> delays;

    private void Start()
    {
        if (time == null) this.Error("time null");
        time.TakeOnNightEvents(corruptionEvents);

    }

    #region Methods

    #endregion

}
