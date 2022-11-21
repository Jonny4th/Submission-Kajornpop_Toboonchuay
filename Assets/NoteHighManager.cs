using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System;
using System.Linq;
using UnityEngine;

public class NoteHighManager : MonoBehaviour
{
    public static NoteHighManager instance;
    public float noteTime;

    [Header("Components")]
    [SerializeField] SpawnLine spawnLine;
    [SerializeField] DespawnLine despawnLine;
    [SerializeField] ActionBar actionBar;

    [SerializeField] public FloatValue keyPos;
    [SerializeField] public FloatValue spawnPos;
    public float despawnPos
    {
        get
        {
            return 2*keyPos.GetValue() - spawnPos.GetValue();
        }
    }

    public static event Action Starting;


    [SerializeField] FloatValue TimeDelay;
    [SerializeField] FloatValue NoteSpeed;
    public static double startTime;

    [SerializeField] NoteTrack[] tracks;

    [Header("MIDI")]
    public static MidiFile midiFile;
    [SerializeField] string fileLocation;
    public AudioSource audioSource;
    [SerializeField] float songDelayInSeconds;



    void OnEnable()
    {
        instance = this;
        NoteHighManager.Starting += StartGame;
        SetNoteSpeed();
    }

    private void Start()
    {
        ReadFromFile();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) 
        {
            Starting?.Invoke();
        }
    }

    void SetNoteSpeed()
    {
        var start = spawnPos.GetValue();
        var stop = keyPos.GetValue();
        var time = TimeDelay.GetValue();
        var speed = (start - stop) / time;
        NoteSpeed.SetValue(speed);
    }
    void ReadFromFile()
    {
        midiFile = MidiFile.Read(Application.dataPath + "/" + fileLocation);
        GetDataFromMidi();
    }

    public void GetDataFromMidi()
    {
        var notes = midiFile.GetNotes();
        var array = new Melanchall.DryWetMidi.Interaction.Note[notes.Count];
        notes.CopyTo(array, 0);

        foreach (var track in tracks) track.SetTimeStamps(array);

        StartGame();
    }

    void StartGame()
    {
        startTime = Time.time;
        Invoke(nameof(StartSong), songDelayInSeconds);
    }

    public void StartSong()
    {
        audioSource.Play();
    }
    public static double GetAudioSourceTime()
    {
        return startTime + (double)instance.audioSource.timeSamples / instance.audioSource.clip.frequency;
    }

}
