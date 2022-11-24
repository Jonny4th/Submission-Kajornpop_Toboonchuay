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
    [SerializeField] DespawnLine despawnLine;
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
    [SerializeField] List<Cue> cues = new List<Cue>();
    [SerializeField] Cue cuePrefab;
    public Color cueColor;
    [SerializeField] Melanchall.DryWetMidi.MusicTheory.NoteName AssociatedNote;
    [SerializeField] int AssociatedNoteOctave;

    [Header("Effects")]
    [SerializeField] ParticleSystem hitEffect;

    //List<Cue> noteList = new List<Cue>();
    public List<double> timeStamps = new List<double>();

    float delay;

    public event Action<float> Scored;
    public event Action CuePrepared;

    private void Awake()
    {
        var highwayManager = GetComponentInParent<NoteHighwayManager>();
        delay = highwayManager.songDelayInSeconds;
        speed = highwayManager.speed;
        if(ActionButton!= null)
        {
            ActionButton.GetComponent<Image>().color = cueColor;
            ActionButtonDisplay = ActionButton.GetComponentInChildren<TMP_Text>();
            ActionButtonDisplay.text = ActionChar.ToUpper();
        }
    }

    private void OnEnable()
    {
        NoteHighwayManager.DataReady += SetTimeStamps;
        NoteHighwayManager.Starting += PrepareCues;
    }

    void Update()
    {
        KeyPressing();
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
        foreach(double timeStamp in timeStamps)
        {
            var startPos = (float)(timeStamp + delay ) * speed * Vector3.up + ActionPosition; 
            Cue cue = Instantiate(cuePrefab, startPos, Quaternion.identity, cueStock.transform);
            cue.SetCueColor(cueColor);
            cue.speed = speed;
            cue.assignedTime = (float)timeStamp;
            cue.Hit += OnCueHit;
        }
        CuePrepared?.Invoke();
    }

    void OnCueHit(Cue cue)
    {
        cue.Hit -= OnCueHit;
        hitEffect.Play();
        AddScore(cue.baseScore);
    }

    void AddScore(float scoreAdd)
    {
        Debug.Log("get score.");
        Scored?.Invoke(scoreAdd);
    }
}
