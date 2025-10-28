using System;
using UnityEngine;

public interface IEventRecipient
{

    #region Privates

    #endregion
    object recipiant => this;
    void Execute(Action<IEventRecipient> call) => call?.Invoke(this);

    #region Methods
        
    #endregion

}
