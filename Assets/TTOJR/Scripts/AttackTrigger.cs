using UnityEngine;
using DependencyInjection;
using Extensions;

public class AttackTrigger : RuntimeInjectableMonoBehaviour
{
    [SerializeField] NPC_Corrupted npc;
    protected override void OnInstantiate()
    {
        base.OnInstantiate();
    }

    #region Privates

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (!other.Has(out Attacked player)) return;
        player.Die();
        this.DelayedCall(() => npc.gameObject.SetActive(false), 0.5f);
    }

    #region Methods

    #endregion

}
