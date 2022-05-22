using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class DayNightCycle : MonoBehaviour
{
    public static DayNightCycle instance;
    
    [Header("Time")]
    public float cycleInMinutes = 1;
   
    // Fractional game time, range 0 <> 1. 0 is midnight, 0.5 is noon..
    private float decimalTime = 0.0f;
    // Get time from other scripts by using DayNightCycle.DecimalTime.
    public float DecimalTime{get{return decimalTime;} private set{decimalTime = value;}}

    [Header("Sun")]
    public Transform sun;
    public AnimationCurve sunBrightness = new AnimationCurve(
        new Keyframe(0 ,0.01f),
        new Keyframe(0.15f,0.01f),
        new Keyframe(0.35f,1),
        new Keyframe(0.65f,1),
        new Keyframe(0.85f,0.01f),
        new Keyframe(1 ,0.01f)
    );
    public Gradient sunColor = new Gradient(){
        colorKeys = new GradientColorKey[3]{
            new GradientColorKey(new Color(1, 0.75f, 0.3f), 0),
            new GradientColorKey(new Color(0.95f, 0.95f, 1), 0.5f),
            new GradientColorKey(new Color(1, 0.75f, 0.3f), 1),
        },
        alphaKeys = new GradientAlphaKey[2]{
            new GradientAlphaKey(1, 0),
            new GradientAlphaKey(1, 1)
        }
    };

    [Header("Sky")]
     [GradientUsage(true)]
    public Gradient skyColorDay = new Gradient(){
        colorKeys = new GradientColorKey[3]{
            new GradientColorKey(new Color(0.75f, 0.3f, 0.17f), 0),
            new GradientColorKey(new Color(0.7f, 1.4f, 3), 0.5f),
            new GradientColorKey(new Color(0.75f, 0.3f, 0.17f), 1),
        },
        alphaKeys = new GradientAlphaKey[2]{
            new GradientAlphaKey(1, 0),
            new GradientAlphaKey(1, 1)
        }
    };
    
    [GradientUsage(true)]
    public Gradient skyColorNight = new Gradient(){
        colorKeys = new GradientColorKey[3]{
            new GradientColorKey(new Color(0.75f, 0.3f, 0.17f), 0),
            new GradientColorKey(new Color(0.44f, 1, 1), 0.5f),
            new GradientColorKey(new Color(0.75f, 0.3f, 0.17f), 1),
        },
        alphaKeys = new GradientAlphaKey[2]{
            new GradientAlphaKey(1, 0),
            new GradientAlphaKey(1, 1)
        }
    };

    [Header("Stars")]
    public float starsSpeed = 8;
    [Header("Clouds")]
    public Vector2 cloudsSpeed = new Vector2(1,-1);

    [Header("Fog")]
    public Gradient fogColor = new Gradient(){
        colorKeys = new GradientColorKey[5]{
            new GradientColorKey(new Color(0.66f, 1, 1), 0),
            new GradientColorKey(new Color(0.88f, 0.62f, 0.43f), 0.25f),
            new GradientColorKey(new Color(0.88f, 0.88f, 1), 0.5f),
            new GradientColorKey(new Color(0.88f, 0.62f, 0.43f), 0.75f),
            new GradientColorKey(new Color(0.66f, 1, 1), 1),
        },
        alphaKeys = new GradientAlphaKey[2]{
            new GradientAlphaKey(1, 0),
            new GradientAlphaKey(1, 1)
        }
    };
    
    [Header("Time Of Day Events")]
    public UnityEvent onMidnight;
    public UnityEvent onMorning;
    public UnityEvent onNoon;
    public UnityEvent onEvening;

    // enum value type data type
    private enum TimeOfDay{Night,Morning,Noon,Evening} 
    
    // variables of enum type TimeOfDay
    private TimeOfDay timeOfDay = TimeOfDay.Night;
    private TimeOfDay TODMessageCheck = TimeOfDay.Night;
    
    private Light sunLight;
    private float sunAngle;


    void Awake()
    {
        if (DayNightCycle.instance == null) instance = this;
        else Debug.Log("Warning; Multiples instances found of {0}, only one instance of {0} allowed.",this);
    }

    void Start() 
    {
        sun.rotation = Quaternion.Euler(0,-90,0);
        sunLight = sun.GetComponent<Light>();
    }

    void Update()
    {
        UpdateSunAngle();

        if(Application.isPlaying)
        {   
            UpdatedecimalTime();
            UpdateTimeOfDay();
            RotateSun();
            MoveClouds();
        }
        SetSunBrightness();
        SetSunColor();
        SetSkyColor();
        MoveStars();
        SetFogColor();

        //print ((Time.time * 6 / cycleInMinutes / 360)%1);
    }


    void RotateSun()
    {
        // Rotate 360 degrees every cycleInMinutes minutes.
        sun.Rotate(Vector3.right * Time.deltaTime * 6 / cycleInMinutes); 
    }

    void SetSunBrightness()
    {
        // angle = Vector3.Dot(Vector3.down,sun.forward); // range -1 <> 1 but with non-linear progression, meaning it will go up and down between -1 and 1. Not very usefull because then we don't know the difference between sunrise and sunset.
        sunAngle = Vector3.SignedAngle(Vector3.down,sun.forward,sun.right); // range -180 <> 180 with linear progression, meaning -180 is midnight -90 is morning 0 is midday and 90 is sunset.
        sunAngle = sunAngle/360+0.5f;

        // Adjust sun brightness by the angle at which the sun is rotated
        sunLight.intensity = sunBrightness.Evaluate(sunAngle);
    }

    void SetSunColor()
    {
        sunLight.color = sunColor.Evaluate(sunAngle);
    }

    void UpdateSunAngle()
    {
        sunAngle = Vector3.SignedAngle(Vector3.down,sun.forward,sun.right);
        sunAngle = sunAngle/360+0.5f;
    }

    void SetSkyColor()
    {
        if(sunAngle >= 0.25f && sunAngle < 0.75f)
        { 
            RenderSettings.skybox.SetColor("_SkyColor2",skyColorDay.Evaluate(sunAngle*2f-0.5f));
        }
        else if(sunAngle > 0.75f)
        {
            RenderSettings.skybox.SetColor("_SkyColorNight2",skyColorNight.Evaluate(sunAngle*2f-1.5f));
        }
        else
        {
            RenderSettings.skybox.SetColor("_SkyColorNight2",skyColorNight.Evaluate(sunAngle*2f+0.5f));
        }
    }

    void MoveStars()
    {
        RenderSettings.skybox.SetVector("_StarsOffset",new Vector2(sunAngle * starsSpeed,0));
    }

    void MoveClouds()
    {
        RenderSettings.skybox.SetVector("_CloudsOffset", (Vector2)RenderSettings.skybox.GetVector("_CloudsOffset") + Time.deltaTime * cloudsSpeed);
    }

    void SetFogColor()
    {
        RenderSettings.fogColor = fogColor.Evaluate(sunAngle);
        // Debug.Log(sunAngle);
    }

    void UpdatedecimalTime()
    {
        // 0.25 because the day starts at morning. Time.time times 6 because 360 degrees in a full rotation.
        // Modulo(%) 1 makes the value go from 0 to 1 repeatedly.
        decimalTime = (0.25f + Time.time * 6 / cycleInMinutes / 360)%1;
        // Debug.Log(decimalTime); // Uncomment to see decimal time in the console
    }

    void UpdateTimeOfDay() //// new
    {
        if(decimalTime > 0.25 && decimalTime < 0.5f)
        {
            timeOfDay = TimeOfDay.Morning;
        }
        else if(decimalTime > 0.5f && decimalTime < 0.75f)
        {
            timeOfDay = TimeOfDay.Noon;
        }
        else if(decimalTime > 0.75f)
        {
            timeOfDay = TimeOfDay.Evening;
        }
        else
        {
            timeOfDay = TimeOfDay.Night;
        }

        // Check if the timeOfDay has changed. If so, invoke the event.
        if(TODMessageCheck != timeOfDay)
        {
            InvokeTimeOfDayEvent();
            TODMessageCheck = timeOfDay;
        }
    }

    void InvokeTimeOfDayEvent()
    {
        switch (timeOfDay) {
            case TimeOfDay.Night:
                if(onMidnight != null) onMidnight.Invoke();
                Debug.Log("OnMidnight");
                break;
            case TimeOfDay.Morning:
                if(onMorning != null) onMorning.Invoke();
                Debug.Log("OnMorning");
                break;
            case TimeOfDay.Noon:
                if (onNoon != null) onNoon.Invoke();
                Debug.Log("OnNoon");
                break;
            case TimeOfDay.Evening:
                if(onEvening != null) onEvening.Invoke();
                Debug.Log("OnEvening");
                break;
        }
    }
}
