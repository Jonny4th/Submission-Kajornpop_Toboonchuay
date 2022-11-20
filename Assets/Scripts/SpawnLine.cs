using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLine : MonoBehaviour
{
    [SerializeField] Note note;
    // Start is called before the first frame update
    void Start()
    {
        SpawnNote();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnNote()
    {
        note.transform.position = gameObject.transform.position;
        note.Spawn();
    }
}
