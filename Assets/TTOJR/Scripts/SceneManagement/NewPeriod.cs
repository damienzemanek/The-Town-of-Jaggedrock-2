using UnityEngine;
using DependencyInjection;
using Extensions;
using TMPro;

public class NewPeriod : RuntimeInjectableMonoBehaviour
{
    [Inject] TimeCycle timeCy;
    [SerializeField] AudioPlay play;
    [SerializeField] AudioClip newDayAudio;
    [SerializeField] AudioClip newNightAudio;
    [SerializeField] float delayToPlayAudio = 2f;

    protected override void OnInstantiate()
    {
        base.OnInstantiate();
    }



    public void PlayNewPeriodAudios()
    {
        //Swapped cause it has not yet switched (its called in the prehook, the actual new period is called in the midhook)
        if (timeCy.IsNight())
        {
            this.DelayedCall(() => 
            play.PlayForSeconds(
            newDayAudio,
            timeCy.dayBlackScreenTime + 9f, //Go over slightly
            80), delayToPlayAudio);
        }
        if (timeCy.IsDay())
        {
            play.PlayForSeconds(
            newNightAudio,
            timeCy.nightBlackScreenTime + 5f,
            80
            );
        }
    }

    public void PlayDay()
    {
        this.DelayedCall(() =>
            play.PlayForSeconds(
            newDayAudio,
            timeCy.dayBlackScreenTime + 9f, //Go over slightly
            80), 
            
            delayToPlayAudio);
    }

    public void PlayNight()
    {
        play.PlayForSeconds(
            newNightAudio,
            timeCy.nightBlackScreenTime + 5f,
            80
            );
    }

    #region Privates

    #endregion


    #region Methods
        
    #endregion

}
