using DependencyInjection;
using UnityEngine;

public class CrowEffigy : RuntimeInjectableMonoBehaviour
{
    CursedRoom room;
    CallbackDetector cbDetector;
    [Inject] Interactor interactor;

    protected override void OnInstantiate()
    {
        base.OnInstantiate();
        room = transform.parent.GetComponentInChildren<CursedRoom>() ??
            throw new System.Exception($"Crow Effigy: No Cursed Room Found");
        cbDetector = GetComponent<CallbackDetector>() ?? throw new System.Exception
            ("Crow Effigy: No Callback Detector Found");

        AssignEffigyUseCallback();
    }

    public void DestroyEffigy()
    {
        print($"Crow Effigy: Destroying Effigy in room {room.name}");
        room.Uncurse();
        Destroy(gameObject);
        interactor.ToggleCanInteract(false);
    }

    void AssignEffigyUseCallback()
    {
        cbDetector.useCallback.AddListener(() => DestroyEffigy());
    }
}
