using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteIndicator : MonoBehaviour
{
    public event Action<Cue> CueArrived;
    public event Action<Cue> CuePassed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Cue cue))
        {
            cue.IsWithinHitRegion = true;
            CueArrived?.Invoke(cue);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Cue cue))
        {
            cue.IsWithinHitRegion = false;
            CuePassed?.Invoke(cue);
        }
    }
}
