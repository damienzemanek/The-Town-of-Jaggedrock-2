using UnityEngine;
using UnityEngine.AI;
using System;
using Sirenix.OdinInspector;
using System.Collections;
using Extensions;

[Serializable]
public abstract class ActionDo 
{
    [field: HideInInspector] [field:ReadOnly] [field:SerializeField] public NavMeshAgent agent { get; protected set; }
    [field: HideInInspector][field: ReadOnly][field: SerializeField] public NPC_Movement NPC_Movement { get; protected set; }
    [field:HideInInspector][field: SerializeField] public ActionChoices fromChoices { get; set; }

    public abstract void Execute(NPC_Area area);
    public void SetAgent(NavMeshAgent agent) => this.agent = agent;
    public void SetResident(NPC_Movement NPC_Movement) => this.NPC_Movement = NPC_Movement;
}


[Serializable]
public class StandHere : ActionDo
{
    [SerializeField] public Vector2 timeStanding = new Vector2(3, 6);
    public override void Execute(NPC_Area area)
    {
        NPC_Movement.StartCoroutine(Stand(area));
    }

    IEnumerator Stand(NPC_Area area)
    {
        this.Log($"(Standing) at area {area.gameObject.name}");
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        float standFor = UnityEngine.Random.Range(minInclusive: timeStanding.x, timeStanding.y);
        yield return new WaitForSeconds(seconds: standFor);
        fromChoices.DoAnAction(area);
    }


}

[Serializable]
public class WalkTo : ActionDo
{
    [field:Required] [field:SerializeField] public NPC_Area destination { get; protected set; }
    public override void Execute(NPC_Area area)
    {
        this.Log($"(Walking) to area {destination} from area {area}");
        Walk();
    }

    void Walk()
    {
        if (!agent.isActiveAndEnabled) return;
        agent.isStopped = false;
        NPC_Movement.stopped = false;
        agent.SetDestination(destination.GetARandLocation());
    }
}
