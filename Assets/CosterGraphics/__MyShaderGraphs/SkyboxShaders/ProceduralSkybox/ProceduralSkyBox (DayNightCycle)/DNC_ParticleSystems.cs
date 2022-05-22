using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DNC_ParticleSystems : MonoBehaviour
{
    private DayNightCycle dayNightCycle;
    private UnityAction setEmissionLow;
    private UnityAction setEmissionHigh;

    void Awake()
    {
        dayNightCycle = FindObjectOfType<DayNightCycle>();
    }

    void OnEnable()
    {
        setEmissionHigh += delegate{SetParticleEmission(100);};
        setEmissionLow += delegate{SetParticleEmission(20);};

        dayNightCycle.onMorning.AddListener(setEmissionHigh);
        dayNightCycle.onMidnight.AddListener(setEmissionLow);
    }

    void OnDisable()
    {
        dayNightCycle.onMorning.RemoveListener(setEmissionHigh);
        dayNightCycle.onMidnight.RemoveListener(setEmissionLow);
    }

    public void SetParticleEmission(int amount)
    {
        foreach(ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
        {
            var em = ps.emission;
            em.rateOverTime = amount;
        }
    }
}
