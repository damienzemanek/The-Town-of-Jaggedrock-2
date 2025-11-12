using System;
using System.Collections.Generic;
using System.Linq;
using DependencyInjection;
using Sirenix.OdinInspector;
using UnityEngine;
using static LocationRandomizer;
using static Quest;
using Random = UnityEngine.Random;
using Extensions;


public class LadyInBlack : Questholder<Questing.Town.Quest>, IDependencyProvider, IEventRecipient
{
    #region Privates
    [Inject] TimeCycle time;
    [Inject] Despawner despawner;
    LocationRandomizer locations;
    [Provide] LadyInBlack Provide() => this;
    #endregion
    public Questing.Section _section = Questing.Section.TOWN;
    public override Questing.Section Section
    {
        get => _section;
        set => _section = value;
    }

    private Questing.Town.Quest _currentQuestBacking;
    public override Questing.Town.Quest currentQuest 
    {
        get => _currentQuestBacking; 
        set => _currentQuestBacking = value;
    }

    //[field: SerializeReference] public List<Quest> hotelQuests;


    #region Class Methods
    protected override void OnInstantiate()
    {
        base.OnInstantiate();
    }

    private void OnEnable()
    {
        if (WontShowUpAtDayAndIsDay()) return;
    }
#endregion


#region Methods
    bool WontShowUpAtDayAndIsDay()
    {
        if (time.IsDay())
        {
            despawner.DisableNPC(gameObject);
            return true;
        }
        return false;
    }



    #endregion


    public bool mania_one_progression_two_flag = false;

}
