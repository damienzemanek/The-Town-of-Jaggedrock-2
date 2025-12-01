using UnityEngine;
using DependencyInjection;
using Extensions;
using TMPro;

public class NewPeriod : RuntimeInjectableMonoBehaviour
{
    [SerializeField] AudioPlay play;
    [SerializeField] AudioClip newDayAudio;
    [SerializeField] AudioClip newNightAudio;
    [SerializeField] float delayToPlayAudio = 2f;

    protected override void OnInstantiate()
    {
        base.OnInstantiate();
    }



    #region Privates

    #endregion


    #region Methods
        
    #endregion

}
