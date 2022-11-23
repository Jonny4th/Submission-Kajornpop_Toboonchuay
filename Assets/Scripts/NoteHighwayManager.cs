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

    //[SerializeField] NoteHighway[] highways;

    [Header("MIDI")]
    public static MidiFile midiFile;
    [SerializeField] string fileLocation;
    public AudioSource audioSource;
    public float songDelayInSeconds;
    Note[] chart; 

    public static event Action<Note[]> DataReady;
    public static event Action Starting;

    void OnEnable()
    {
        Instance = this;
        Starting += StartGame;
        var highways = GetComponentsInChildren<NoteHighway>();
        foreach(var highway in highways)
        {
            highway.Scored += UpdateScore;
        }
    }
    private void Start()
    {
        ReadFromFile();
    }

    private void Update()
    {
        if(!IsPlaying)
        {
            if(Input.GetKeyDown(KeyCode.Escape) )
            {
                Application.Quit();
            }
            if(Input.GetKeyDown(KeyCode.Space))
            {
                Starting?.Invoke();
            }
        }
        
    }

    void ReadFromFile()
    {
        string path = Application.streamingAssetsPath + "/" + fileLocation;
        midiFile = MidiFile.Read(path);
        var notes = midiFile.GetNotes();
        chart = new Note[notes.Count];
        notes.CopyTo(chart, 0);
        DataReady?.Invoke(chart);
    }
    void StartGame()
    {
        startTime = Time.time;
        ResetScore();
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
        Debug.Log("Score update");
        score += add;
        scoreDisplay.text = score.ToString("0");
    }
    void ResetScore()
    {
        score = 0;
        scoreDisplay.text = score.ToString("0");
    }
}
