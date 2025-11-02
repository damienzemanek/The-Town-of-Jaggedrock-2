using DependencyInjection;
using Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class CrowEffigy : RuntimeInjectableMonoBehaviour, IDetectorBuilder
{
    #region Privates
    [Inject] Interactor interactor;
    CallbackDetector cbDetector;
    CursedRoom _room;
    #endregion

    public CursedRoom room { get => _room; set => _room = value;}

    public UnityEvent DestroyedHook;

    protected override void OnInstantiate()
    {
        base.OnInstantiate();
        BuildDetector();
        gameObject.layer = LayerMask.NameToLayer("Interactable");
        AssignEffigyUseCallback();
        AssignInteractorCallbacks();
        DestroyedHook = new UnityEvent();
    }


    void AssignEffigyUseCallback()
    {
        cbDetector.useCallback.AddListener(() => DestroyEffigy());

    }
    void AssignInteractorCallbacks()
    {
        cbDetector.Enter.AddListener(call: () => interactor.ToggleCanInteract(true));
        cbDetector.Enter.AddListener(call: () => interactor.SetInteractText("Search (Hold E)"));
        cbDetector.Exit.AddListener(call: () => interactor.ToggleCanInteract(false));
    }

    public void DestroyEffigy()
    {
        this.Log($"Destroying Effigy in room {room.name}");
        room.Uncurse();
        interactor.ToggleCanInteract(false);
        DestroyedHook?.Invoke();
        DestroyedHook?.RemoveAllListeners();
        Destroy(gameObject);
    }

    public void BuildDetector()
    {
        cbDetector = new CallbackDetector.Builder(gameObject)
            .WithRaycast()
            .WithEventHooks(true, true, true)
            .Build();
    }

}
