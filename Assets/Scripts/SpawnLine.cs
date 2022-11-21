using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLine : MonoBehaviour
{
    [SerializeField] Note[] notes;

    private void Awake()
    {
        GetComponentInParent<NoteTrack>().NoteSpawning += SpawnNote;
    }

    private void SpawnNote()
    {
        foreach (var note in notes)
        {
            if (!note.gameObject.activeSelf)
            {
                note.transform.position = gameObject.transform.position;
                note.Spawn();
                break;
            }
        }
    }
}
