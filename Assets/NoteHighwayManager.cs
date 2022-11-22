using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class NoteHighwayManager : MonoBehaviour
{
    public static NoteHighwayManager Instance;
    public float noteTime;

    public static event Action Starting;

    public static double startTime;

    bool isPlaying;

    [SerializeField] NoteHighway[] tracks;

    [Header("MIDI")]
    public static MidiFile midiFile;
    [SerializeField] string fileLocation;
    public AudioSource audioSource;
    [SerializeField] float songDelayInSeconds;

    public static event Action<Note[]> DataReady;

    void OnEnable()
    {
        Instance = this;
        Starting += StartSong;
        NoteHighway.SongFinished += OnGameStop;
    }

    private void Start()
    {
        ReadFromFile();
    }

    void Update()
    {
        if(!isPlaying && Input.GetKeyDown(KeyCode.Space)) 
        {
            Starting?.Invoke();
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
        isPlaying = true;
        startTime = Time.time;
        Starting?.Invoke();
    }

    void OnGameStop()
    {
        isPlaying= false;
    }

    public void StartSong()
    {
        audioSource.PlayDelayed(songDelayInSeconds);
    }


    public static double GetAudioSourceTime()
    {
        return startTime + (double)Instance.audioSource.timeSamples / Instance.audioSource.clip.frequency;
    }

}
