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
    [SerializeField] List<Cue> cues = new List<Cue>();
    [SerializeField] Cue cuePrefab;
    public Color cueColor;
    [SerializeField] Melanchall.DryWetMidi.MusicTheory.NoteName AssociatedNote;
    [SerializeField] int AssociatedNoteOctave;

    //List<Cue> noteList = new List<Cue>();
    public List<double> timeStamps = new List<double>();

    int spawnIndex = 0;
    int cueIndex = 0;

    public event Action Scored;

    private void Awake()
    {
        trackButton.GetComponent<Image>().color = cueColor;
        trackButtonDisplay = trackButton.GetComponentInChildren<TMP_Text>();
        trackButtonDisplay.text = trackChar.ToUpper();
        noteIndicator.CueArrived += OnCueArrived;
        noteIndicator.CuePassed += OnCuePassed;
        foreach (Cue c in cues) { c.SetCueColor(cueColor); }
    }


    private void OnEnable()
    {
        NoteHighwayManager.DataReady += SetTimeStamps;
        NoteHighwayManager.Starting += OnGameStart;
    }

    void Update()
    {
        KeyPressing();
    }

    void OnGameStart()
    {
        StartCoroutine(RunGame());
    }

    IEnumerator RunGame()
    {
        spawnIndex = 0;
        cueIndex = 0;
        while (spawnIndex < timeStamps.Count)
        {
            if (NoteHighwayManager.GetAudioSourceTime() >= timeStamps[spawnIndex] - (double)NoteHighwayManager.Instance.noteTime)
            {
                Cue cue = cues.Find(x => !x.gameObject.activeSelf);
                if (cue == null)
                {
                    cue = Instantiate(cuePrefab, spawnLine.transform.position, Quaternion.identity, cueStock.transform);
                    cue.SetCueColor(cueColor);
                    cues.Add(cue);
                }
                //noteList.Add(note.GetComponent<Cue>());
                cue.AssignTime((float)timeStamps[spawnIndex]);
                cue.Spawn();
                spawnIndex++;
            }
            yield return null;
        }
    }

    void KeyPressing()
    {
        if( Input.GetKeyDown(trackChar) )
        {
            
            if (cueIndex < timeStamps.Count && timeStamps[cueIndex] - NoteHighwayManager.GetAudioSourceTime() < .5 )
            {
                Scored?.Invoke();
            }
            var ped = new PointerEventData(EventSystem.current);
            ExecuteEvents.Execute(trackButton.gameObject, ped, ExecuteEvents.pointerEnterHandler);
            ExecuteEvents.Execute(trackButton.gameObject, ped, ExecuteEvents.submitHandler);
        }
    }

    void SetTimeStamps(Note[] array)
    {
        foreach (var note in array)
        {
            if (note.NoteName == AssociatedNote && note.Octave == AssociatedNoteOctave)
            {
                var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, NoteHighwayManager.midiFile.GetTempoMap());
                timeStamps.Add((double)metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + (double)metricTimeSpan.Milliseconds / 1000f);
            }
        }
    }
    private void OnCueArrived(Cue obj)
    {
        if(Math.Abs( NoteHighwayManager.GetAudioSourceTime() - timeStamps[cueIndex]) < 1.0)
        {
            Debug.Log(obj.assignedTime);
        }
    }

    private void OnCuePassed(Cue obj)
    {
        cueIndex++;
    }
}
