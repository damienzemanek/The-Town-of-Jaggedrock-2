using System.Collections.Generic;
using DependencyInjection;
using Extensions;
using NodeCanvas.DialogueTrees;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

[DefaultExecutionOrder(300)]
public class Town : RuntimeInjectableMonoBehaviour
{
    [SerializeField][Inject] public EntityControls player;
    [SerializeField] public Renderer obj;

    #region Privates
    [SerializeField] bool _corrupted;
    #endregion

    public int currentCorruptionLevel = 0;
    public bool corrupted { get => _corrupted; set => _corrupted = value; }

    GameObject _playerObj;
    public GameObject playerObj { get => player.gameObject; set => _playerObj = value; }

    protected override void OnInstantiate()
    {
        base.OnInstantiate();
        playerObj = player.gameObject;
    }


    [Button]
    public void IncreaseCorruption()
    {
        currentCorruptionLevel++;
        if (currentCorruptionLevel < CorruptionManager.instance.corrMats.Length)
            obj.Get<Renderer>().material = CorruptionManager.instance.corrMats[currentCorruptionLevel];

        if (currentCorruptionLevel > 2)
        {
            corrupted = true;
            obj.Get<Renderer>().material = CorruptionManager.instance.fullyCorrupt;
            CorruptionManager.instance.LoseGame();
        }
    }

    public void Corrupt() => corrupted = true;


    #region Methods
        
    #endregion

}
