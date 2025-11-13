using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DependencyInjection;
using UnityEngine;
using Extensions;

public class Photographer : RuntimeInjectableMonoBehaviour, IDependencyProvider, IEventRecipient
{
    #region Privates
    [Provide] Photographer Provide() => this;
    [Inject] TimeCycle time;
    [Inject] Despawner despawner;
    LocationRandomizer locations;
    #endregion

    [Serializable]
    public class PhotographerDayData
    {
        public bool playerGaveCorrectLocation;
        public LocationRandomizer.Locations correctLocation;

        public PhotographerDayData(bool val, LocationRandomizer.Locations loc)
        {
            playerGaveCorrectLocation = val;
            correctLocation = loc;
        }
    }


    public LocationRandomizer.Locations locationIWantToPhotograph;
    public LocationRandomizer.Locations locationGivenByPlayerToPhotograph;
    [SerializeField] List<PhotographerDayData> playerGaveCorrectLocationOnDay;

#pragma warning disable IDE0052 // Remove unread private members
    [SerializeField] bool givenLoc;
#pragma warning restore IDE0052 // Remove unread private members
    public bool givenCorrectLocation { get => GetWasGivenTheCorrectLocationOnThePreviousDay(); }
    public LocationRandomizer.Locations theCorrectLocation { get => GetWasGivenTheCorrectLocationTheLocation(); }

#region Class Methods 
    protected override void OnInstantiate()
    {
        base.OnInstantiate();
        locations = this.Get<LocationRandomizer>();
    }

    private void OnEnable()
    {
        if (WontShowUpAtNightAndIsNight()) return;
        DetermineIfPlayerGaveCorrectLocation();
        SetNewLocationIWantToPhotograph();
    }
#endregion

#region Methods
    public bool GetWasGivenTheCorrectLocationOnThePreviousDay() 
        => (playerGaveCorrectLocationOnDay.Count > 0) ? playerGaveCorrectLocationOnDay.Last().playerGaveCorrectLocation : false;
    public LocationRandomizer.Locations GetWasGivenTheCorrectLocationTheLocation()
        => (playerGaveCorrectLocationOnDay.Count > 0) ? playerGaveCorrectLocationOnDay.Last().correctLocation : LocationRandomizer.Locations.Hotel;


    void SetNewLocationIWantToPhotograph() => locationIWantToPhotograph = locations.RandLocEnumExclude(LocationRandomizer.Locations.Hotel);
    public void PlayerGivenPhotographerALocation() => givenLoc = true;

    bool WontShowUpAtNightAndIsNight()
    {
        if (time.IsNight())
        {
            despawner.DisableNPC(gameObject);
            return true;
        }
        return false;
    }

    void DetermineIfPlayerGaveCorrectLocation()
    {
        if (time.periods.Count <= 1) return;
        if (time.GetCurrentPeriod().type == TimeCycle.Period.Type.Night) return;

        bool correct = (locationIWantToPhotograph == locationGivenByPlayerToPhotograph);
        PhotographerDayData newPhotographerData = new PhotographerDayData(correct, locationGivenByPlayerToPhotograph);
        playerGaveCorrectLocationOnDay.Add(newPhotographerData);

        givenLoc = false;
    }
    #endregion

    #region LadyInBlackLinks

    public bool willGiveASpecialItem { get => maniaQuestGiveSpecialItem || false; }

    public bool maniaQuestInitial = false;
    public bool maniaQuestGiveSpecialItem = false;
       
#endregion

}
