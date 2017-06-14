using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CorruptionMeter : MonoBehaviour
{
    public Image corruption_Bar;

    public float currentCorruption;
    private float newCorruption;
    private float maxCorruption = 500;
    private float barSpeed = 20;

    public bool updateMeter = false;
    public bool meterDown = false;
	
    void Start()
    {
        InitializeCorruptionMeter();
    }

    void Update ()
    {
        if (updateMeter)
        {
            if (meterDown)
                LowerCorruptionMeter();
            else
                RaiseCorruptionMeter();
        }
	}

    // Add a set value to the corruption level
    public void RaiseCorruption(float _corruptionChange)
    {
        newCorruption = currentCorruption;
        newCorruption += _corruptionChange;
        if (newCorruption > maxCorruption) newCorruption = maxCorruption;

        updateMeter = true;
        meterDown = false;
    }

    // Subtract a set value from the corruption level
    public void LowerCorruption(float _corruptionChange)
    {
        newCorruption = currentCorruption;
        newCorruption -= _corruptionChange;
        if (newCorruption < 0) newCorruption = 0;

        updateMeter = true;
        meterDown = true;
    }

    // Set the meter to the current value of the corruption level when entering the battle
    private void InitializeCorruptionMeter()
    {
        float corruption_FillPercentage = currentCorruption / maxCorruption;
        corruption_Bar.transform.localScale = new Vector3(Mathf.Clamp(corruption_FillPercentage, 0, 1),
                         corruption_Bar.transform.localScale.y, corruption_Bar.transform.localScale.z);
    }

    // Makes a smooth transistion of the corruption meter from the old value to current value
    public void LowerCorruptionMeter()
    {
        if (newCorruption < currentCorruption)
        {
            currentCorruption -= barSpeed * Time.deltaTime;
            float corruption_FillPercentage = currentCorruption / maxCorruption;
            corruption_Bar.transform.localScale = new Vector3(Mathf.Clamp(corruption_FillPercentage, 0, 1),
                             corruption_Bar.transform.localScale.y, corruption_Bar.transform.localScale.z);
        }
        else
        {
            currentCorruption = newCorruption;
            float corruption_FillPercentage = currentCorruption / maxCorruption;
            corruption_Bar.transform.localScale = new Vector3(Mathf.Clamp(corruption_FillPercentage, 0, 1),
                             corruption_Bar.transform.localScale.y, corruption_Bar.transform.localScale.z);

            updateMeter = false;
        }
    }

    // Makes a smooth transistion of the corruption meter from the old value to current value
    public void RaiseCorruptionMeter()
    {
        if (newCorruption > currentCorruption)
        {
            currentCorruption += barSpeed * Time.deltaTime;
            float corruption_FillPercentage = (currentCorruption / maxCorruption);
            corruption_Bar.transform.localScale = new Vector3(Mathf.Clamp(corruption_FillPercentage, 0, 1),
                             corruption_Bar.transform.localScale.y, corruption_Bar.transform.localScale.z);
        }
        else
        {
            currentCorruption = newCorruption;
            float corruption_FillPercentage = currentCorruption / maxCorruption;
            corruption_Bar.transform.localScale = new Vector3(Mathf.Clamp(corruption_FillPercentage, 0, 1),
                             corruption_Bar.transform.localScale.y, corruption_Bar.transform.localScale.z);

            updateMeter = false;
        }
    }
}
