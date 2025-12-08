using System.Collections;
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

    public CorruptonLocation loc;

    protected override void OnInstantiate()
    {
        base.OnInstantiate();
        BuildDetector();
        gameObject.layer = LayerMask.NameToLayer("Interactable");
        AssignEffigyUseCallback();
    }


    void AssignEffigyUseCallback()
    {
        cbDetector.useCallback.AddListener(() => DestroyEffigy());
    }

    public void DestroyEffigy()
    {
        this.Log($"Destroying Effigy in room {room.name}");
        room.Uncurse();
        interactor.ToggleCanInteract(false);
        Destroy(gameObject);
        loc.haltedHook?.Invoke();
    }

    public void BuildDetector()
    {
        cbDetector = new CallbackDetector.Builder(gameObject)
            .WithRaycast()
            .WithEventHooks(stay: true, exit: true)
            .WithInteractAssignments(interactor, "Destroy (E)")
            .Build();
    }

    public CrowEffigy SetLoc(CorruptonLocation _loc)
    {
        loc = _loc;
        return this;
    }


}
