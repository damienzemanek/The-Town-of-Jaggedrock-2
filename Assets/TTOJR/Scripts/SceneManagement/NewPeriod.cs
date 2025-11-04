using UnityEngine;
using DependencyInjection;
using Extensions;

public class NewPeriod : RuntimeInjectableMonoBehaviour, IAssigner
{
    [Inject] TimeCycle timeCy;
    [SerializeField] AudioPlay play;
    [SerializeField] AudioClip newDayAudio;
    [SerializeField] AudioClip newNightAudio;

    protected override void OnInstantiate()
    {
        base.OnInstantiate();
        Assign();
    }

    public void Assign()
    {
        timeCy.OnDayStart.AddListener(() => play.PlayForSeconds(
            newDayAudio,
            timeCy.blackScreenTime + 5f, //Go over slightly
            80
        ));

        timeCy.OnNightStart.AddListener(() => play.PlayForSeconds(
           newNightAudio,
           timeCy.blackScreenTime,
           80
        ));
    }

    public void DeAssign()
    {
        throw new System.NotImplementedException();
    }

    #region Privates

    #endregion


    #region Methods
        
    #endregion

}
