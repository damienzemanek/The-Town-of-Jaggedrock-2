using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class UnityLifecycleHook : MonoBehaviour
{
    public bool awake, onenable, start, update, fixedupdate;

    [ShowIf("awake")] public UnityEvent OnAwake;
    [ShowIf("onenable")] public UnityEvent OnOnEnable;
    [ShowIf("start")] public UnityEvent OnStart;
    [ShowIf("update")] public UnityEvent OnUpdate;
    [ShowIf("fixedupdate")] public UnityEvent OnFixedUpdate;

    private void Awake()
    {
        if (awake) OnAwake?.Invoke();
    }

    private void OnEnable()
    {
        if (onenable) OnOnEnable?.Invoke();
    }

    private void Start()
    {
        if (start) OnStart?.Invoke();
    }

    private void Update()
    {
        if (update) OnUpdate?.Invoke();
    }

    private void FixedUpdate()
    {
        if (fixedupdate) OnFixedUpdate?.Invoke();
    }
}