using DependencyInjection;
using Extensions;
using UnityEngine;

public class TownQuestGiver : Questholder<Questing.Activity.Quest>, IEventRecipient
{
    #region Privates
    [Inject] TimeCycle time;
    [Inject] Despawner despawner;
    [SerializeField] Town _townie;    
    
    LocationRandomizer locations;

    public Town townie { get => _townie; set => _townie = value; }

    #endregion
    public Questing.Section _section = Questing.Section.ACTIVITY;
    public override Questing.Section Section
    {
        get => _section;
        set => _section = value;
    }

    private Questing.Activity.Quest _currentQuestBacking;
    public override Questing.Activity.Quest currentQuest
    {
        get => _currentQuestBacking;
        set => _currentQuestBacking = value;
    }

    //[field: SerializeReference] public List<Quest> hotelQuests;


    #region Class Methods
    protected override void OnInstantiate()
    {
        base.OnInstantiate();
        townie = this.Get<Town>();
    }

    #endregion


    #region Methods



    #endregion


    public bool mania_one_progression_two_flag = false;
}
