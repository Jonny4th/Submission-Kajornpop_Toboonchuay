using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System;
using TMPro;
using UnityEngine;

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
    [SerializeField] TMP_Text scoreDisplay;
    [SerializeField] TMP_Text openingText;
    float currentScore;

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
        Array.ForEach(GetComponentsInChildren<NoteHighway>(), x => x.Scored += UpdateScore);
        Starting += StartGame;
    }

    void OnDisable()
    {
        //Unsubscription
        Array.ForEach(GetComponentsInChildren<NoteHighway>(), x => x.Scored -= UpdateScore);
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
            QuittingGame(KeyCode.Escape);
            RestartingGame(KeyCode.Space);
        }
        
    }
    #endregion

    void QuittingGame(KeyCode trigger)
    {
        if(Input.GetKeyDown(trigger))
        {
            //put things to do before quitting game here.
            Application.Quit();
        }
    }

    void RestartingGame(KeyCode trigger)
    {
        if(Input.GetKeyDown(trigger))
        {
            Starting?.Invoke();
        }
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
        ResetScore();
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

    void UpdateScore(float add)
    {
        currentScore += add;
        scoreDisplay.text = currentScore.ToString("0");
    }
    void ResetScore()
    {
        currentScore = 0;
        scoreDisplay.text = currentScore.ToString("0");
    }
}
