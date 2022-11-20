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
    [SerializeField] Note note;
    [SerializeField] Color noteColor;
    [SerializeField] FloatValue noteSpeed;

    private void Awake()
    {
        trackButton.GetComponent<Image>().color = noteColor;
        trackButtonDisplay = trackButton.GetComponentInChildren<TMP_Text>();
        trackButtonDisplay.text = trackChar.ToUpper();
        note.SetNoteSpeed(noteSpeed.value);
        note.SetNoteColor(noteColor);
    }

    void FixedUpdate()
    {
        KeyPress();
    }

    public void KeyPress()
    {
        if(Input.GetKeyDown(trackChar))
        {
            var go = trackButton.gameObject;
            var ped = new PointerEventData(EventSystem.current);
            ExecuteEvents.Execute(go, ped, ExecuteEvents.pointerEnterHandler);
            ExecuteEvents.Execute(go, ped, ExecuteEvents.submitHandler);
        }
    }
    
}
