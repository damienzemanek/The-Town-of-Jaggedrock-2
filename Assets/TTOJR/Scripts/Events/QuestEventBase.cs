using DependencyInjection;
using Extensions;

public abstract class QuestEventBase : RuntimeInjectableMonoBehaviour
{
    protected override void OnInstantiate()
    {
        base.OnInstantiate();
    }
    public abstract void Execute();
}

//Generic constraint, of: (1) class (2) IEventRecipient
public abstract class QuestEvent<RecipientType> : QuestEventBase where RecipientType : class, IEventRecipient
{
    protected abstract RecipientType recipient { get; }

    protected abstract void Implementation(RecipientType recipient);

    public override sealed void Execute()
    {
        var r = recipient;
        if (r == null) { this.Error($"[{GetType().Name}] recipient is null"); return; }
        Implementation(recipient);
    }
}
