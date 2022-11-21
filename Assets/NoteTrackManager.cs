using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteTrackManager : MonoBehaviour
{
    [SerializeField] FloatValue keyPos;
    [SerializeField] FloatValue spawnPos;
    [SerializeField] FloatValue TimeDelay;
    [SerializeField] FloatValue NoteSpeed;

    [SerializeField] NoteTrack[] tracks;

    void OnEnable()
    {
        SetNoteSpeed();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) 
        {
            int n = Random.Range(0, tracks.Length);
            tracks[n].SpawnNote();
        }
        
    }

    void SetNoteSpeed()
    {
        var start = spawnPos.GetValue();
        var stop = keyPos.GetValue();
        var time = TimeDelay.GetValue();
        var speed = (start - stop) / time;
        NoteSpeed.SetValue(speed);
    }

}
