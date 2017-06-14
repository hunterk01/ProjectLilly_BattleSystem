using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptionParticleSlider : MonoBehaviour {

    public GameObject corruptionGO;
    private ParticleSystem corruption;
    ParticleSystem.EmissionModule emCorruption;

    public GameObject corruptionBodyGO;
    private ParticleSystem bodyCorruption;
    ParticleSystem.EmissionModule emBodyCorruption;
    
	void Start ()
    {
        corruption = corruptionGO.GetComponent<ParticleSystem>();
        emCorruption = corruption.emission;
        bodyCorruption = corruptionBodyGO.GetComponent<ParticleSystem>();
        emBodyCorruption = bodyCorruption.emission;
    }
	
    //Update the corruption particle effect to match the current corruption level
    public void UpdateCorruption(float curretCorruption)
    {
        emCorruption.rateOverTime = curretCorruption * 10;
        emBodyCorruption.rateOverTime = curretCorruption * 100;
    }
}
