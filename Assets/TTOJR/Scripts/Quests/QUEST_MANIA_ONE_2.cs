using System.ComponentModel;
using DependencyInjection;
using Sirenix.OdinInspector;
using UnityEngine;
using ReadOnlyAttribute = Sirenix.OdinInspector.ReadOnlyAttribute;

public class QUEST_MANIA_ONE_2 : QuestEvent<LadyInBlack>
{

    #region Privates
    [Inject, ReadOnly, ShowInInspector] LadyInBlack lady;

    protected override LadyInBlack recipient => lady;

    protected override void Implementation(LadyInBlack recipient)
    {
        print("attempting to continue lady quest");

        if(lady.currentQuest == Questing.Town.Quest.MANIA_OF_INJUSTICE)
        {
            print($"correct quest, progress is {lady.currentQuestReferece.currentProggressLevel}");
            if (lady.currentQuestReferece.currentProggressLevel == 0)
            {
                lady.IncreaseProgressionOfCurrentQuest();
                print("sucess");
            }
        }

    }
    #endregion



    #region Methods

    #endregion

}
