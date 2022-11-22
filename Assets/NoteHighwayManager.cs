using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System;
using System.Linq;
using UnityEngine;

public class NoteHighwayManager : MonoBehaviour
{
    public static NoteHighwayManager Instance;
    public float noteTime;

    public static event Action Starting;

    public static double startTime;

    [SerializeField] NoteHighway[] tracks;

    [Header("MIDI")]
    public static MidiFile midiFile;
    [SerializeField] string fileLocation;
    public AudioSource audioSource;
    [SerializeField] float songDelayInSeconds;

    public static event Action<Melanchall.DryWetMidi.Interaction.Note[]> DataReady;

    void OnEnable()
    {
        Instance = this;
        NoteHighwayManager.Starting += StartGame;
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
        //var start = spawnPos.GetValue();
        //var stop = keyPos.GetValue();
        //var time = TimeDelay.GetValue();
        //var speed = (start - stop) / time;
        //NoteSpeed.SetValue(speed);
    }
    void ReadFromFile()
    {
        midiFile = MidiFile.Read(Application.dataPath + "/" + fileLocation);
        GetDataFromMidi();
    }

    void GetDataFromMidi()
    {
        var notes = midiFile.GetNotes();
        var array = new Melanchall.DryWetMidi.Interaction.Note[notes.Count];
        notes.CopyTo(array, 0);

        DataReady?.Invoke(array);

        //foreach (var track in tracks) track.SetTimeStamps(array);

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
        return startTime + (double)Instance.audioSource.timeSamples / Instance.audioSource.clip.frequency;
    }

}
