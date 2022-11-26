using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] TMP_Text scoreDisplay;
    float currentScore;

    private void OnEnable()
    {
        NoteHighwayManager.Starting += ResetScore;
        Array.ForEach(GetComponentsInChildren<NoteHighway>(), x => x.Scored += UpdateScore);
    }
    private void OnDisable()
    {
        NoteHighwayManager.Starting -= ResetScore;
        Array.ForEach(GetComponentsInChildren<NoteHighway>(), x => x.Scored -= UpdateScore);
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
