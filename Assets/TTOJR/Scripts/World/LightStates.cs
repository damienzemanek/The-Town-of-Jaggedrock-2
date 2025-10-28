using System;
using System.Collections.Generic;
using System.Linq;
using DependencyInjection;
using NUnit.Framework;
using UnityEngine;

[DefaultExecutionOrder(400)]
public class LightStates : MonoBehaviour
{
    [Inject] TimeCycle time;

    [Serializable]
    public class States
    {
        public enum State
        {
            None,
            Dim,
            Full
        }
        public State state;
        public float intensity;

        public States()
        {
            state = State.None;
            intensity = 1;
        }
    }

    public List<States> states;
    public List<Light> lights;

    public void OnEnable()
    {
        time.OnNightStart.AddListener(() => SetIntensity(States.State.Dim));
        time.OnDayStart.AddListener(() => SetIntensity(States.State.Full));
    }

    private void OnDisable()
    {
        time.OnNightStart.RemoveAllListeners();
        time.OnDayStart.RemoveAllListeners();
    }

    void SetIntensity(States.State toState)
    {
        States state = states.FirstOrDefault(s => s.state == toState);
        lights?.Where(l => l != null)
            .ToList()
            .ForEach(l => l.intensity = state.intensity);
    }
}
