using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    Rigidbody2D _rigidbody;
    float speed;
    Color color;
    [SerializeField] Color noteColorMiss = Color.gray;
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void Move()
    {
        _rigidbody.velocity = Vector3.down * speed;
    }
    void Stop()
    {
        _rigidbody.velocity = Vector3.zero;
    }
    public void Spawn()
    {
        gameObject.SetActive(true);
        Move();
    }

    public void Despawn()
    {
        gameObject.SetActive(false);
        Stop();
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

    public void NoteKeyPressed()
    {

    }
}
