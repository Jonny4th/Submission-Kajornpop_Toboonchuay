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

    public float AssignedTime { get;  set; }
    public float Speed { get; set; }
    public double MarginOfError { get; set; }
    string ActionChar { get; set; }
    public float baseScore;
    public bool IsWithinHitRegion { get; set; }

    void Awake()
    {
        highway = GetComponentInParent<NoteHighway>();
        ActionChar = highway.ActionChar;
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    void OnEnable()
    {
        highway.CuePrepared += OnStart;
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
            _rigidbody.velocity = Vector3.down * Speed;
        }
    }
    public void OnHit()
    {
        IsWithinHitRegion = false;
        _rigidbody.velocity = Vector3.zero;
        transform.position = highway.ActionPosition;
        GetComponent<Animator>().SetTrigger("Hit");
        Destroy(gameObject, 1f);
    }

    public void AssignTime(float time)
    {
        AssignedTime = time;
    }

    public void SetCueColor(Color cueColor)
    {
        GetComponent<SpriteRenderer>().color = cueColor;
    }

}
