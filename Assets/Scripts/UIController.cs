using System;
using System.Globalization;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    #region singleton

    public static UIController Instance { get; set; }

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
    
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI timeLeftText;
    [SerializeField] RectTransform levelImage;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] Image filledBarUpgrade;
    [SerializeField] RectTransform fillBarEmpty;

    [SerializeField] Button pauseButton;
    [SerializeField] private GameObject pauseWindow;
    
    [SerializeField] GameObject ReadyMsg;
    [SerializeField] GameObject GoMsg;
    [SerializeField] GameObject LevelUpMsg;
    [SerializeField] GameObject FinishMsg;

    private bool _animateScore = false;
    private double currentScore = 0;
    private double targetScore = 0;
    private double speedScore = 0;

    private void Start()
    {
        ReadyMsg.SetActive(false);
        GoMsg.SetActive(false);
        LevelUpMsg.SetActive(false);
        FinishMsg.SetActive(false);

        pauseWindow.SetActive(false);
        pauseButton.onClick.AddListener(PauseButtonListener);
    }

    private void Update()
    {
        UpdateTimeText();
        
        UpdateBarFill();
        UpdateLevelText();
        if (_animateScore)
        {
            UpdateScoreText();
        }
    }

    private void UpdateTimeText()
    {
        timeLeftText.text = CommonVars.FloatToTimeString(GameController.Instance.timePlayLeft);
    }
    
    private void UpdateScoreText()
    {
        currentScore += speedScore * Time.deltaTime;
        if (currentScore >= targetScore)
        {
            currentScore = targetScore;
            _animateScore = false;
        }
        scoreText.text = currentScore.ToString("N0");
    }

    public void StartAnimateScore()
    {
        _animateScore = true;
        targetScore = GameController.Instance.scorePlay;
        speedScore = (targetScore - currentScore) * 2d;
        
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(scoreText.transform.DOScale(Vector3.one * 1.5f, 0.2f));
        mySequence.Append(scoreText.transform.DOScale(Vector3.one, 0.2f));
        //mySequence.Play();
    }
    
    private void UpdateBarFill()
    {
        filledBarUpgrade.fillAmount = GameController.Instance.curUpgradeScore / GameController.Instance.maxUpgradeScore;
    }

    public void StartAnimateFillBar()
    {
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(fillBarEmpty.transform.DOScaleY(1.3f, 0.2f));
        mySequence.Append(fillBarEmpty.transform.DOScaleY(1f, 0.2f));
    }
    
    private void UpdateLevelText()
    {
        levelText.text = GameController.Instance.levelScore.ToString();
    }

    public void PauseButtonListener()
    {
        if (GameController.Instance._gameMode == CommonVars.GameMode.Play ||
            GameController.Instance._gameMode == CommonVars.GameMode.Pause)
        {
            GameController.Instance.PauseGame();
        }
    }

    public void StartAnimateLevelText()
    {
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(levelImage.transform.DOScale(Vector3.one * 1.5f, 0.2f));
        mySequence.Append(levelImage.transform.DOScale(Vector3.one, 0.2f));
    }

    public void StartAnimateReady()
    {
        ReadyMsg.SetActive(true);
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(ReadyMsg.transform.DOScale(Vector3.one * 1.5f, 0.3f));
        mySequence.Append(ReadyMsg.transform.DOScale(Vector3.one, 0.3f));
        mySequence.onComplete = () => { ReadyMsg.SetActive(false); };
    }

    public void StartAnimateGo()
    {
        GoMsg.SetActive(true);
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(GoMsg.transform.DOScale(Vector3.one * 1.5f, 0.2f));
        mySequence.Append(GoMsg.transform.DOScale(Vector3.one, 0.2f));
        mySequence.onComplete = () => { GoMsg.SetActive(false); };
    }

    public void StartAnimateLevelUp()
    {
        LevelUpMsg.SetActive(true);
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(LevelUpMsg.transform.DOScale(Vector3.one * 1.5f, 0.1f));
        mySequence.Append(LevelUpMsg.transform.DOScale(Vector3.one, 0.1f));
        mySequence.onComplete = () => { LevelUpMsg.SetActive(false); };
    }

    public void StartAnimateFinish()
    {
        FinishMsg.SetActive(true);
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(FinishMsg.transform.DOScale(Vector3.one * 1.5f, 0.3f));
        mySequence.Append(FinishMsg.transform.DOScale(Vector3.one, 0.3f));
        mySequence.onComplete = () => { FinishMsg.SetActive(false); };
    }

    public void ShowPauseWindow()
    {
        pauseWindow.SetActive(true);
    }
}