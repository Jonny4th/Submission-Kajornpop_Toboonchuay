using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleLife : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<ParticleSystem>().Play();
        Destroy(gameObject,GetComponent<ParticleSystem>().main.duration);
    }
}
