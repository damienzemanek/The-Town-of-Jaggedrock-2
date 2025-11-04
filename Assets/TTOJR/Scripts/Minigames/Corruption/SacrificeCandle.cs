using DependencyInjection;
using TMPro;
using UnityEngine;

public class SacrificeCandle : RuntimeInjectableMonoBehaviour, IDetectorBuilder
{

    #region Privates
    [Inject] Interactor interactor;
    Sacrifice mySacrifice;
    CallbackDetector cbd;
    [SerializeField] GameObject fireEffect;
    #endregion

    public int givenNum;
    public TextMeshPro text;

    protected override void OnInstantiate()
    {
        base.OnInstantiate();
        BuildDetector();
        fireEffect.SetActive(true);
    }

    public SacrificeCandle InitializeCandle(int num, Sacrifice _sacrifice)
    {
        if(cbd) cbd.enabled = true;
        fireEffect.SetActive(true);
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
        fireEffect.SetActive(false);
        mySacrifice.AttemptToBlowout(givenNum);
        cbd.enabled = false;
    }


    #region Methods

    #endregion

}
