using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace Extensions
{
    public static class NavEX
    {
        [Serializable]
        public struct Teleportable
        {
            public GameObject objToTeleport;
            public bool isTeleporting;
        }

        public static void Teleport(Transform tpLoc, ref Teleportable tp)
        {
            tp.isTeleporting = true;
            if (!tpLoc || !tp.objToTeleport) return;

            bool foundTpLocOnNavMesh = NavMeshUtility.NearestLocOnNavMesh(tpLoc.position, 5f, out Vector3 tpLocOnNavMesh);
            if (tp.objToTeleport.gameObject.TryGetComponent<NavMeshAgent>(out NavMeshAgent agent))
            {
                if (foundTpLocOnNavMesh) agent.Warp(tpLocOnNavMesh);
                else
                {
                    agent.enabled = false;
                    tp.objToTeleport.transform.position = tpLoc.position;
                    agent.enabled = true;
                }
            }
            else
                tp.objToTeleport.transform.position = foundTpLocOnNavMesh ? tpLocOnNavMesh : tpLoc.position;

            tp.isTeleporting = false;
        }

    }
}