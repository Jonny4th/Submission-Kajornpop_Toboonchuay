using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Melanchall.DryWetMidi.Interaction;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NoteHighway : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] SpawnLine spawnLine;
    [SerializeField] DespawnLine despawnLine;
    [SerializeField] NoteIndicator noteIndicator;
    [SerializeField] GameObject cueStock;

    public Vector3 cueStart
    {
        get
        {
            return spawnLine.transform.position;
        }
    }

    Vector3 ActionPosition
    {
        get
        {
            return noteIndicator.transform.position;
        }
    }

    public Vector3 cueDestination //used in cue's lerb method
    {
        get
        {
            // 2 times the distance from spawn point to indicator bar.
            return ActionPosition - ( cueStart - ActionPosition ) ;
        }
    }

    [Header("ActionKey")]
    [SerializeField] Button trackButton;
    TMP_Text trackButtonDisplay;
    [SerializeField] string trackChar;

    [Header("Cue")]
    [SerializeField] List<Cue> cues;
    [SerializeField] Cue cuePrefab;
    [SerializeField] Color noteColor;
    [SerializeField] FloatValue noteSpeed;

    public event Action NoteSpawning;

    [SerializeField] Melanchall.DryWetMidi.MusicTheory.NoteName noteRestriction;
    [SerializeField] int octaveRestriction;

    //List<Cue> noteList = new List<Cue>();
    List<double> timeStamps = new List<double>();

    int spawnIndex = 0;

    private void Awake()
    {
        trackButton.GetComponent<Image>().color = noteColor;
        trackButtonDisplay = trackButton.GetComponentInChildren<TMP_Text>();
        trackButtonDisplay.text = trackChar.ToUpper();
        //foreach (var cue in cues)
        //{
        //    cue.SetNoteSpeed(noteSpeed.GetValue());
        //    cue.SetNoteColor(noteColor);
        //}
    }

    private void OnEnable()
    {
        NoteHighwayManager.DataReady += SetTimeStamps;
    }

    void Update()
    {
        if (spawnIndex < timeStamps.Count)
        {
            if (NoteHighwayManager.GetAudioSourceTime() >= timeStamps[spawnIndex] - NoteHighwayManager.Instance.noteTime)
            {
                Cue note = cues.Find(x => !x.gameObject.activeSelf);
                if (note == null)
                {
                    note = Instantiate(cuePrefab, spawnLine.transform.position, Quaternion.identity, cueStock.transform);
                    cues.Add(note);
                }
                //noteList.Add(note.GetComponent<Cue>());
                note.GetComponent<Cue>().assignedTime = (float)timeStamps[spawnIndex];
                spawnIndex++;
            }
        }
    }


    void KeyPressing()
    {
        if( Input.GetKeyDown(trackChar) )
        {
            var go = trackButton.gameObject;
            var ped = new PointerEventData(EventSystem.current);
            ExecuteEvents.Execute(go, ped, ExecuteEvents.pointerEnterHandler);
            ExecuteEvents.Execute(go, ped, ExecuteEvents.submitHandler);
        }
    }
    
    public void SpawnNote()
    {
        NoteSpawning?.Invoke();
    }
    void SetTimeStamps(Melanchall.DryWetMidi.Interaction.Note[] array)
    {
        foreach (var note in array)
        {
            if (note.NoteName == noteRestriction && note.Octave == octaveRestriction)
            {
                var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, NoteHighwayManager.midiFile.GetTempoMap());
                timeStamps.Add((double)metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + (double)metricTimeSpan.Milliseconds / 1000f);
            }
        }
    }
}
