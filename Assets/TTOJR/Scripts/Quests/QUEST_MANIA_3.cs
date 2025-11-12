using System.ComponentModel;
using DependencyInjection;
using Sirenix.OdinInspector;
using UnityEngine;
using ReadOnlyAttribute = Sirenix.OdinInspector.ReadOnlyAttribute;

public class QUEST_MANIA_3 : QuestEvent<PsychiatricPatient>
{

    #region Privates
    [Inject, ReadOnly, ShowInInspector] PsychiatricPatient psycho;

    protected override PsychiatricPatient recipient => psycho;

    protected override void Implementation(PsychiatricPatient recipient)
    {
        print("attempting to continue lady quest");
        psycho.mania_give_information = true;

    }
    #endregion



    #region Methods

    #endregion

}
