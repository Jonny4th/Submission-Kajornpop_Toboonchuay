using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    Rigidbody2D _rigidbody;
    [SerializeField] private float speed = 1;
    [SerializeField] Color noteColor = Color.white;
    [SerializeField] Color noteColorMiss = Color.gray;
    

    // Start is called before the first frame update

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        GetComponent<SpriteRenderer>().color = noteColor;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

}
