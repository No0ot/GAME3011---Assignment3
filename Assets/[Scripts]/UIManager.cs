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

}
