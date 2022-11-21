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
    bool isWithinHitRegion;
    NoteTrack track;

    public static event Action Spawned;
    public event Action Despawned;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        gameObject.SetActive(false);
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
        isWithinHitRegion = false;
        gameObject.SetActive(true);
        Move();
        Spawned?.Invoke();
    }

    public void Despawn()
    {
        
        isWithinHitRegion = false;
        gameObject.SetActive(false);
        Stop();
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
