using DependencyInjection;
using Extensions;
using UnityEngine;

public class CrowEffigy : RuntimeInjectableMonoBehaviour
{
    #region Privates
    [Inject] Interactor interactor;
    CallbackDetector cbDetector;
    CursedRoom _room;
    #endregion

    public CursedRoom room { get => _room; set => _room = value;}


    protected override void OnInstantiate()
    {
        base.OnInstantiate();
        cbDetector = this.TryGetOrAdd<CallbackDetector>().Init(enter: true, exit: true);

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
