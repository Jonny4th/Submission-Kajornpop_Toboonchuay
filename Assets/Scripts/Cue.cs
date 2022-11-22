using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cue : MonoBehaviour
{
    Rigidbody2D _rigidbody;
    float speed;
    Color color;
    [SerializeField] Color noteColorMiss = Color.gray;
    bool isWithinHitRegion;
    NoteHighway highway;

    public static event Action Spawned;
    public event Action Despawned;

    double timeInstantiated;
    public float assignedTime;

    void Awake()
    {
        highway = GetComponentInParent<NoteHighway>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        timeInstantiated = NoteHighwayManager.GetAudioSourceTime();
        StartCoroutine(nameof(Move));
    }
    IEnumerator Move()
    {
        double timeSinceInstantiated = NoteHighwayManager.GetAudioSourceTime() - timeInstantiated;
        float t = (float)(timeSinceInstantiated / (NoteHighwayManager.Instance.noteTime * 2));
        Vector3 start = highway.cueStart;
        Vector3 stop = highway.cueDestination; 
        gameObject.SetActive(true);
        while (true)
        {
            transform.position = Vector3.Lerp(start, stop, t);
            yield return null;
        }
    }

    public void Spawn()
    {
        isWithinHitRegion = false;
        gameObject.SetActive(true);
        Move();
        Spawned?.Invoke();
    }

    public void Despawn()
    {
        isWithinHitRegion = false;
        gameObject.SetActive(false);
        Despawned?.Invoke();
    }

    public void SetNoteSpeed(float noteSpeed)
    {
        speed = noteSpeed;
    }

    public void SetNoteColor(Color noteColor)
    {
        color = noteColor;
        GetComponent<SpriteRenderer>().color = color;
    }

    public void IsWithinHitRegion(bool value)
    {
        isWithinHitRegion = value;
    }

    public void NoteKeyPressed()
    {
        if (isWithinHitRegion)
        {
            Debug.Log("Hit");
        }
        else
        {
            Debug.Log("Miss");
        }
    }
}
