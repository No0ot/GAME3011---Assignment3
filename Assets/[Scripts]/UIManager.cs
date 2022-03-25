using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TMP_Text timer;
    public TMP_Text score;
    public Slider bar;

    public Popup popupPrefab;

    public Canvas canvas;

    public GameObject losePanel;
    public GameObject winPanel;

    public void UpdateGoalBar(float val)
    {
        bar.value = val;
    }

    public void UpdateTimerText(int newtime)
    {
        int minutes = newtime / 60;
        int seconds = newtime % 60;

        string minText = minutes + "";
        string secText;
        if (seconds < 10)
            secText = "0" + seconds;
        else
            secText = seconds + "";

        timer.text = "Time: " + minText + ":" + secText;
    }

    public void UpdateScoreText(int newScore)
    {
        score.text = "Score: " + newScore;
    }


    public void SpawnPopup(int type, int face, int amount)
    {
        Popup newPopup = Instantiate(popupPrefab, canvas.transform);
        newPopup.type = type;
        newPopup.face = face;
        newPopup.amount = amount;
        newPopup.UpdateReferences();
    }

    public void LoseGame()
    {
        losePanel.SetActive(true);
    }

    public void WinGame()
    {
        winPanel.SetActive(true);
    }
}
