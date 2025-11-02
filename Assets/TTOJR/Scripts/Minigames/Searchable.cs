using System;
using System.Linq;
using DependencyInjection;
using UnityEngine;
using UnityEngine.Events;

public class Searchable : RuntimeInjectableMonoBehaviour, IDetectorBuilder
{
    #region Privates
    [Inject] EntityControls controls;
    [Inject] Interactor interactor;
    CallbackDetector cbDetector;
    float increment;
    Transform _foundLoc;
    [SerializeField] bool _correct;
    [SerializeField] float progress;
    [SerializeField] float _timeToComplete;
    #endregion

    public bool correct { get => _correct; set => _correct = value; }
    public Transform foundLoc { get => _foundLoc; set => _foundLoc = value; }
    public float timeToComplete { get => _timeToComplete; set => _timeToComplete = value; }  
    [field: SerializeField] public bool complete { get; private set; }
    [field: SerializeField] public UnityEvent completeEvent { get; private set; }

    protected override void OnInstantiate()
    {
        base.OnInstantiate();
        BuildDetector();
        foundLoc = GetComponentsInChildren<Transform>()
            .FirstOrDefault(t => t != transform);
        increment = 0.1f;
        AssignSearchableCallbacks();

        completeEvent = new UnityEvent();
    }
    public void IncreaseProgress()
    {
        print("Searchable: Increasing progress");
        if (complete) return;

        progress += increment;

        if (progress > timeToComplete)
            Complete();
    }

    public void ResetProgress()
    {
        progress = 0;
    }

    public void SetAsCorrect(Action correctCompletionHook)
    {
        correct = true;
        completeEvent.AddListener(() => correctCompletionHook?.Invoke());
    }

    void Complete()
    {
        completeEvent?.Invoke();
        progress = timeToComplete;
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

    public void BuildDetector()
    {
        cbDetector = new CallbackDetector.Builder(gameObject)
            .WithRaycast()
            .WithEventHooks(enter: true, stay: true, exit: true)
            .WithHoldingUse()
            .Build();

    }

}
