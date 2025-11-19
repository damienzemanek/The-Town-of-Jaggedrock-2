using System;
using System.Collections.Generic;
using System.Linq;
using DependencyInjection;
using Extensions;
using NUnit.Framework;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class RoomManager : MonoBehaviour
{

    #region Privates
    [Inject] NPC_Spawner spawner;
    [Inject] TimeCycle time;
    #endregion

    public List<Room> rooms;

    //Save later
    [SerializeField] public List<ResidentWithRoom> residents;

    [Serializable]
    public struct ResidentWithRoom
    {
        public GameObject resident;
        public Room room;

        public ResidentWithRoom(GameObject _resident, Room _room)
        {
            resident = _resident;
            room = _room;
        }

    }

    private void Awake()
    {
        residents = new List<ResidentWithRoom>();
    }
    private void OnEnable()
    {
        time.OnNightStart.AddListener(TeleportBackToRooms);
    }

    private void OnDisable()
    {
        time.OnNightStart.RemoveListener(TeleportBackToRooms);

    }


    private void Start()
    {
        this.Log("Room Manager Starting");
        if (rooms == null || rooms.Count == 0)
            rooms = gameObject.GetComponentsInChildren<Room>().ToList();

        AssignRooms();
    }


    void AssignRooms()
    {
        this.Log("Assining rooms");

        List<GameObject> townNPCS = spawner.npcs
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
        room.SetResident(npc.Get<Town>());
        residents.Add(new ResidentWithRoom(npc, room));
    }

    public void TeleportBackToRooms()
    {
        foreach(ResidentWithRoom r in residents)
        {

        }
    }

    #region Methods

    #endregion

}
