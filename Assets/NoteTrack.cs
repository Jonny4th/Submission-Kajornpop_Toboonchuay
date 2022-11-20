using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class NoteTrack : MonoBehaviour
{
    [SerializeField] Button trackButton;
    TMP_Text trackButtonDisplay;
    [SerializeField] char trackChar;
    // Start is called before the first frame update
    private void Awake()
    {
        trackButtonDisplay = trackButton.GetComponentInChildren<TMP_Text>();
        trackButtonDisplay.text = trackChar.ToString().ToUpper();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
}
