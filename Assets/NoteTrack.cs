using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NoteTrack : MonoBehaviour
{
    [Header("Key")]
    [SerializeField] Button trackButton;
    TMP_Text trackButtonDisplay;
    [SerializeField] string trackChar;

    [Header("Note Indicator")]
    [SerializeField] Note[] notes;
    [SerializeField] Color noteColor;
    [SerializeField] FloatValue noteSpeed;

    public event Action NoteSpawning;

    List<Note> activeNoteList;

    private void Awake()
    {
        trackButton.GetComponent<Image>().color = noteColor;
        trackButtonDisplay = trackButton.GetComponentInChildren<TMP_Text>();
        trackButtonDisplay.text = trackChar.ToUpper();
        foreach (var note in notes)
        {
            note.SetNoteSpeed(noteSpeed.GetValue());
            note.SetNoteColor(noteColor);
        }
    }

    void Update()
    {
        KeyPressing();
    }

    void KeyPressing()
    {
        if( Input.GetKeyDown(trackChar) )
        {
            activeNoteList[0].NoteKeyPressed();
            var go = trackButton.gameObject;
            var ped = new PointerEventData(EventSystem.current);
            ExecuteEvents.Execute(go, ped, ExecuteEvents.pointerEnterHandler);
            ExecuteEvents.Execute(go, ped, ExecuteEvents.submitHandler);
        }
    }
    
    public void AddActiveNote(Note note)
    {
        activeNoteList.Add(note);
    }

    public void RemoveActiveNote(Note note)
    {
        activeNoteList.Remove(note);
    }

    public void SpawnNote()
    {
        NoteSpawning?.Invoke();
    }

}
