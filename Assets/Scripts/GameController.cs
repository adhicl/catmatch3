using System;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    #region singleton

    public static GameController Instance { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    #endregion

    public float timePlayLeft;
    public double scorePlay;

    public int levelScore;
    private float maxUpgradeScore;
    private float curUpgradeScore;

    private float timerReduceScore;
    
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI timeLeftText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] private Image filledBarUpgrade;

    private void Start()
    {
        Initialization();
    }

    private void Initialization()
    {
        timePlayLeft = 120f;
        scorePlay = 0d;
        levelScore = 1;
        maxUpgradeScore = 20f;
        curUpgradeScore = 0f;
    }

    private void Update()
    {
        if (timePlayLeft > 0)
        {
            timePlayLeft -= Time.deltaTime;
            if (timePlayLeft <= 0) timePlayLeft = 0f;
        }

        if (curUpgradeScore > 0)
        {
            curUpgradeScore -= Time.deltaTime;
            if (curUpgradeScore <= 0)
            {
                if (levelScore > 1) curUpgradeScore = maxUpgradeScore;
                levelScore--;
                if (levelScore < 1) levelScore = 1;
            }
        }
        
        UpdateInterface();
    }

    private void UpdateInterface()
    {
        timeLeftText.text = FloatToTimeString(timePlayLeft);
        filledBarUpgrade.fillAmount = curUpgradeScore / maxUpgradeScore;

        levelText.text = levelScore.ToString();
    }

    private String FloatToTimeString(float time)
    {
        float hour = time / 60;
        float minute = time % 60;
        
        return hour.ToString("00")+":"+minute.ToString("00");
    }

    public void UpdateScore(double  score)
    {
        scorePlay += score;
        Debug.Log("Update score "+scorePlay);
        scoreText.text = scorePlay.ToString("N0");
    }

    public void AddStoneBreak(float totalBreak)
    {
        curUpgradeScore += totalBreak;
        if (curUpgradeScore >= maxUpgradeScore)
        {
            Debug.Log("Upgrading level "+levelScore);
            curUpgradeScore -= maxUpgradeScore;
            levelScore++;
        }
    }
}