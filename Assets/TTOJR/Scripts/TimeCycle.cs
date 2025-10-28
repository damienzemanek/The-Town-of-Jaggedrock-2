using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DependencyInjection;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Extensions;

public class TimeCycle : MonoBehaviour, IDependencyProvider
{
    [Serializable]
    public class Period
    {
        public enum Type
        {
            Day,
            Night
        }
        public Type type;
        public bool inProgress;
        public bool complete;

        public Period(Type type)
        {
            this.type = type;
            inProgress = true;
            complete = false;
        }
        public void Complete()
        {
            inProgress = false;
            complete = true;
        }
    }


    [Inject] EntityControls controls;
    [Provide] TimeCycle Provide() => this;
    public static TimeCycle instance;
    public Light dayLight;
    public float nightIntensity = 0f;
    float initialIntensity;

    public List<Period> periods = new List<Period>();
    public Period GetCurrentPeriod() => (periods.Count > 0) ? periods.Last() : null;

    public UnityEvent OnDayStart;
    public UnityEvent OnNightStart;

    private void Awake()
    {
        instance = this;
        initialIntensity = dayLight.intensity;
        periods = new List<Period>();
    }



    public float currentTime;

    [Title("Fade Settings")]
    [Range(0f, 100f)] public float dayFadeStartPercent = 75f;
    [Range(0f, 100f)] public float nightFadeStartPercent = 75f;

    [Title("Cycle Lengths (Minutes)")]
    public float dayLengthInMinutes = 5f * 60f;
    public float nightLengthInMinutes = 3f * 60f;

    [Title("States")]
    [SerializeField] bool isDay = true;
    public bool timeFrozen = false;
    public bool transitioning = false;

    private void Start() => SetToDay();

    public void Update()
    {
        if (timeFrozen || transitioning) return;
        currentTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.KeypadMultiply)) currentTime += 9999999f;

        if (isDay) CheckDay();
        if (!isDay) CheckNight();
    }

    void NewDay() => periods.Add(new Period(Period.Type.Day));
    void NewNight() => periods.Add(new Period(Period.Type.Night));

    void CheckDay()
    {
        FadeDayLightToNight();
        if (currentTime > dayLengthInMinutes * 60)
            Transition();
    }

    void CheckNight()
    {
        FadeNightLightToDay(); 

        if (currentTime > nightLengthInMinutes * 60)
            Transition();
    }

    void Transition()
    {
        FadeScreen fade = controls.TryGet<FadeScreen>();

        fade.FadeInAndOutCallback((isDay) ? SetToNight : SetToDay);
        currentTime = 0;
    }


    void SetToNight()
    {
        GetCurrentPeriod()?.Complete();
        isDay = false;
        NewNight();
        dayLight.intensity = nightIntensity;
        OnNightStart?.Invoke();
    }

    void SetToDay()
    {
        GetCurrentPeriod()?.Complete();
        isDay = true;
        NewDay();
        dayLight.intensity = initialIntensity;
        OnDayStart?.Invoke();
    }

    void FadeDayLightToNight()
    {
        float fadeStart = (dayFadeStartPercent / 100f) * (dayLengthInMinutes * 60f);
        float fadeEnd = dayLengthInMinutes * 60f;

        if (currentTime >= fadeStart)
        {
            float t = Mathf.InverseLerp(fadeStart, fadeEnd, currentTime);
            dayLight.intensity = Mathf.Lerp(initialIntensity, nightIntensity, t);
        }
    }

    void FadeNightLightToDay()
    {
        float fadeStart = (nightFadeStartPercent / 100f) * (nightLengthInMinutes * 60f);
        float fadeEnd = nightLengthInMinutes * 60f;

        if (currentTime >= fadeStart)
        {
            float t = Mathf.InverseLerp(fadeStart, fadeEnd, currentTime);
            dayLight.intensity = Mathf.Lerp(nightIntensity, initialIntensity, t);
        }
    }

    public bool IsDay() => (GetCurrentPeriod().type == TimeCycle.Period.Type.Day);
    public bool IsNight() => (GetCurrentPeriod().type == TimeCycle.Period.Type.Night);

}
