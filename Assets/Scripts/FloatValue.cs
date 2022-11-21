using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FloatValue", menuName = "Float Value")]
public class FloatValue : ScriptableObject
{
    [SerializeField] float value;

    public void SetValue(float value)
    { 
        this.value = value; 
    }

    public float GetValue()
    {
        return this.value;
    }
}
