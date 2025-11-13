using UnityEngine;
using DependencyInjection;
using Extensions;
using TMPro;

public class NewPeriod : RuntimeInjectableMonoBehaviour, IAssigner
{
    [Inject] TimeCycle timeCy;
    [SerializeField] AudioPlay play;
    [SerializeField] AudioClip newDayAudio;
    [SerializeField] AudioClip newNightAudio;
    [SerializeField] float delayToPlayAudio = 2f;

    protected override void OnInstantiate()
    {
        base.OnInstantiate();
        Assign();
    }

    public void Assign()
    {
        timeCy.newPeriodHook.AddListener(PlayNewPeriodAudios);
    }

    public void DeAssign()
    {
        throw new System.NotImplementedException();
    }


    void PlayNewPeriodAudios()
    {
        if (timeCy.IsDay())
        {
            this.DelayedCall(() => 
            play.PlayForSeconds(
            newDayAudio,
            timeCy.blackScreenTime + 9f, //Go over slightly
            80), delayToPlayAudio);
        }
        if (timeCy.IsNight())
        {
            play.PlayForSeconds(
            newNightAudio,
            timeCy.blackScreenTime + 5f,
            80
            );
        }
    }

    #region Privates

    #endregion


    #region Methods
        
    #endregion

}
