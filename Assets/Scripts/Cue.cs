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

    public float assignedTime { get;  set; }
    public float speed { get; set; }
    string actionChar { get; set; }
    public float baseScore;

    public bool isWithinHitRegion { get; set; }

    public event Action<Cue> Hit;

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
        //if(isWithinHitRegion && Input.GetKeyDown(actionChar))
        if (isWithinHitRegion && Input.GetKeyDown(actionChar) && Math.Abs(assignedTime - NoteHighwayManager.GetAudioSourceTime()) < 0.1)
        {
            isWithinHitRegion = false;
            Debug.Log(assignedTime - NoteHighwayManager.GetAudioSourceTime());
            _rigidbody.velocity = Vector3.zero;
            transform.position = highway.ActionPosition;
            GetComponent<Animator>().SetTrigger("Hit");
            Hit?.Invoke(this);
            Destroy(gameObject, 1f);
        }
    }

    public void Despawn()
    {
        _rigidbody.velocity = Vector3.zero;
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    private void OnStart()
    {
        if(_rigidbody != null)
        {
            _rigidbody.velocity = Vector3.down * speed;
        }
    }

    public void AssignTime(float time)
    {
        assignedTime = time;
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
