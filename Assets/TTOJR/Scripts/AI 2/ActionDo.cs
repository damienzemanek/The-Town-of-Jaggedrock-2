using UnityEngine;
using UnityEngine.AI;
using System;
using Sirenix.OdinInspector;
using System.Collections;
using Extensions;

[Serializable]
public abstract class ActionDo 
{
    [SerializeField] public bool complete = false;

    [field: HideInInspector] [field:ReadOnly] [field:SerializeField] public NavMeshAgent agent { get; protected set; }
    [field: HideInInspector][field: ReadOnly][field: SerializeField] public NPC_Movement NPC_Movement { get; protected set; }
    [field:HideInInspector][field: SerializeField] public ActionChoices fromChoices { get; set; }

    public ActionDo(ActionDo orig)
    {
        complete = false;
        agent = orig.agent;
        NPC_Movement = orig.NPC_Movement;
        fromChoices = orig.fromChoices;
    }

    public void Execute(NPC_Area area)
    {
        complete = false;
        ExecuteImplement(area);
    }

    public abstract void ExecuteImplement(NPC_Area area);

    public void SetAgent(NavMeshAgent agent) => this.agent = agent;
    public void SetResident(NPC_Movement NPC_Movement) => this.NPC_Movement = NPC_Movement;
}


[Serializable]
public class StandHere : ActionDo
{
    [SerializeField] public Vector2 timeStanding = new Vector2(3, 6);

    public StandHere(ActionDo orig) : base(orig)
    {
    }

    public override void ExecuteImplement(NPC_Area area)
    {
        NPC_Movement.StartCoroutine(Stand(area));
    }

    IEnumerator Stand(NPC_Area area)
    {
        this.Log($"(Standing) at area {area.gameObject.name}");
        if (agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }
        float standFor = UnityEngine.Random.Range(minInclusive: timeStanding.x, timeStanding.y);
        yield return new WaitForSeconds(seconds: standFor);
        fromChoices.DoAnAction(area);
        
        complete = true;
    }


}

//[Serializable]
//public class WalkTo : ActionDo
//{
//    [field:Required] [field:SerializeField] public NPC_Area destination { get; protected set; }
//    public override void ExecuteImplement(NPC_Area area)
//    {
//        this.Log($"(Walking) to area {destination} from area {area}");
//        NPC_Movement.StartCoroutine(Walk());
//    }

//    IEnumerator Walk()
//    {
//        if (!agent.isActiveAndEnabled) yield break;
//        if (destination == null)
//        {
//            this.Error("Destination not set on NPC_Area Action: WalkTo");
//            yield break;
//        }


//        agent.isStopped = false;
//        agent.SetDestination(destination.GetARandLocation());

//        yield return new WaitUntil(() =>
//            !agent.pathPending &&
//            agent.remainingDistance <= agent.stoppingDistance &&
//            (!agent.hasPath || agent.velocity.sqrMagnitude <= 0.001f)
//        );

//        complete = true;
//    }
//}


//[Serializable]
//public class Despawn : ActionDo
//{
//    public override void ExecuteImplement(NPC_Area area)
//    {
//        DespawnMe();
//    }

//    void DespawnMe()
//    {
//        agent.isStopped = true;
//        NPC_Movement.Despawn();
//        complete = true;
//    }
//}
