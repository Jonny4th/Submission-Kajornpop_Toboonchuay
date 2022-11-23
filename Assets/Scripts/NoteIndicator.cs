using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NoteIndicator : MonoBehaviour
{
    public event Action<Cue> CueArrived;
    public event Action<Cue> CuePassed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Cue cue))
        {
            CueArrived?.Invoke(cue);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Cue cue))
        {
            CuePassed?.Invoke(cue);
        }
    }
}
