using DependencyInjection;
using Extensions;
using UnityEngine;

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
        invUpdater = this.TryGet<InventoryUpdater>();
    }

    public void Place(GameObject go)
    {
        GameObject spawned = UnityEngine.Object.Instantiate(
            go,
            loc.position,
            Quaternion.identity,
            loc
        );
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
