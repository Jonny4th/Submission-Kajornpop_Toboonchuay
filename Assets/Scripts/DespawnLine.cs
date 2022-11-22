using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnLine : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Cue>(out var note))
        {
            note.Despawn();
        }
    }
}
