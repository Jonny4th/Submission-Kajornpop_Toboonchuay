using Melanchall.DryWetMidi.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cue : MonoBehaviour
{
    Rigidbody2D _rigidbody;
    Color color;
    [SerializeField] Color noteColorMiss = Color.gray;
    public bool isWithinHitRegion { get; private set; }
    Vector3 start;
    Vector3 stop;

    public static event Action Spawned;
    public event Action Despawned;

    double timeInstantiated;
    public float assignedTime;

    public float speed;
    float t;
    double timeSinceInstantiated;

    private void OnEnable()
    {
        NoteHighwayManager.Starting += OnStart;
    }

    private void OnStart()
    {
        _rigidbody.velocity = Vector3.down * speed;
    }

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        var highway = GetComponentInParent<NoteHighway>();
        start = highway.cueStart;
        stop = highway.cueDestination;
    }

    private void Start()
    {
    }


    IEnumerator Move()
    {
        while (Vector3.Distance(transform.position,stop) > 0.01f)
        {
            //t = (float)(timeSinceInstantiated / (NoteHighwayManager.Instance.noteTime * 2));
            t = (float)((NoteHighwayManager.GetAudioSourceTime() - timeInstantiated) / 2/(assignedTime - timeInstantiated));
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
        t = (float)((NoteHighwayManager.GetAudioSourceTime() - timeInstantiated) / (assignedTime - timeInstantiated) * 2);
        //timeSinceInstantiated = NoteHighwayManager.GetAudioSourceTime() - timeInstantiated;
        //t = (float)(timeSinceInstantiated / (NoteHighwayManager.Instance.noteTime * 2));
        transform.position = Vector3.Lerp(start, stop, t);
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

    public void SetCueColor(Color cueColor)
    {
        GetComponent<SpriteRenderer>().color = cueColor;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(assignedTime);
        if (collision.TryGetComponent(out NoteIndicator _))
        {
            isWithinHitRegion = true;
        }
    }

}
