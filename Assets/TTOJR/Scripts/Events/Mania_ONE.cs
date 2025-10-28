using DependencyInjection;
using Sirenix.OdinInspector;
using System;
using Extensions;

[Serializable]
public sealed class Mania_ONE : QuestEvent<Photographer>
{
    [Inject, ShowInInspector, ReadOnly] Photographer photographer;
    protected sealed override Photographer recipient => photographer;
    protected override void Implementation(Photographer r)
    {
        r.maniaQuestInitial = true;
        this.Log("" + r.maniaQuestInitial);
    }

}