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
    [Inject] Despawner despawner;

    public NavMeshAgent agent;
    public LayerMask residentAreaMask;
    public float delayUseArea;
    public bool stopped = false;
    public bool usingArea = false;
    public float stuckTimeout = 8f;
    public float retryCooldown = 1.5f;
    [ReadOnly] public float stopCheckProgressValue = 0f;
    [ReadOnly] public NPC_Area area;


    protected override void OnInstantiate()
    {
        base.OnInstantiate();
    }

    public void UseSpawnArea(NPC_Area spawnArea)
    {
        if (area == null)
        {
            area = spawnArea;

            StartCoroutine(UseArea(spawnArea));
        }
    }


    void TryUseArea()
    {
        if (!stopped) return;
        if (usingArea) return;
        if (area == null) Debug.Log("NPC_Movement: last are in contact null");
        if (agent == null) Debug.Log("NPC_Movement: Does not have an agent (null)");
        if (!agent.isOnNavMesh) return;
        if (agent.pathPending) return;

        StartCoroutine(UseArea(area));
        stopCheckProgressValue = 0f;
    }
    
    public IEnumerator UseArea(NPC_Area area)
    {
        usingArea = true;
        stopped = false;

        yield return new WaitForSeconds(delayUseArea);

        if (area == null) this.Error("No area found to use");
        print("NPC_Movement: Using Area");
        area.RemoveAgentToActions();
        area.SetAgentInAllActions(agent);
        area.SetResidentInAllActions(this);
        area.choices?.DoAnAction(area);
        usingArea = false;
        stopCheckProgressValue = 0;
    }

    private void FixedUpdate()
    {
        stopCheckProgressValue += Time.fixedDeltaTime;

        CheckIfStoppedSetStopped();

        if(Time.time >= stuckTimeout)
        {
            if (stopped) TryUseArea();
            else if (stopCheckProgressValue > stuckTimeout)
            {
                //Keep going
                agent.isStopped = false;
                agent.SetDestination(agent.destination);
            }
        }
    }

    //This will check if we are stopped
    void CheckIfStoppedSetStopped()
    {
        if (agent == null) throw new System.Exception("NPC_Movement: No Agent found");

        if (stopped) return;
        if (agent.pathPending) return;
        if (!agent.isOnNavMesh) return;


        if(agent.pathStatus == NavMeshPathStatus.PathInvalid ||
        agent.pathStatus == NavMeshPathStatus.PathPartial)
            StopInArea();


        //Close Enough
        if (agent.velocity.magnitude > 0.1f) return;
        if (agent.remainingDistance > agent.stoppingDistance + 0.2f) return;


        StopInArea();
    }

    void StopInArea()
    {
        if (area != null) 
            if (area.whoIsHere != null)
                if (!area.whoIsHere.Contains(this))
                    area.whoIsHere.Add(this);

        this.Log("Stopped Moving");
        stopped = true;
    }


    //This will get the area we are currently in
    private void OnTriggerStay(Collider other)
    {
        if (!stopped) return;
        this.Log("chosen area");
        if (((1 << other.gameObject.layer) & residentAreaMask) == 0) return;
        if (!other.gameObject.Has(out NPC_Area _area)) return;
        area = _area;
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & residentAreaMask) == 0) return;
        if (!other.gameObject.Has(out NPC_Area _area)) return;
        area.whoIsHere?.Remove(this);
    }


    private void OnDisable()
    {
        if (area != null)
            area.whoIsHere?.Remove(this);
    }

    public void Despawn() => despawner.DirectDisable(gameObject);



}


