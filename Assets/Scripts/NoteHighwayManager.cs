using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class NoteHighwayManager : MonoBehaviour
{
    private static NoteHighwayManager instance;
    public static NoteHighwayManager Instance { get => instance; set => instance = value; }

    [Header("Highway Parameters")]
    [Tooltip("Cue moving speed")]
    public float speed; // -> Highways; -> Cues
    [Tooltip("Action time window in second.")]
    public double actionMarginOfError; // -> Highways

    [Header("UIs")]
    [SerializeField] TMP_Text openingText;

    [Header("MIDI")]
    [SerializeField] string fileLocation;
    [SerializeField] AudioSource audioSource;
    [Tooltip("Time in seconds before the song starts.")]
    public float songDelay; // -> Highways
    Note[] chart;
    public static MidiFile MidiFile { get; private set; }

    bool IsPlaying
    {
        get
        {
            return audioSource.isPlaying;
        }
    }

    public static event Action<Note[]> DataReady;
    public static event Action Starting;

    #region MonoBehaviours
    void OnEnable()
    {
        Instance = this;

        //Subscription
        Starting += StartGame;
    }

    void OnDisable()
    {
        //Unsubscription
        Starting -= StartGame;
    }

    void Start()
    {
        ReadFromFile();
    }

    void Update()
    {
        if(!IsPlaying)
        {
            openingText.gameObject.SetActive(true);
        }
        
    }
    #endregion

    public void QuittingGame(InputAction.CallbackContext context)
    {
        if(context.performed && !IsPlaying)
        {
            //put things to do before quitting game here.
            Application.Quit();
        }
    }

    public void RestartingGame(InputAction.CallbackContext context)
    {
        if(context.performed && !IsPlaying)
            Starting?.Invoke();
    }

    void ReadFromFile()
    {
        string path = Application.streamingAssetsPath + "/" + fileLocation;
        MidiFile = MidiFile.Read(path);
        
        var notes = MidiFile.GetNotes();
        chart = new Note[notes.Count]; // allocate array for note data.
        notes.CopyTo(chart, 0);
        DataReady?.Invoke(chart); // sending data for preparation.
    }

    void StartGame()
    {
        openingText.gameObject.SetActive(false);
        StartSong();
    }

    void StartSong()
    {
        audioSource.PlayDelayed(songDelay);
    }

    public static double GetAudioSourceTime()
    {
        return (double)Instance.audioSource.timeSamples / Instance.audioSource.clip.frequency;
    }
}
