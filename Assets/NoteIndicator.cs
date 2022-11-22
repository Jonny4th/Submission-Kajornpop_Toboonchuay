using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NoteIndicator : MonoBehaviour
{
    public event Action<Cue> CueHit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<Cue>(out Cue cue))
        {
            CueHit?.Invoke(cue);
        }
    }
}
