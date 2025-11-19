using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.AI;
using Extensions;
using DependencyInjection;

public class NPC_Movement : RuntimeInjectableMonoBehaviour
{
    [ShowInInspector] [SerializeReference] ActionDo currentAction;

    public NavMeshAgent agent;
    public LayerMask residentAreaMask;
    public float delayUseArea;
    public bool inAnArea = false;
    public bool usingArea = false;
    public float stuckTimeout = 8f;
    [ReadOnly] public float stopCheckProgressValue = 0f;
    [ReadOnly] public NPC_Area area;

    Coroutine noLongerInArea;

    protected override void OnInstantiate()
    {
        base.OnInstantiate();
    }

    public void DirectUseArea(NPC_Area _area)
    {
        if (usingArea)
            currentAction.complete = true;

        area = _area;
        StartCoroutine(UseArea(area));
    }

    private void FixedUpdate()
    {
        if (!inAnArea || area == null) return;

        if(!usingArea) StartCoroutine(UseArea(area));
    }


    public IEnumerator UseArea(NPC_Area area)
    {
        usingArea = true;

        area.RemoveAgentToActions();
        area.SetAgentInAllActions(agent);
        area.SetResidentInAllActions(this);

        //currentAction = new ActionDo(area.choices?.DoAnAction(area));

        

        if (currentAction == null)
        {
            this.Error("No action returned from DoAnAction");
            usingArea = false;
            yield break;
        }

        yield return new WaitUntil(() => currentAction.complete);

        currentAction = null;
        usingArea = false;
    }


    //This will get the area we are currently in
    private void OnTriggerStay(Collider other)
    {
        if (((1 << other.gameObject.layer) & residentAreaMask) == 0) return;

        if (!other.gameObject.Has(out NPC_Area _area)) return;
        print("In area");

        inAnArea = true;
        area = _area;

        if (noLongerInArea != null)
            StopCoroutine(noLongerInArea);

        noLongerInArea = StartCoroutine(C_NoLongerInArea());
    }

    IEnumerator C_NoLongerInArea()
    {
        yield return new WaitForSeconds(0.5f);
        inAnArea = false;
        area = null;
    }




}


