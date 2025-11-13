using System.Collections.Generic;
using System.Linq;
using DependencyInjection;
using Extensions;
using NUnit.Framework;
using UnityEngine;

public class RoomManager : MonoBehaviour
{

    #region Privates
    [Inject] NPC_Spawner spawner;
    [Inject] Despawner despawner;
    #endregion

    public List<Room> rooms;
    private void Start()
    {
        this.Log("Room Manager Starting");
        if (rooms == null || rooms.Count == 0)
            rooms = gameObject.GetComponentsInChildren<Room>().ToList();

        spawner.spawningCompleteHook.AddListener(AssignRooms);
    }


    void AssignRooms()
    {
        this.Log("Assining rooms");

        List<GameObject> townNPCS = despawner.spawnedNPCs
            .Where(g => g.Has<Town>())
            .ToList();

        foreach (Room room in rooms)
        {
            if (townNPCS.Count <= 0) break;
            GameObject chosen = townNPCS.Rand();
            this.Log($"room {room.name} to npc {chosen.name}");
            AssignARoom(room, chosen);
            if(chosen.TryGetComponent(out IdentifiableInformationSystem iis))
            {
                iis.isResident = true;
                iis.roomNum = room.roomNum;
            }
            townNPCS.Remove(chosen);
        }

    }

    void AssignARoom(Room room, GameObject npc)
    {
        room.SetResident(npc.TryGet<Town>());
    }

    #region Methods

    #endregion

}
