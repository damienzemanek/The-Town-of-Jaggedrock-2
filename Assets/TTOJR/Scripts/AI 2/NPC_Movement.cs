using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.AI;
using Extensions;

public class NPC_Movement : MonoBehaviour
{
    public NavMeshAgent agent;
    public LayerMask residentAreaMask;
    public float delayUseArea;
    public bool stopped = false;
    public bool usingArea = false;
    public float stuckTimeout = 8f;
    public float retryCooldown = 1.5f;
    [ReadOnly] public float nextRetryTime = 0f;
    [ReadOnly] public float stopCheckProgressValue = 0f;
    [ReadOnly] public NPC_Area lastAreaInContact;


    public void UseSpawnArea(NPC_Area spawnArea)
    {
        lastAreaInContact = spawnArea;
        StartCoroutine(UseArea(spawnArea));
    }


    void TryUseArea()
    {
        if (usingArea) return;
        if (lastAreaInContact == null) Debug.Log("NPC_Movement: last are in contact null");
        if (agent == null) Debug.Log("NPC_Movement: Does not have an agent (null)");
        if (!agent.isOnNavMesh) return;
        if (agent.pathPending) return;

        StartCoroutine(UseArea(lastAreaInContact));
        nextRetryTime = Time.time + retryCooldown;
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

        if(Time.time >= nextRetryTime)
        {
            if (stopped) TryUseArea();
            else if (stopCheckProgressValue > stuckTimeout) TryUseArea();
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
        if (lastAreaInContact == null) this.Error("StopInArea failed, lastAreaInContact is NULL");
        if (lastAreaInContact.whoIsHere != null)
            if (!lastAreaInContact.whoIsHere.Contains(this))
                lastAreaInContact.whoIsHere.Add(this);

        this.Log("Stopped Moving");
        stopped = true;
    }


    //This will get the area we are currently in
    private void OnTriggerStay(Collider other)
    {
        print("aaaaaaaaaaa");
        if (((1 << other.gameObject.layer) & residentAreaMask) == 0) return;
        if (!other.gameObject.TryGetComponent<NPC_Area>(out NPC_Area area)) return;
        lastAreaInContact = area;
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & residentAreaMask) == 0) return;
        if (!other.gameObject.TryGetComponent<NPC_Area>(out NPC_Area area)) return;
        area.whoIsHere?.Remove(this);
    }


    private void OnDisable()
    {
        if (lastAreaInContact != null)
            lastAreaInContact.whoIsHere?.Remove(this);
    }



}


