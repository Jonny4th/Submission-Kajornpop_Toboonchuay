using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System;
using TMPro;
using UnityEngine;

public class NoteHighwayManager : MonoBehaviour
{
    public static NoteHighwayManager Instance;
    public float noteTime;
    public static double startTime;
    public bool IsPlaying
    {
        get
        {
            return audioSource.isPlaying;
        }
    }

    [SerializeField] float baseScore;
    [SerializeField] float score;
    [SerializeField] TMP_Text scoreDisplay;

    [SerializeField] NoteHighway[] highways;

    [Header("MIDI")]
    public static MidiFile midiFile;
    [SerializeField] string fileLocation;
    public AudioSource audioSource;
    [SerializeField] float songDelayInSeconds;

    public static event Action<Note[]> DataReady;
    public static event Action Starting;

    void OnEnable()
    {
        Instance = this;
        Starting += StartGame;
        foreach(var highway in highways)
        {
            highway.Scored += UpdateScore;
        }
    }

    private void Awake()
    {
    }

    private void Start()
    {
        ReadFromFile();
    }

    void Update()
    {
        if(!IsPlaying && Input.GetKeyDown(KeyCode.Space)) 
        {
            StartGame();
        }
        if(Input.GetKeyDown(KeyCode.Escape)) 
        {
            Application.Quit();
        }
    }

    void ReadFromFile()
    {
        string path = Application.streamingAssetsPath + "/" + fileLocation;
        midiFile = MidiFile.Read(path);
        GetDataFromMidi();
    }

    void GetDataFromMidi()
    {
        var notes = midiFile.GetNotes();
        var array = new Note[notes.Count];
        notes.CopyTo(array, 0);

        DataReady?.Invoke(array);
        Starting?.Invoke();

    }

    void StartGame()
    {
        startTime = Time.time;
        score = 0;
        UpdateScore(0);
        StartSong();
    }


    public void StartSong()
    {
        audioSource.PlayDelayed(songDelayInSeconds);
    }

    public static double GetAudioSourceTime()
    {
        return (double)Instance.audioSource.timeSamples / Instance.audioSource.clip.frequency;
    }

    void UpdateScore(float add)
    {
        score += add;
        scoreDisplay.text = score.ToString("0");
    }

    void ResetScore()
    {
        score = 0;
        scoreDisplay.text = score.ToString("0");
    }

}
