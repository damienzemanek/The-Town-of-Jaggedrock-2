using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using Extensions;

public class PreRequisiteCallbackDetector : CallbackDetector
{
    [SerializeField] bool _preRequisite;
    [field:SerializeField] public Item lookingForChangesToItem { get; set; }
    public static void HasItemPrequisitesReset() => hasItemPreRequisite?.Invoke(null, false);
    public static Action<Item, bool> hasItemPreRequisite;

    //1 -> Has items

    private void OnEnable()
    {
        hasItemPreRequisite += SetPreRequisite;
    }

    private void OnDisable()
    {
        hasItemPreRequisite -= SetPreRequisite;
    }

    void SetPreRequisite(Item item, bool val)
    {
        this.Log("Inv: Attempting to set prereq");
        if (item == null) 
        { 
            _preRequisite = false;
            this.Log("Resseting Prereqs");
            return; 
        }
        if (lookingForChangesToItem == null) return;
        if (item.type == lookingForChangesToItem.type)
            _preRequisite = val;
    }
    protected override void OnTriggerEnter(Collider other)
    {
        if (!_preRequisite) return;
        base.OnTriggerEnter(other);
    }

    protected override void OnTriggerStay(Collider other)
    {
        if (obj != null && !_preRequisite) base.OnTriggerExit(other);
        if (!_preRequisite) return;
        if (somethingCollided && obj == null) { this.Log("Something collided"); OnTriggerEnter(other); }
        base.OnTriggerStay(other);
    }

    protected override void OnTriggerExit(Collider other)
    {
        if (!_preRequisite) return; 
        base.OnTriggerExit(other);
    }


    public override void OnRaycastedEnter(GameObject caster)
    {
        if (!_preRequisite) return;
        base.OnRaycastedEnter(caster);
    }

    public override void OnRaycastedStay(GameObject caster)
    {
        if (!_preRequisite) return;
        base.OnRaycastedStay(caster);
    }

    public override void OnRaycastedExit(GameObject caster)
    {
        base.OnRaycastedExit(caster);
    }
}



