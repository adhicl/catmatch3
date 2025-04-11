using System;
using DG.Tweening;
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
    public float maxUpgradeScore;
    public float curUpgradeScore;

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
                levelScore--;
                if (levelScore < 1) levelScore = 1;
                ChangeLevelScore();
                
                if (levelScore > 1) curUpgradeScore = maxUpgradeScore;
            }
        }
        
        UpdateInterface();
    }

    private void UpdateInterface()
    {
        timeLeftText.text = FloatToTimeString(timePlayLeft);
        //filledBarUpgrade.fillAmount = curUpgradeScore / maxUpgradeScore;
        //levelText.text = levelScore.ToString();
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
        UIController.Instance.StartAnimateScore();
        //scoreText.text = scorePlay.ToString("N0");
    }

    public void AddStoneBreak(float totalBreak)
    {
        curUpgradeScore += totalBreak;
        UIController.Instance.StartAnimateFillBar();
        if (curUpgradeScore >= maxUpgradeScore)
        {
            //Debug.Log("Upgrading level "+levelScore);
            curUpgradeScore -= maxUpgradeScore;
            levelScore++;
            ChangeLevelScore();
            UIController.Instance.StartAnimateLevelText();
        }
    }

    public void CreateSmokeObject(Vector2 start, Vector2 last)
    {
        // add smoke object
        Transform smoke = VFXFactory.Instance.GetSmokeObject();
        smoke.position = start;
        smoke.DOMove(last, 0.2f);
    }

    public void CreateTrailObject(Vector2 start, Vector2 last)
    {
        // add trail object
        Transform trail = VFXFactory.Instance.GetTrailObject();
        trail.position = start;
        trail.DOMove(last, 0.1f);
    }

    private void ChangeLevelScore()
    {
        switch (levelScore)
        {
            case 1: maxUpgradeScore = 20; break;
            case 2: maxUpgradeScore = 40; break;
            case 3: maxUpgradeScore = 50; break;
            case 4: maxUpgradeScore = 70; break;
            case 5: maxUpgradeScore = 90; break;
            case 6: maxUpgradeScore = 110; break;
            case 7: maxUpgradeScore = 130; break;
            case 8: maxUpgradeScore = 150; break;
            case 9: maxUpgradeScore = 175; break;
            case 10: maxUpgradeScore = 200; break;
        }
    }
}