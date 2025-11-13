using UnityEngine;
using DependencyInjection;

public class AttackTrigger : RuntimeInjectableMonoBehaviour
{
    [Inject] EntityControls player;

    protected override void OnInstantiate()
    {
        base.OnInstantiate();
    }

    #region Privates

    #endregion



    #region Methods

    #endregion

}
