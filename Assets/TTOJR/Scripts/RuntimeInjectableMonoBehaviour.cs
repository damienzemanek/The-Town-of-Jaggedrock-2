using DependencyInjection;
using UnityEngine;

namespace DependencyInjection
{
    //Make thuis template method pattern later, to enforce overrideing
    public class RuntimeInjectableMonoBehaviour : MonoBehaviour
    {
        protected virtual void OnInstantiate()
        {
            if (this is IDependencyProvider)
                Injector.Instance.RuntimeProvide(this as IDependencyProvider);

            Injector.Instance.RuntimeInject(this);
        }
        protected virtual void Awake() => OnInstantiate();
    }
}
