using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Melanchall.DryWetMidi.Interaction;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using NAudio.Wave;

public class NoteHighway : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] NoteIndicator noteIndicator;
    [SerializeField] GameObject cueStock;

    public Vector3 ActionPosition
    {
        get
        {
            return noteIndicator.transform.position;
        }
    }

    [Header("ActionKey")]
    [SerializeField] Button ActionButton;
    TMP_Text ActionButtonDisplay;
    public string ActionChar;

    [Header("Cue")]
    float speed;
    double marginOfError;
    [SerializeField] Cue cuePrefab;
    public Color cueColor;
    [SerializeField] Melanchall.DryWetMidi.MusicTheory.NoteName AssociatedNote;
    [SerializeField] int AssociatedNoteOctave;

    [Header("Effects")]
    [SerializeField] ParticleSystem hitEffect;

    public List<Cue> cueList = new List<Cue>();
    public List<double> timeStamps = new List<double>();
    int currentCueIndex;

    float delay;

    public event Action<float> Scored;

    public event Action CuePrepared;

    private void Awake()
    {
        var highwayManager = GetComponentInParent<NoteHighwayManager>();
        delay = highwayManager.songDelayInSeconds;
        speed = highwayManager.speed;
        marginOfError = highwayManager.actionMarginOfError;
        if(ActionButton!= null)
        {
            ActionButton.GetComponent<Image>().color = cueColor;
            ActionButtonDisplay = ActionButton.GetComponentInChildren<TMP_Text>();
            ActionButtonDisplay.text = ActionChar.ToUpper();
        }
        
        noteIndicator.CueArrived += OnCueArrived;
        noteIndicator.CuePassed += OnCuePassed;
    }


    private void OnEnable()
    {
        NoteHighwayManager.DataReady += SetTimeStamps;
        NoteHighwayManager.Starting += PrepareCues;
    }

    void Update()
    {
        KeyPressing();
        if (currentCueIndex < cueList.Count)
        {
            if (Input.GetKeyDown(ActionChar) && Math.Abs(timeStamps[currentCueIndex] - NoteHighwayManager.GetAudioSourceTime()) < marginOfError && cueList[currentCueIndex].IsWithinHitRegion)
            {
                var cue = cueList[currentCueIndex];
                cue.OnHit();
                OnCueHit(cue);
            }
        }
    }

    void KeyPressing()
    {
        if( Input.GetKeyDown(ActionChar))
        {
            var ped = new PointerEventData(EventSystem.current);
            ExecuteEvents.Execute(ActionButton.gameObject, ped, ExecuteEvents.pointerEnterHandler);
            ExecuteEvents.Execute(ActionButton.gameObject, ped, ExecuteEvents.submitHandler);
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

    void PrepareCues()
    {
        currentCueIndex = 0;
        cueList.Clear();
        foreach(double timeStamp in timeStamps)
        {
            var startPos = (float)(timeStamp + delay ) * speed * Vector3.up + ActionPosition; 
            Cue cue = Instantiate(cuePrefab, startPos, Quaternion.identity, cueStock.transform);
            cueList.Add(cue);
            cue.SetCueColor(cueColor);
            cue.Speed = speed;
            cue.AssignedTime = (float)timeStamp;
            cue.MarginOfError = marginOfError;
        }
        CuePrepared?.Invoke();
    }

    void OnCueHit(Cue cue)
    {
        if(hitEffect!= null)
        {
            _ = Instantiate(hitEffect, noteIndicator.transform.position, Quaternion.identity);
        }
        Scored?.Invoke(cue.baseScore);
        OnCuePassed(cue);
    }
    private void OnCueArrived(Cue obj)
    {
    }
    private void OnCuePassed(Cue obj)
    {
        UpdateCurrentCueIndex();
    }

    private void UpdateCurrentCueIndex()
    {
        if(currentCueIndex < cueList.Count)
        {
            currentCueIndex++;
        }
    }

}
