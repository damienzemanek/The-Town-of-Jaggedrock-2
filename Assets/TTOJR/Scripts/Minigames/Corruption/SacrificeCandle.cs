using DependencyInjection;
using Extensions;
using TMPro;
using UnityEngine;
using static Extensions.AudioEX;

public class SacrificeCandle : RuntimeInjectableMonoBehaviour, IDetectorBuilder
{

    #region Privates
    [Inject] Interactor interactor;
    Sacrifice mySacrifice;
    CallbackDetector cbd;
    [SerializeField] GameObject[] fireEffects;
    #endregion

    public int givenNum;
    public TextMeshPro text;
    public AudioSource source;
    public AudioClip blow;

    protected override void OnInstantiate()
    {
        base.OnInstantiate();
        BuildDetector();
        fireEffects.SetAllActive(true);
        if (source == null) source = this.TryGetOrAdd<AudioSource>();
    }

    public SacrificeCandle InitializeCandle(int num, Sacrifice _sacrifice)
    {
        if(cbd) cbd.enabled = true;
        fireEffects.SetAllActive(true);
        givenNum = num;
        text.text = "" + givenNum;
        mySacrifice = _sacrifice;
        return this;
    }

    public void BuildDetector()
    {
        cbd = new CallbackDetector.Builder(gameObject)
            .WithRaycast()
            .WithEventHooks(stay: true, exit: true)
            .WithInteractAssignments(interactor, "Blow out (E)")
            .WithUseHook(Blowout)
            .Build();
    }

    public void Blowout()
    {
        fireEffects.SetAllActive(false);
        mySacrifice.AttemptToBlowout(givenNum);
        cbd.enabled = false;
        source.Play(blow);
    }


    #region Methods

    #endregion

}
