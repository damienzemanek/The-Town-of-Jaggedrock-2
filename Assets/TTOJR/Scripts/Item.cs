using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Extensions;

[Serializable]
public abstract class ItemVariationData 
{
    public bool usable = false;

    protected Interactor interactor;
    public virtual ItemVariationData Clone()
        => (ItemVariationData)CloneUtility.DeepClonePolymorph(this);
    public virtual void Reset() { }
    public virtual ItemVariationData UpdateValueThenGet(ItemVariationData newVariationData = null) => this;
    public virtual bool AllowUse() => true;
    public virtual void SetInteractor(Interactor interactor) => this.interactor = interactor;
    public virtual object UseVariantGetData(List<ItemVariationData> variations) => throw new NotImplementedException();
}


[Serializable]
public sealed class Uses : ItemVariationData
{
    [field: SerializeField] public Inventory inv { get; set; }
    [field: SerializeField] public bool usedUp { get; set; }
    [field: SerializeField] public int uses { get; set; }
    [field: SerializeField] public int initialUses { get; set; }

    public override void Reset()
    {
        usedUp = false;
        uses = initialUses;
    }

    public override bool AllowUse() => (uses > 0);

    public override ItemVariationData UpdateValueThenGet(ItemVariationData newVariationData = null)
    {
        Use();
        return this;
    }

    public void Use()
    {
        Debug.Log($"Item: VARIATION Uses item used, current uses: {uses}.");
        uses--;
        if (uses <= 0) RunOutOfUses();
    }    
    public void RunOutOfUses()
    { 
        usedUp = true;
        inv.RemoveCurrentSelectedItem();
        inv.interactor.ToggleCanInteract(false);
        Debug.Log("Item: VARIATION Uses out of uses");
    }
    public void UsesInit() { uses = initialUses; usedUp = false; }
}

[Serializable]
public sealed class PolaroidImage : ItemVariationData
{
    [field: SerializeField] public Sprite sprite;

    public override object UseVariantGetData(List<ItemVariationData> variations)
    {
        this.Log("Using variant");
        return variations?.OfType<PolaroidImage>()
            .FirstOrDefault()
            .sprite;
    }
}

[Serializable]
public sealed class Placable : ItemVariationData
{
    [field:SerializeField] GameObject objectToPlace { get; set; }
    [SerializeReference, GUIColor("RGB(0, 1, 0)"), ReadOnly] PlaceLocation placeLocation;
    public override object UseVariantGetData(List<ItemVariationData> variations)
    {
        placeLocation.Place(objectToPlace);

        return variations?.OfType<Placable>()
            .FirstOrDefault();
    }
}




[Serializable]
[CreateAssetMenu(fileName = "New Item", menuName = "ScriptableObjects/Item")]
public class Item : ScriptableObject
{
    public enum ItemType
    {
        none,
        towels,
        cleaningspray,
        deadcroweffigy,
        notebook,
        polaroid,
        revolver,
        key1,
        key2,
        key3,
        key4,
        key5,
        key6,
        key7,
        key8,
        key9,
        key10,
        key11,
        key12,
        key13,
        key14,
        key15,
        key16,
        key17,
        key18,
        key19,
        key20,
    }

    [field: DisableInInlineEditors] [field:SerializeField] public ItemType type { get; set; }
    [DisableInInlineEditors] public Sprite icon;
    [field: SerializeReference] public IItemFunctionality functionality;
    [DisableInInlineEditors] public bool canPhysicallyHold = false;

    [ShowIf("canPhysicallyHold")] public GameObject itemObj;
    
    public Item Clone(string namesuff = " instance")
    {
        var clone = CreateInstance<Item>();
        clone.name = name + namesuff;
        clone.type = type;
        clone.icon = icon;
        clone.functionality = functionality?.Clone();
        var vars = clone.functionality?.variations;
        vars?.ForEach(v => v.Reset());
        return clone;
    }

}

[field: Serializable]
public interface IItemFunctionality 
{
    public abstract bool CanUse_ThenUse(UnityEvent callback = null);
    public abstract void UpdateFunctionalityData(object newData);
    [field: SerializeReference] public List<ItemVariationData> variations { get; set; }
    public IItemFunctionality Clone();
    [field: SerializeReference] public object Data { get; set; }
}

[Serializable]
public abstract class ItemFunctionality<T> : IItemFunctionality
{
    public bool VariantsAllowUse()
    {
        ItemVariationData dataBlocked = variations.FirstOrDefault(v => v.AllowUse() == false);
        if (dataBlocked == null) return true;
        else return false;
    }

    public T GetData<T>() where T: ItemVariationData
    {
        return variations.OfType<T>().FirstOrDefault();
    }

    public abstract bool CanUse_ThenUse(UnityEvent callback = null);
    public abstract void UpdateFunctionalityData(object newData);
    [field: SerializeReference] public List<ItemVariationData> variations { get; set; }
    public abstract T data { get; set; }
    public object Data { get => (T)data; set => data = (T)value; }
    public virtual IItemFunctionality Clone()
    {
        var cloneObj = (IItemFunctionality)Activator.CreateInstance(GetType());
        var typed = (ItemFunctionality<T>)cloneObj;
        typed.data = CloneUtility.DeepClone(data);

        typed.variations = variations?.
            Select(v => v?.Clone())
            .ToList();
        
        return typed;
    }
}


[Serializable]
class DestinationUser : ItemFunctionality<DestinationUser.Data>
{
    [field: SerializeReference] public override Data data { get; set; }
    [Serializable]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    public class Data
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
    {
        [field:SerializeField, GUIColor("RGB(0, 1, 0)")]  public Destination useDestination { get; private set; }
        public void SetUseLocation(Destination val) => useDestination = val;
    }

    public override bool CanUse_ThenUse(UnityEvent callback = null)
    {
        if (!VariantsAllowUse()) return false;

        Debug.Log($"Item: Successfully Using {GetType()}");

        //Variation Utilization


        //Functionality Utilization
        callback?.Invoke();
        if (data.useDestination == null) throw new System.Exception(
            "Item: (Destination User) does not have a destination set");

        if (!data.useDestination.preventContact)
            data.useDestination.MakeContact();

        return true;
    }

    public override void UpdateFunctionalityData(object input)
    {
        var newData = (DestinationUser.Data)input;
        data.SetUseLocation(newData.useDestination);
    }
}

[Serializable]
public class Gun : ItemFunctionality<Gun.Data>
{
    [field: SerializeReference] public override Data data { get; set; }
    [Serializable]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    public class Data
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
    {
        [field: SerializeField][field: ReadOnly] public Raycaster caster { get; private set; }
        [field: SerializeField][field: ReadOnly] public EntityControls controls { get; private set; }

        public LayerMask mask;
        public float damage;

        public void SetCaster(Raycaster _caster) => caster = _caster;
        public void SetControlsBindGun(EntityControls _controls, Gun gun)
        {
            controls = _controls;
            BindControls(controls, gun);
        }

        void BindControls(EntityControls controls, Gun gun)
        {
            controls.mouse1 += () => gun.CanUse_ThenUse();
        }

    }

    public override bool CanUse_ThenUse(UnityEvent callback = null)
    {
        if (!VariantsAllowUse()) return false;
        Uses usesData = GetData<Uses>();

        Debug.Log($"Item: Successfully Using {GetType()}");

        //Variation Utilization
        usesData.Use();

        //Functionality Utilization
        callback?.Invoke();

        if (!Fire(data)) Debug.Log("Item: (Gun) Missed");

        return true;
    }



    bool Fire(Data data)
    {
        Debug.Log("Firing");
        if (!data.caster.Raycast(out RaycastHit hit, data.mask)) return false;
        Debug.Log("hit a target");
        if (!hit.transform.gameObject.TryGetComponent<Health>(out Health health)) 
        { Debug.Log("Item: (Gun) Did not find a target with Health"); return false; }
        Debug.Log($"Target {health.gameObject.name} taking damage");
        
        health.TakeDamage(data.damage);
        return true;
    }


    public override void UpdateFunctionalityData(object input)
    {
        var newData = (Gun.Data)input;
        data.SetCaster(newData.caster);
    }

}


[Serializable]
public class InventoryUsable : ItemFunctionality<InventoryUsable.Data>
{
    [field: SerializeReference] public override Data data { get; set; }

    [Serializable]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    public class Data
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
    {
        public enum Type
        {
            Polaroid,
        }

        [field: ReadOnly] [field: SerializeField] public Item requiredItem { get; set; }
        [field: ReadOnly] [field: SerializeField] public Use use { get; set; }
        [field: ReadOnly] [field: SerializeField] public EntityControls controls { get; set; }
        [field:SerializeField] public Type type { get; set; }


        private InventoryUsable pickedUpInvUsable;
        private Func<bool> CanBeUsed;

        public Data(Use _use, EntityControls _controls, InventoryUsable _pickedUpInvUsable, Func<bool> canBeUsed, Item _requiredItem)
        {
            requiredItem = _requiredItem ?? throw new ArgumentNullException(nameof(requiredItem));
            use = _use ?? throw new ArgumentNullException(nameof(_use));
            controls = _controls ?? throw new ArgumentNullException(nameof(_controls));
            pickedUpInvUsable = _pickedUpInvUsable;
            CanBeUsed = canBeUsed;

            BindControls();
        }

        private void BindControls()
        {
            controls.interact -= OnInteract;
            controls.interact += OnInteract;
        }
        void OnInteract()
        {
            if (CanBeUsed == null) this.Error("No Func<bool> CanBeUsed found");

            this.Log("Attempting InvUsable Use");

            if (CanBeUsed.Invoke())
                pickedUpInvUsable.CanUse_ThenUse();
            else
                this.Log("Failed Use");

        }
    }


    public override bool CanUse_ThenUse(UnityEvent callback = null)
    {
        if (!VariantsAllowUse()) return false;

        this.Log($"Successfully Using {GetType().Name}");

        
        //Variation Utilization
        object useInput = variations?
            .Where(v => v.usable)
            .Select(v => useInput = v.UseVariantGetData(variations))
            .FirstOrDefault(x => x != null);
        

        //Functionality Utilization
        callback?.Invoke();

        if (!data.use) { this.Error("No Use variable Found"); return false; }

        data.use.UseAction(data.type, useInput);
        return true;
    }


    public override void UpdateFunctionalityData(object input)
    {
        var newData = (Data)input;
        data.use = newData.use;
    }

    public bool CheckCanBeUsed(Inventory inv)
    {
        if(inv.GetCurrentItem() == null) return false;
        bool check = inv.GetCurrentItem().functionality == this;

        this.Log($"CheckCanBeUsed = [{inv.GetCurrentItem().functionality}] == [{this}] : {check},");
        return check;
    }
}