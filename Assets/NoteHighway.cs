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

    public static event Action SongFinished;

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
    public Color noteColor;

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
    }

    private void OnEnable()
    {
        NoteHighwayManager.DataReady += SetTimeStamps;
        NoteHighwayManager.Starting += OnGameStart;
    }

    void OnGameStart()
    {
        StartCoroutine(RunGame());
    }

    IEnumerator RunGame()
    {
        spawnIndex = 0;
        while (spawnIndex < timeStamps.Count)
        {
            if (NoteHighwayManager.GetAudioSourceTime() >= NoteHighwayManager.startTime + timeStamps[spawnIndex] - NoteHighwayManager.Instance.noteTime)
            {
                Cue cue = cues.Find(x => !x.gameObject.activeSelf);
                if (cue == null)
                {
                    cue = Instantiate(cuePrefab, spawnLine.transform.position, Quaternion.identity, cueStock.transform);
                    cues.Add(cue);
                }
                //noteList.Add(note.GetComponent<Cue>());
                cue.AssignTime((float)timeStamps[spawnIndex]);
                cue.Spawn();
                spawnIndex++;
            }
            yield return null;
        }
        SongFinished?.Invoke();
    }


    
    public void SpawnNote()
    {
        NoteSpawning?.Invoke();
    }

    void SetTimeStamps(Note[] array)
    {
        foreach (var note in array)
        {
            if (note.NoteName == noteRestriction && note.Octave == octaveRestriction)
            {
                var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, NoteHighwayManager.midiFile.GetTempoMap());
                timeStamps.Add(NoteHighwayManager.startTime + (double)metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + (double)metricTimeSpan.Milliseconds / 1000f);
            }
        }
    }
}
