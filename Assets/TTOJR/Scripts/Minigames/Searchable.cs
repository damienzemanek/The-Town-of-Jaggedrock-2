using DependencyInjection;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CallbackDetector))]
public class Searchable : RuntimeInjectableMonoBehaviour
{
    [field:SerializeField] public float increment { get; private set; }
    [field: SerializeField] public float progress { get; private set; }
    [field: SerializeField] public float completeProgressValue { get; private set; }
    [field: SerializeField] public bool complete { get; private set; }
    [field: SerializeField] public UnityEvent completeEvent { get; private set; }

    CallbackDetector cbDetector;
    [Inject] EntityControls controls;
    [Inject] Interactor interactor;

    protected override void OnInstantiate()
    {
        base.OnInstantiate();
        cbDetector = GetComponent<CallbackDetector>();
        AssignSearchableCallbacks();
    }
    public void IncreaseProgress()
    {
        print("Searchable: Increasing progress");
        if (complete) return;

        progress += increment;

        if (progress > completeProgressValue)
            Complete();
    }

    public void ResetProgress()
    {
        progress = 0;
    }

    void Complete()
    {
        completeEvent?.Invoke();
        progress = completeProgressValue;
        complete = true;
        controls.ForceStopHold();
        cbDetector.rayCastDetector = false; //Turns the CBDetector Off
        interactor.SetHoldingInteraction(false);
        interactor.ToggleCanInteract(false);
    }
    void AssignSearchableCallbacks()
    {
        AssignHoldingIntactorCallbacks();
        AssignUseHoldBacks();
        AssignInteractorCallbacks();
    }

    void AssignInteractorCallbacks()
    {
        cbDetector.Enter.AddListener(call: () => interactor.ToggleCanInteract(true));
        cbDetector.Enter.AddListener(call: () => interactor.SetInteractText("Search (Hold E)"));
        cbDetector.Exit.AddListener(call: () => interactor.ToggleCanInteract(false));
    }
    
    void AssignHoldingIntactorCallbacks()
    {
        cbDetector.Enter.AddListener(() => interactor.SetHoldingInteraction(true));
        cbDetector.Exit.AddListener(() => interactor.SetHoldingInteraction(false));
    }
    void AssignUseHoldBacks()
    {
        cbDetector.useCallback.AddListener( () => IncreaseProgress());
        cbDetector.holdCancledCallback.AddListener( () => ResetProgress());
    }

}
