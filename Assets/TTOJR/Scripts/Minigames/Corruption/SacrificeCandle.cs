using DependencyInjection;
using TMPro;
using UnityEngine;

public class SacrificeCandle : RuntimeInjectableMonoBehaviour, IDetectorBuilder
{

    #region Privates
    [Inject] Interactor interactor;
    CallbackDetector cbd;
    #endregion

    public int givenNum;
    public TextMeshProUGUI text;

    protected override void OnInstantiate()
    {
        base.OnInstantiate();
        BuildDetector();
    }

    SacrificeCandle InitializeCandle(int num)
    {
        givenNum = num;
        text.text = $"{num}";
        return this;
    }

    public void BuildDetector()
    {
        cbd = new CallbackDetector.Builder(gameObject)
            .WithRaycast()
            .WithEventHooks(stay: true, exit: false)
            .WithInteractAssignments(interactor, "Blow out (E)")
            .Build();
    }


    #region Methods

    #endregion

}
