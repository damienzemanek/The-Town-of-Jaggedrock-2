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
    [TabGroup("Day")][SerializeField] int day;
    [TabGroup("Day")] public UnityEvent OnDayStart;
    [TabGroup("Day")][SerializeField] List<UnityEventPlus> dayEvents;

    [TabGroup("Night")][SerializeField] int night;
    [TabGroup("Night")] public UnityEvent OnNightStart;
    [TabGroup("Night")][SerializeField] List<UnityEventPlus> nightEvents;



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

    private void Awake()
    {
        instance = this;
        initialIntensity = dayLight.intensity;
        periods = new List<Period>();
    }

    public void TakeOnNightEvents(List<UnityEventPlus> events)
    {
        if (events == null) this.Error("Please assign events to give TimeCycle");
        while (nightEvents.Count < events.Count)
            nightEvents.Add(new UnityEventPlus());

        for(int i = 0 ; i < events.Count; i++)
        {
            UnityEventPlus e = events[i]; if (e == null) continue;
            nightEvents[i] = events[i];
        }
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
        night++;
        GetCurrentPeriod()?.Complete();
        isDay = false;
        NewNight();
        dayLight.intensity = nightIntensity;
        OnNightStart?.Invoke();

        if (night > 0 && night <= nightEvents.Count)
            nightEvents[night - 1]?.InvokeWithDelay(this);


    }

    void SetToDay()
    {
        day++;
        GetCurrentPeriod()?.Complete();
        isDay = true;
        NewDay();
        dayLight.intensity = initialIntensity;
        OnDayStart?.Invoke();
        if (day > 0 && day <= dayEvents.Count)
            dayEvents[day - 1]?.InvokeWithDelay(this);

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
