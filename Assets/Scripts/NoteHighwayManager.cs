using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor.Experimental.GraphView;
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

    [SerializeField] NoteHighway[] tracks;

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
        Starting += StartSong;
        foreach(var track in tracks)
        {
            track.Scored += UpdateScore;
        }
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

        StartGame();
    }

    void StartGame()
    {
        startTime = Time.time;
        score = 0;
        Starting?.Invoke();
    }


    public void StartSong()
    {
        audioSource.PlayDelayed(songDelayInSeconds);
    }

    public static double GetAudioSourceTime()
    {
        return (double)Instance.audioSource.timeSamples / Instance.audioSource.clip.frequency;
    }

    private void UpdateScore()
    {
        score += baseScore;
        scoreDisplay.text = score.ToString("0");
    }

}
