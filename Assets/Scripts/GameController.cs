using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEditor;
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
    
    public CommonVars.GameMode _gameMode;

    private void Start()
    {
        Initialization();
    }

    private void Initialization()
    {
        timePlayLeft = 30f;
        scorePlay = 0d;
        levelScore = 1;
        maxUpgradeScore = 20f;
        curUpgradeScore = 0f;

        ChangeLevelScore();
        
        BackFromMenu();
    }

    private void Update()
    {
        if (_gameMode == CommonVars.GameMode.Play || _gameMode == CommonVars.GameMode.Pause)
        {
            //update time
            if (timePlayLeft > 0)
            {
                timePlayLeft -= Time.deltaTime;
                if (timePlayLeft <= 0)
                {
                    timePlayLeft = 0f;
                    StartCoroutine(FinishGame());
                }
            }

            //update reduce score level
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
        }
    }
    
    #region pause_game

    public void BackFromMenu()
    {
        _gameMode = CommonVars.GameMode.Start;

        StartCoroutine(CountDown());
    }

    private IEnumerator CountDown()
    {
        yield return new WaitForSeconds(0.1f);
        SoundController.Instance.PlayRandomKittenSFX();
        UIController.Instance.StartAnimateReady();
        yield return new WaitForSeconds(1f);
        SoundController.Instance.PlayRandomKittenSFX();
        UIController.Instance.StartAnimateGo();
        _gameMode = CommonVars.GameMode.Play;
    }

    public void PauseGame()
    {
        _gameMode = CommonVars.GameMode.Menu;
        UIController.Instance.ShowPauseWindow();
    }
    
    #endregion
    
    #region finish_game

    private IEnumerator FinishGame()
    {
        _gameMode = CommonVars.GameMode.Finish;
        SoundController.Instance.PlayRandomKittenSFX();
        UIController.Instance.StartAnimateFinish();
        yield return new WaitForSeconds(1f);
        PuzzleController.Instance.DoStartFinish();
    }

    public void ChangeSceneResult()
    {
        Debug.Log("Finish Game");
    }
    
    #endregion
    
    #region update_score
    
    public void UpdateScore(double  score)
    {
        scorePlay += score;
        UIController.Instance.StartAnimateScore();
    }

    public void AddStoneBreak(float totalBreak)
    {
        curUpgradeScore += totalBreak;
        UIController.Instance.StartAnimateFillBar();
        if (curUpgradeScore >= maxUpgradeScore)
        {
            //Debug.Log("Upgrading level "+levelScore);
            levelScore++;
            ChangeLevelScore();
            
            curUpgradeScore = Mathf.Floor(maxUpgradeScore / 2);
            
            SoundController.Instance.PlayRandomKittenSFX();
            UIController.Instance.StartAnimateLevelUp();
            UIController.Instance.StartAnimateLevelText();
        }
    }
    
    private void ChangeLevelScore()
    {
        switch (levelScore)
        {
            case 1: maxUpgradeScore = 10; break;
            case 2: maxUpgradeScore = 20; break;
            case 3: maxUpgradeScore = 40; break;
            case 4: maxUpgradeScore = 50; break;
            case 5: maxUpgradeScore = 70; break;
            case 6: maxUpgradeScore = 90; break;
            case 7: maxUpgradeScore = 110; break;
            case 8: maxUpgradeScore = 130; break;
            case 9: maxUpgradeScore = 150; break;
            case 10: maxUpgradeScore = 175; break;
            default: maxUpgradeScore = 200; break;
        }
    }

    #endregion
    
    #region vfx
    
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

    #endregion
}