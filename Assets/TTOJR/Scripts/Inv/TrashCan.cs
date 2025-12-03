using UnityEngine;
using DependencyInjection;
using Extensions;
using Sirenix.OdinInspector;

[DefaultExecutionOrder(1)]
public class TrashCan : MonoBehaviour, IDetectorBuilder
{

    #region Privates
    [Inject, ShowInInspector, ReadOnly] Interactor interactor;
    [ShowInInspector, ReadOnly] InventoryUpdater invUpdater;
    [ShowInInspector, ReadOnly] PreRequisiteCallbackDetector pcbd;
    #endregion

    private void Awake()
    {
        invUpdater = this.Get<InventoryUpdater>();
        BuildDetector();
    }

    private void FixedUpdate()
    {
        if(interactor.Get<Inventory>().GetCurrentItem() != null)
            pcbd.lookingForChangesToItem = interactor.Get<Inventory>().GetCurrentItem();
    }

    void RemoveItem()
    {
        pcbd.lookingForChangesToItem = null;
        pcbd.hasPreRequisite = false;
        invUpdater.RemoveItem();
    }

    #region Methods
    public void BuildDetector()
    {
        pcbd = (PreRequisiteCallbackDetector)new PreRequisiteCallbackDetector.Builder(gameObject)
            .WithEventHooks(false, true, true)
            .WithUseHook(RemoveItem)
            .WithInteractAssignments(interactor, "Throw Away (E)")
            .WithRaycast()
            .Build();

        this.Log(pcbd.ToString());
    }
    #endregion

}
