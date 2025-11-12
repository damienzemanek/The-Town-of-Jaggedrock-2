using DependencyInjection;
using UnityEngine;

public class PsychiatricPatient : RuntimeInjectableMonoBehaviour, IDependencyProvider, IEventRecipient
{

    #region Privates
    [Provide] PsychiatricPatient Provide() => this;
    #endregion

    public bool mania_give_information = false;


    #region Methods
        
    #endregion

}
