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
    public UIManager uiManager;

    public Difficulty difficulty = Difficulty.EASY;

    private int score;
    public int Score => score;

    public int goalAmount = 10000;

    float timer;
    public float maxTime;

    public bool isGameOver = false;

    float scoreDecrementTimer = 0.0f;
    float scoreTimerMax = 0.1f;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        uiManager = GetComponent<UIManager>();
        transform.parent.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        timer = maxTime;
        score = 0;
        isGameOver = false;
        uiManager.losePanel.SetActive(false);
        uiManager.winPanel.SetActive(false);
        scoreDecrementTimer = scoreTimerMax;

    }

    // Update is called once per frame
    void Update()
    {
        if (!isGameOver)
        {
            float val = (float)score / (float)goalAmount;
            uiManager.UpdateGoalBar(val);
            uiManager.UpdateScoreText(score);
            uiManager.UpdateTimerText((int)timer);

            if (val >= 1)
            {
                uiManager.WinGame();
                isGameOver = true;
            }
            if (timer > 0)
                timer -= Time.deltaTime;
            else
            {
                uiManager.LoseGame();
                isGameOver = true;
            }
        }

        if (!BoardManager.instance.isShifting)
        {
            if (scoreDecrementTimer > 0)
                scoreDecrementTimer -= Time.deltaTime;
            else
            {
                scoreDecrementTimer = scoreTimerMax;
                IncrementScore(-5);
            }
        }
    }

    public void IncrementScore(int addedscore)
    {
        
        score += addedscore;
        score = Mathf.Clamp(score, 0, int.MaxValue);
        //if (score < 0)
        //    score = 0;
    }

    public void SetDifficulty(int diff)
    {
        difficulty = (Difficulty)diff;
    }
}
