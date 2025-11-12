using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using Extensions;
using UnityEngine.Events;

public class PreRequisiteCallbackDetector : CallbackDetector
{
    [SerializeField] bool hasPreRequisite;
    [field:SerializeField] public Item lookingForChangesToItem { get; set; }

    //Calls the hasItemPreqrequisite to NOT has the item. calls it with null (no item) and sets it to (false)
    public static void HasItemPrequisitesReset() => hasItemPreRequisite?.Invoke(null, false);

    //Hook slot to check if the player has the item required
    public static Action<Item, bool> hasItemPreRequisite;

    //1 -> Has items

    public class Builder : CallbackDetector.Builder
    {
        PreRequisiteCallbackDetector pcbd;

        protected override CallbackDetector cbd { get => pcbd; set => pcbd = (PreRequisiteCallbackDetector)value; }

        public Builder(GameObject on) : base()
        {
            objOn = on ?? throw new System.ArgumentNullException(nameof(on));
            cbd = objOn.TryGetOrAdd<PreRequisiteCallbackDetector>();
            cbd.useCallback = new UnityEvent();
        }

        public Builder WithRequiredItem(Item item)
        {
            pcbd.lookingForChangesToItem = item;
            return this;
        }

        public override CallbackDetector Build() => pcbd;
    }


    private void OnEnable()
    {
        hasItemPreRequisite += SetPreRequisite;
    }

    private void OnDisable()
    {
        hasItemPreRequisite -= SetPreRequisite;
    }

    //Hook for setting hasPreRequisite
    void SetPreRequisite(Item item, bool val)
    {
        if (lookingForChangesToItem == null) { this.Error("Prereq callback detector looking for item not set"); return; }
        this.Log("Checking if the player has the needed");

        //If thers no item, the player does not have the prerequisite
        if (item == null) 
        {
            hasPreRequisite = false;
            this.Log("Resseting Prereqs");
            return; 
        }

        //If the item types (the actual item) is the same, then the player HAS the prerequsite
        if (item.type == lookingForChangesToItem.type)
            hasPreRequisite = val;
    }
    protected override void OnTriggerEnter(Collider other)
    {
        if (!hasPreRequisite) return;
        base.OnTriggerEnter(other);
    }

    protected override void OnTriggerStay(Collider other)
    {
        if (obj != null && !hasPreRequisite) base.OnTriggerExit(other);
        if (!hasPreRequisite) return;
        if (somethingCollided && obj == null) { this.Log("Something collided"); OnTriggerEnter(other); }
        base.OnTriggerStay(other);
    }

    protected override void OnTriggerExit(Collider other)
    {
        if (!hasPreRequisite) return; 
        base.OnTriggerExit(other);
    }


    public override void OnRaycastedEnter(GameObject caster)
    {
        if (!hasPreRequisite) return;
        base.OnRaycastedEnter(caster);
    }

    public override void OnRaycastedStay(GameObject caster)
    {
        if (!hasPreRequisite) return;
        base.OnRaycastedStay(caster);
    }

    public override void OnRaycastedExit(GameObject caster)
    {
        base.OnRaycastedExit(caster);
    }
}



