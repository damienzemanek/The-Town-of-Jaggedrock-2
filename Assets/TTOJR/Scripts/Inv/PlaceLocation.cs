using DependencyInjection;
using Extensions;
using UnityEngine;
using static Extensions.AudioEX;

public class PlaceLocation : RuntimeInjectableMonoBehaviour, IDetectorBuilder
{

    #region Privates
    [Inject] Interactor interactor;
    [SerializeField] Transform loc;
    [SerializeField] Item itemToPlace;
    [SerializeField] InventoryUpdater invUpdater;
    PreRequisiteCallbackDetector pcbd;
    #endregion

    protected override void OnInstantiate()
    {
        base.OnInstantiate();
        BuildDetector();
        invUpdater = this.Get<InventoryUpdater>();
    }

    private void Start()
    {
        if (loc == null) this.Error("no location set");
    }

    public void Place(GameObject go)
    {
        GameObject spawned = UnityEngine.Object.Instantiate(
            go,
            loc
        );

        print($"Placed Object {go.name}");
        interactor.TryGetOrAdd<AudioSource>().Play(itemToPlace.pickupSFX);
    }




    #region Methods

    public void BuildDetector()
    {
        pcbd = (PreRequisiteCallbackDetector)new PreRequisiteCallbackDetector.Builder(gameObject)
            .WithRequiredItem(itemToPlace)
            .WithStayHook(() => invUpdater.UpdateItem(0))
            .WithExitHook(() => invUpdater.UpdateItem(1))
            .WithUseHook(() => invUpdater.UseItem())
            .WithInteractAssignments(interactor, "Place (E)")
            .WithRaycast()
            .Build();
    }

    #endregion

}
