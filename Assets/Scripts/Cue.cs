using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cue : MonoBehaviour
{
    Rigidbody2D _rigidbody;
    Color color;
    [SerializeField] Color noteColorMiss = Color.gray;
    bool isWithinHitRegion;
    NoteHighway highway;
    Vector3 start;
    Vector3 stop;
    [SerializeField] float baseScore;

    public static event Action Spawned;
    public event Action Despawned;

    double timeInstantiated;
    public float assignedTime;

    void Awake()
    {
        highway = GetComponentInParent<NoteHighway>();
        _rigidbody = GetComponent<Rigidbody2D>();
        start = highway.cueStart;
        stop = highway.cueDestination; 
    }

    IEnumerator Move()
    {
        while (true)
        {
            double timeSinceInstantiated = NoteHighwayManager.GetAudioSourceTime() - timeInstantiated;
            float t = (float)(timeSinceInstantiated / (NoteHighwayManager.Instance.noteTime * 2));
            transform.position = Vector3.Lerp(start, stop, t);
            yield return null;
        }
    }

    public void AssignTime(float time)
    {
        assignedTime = time;
    }

    public void Spawn()
    {
        gameObject.SetActive(true);
        timeInstantiated = NoteHighwayManager.GetAudioSourceTime();
        StartCoroutine(Move());
        Spawned?.Invoke();
    }

    public void Despawn()
    {
        StopAllCoroutines();
        assignedTime = 0;
        isWithinHitRegion = false;
        Despawned?.Invoke();
        gameObject.SetActive(false);
    }

    public void SetNoteColor(Color noteColor)
    {
        color = noteColor;
        GetComponent<SpriteRenderer>().color = color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<NoteIndicator>(out NoteIndicator indicator))
        {
            isWithinHitRegion = true;
        }
    }

}
