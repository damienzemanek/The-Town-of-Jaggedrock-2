using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace Extensions
{
    public static class NavEX
    {

        public static void Teleport(Transform tpLoc, GameObject objToTeleport, out bool teleporting)
        {
            teleporting = true; if (!tpLoc || !objToTeleport) return;

            bool foundTpLocOnNavMesh = NavMeshUtility.NearestLocOnNavMesh(tpLoc.position, 5f, out Vector3 tpLocOnNavMesh);
            if (objToTeleport.gameObject.TryGetComponent<NavMeshAgent>(out NavMeshAgent agent))
            {
                if (foundTpLocOnNavMesh) agent.Warp(tpLocOnNavMesh);
                else
                {
                    agent.enabled = false;
                    objToTeleport.transform.position = tpLoc.position;
                    agent.enabled = true;
                }
            }
            else
                objToTeleport.transform.position = foundTpLocOnNavMesh ? tpLocOnNavMesh : tpLoc.position;

            teleporting = false;
        }

    }
}