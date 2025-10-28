using UnityEngine;
using Extensions;

[RequireComponent(typeof(Pickup))]
public class QuickAssigner : MonoBehaviour
{
    Pickup pickup;
    private void Awake()
    {
        pickup = this.TryGet<Pickup>();
    }

    private void OnEnable()
    {
        pickup.assignBindings += GunAssign;
        pickup.assignBindings += InventoryUsableAssign;

    }

    private void OnDisable()
    {
        pickup.assignBindings -= GunAssign;
        pickup.assignBindings -= InventoryUsableAssign;

    }

    public IItemFunctionality functionality;

    public void GunAssign(Inventory inv, Item pickedUpItem)
    {
        if (pickedUpItem.functionality is not Gun pickedUpGun) return;

        Gun.Data newData = new Gun.Data();
        Raycaster caster = inv.TryGet<Raycaster>();
        EntityControls controls = inv.TryGet<EntityControls>();

        newData.SetCaster(caster);
        newData.SetControlsBindGun(controls, pickedUpGun);
        pickedUpGun.UpdateFunctionalityData(newData);
    }

    public void InventoryUsableAssign(Inventory inv, Item pickedUpItem)
    {
        if (pickedUpItem.functionality is not InventoryUsable pickedUpUsable) return;

        EntityControls controls = inv.TryGet<EntityControls>();
        Use use = inv.TryGet<Use>();

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
