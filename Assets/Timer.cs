using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    float t;
    private void OnEnable()
    {
        Cue.Spawned += TimerStart;
    }

    private void OnDisable()
    {
        Cue.Spawned -= TimerStart;
    }

    private void TimerStart()
    {
        t = Time.time;
    }

    private void TimerStop()
    {
        t = Time.time - t;
        Debug.Log(t);
    }
}
