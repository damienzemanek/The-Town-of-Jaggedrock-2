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
    [Inject] TimeCycle timeCy;
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
        DestroyedHook = new UnityEvent();
        StartCoroutine(C_CheckIfDay());
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
        DestroyedHook?.Invoke();
        DestroyedHook?.RemoveAllListeners();
        Destroy(gameObject);
    }

    public void BuildDetector()
    {
        cbDetector = new CallbackDetector.Builder(gameObject)
            .WithRaycast()
            .WithEventHooks(stay: true, exit: true)
            .WithInteractAssignments(interactor, "Destroy (E)")
            .Build();
    }

    IEnumerator C_CheckIfDay()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();

            if(timeCy.IsDay())
            {
                DestroyedHook?.RemoveAllListeners();
                Destroy(gameObject);
                break;
            }
        }
    }

}
