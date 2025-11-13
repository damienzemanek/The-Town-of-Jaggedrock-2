using UnityEngine;
using Extensions;

[RequireComponent(typeof(Pickup))]
public class QuickAssigner : MonoBehaviour
{
    Pickup pickup;
    private void Awake()
    {
        pickup = this.Get<Pickup>();
    }

    private void OnEnable()
    {
        pickup.physicalUseHookPoint += PhysicallyUseGun;
        pickup.physicalUseHookPoint += PhysicallyUseInventoryUsable;

    }

    private void OnDisable()
    {
        pickup.physicalUseHookPoint -= PhysicallyUseGun;
        pickup.physicalUseHookPoint -= PhysicallyUseInventoryUsable;

    }

    public IItemFunctionality functionality;

    public void PhysicallyUseGun(Inventory inv, Item pickedUpItem)
    {
        if (pickedUpItem.functionality is not Gun pickedUpGun) return;

        Gun.Data newData = new Gun.Data();
        Raycaster caster = inv.Get<Raycaster>();
        EntityControls controls = inv.Get<EntityControls>();

        newData.SetCaster(caster);
        newData.SetControlsBindGun(controls, pickedUpGun);
        pickedUpGun.UpdateFunctionalityData(newData);
    }

    public void PhysicallyUseInventoryUsable(Inventory inv, Item pickedUpItem)
    {
        if (pickedUpItem.functionality is not InventoryUsable pickedUpUsable) return;

        EntityControls controls = inv.Get<EntityControls>();
        Use use = inv.Get<Use>();

        InventoryUsable.Data newData = new InventoryUsable.Data(
            use,
            controls,
            pickedUpUsable,
            () => pickedUpUsable.CheckCanBeUsed(inv),
pickedUpItem
            );

        pickedUpUsable.UpdateFunctionalityData(newData);
    }

}
