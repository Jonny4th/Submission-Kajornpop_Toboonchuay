using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyLine : MonoBehaviour
{
    [SerializeField] FloatValue positionY;

    private void Awake()
    {
        positionY.SetValue(transform.position.y);
    }
}
