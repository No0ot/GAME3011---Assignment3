using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Difficulty
{
    VERY_HARD = 1,
    HARD,
    MEDIUM,
    EASY
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    UIManager uiManager;

    public Difficulty difficulty = Difficulty.EASY;

    private int score;
    public int Score => score;

    public int goalAmount = 10000;

    float timer;
    public float maxTime;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        uiManager = GetComponent<UIManager>();
        timer = maxTime;
    }

    // Update is called once per frame
    void Update()
    {
        float val = (float)score / (float)goalAmount;
        uiManager.UpdateGoalBar(val);
        uiManager.UpdateScoreText(score);
        uiManager.UpdateTimerText((int)timer);
        if(timer > 0)
            timer -= Time.deltaTime;
    }

    public void IncrementScore(int addedscore)
    {
        score += addedscore;
    }
}
