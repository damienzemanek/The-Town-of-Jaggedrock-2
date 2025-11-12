using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Extensions;
using UnityEngine.EventSystems;

[Serializable]
public abstract class ItemVariationData 
{
    public bool usable = false;
    public abstract bool conditionsAllowUse { get; }

    [field: SerializeField] [field: ReadOnly] public Inventory inv { get; set; }
    [field: SerializeField][field: ReadOnly] public Interactor interactor { get; set; }

    public void WithDependancies(Inventory _inv, Interactor _interactor)
    {
        inv = _inv; 
        interactor = _interactor;
    }


    public virtual ItemVariationData Clone() => (ItemVariationData)CloneUtility.DeepClonePolymorph(this);
    public virtual void Reset() { }

    // Template method UpdateValue
    // Part of a LINQ chain
    public void UpdateData(ItemVariationData newVariationData = null)
    {
        if(newVariationData != null) usable = newVariationData.usable;
        UpdateDataImplementation(newVariationData);
    }

    protected abstract ItemVariationData UpdateDataImplementation(ItemVariationData newVariationData = null);

    public virtual object UseVariant(List<ItemVariationData> variations) => throw new NotImplementedException();
}


[Serializable]
public sealed class Uses : ItemVariationData
{
    [field: SerializeField] public override bool conditionsAllowUse => (uses > 0);
    [field: SerializeField] public bool usedUp { get; set; }
    [field: SerializeField] public int uses { get; set; }
    [field: SerializeField] public int initialUses { get; set; }

    public override void Reset()
    {
        usedUp = false;
        uses = initialUses;
    }

    protected override ItemVariationData UpdateDataImplementation(ItemVariationData newVariationData = null)
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

    public override bool conditionsAllowUse => true;

    public override object UseVariant(List<ItemVariationData> variations)
    {
        this.Log("Using variant");
        return variations?.OfType<PolaroidImage>()
            .FirstOrDefault()
            .sprite;
    }
    protected override ItemVariationData UpdateDataImplementation(ItemVariationData newVariationData = null) => throw new NotImplementedException();
}

[Serializable]
public sealed class Placable : ItemVariationData
{
     Color GetPlaceColor() => placeLocation == null ? Color.red : Color.green;
    [field:SerializeField] GameObject objectToPlace { get; set; }
    public override bool conditionsAllowUse => (objectToPlace != null 
                                                && placeLocation != null 
                                                && inv != null);

    [SerializeReference, GUIColor(nameof(GetPlaceColor))] PlaceLocation placeLocation;

    protected override ItemVariationData UpdateDataImplementation(ItemVariationData newVariationData = null)
    {
        if(newVariationData is Placable p)
        {
            placeLocation = p.placeLocation;
        }

        return this;
    }

    //Used in a LINQ chain
    //Uses the variant data
    public override object UseVariant(List<ItemVariationData> inventoryVariations)
    {
        if (conditionsAllowUse)
        {
            placeLocation.Place(objectToPlace);
            inv.RemoveCurrentSelectedItem();
        }
        return inventoryVariations?.OfType<Placable>().FirstOrDefault();
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
    public abstract bool UseIfVariantsAllow(UnityEvent extraFunctionalityHook = null);
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
        ItemVariationData dataBlocked = variations.FirstOrDefault(v => v.conditionsAllowUse == false);
        if (dataBlocked == null) return true;
        else return false;
    }

    public object UseVariansThatAreUsable()
    {
        //Variation Utilization
        object useInput = variations?
            .Where(v => v.usable)
            .Select(v => useInput = v.UseVariant(variations))
            .FirstOrDefault(x => x != null);

        return useInput;
    }

    public T GetData<T>() where T: ItemVariationData
    {
        return variations.OfType<T>().FirstOrDefault();
    }

    public abstract bool UseIfVariantsAllow(UnityEvent callback = null);
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
        Color GetUseDestColor() => useDestination == null ? Color.red : Color.green;

        [field:SerializeField, GUIColor(nameof(GetUseDestColor))]  public Destination useDestination { get; private set; }
        public void SetUseLocation(Destination val) => useDestination = val;
        [field: SerializeField] GameObject objectToPlace { get; set; }
    }

    public override bool UseIfVariantsAllow(UnityEvent callback = null)
    {
        //Variants won't determine if a destination can be used (due to placeable variant functionality)
        //if (!VariantsAllowUse()) return false;

        Debug.Log($"Item: Successfully Using {GetType()}");

        //Variation Utilization
        UseVariansThatAreUsable();

        //Functionality Utilization
        callback?.Invoke();

        if (data.useDestination == null) this.Log("Item: (Destination User) does not have a destination set");

        //Sometimes the cbd wont have a use destination
        if(data.useDestination != null)
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
            controls.mouse1 += () => gun.UseIfVariantsAllow();
        }

    }

    public override bool UseIfVariantsAllow(UnityEvent extraFunctionalityHook = null)
    {
        if (!VariantsAllowUse()) return false;
        UseVariansThatAreUsable();

        Uses usesData = GetData<Uses>();

        Debug.Log($"Item: Successfully Using {GetType()}");

        //Variation Utilization
        usesData.Use();

        //Functionality Utilization
        extraFunctionalityHook?.Invoke();

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
                pickedUpInvUsable.UseIfVariantsAllow();
            else
                this.Log("Failed Use");

        }
    }


    public override bool UseIfVariantsAllow(UnityEvent calextraFunctionalityHook = null)
    {
        if (!VariantsAllowUse()) return false;

        this.Log($"Successfully Using {GetType().Name}");

        object useInput = UseVariansThatAreUsable();


        //Functionality Utilization
        calextraFunctionalityHook?.Invoke();

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