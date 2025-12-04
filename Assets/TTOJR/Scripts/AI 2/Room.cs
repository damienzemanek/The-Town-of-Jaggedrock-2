using System.Collections.Generic;
using System.Linq;
using DependencyInjection;
using Extensions;
using NUnit.Framework;
using Sirenix.OdinInspector;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] public int roomNum;
    [SerializeField] public NPC_Area tpBackArea;
    [SerializeField] public NPC_Area outsideRoomArea;

    #region Privates
    [ShowInInspector, ReadOnly] Teleport tp;
    [SerializeField] Town resident;
    #endregion



    private void Awake()
    {
        tp = this.Get<Teleport>();
    }


    #region Methods
        
    #endregion

}
