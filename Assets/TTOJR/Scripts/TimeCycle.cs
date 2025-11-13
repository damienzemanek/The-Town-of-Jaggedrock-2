using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DependencyInjection;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Extensions;
using TMPro;

public class TimeCycle : MonoBehaviour, IDependencyProvider
{
    [TabGroup("All Periods")] public UnityEvent newPeriodHook;
    [TabGroup("All Periods")] public float blackScreenTime;

    [TabGroup("Day")][SerializeField] int day;
    [TabGroup("Day")][SerializeField] float dayStartDisplayDelay = 5f;
    [TabGroup("Day")] public UnityEvent OnDayStart;
    [TabGroup("Day")][SerializeField] List<UnityEventPlus> dayEvents;
    [TabGroup("Day")][SerializeField] TextMeshProUGUI newDayText;
    [TabGroup("Day")][SerializeField] GameObject newDayObj;
    [TabGroup("Day")][SerializeField] float delayOnDisplayTextDigits;
    [TabGroup("Day")][SerializeField] AudioPlay newDayAudTyping;
    [TabGroup("Day")][SerializeField] AudioClip newDayAud;



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
        if (newPeriodHook == null) newPeriodHook = new UnityEvent();

        if (OnNightStart == null) OnNightStart = new();
        if(OnDayStart == null) OnDayStart = new();
    }

    public void TakeOnNightEvents(List<UnityEventPlus> events)
    {
        if (events == null) this.Error("Please assign events to give TimeCycle");
        while (nightEvents.Count < events.Count)
            nightEvents.Add(new UnityEventPlus());

        for (int i = 0; i < events.Count; i++)
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


    private void Start()
    {
        SetToDay();
        currentTime = 0;
        newPeriodHook?.Invoke();
    }

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
        FadeScreen fade = controls.Get<FadeScreen>();

        fade.FadeInAndOutCallback(
            (isDay) ? SetToNight : SetToDay,
            midhook: () => newPeriodHook?.Invoke(),
            blackScreenTime: blackScreenTime + dayStartDisplayDelay
        );

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
        StartCoroutine(C_DisplayDayConcats());
        GetCurrentPeriod()?.Complete();
        isDay = true;
        NewDay();
        dayLight.intensity = initialIntensity;
        OnDayStart?.Invoke();
        if (day > 0 && day <= dayEvents.Count)
            dayEvents[day - 1]?.InvokeWithDelay(this);

    }

    IEnumerator C_DisplayDayConcats()
    {
        yield return new WaitForSeconds(dayStartDisplayDelay);
        
        newDayObj.SetActive(true);
        newDayText.text = "";

        string[] msg = {"D", "A", "Y", " "};

        int digits = 0;
        int neededDigits = 3;

        yield return new WaitForSeconds(seconds: delayOnDisplayTextDigits);

        while (digits <= neededDigits)
        {
            newDayAudTyping?.Play(newDayAud);
            newDayText.text = newDayText.text + msg[digits];
            yield return new WaitForSeconds(delayOnDisplayTextDigits);
            digits++;
        }

        yield return new WaitForSeconds(delayOnDisplayTextDigits / 2);

        newDayText.text = newDayText.text + day;

        yield return new WaitForSeconds(delayOnDisplayTextDigits * 2.4f);

        newDayObj.SetActive(value: false);
        newDayText.text = "";
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
