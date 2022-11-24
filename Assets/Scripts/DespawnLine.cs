using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnLine : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Cue cue))
        {
            cue.Despawn();
        }
    }
}
