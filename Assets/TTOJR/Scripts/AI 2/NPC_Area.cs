using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using NUnit.Framework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class NPC_Area : MonoBehaviour
{
    [SerializeReference] public HashSet<NPC_Movement> whoIsHere = new HashSet<NPC_Movement>();
    [Required] [SerializeReference] public ActionChoices choices;

    public float fixedY;
    Collider collider;

    private void Awake()
    {
        collider = GetComponent<Collider>();
        if (choices == null) Debug.LogError(message: "NPC_Area: Choices not set");
    }

    private void Start()
    {
        string ResidentAreaLayerMask = "NPC_Area";
        if (gameObject.layer != LayerMask.NameToLayer(ResidentAreaLayerMask))
            throw new System.Exception($"NPC_Area: ({gameObject.name})'s Layer is not set correctly, set it to NPC_Area");

        gameObject.layer = LayerMask.NameToLayer(ResidentAreaLayerMask);
    }

    public Vector3 GetARandLocation()
    {
        if (collider == null) throw new System.Exception("NPC_Area: Collider not found, add it");
        Bounds bounds = collider.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float z = Random.Range(bounds.min.z, bounds.max.z);

        Vector3 randomPoint = new Vector3(x, fixedY, z);

        //loc the loc to nav mesh
        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            return hit.position;

        return randomPoint;
    }

    public void SetAgentInAllActions(NavMeshAgent agentInContact)
    {
        print($"NPC_Area: {name} Setting NavMeshAgent in my list of Actions");
        choices.actions.ForEach(a => a.action.SetAgent(agentInContact));
    }
    public void SetResidentInAllActions(NPC_Movement NPC_Movement)
    {
        choices.actions.ForEach(a => a.action.SetResident(NPC_Movement));
    }

    public void RemoveAgentToActions() =>
                choices.actions.ForEach(a => a.action.SetAgent(null));

    void OnValidate()
    {
        if(choices.actions != null && choices.actions.Count > 0)
            choices.actions
                .Where(wa => wa.action != null)
                .ToList()
                .ForEach(wa => wa.action.fromChoices = choices);
    }
}
