using System;
using System.Collections.Generic;
using TMPro;
using Melanchall.DryWetMidi.Interaction;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class NoteHighway : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] NoteIndicator noteIndicator;
    [SerializeField] GameObject cueStock;

    public Vector3 IndicatorPosition
    {
        get
        {
            return noteIndicator.transform.position;
        }
    }

    [Header("ActionKey")]
    [SerializeField] Button ActionButton;
    TMP_Text ActionButtonDisplay;
    [Tooltip("Letter on the keyboard to be register as action button. Use small case.")]
    public string ActionChar;
    double marginOfError;


    [Header("Cue")]
    [SerializeField] Cue cuePrefab;
    public Color cueColor; // -> Cues
    [SerializeField] Melanchall.DryWetMidi.MusicTheory.NoteName AssociatedNote;
    [SerializeField] int AssociatedNoteOctave;
    public float Speed { get; private set; }

    [Header("Effects")]
    [SerializeField] ParticleSystem hitEffect;

    public List<Cue> cueList = new List<Cue>(); // cues instanciated in each highways.
    public List<double> timeStamps = new List<double>(); // note time stamps from midi file.
    int currentCueIndex; // use for action-cue detection.

    float delay; // (<- HighwayManager) for cue preparing.

    public event Action<float> Scored; // send score to manager.
    public event Action CuePrepared;

    #region Monobehaviours
    private void Awake()
    {
        // Get parameters
        var highwayManager = GetComponentInParent<NoteHighwayManager>();
        delay = highwayManager.songDelay;
        Speed = highwayManager.speed;
        marginOfError = highwayManager.actionMarginOfError;

        //Set UI button
        if(ActionButton!= null)
        {
            ActionButton.GetComponent<Image>().color = cueColor;
            ActionButtonDisplay = ActionButton.GetComponentInChildren<TMP_Text>();
            ActionButtonDisplay.text = ActionChar.ToUpper();
        }
    }

    private void OnEnable()
    {
        //Subscription
        noteIndicator.CueArrived += OnCueArrived;
        noteIndicator.CuePassed += OnCuePassed;
        NoteHighwayManager.DataReady += SetTimeStamps;
        NoteHighwayManager.Starting += PrepareCues;
    }

    private void OnDisable()
    {
        //Unsubscription
        noteIndicator.CueArrived -= OnCueArrived;
        noteIndicator.CuePassed -= OnCuePassed;
        NoteHighwayManager.DataReady -= SetTimeStamps;
        NoteHighwayManager.Starting -= PrepareCues;
    }
    #endregion

    public void PressingActionKey(InputAction.CallbackContext context)
    {
        if(context.performed)
        { 
            //Interact with the button UI.
            var ped = new PointerEventData(EventSystem.current);
            ExecuteEvents.Execute(ActionButton.gameObject, ped, ExecuteEvents.pointerEnterHandler);
            ExecuteEvents.Execute(ActionButton.gameObject, ped, ExecuteEvents.submitHandler);
            
            ExecutingAction();
        }
    }

    public void ExecutingAction()
    {
        if (currentCueIndex < cueList.Count)
        {
            
            //Check if the cue time and the action time is within time window.
            bool isWithinMargin = Math.Abs(timeStamps[currentCueIndex] - NoteHighwayManager.GetAudioSourceTime()) < marginOfError;
            
            //Get current focused cue.
            var cue = cueList[currentCueIndex];

            //Check if the cue is within reactable range.
            bool isWithinActionRange = cue.IsWithinHitRegion;

            if (isWithinActionRange && isWithinMargin)
            {
                cue.OnHit();
                OnCueHit(cue);
            }
        }
    }

    void SetTimeStamps(Note[] array)
    {
        //Get time stamp from Manager.
        foreach (var note in array)
        {
            if (note.NoteName == AssociatedNote && note.Octave == AssociatedNoteOctave)
            {
                var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, NoteHighwayManager.MidiFile.GetTempoMap());
                timeStamps.Add((double)metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + (double)metricTimeSpan.Milliseconds / 1000f);
            }
        }
    }

    void PrepareCues()
    {
        //reset cue's index and list.
        currentCueIndex = 0;
        cueList.Clear();

        //Create cues with time stamp on, and put into list.
        foreach(double timeStamp in timeStamps)
        {
            var startPos = (float)(timeStamp + delay ) * Speed * Vector3.up + IndicatorPosition; 
            Cue cue = Instantiate(cuePrefab, startPos, Quaternion.identity, cueStock.transform);
            cueList.Add(cue);
            cue.SetColor(cueColor);
        }

        CuePrepared?.Invoke();
    }

    void OnCueHit(Cue cue)
    {
        //execute effects
        if(hitEffect!= null)
        {
            _ = Instantiate(hitEffect, noteIndicator.transform.position, Quaternion.identity);
        }

        //send score
        Scored?.Invoke(cue.GetScore());

        OnCuePassed(cue);
    }
    private void UpdateCurrentCueIndex()
    {
        if(currentCueIndex < cueList.Count)
        {
            currentCueIndex++;
        }
    }

    private void OnCueArrived(Cue obj)
    {
    }

    private void OnCuePassed(Cue obj)
    {
        UpdateCurrentCueIndex();
    }
}
