using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitRegion : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<Cue>(out var note))
        {
            note.IsWithinHitRegion(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<Cue>(out var note))
        {
            note.IsWithinHitRegion(false);
        }
    }
}
