using Melanchall.DryWetMidi.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Cue : MonoBehaviour
{
    NoteHighway highway;
    Rigidbody2D _rigidbody;
    Color color;
    [SerializeField] Color noteColorMiss = Color.gray;
    Vector3 start;
    Vector3 stop;

    public float assignedTime { get;  set; }
    public float speed;
    string actionChar;
    [SerializeField] float baseValue;

    public bool isWithinHitRegion;

    public event Action Despawned;

    void OnEnable()
    {
        NoteHighwayManager.Starting += OnStart;
        GetComponentInParent<NoteHighway>().CuePrepared += OnStart;
    }

    void Awake()
    {
        highway = GetComponentInParent<NoteHighway>();
        actionChar = highway.ActionChar;
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(isWithinHitRegion && Input.GetKeyDown(actionChar))
        {
            Debug.Log("Hit");
            GetComponent<Animator>().SetTrigger("Hit");
            _rigidbody.velocity = Vector3.zero;
            highway.AddScore(baseValue);
        }
    }

    private void OnStart()
    {
        _rigidbody.velocity = Vector3.down * speed;
        //StartCoroutine(Step());
    }

    public void AssignTime(float time)
    {
        assignedTime = time;
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
        if (collision.TryGetComponent(out NoteIndicator _))
        {
            isWithinHitRegion = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out NoteIndicator _))
        {
            isWithinHitRegion = false;
        }
    }

}
