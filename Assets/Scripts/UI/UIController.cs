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

    public GameSetting gameSetting;
    
    [Header("Gameplay")]
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI timeLeftText;
    [SerializeField] Image timeLeftImage;
    [SerializeField] RectTransform levelImage;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] Slider filledBarUpgrade;
    // [SerializeField] RectTransform fillBarEmpty;

    [Header("Tutorial")]
    [SerializeField] private GameObject objStart;
    [SerializeField] private Transform startInfoPnl;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private GameObject[] tutorPages;
    [SerializeField] private Button playButton;

    [Header("Pause")]
    [SerializeField] Button pauseButton;
    [SerializeField] private GameObject pauseWindow;
    
    [Header("EnterName")]
    [SerializeField] private GameObject objEnterName;
    [SerializeField] private TMP_InputField inputEnterName;
    [SerializeField] private Button btnEnterName;
    
    [Header("Close Curtain")]
    [SerializeField] private GameObject obCloseCurtain;
    [SerializeField] private Transform tCloseCurtain;

    [Header("VFX Message")]
    [SerializeField] GameObject ReadyMsg;
    [SerializeField] GameObject GoMsg;
    [SerializeField] GameObject LevelUpMsg;
    [SerializeField] GameObject FinishMsg;
    [SerializeField] GameObject MarvelousMsg;
    [SerializeField] GameObject SuperMsg;
    [SerializeField] GameObject GreatMsg;
   

    private bool _animateScore = false;
    private double currentScore = 0;
    private double targetScore = 0;
    private double speedScore = 0;
    
    private int _tutorPageIndex = 0;

    private void Start()
    {
        ReadyMsg.SetActive(false);
        GoMsg.SetActive(false);
        LevelUpMsg.SetActive(false);
        FinishMsg.SetActive(false);
        SuperMsg.SetActive(false);
        MarvelousMsg.SetActive(false);
        objStart.gameObject.SetActive(false);
        objEnterName.gameObject.SetActive(false);
        obCloseCurtain.SetActive(false);

        pauseWindow.SetActive(false);
        pauseButton.onClick.AddListener(PauseButtonListener);
        playButton.onClick.AddListener(PlayButtonListener);
        
        btnEnterName.onClick.AddListener(EnterNameButtonListener);
        inputEnterName.onValueChanged.AddListener(InputEnterNameListener);

        timePlaySound = 5;
        
        scoreText.text = currentScore.ToString("N0");

        if (CommonVars.StartGame && !gameSetting.hasShowTutorial)
        {
            StartAnimateSlideStart();
        }
        else
        {
            GameController.Instance.BackFromMenu();
        }
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

    private int timePlaySound = 0;
    private void UpdateTimeText()
    {
        if (GameController.Instance.timePlayLeft <= 5f)
        {
            timeLeftText.color = Color.red;
            if ((GameController.Instance.timePlayLeft < 5f && timePlaySound == 5) ||
                (GameController.Instance.timePlayLeft < 4f && timePlaySound == 4) ||
                (GameController.Instance.timePlayLeft < 3f && timePlaySound == 3) ||
                (GameController.Instance.timePlayLeft < 2d && timePlaySound == 2) ||
                (GameController.Instance.timePlayLeft < 1f && timePlaySound == 1))
            {
                timePlaySound--;
                
                Sequence mySequence = DOTween.Sequence();
                mySequence.Append(timeLeftImage.transform.DOScale(Vector3.one * 1.3f, 0.2f));
                mySequence.Append(timeLeftImage.transform.DOScale(Vector3.one, 0.2f));
                
                SoundController.Instance.PlayCountdownClip();
            }
        }
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
        mySequence.Append(scoreText.transform.DOScale(Vector3.one * 1.2f, 0.2f));
        mySequence.Append(scoreText.transform.DOScale(Vector3.one, 0.2f));
        //mySequence.Play();
    }
    
    private void UpdateBarFill()
    {
        filledBarUpgrade.value = GameController.Instance.curUpgradeScore / GameController.Instance.maxUpgradeScore;
    }

    // public void StartAnimateFillBar()
    // {
    //     Sequence mySequence = DOTween.Sequence();
    //     mySequence.Append(fillBarEmpty.transform.DOScaleY(1.3f, 0.2f));
    //     mySequence.Append(fillBarEmpty.transform.DOScaleY(1f, 0.2f));
    // }
    
    private void UpdateLevelText()
    {
        levelText.text = GameController.Instance.levelScore.ToString();
    }

    public void PauseButtonListener()
    {
        //if (GameController.Instance._gameMode == CommonVars.GameMode.Play ||
        //    GameController.Instance._gameMode == CommonVars.GameMode.Pause)
        //{
            GameController.Instance.PauseGame();
            SoundController.Instance.PlayButtonClip();
        //}
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
        mySequence.Append(ReadyMsg.transform.DOShakeScale(.5f, .5f));
        mySequence.onComplete = () => { ReadyMsg.SetActive(false); };
    }

    public void StartAnimateGo()
    {
        GoMsg.SetActive(true);
        
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(GoMsg.transform.DOShakeScale(.5f, .5f));
        mySequence.onComplete = () => { GoMsg.SetActive(false); };
    }

    public void StartAnimateBonus(int numClip)
    {
        Sequence mySequence = DOTween.Sequence();
        switch (numClip)
        {
            case 0:
                GreatMsg.SetActive(true);
                
                mySequence.AppendInterval(1f);
                mySequence.Append(GreatMsg.transform.DOShakeScale(.5f, .5f));
                mySequence.onComplete = () => { GreatMsg.SetActive(false); };
                break;
            case 1:
                break;
            case 2:
                SuperMsg.SetActive(true);
                
                mySequence.Append(SuperMsg.transform.DOShakeScale(.5f, .5f));
                mySequence.onComplete = () => { SuperMsg.SetActive(false); };
                break;
            default:
                MarvelousMsg.SetActive(true);
                
                mySequence.Append(MarvelousMsg.transform.DOShakeScale(.5f, .5f));
                mySequence.onComplete = () => { MarvelousMsg.SetActive(false); };
                break;
        }
    }

    public void StartAnimateLevelUp()
    {
        LevelUpMsg.SetActive(true);
        
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(LevelUpMsg.transform.DOShakeScale(.5f, .5f));
        mySequence.onComplete = () => { LevelUpMsg.SetActive(false); };
    }

    public void StartAnimateFinish()
    {
        FinishMsg.SetActive(true);
        
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(FinishMsg.transform.DOShakeScale(.5f, .5f));
        mySequence.onComplete = () => { FinishMsg.SetActive(false); };
    }

    public void StartAnimateCurtainClose()
    {
        obCloseCurtain.SetActive(true);
        
        tCloseCurtain.localPosition = new Vector3(0f, 1900f, 0f);
        tCloseCurtain.DOLocalMoveY(0f, 1f).SetEase(Ease.OutElastic);
    }

    public void ShowPauseWindow()
    {
        pauseWindow.SetActive(true);
    }

    private void PlayButtonListener()
    {
        SoundController.Instance.PlayButtonClip();

        _tutorPageIndex++;
        //Debug.Log("tutorl "+_tutorPageIndex);
        
        if (_tutorPageIndex >= tutorPages.Length)
        {
            objStart.gameObject.SetActive(false);
        
            CommonVars.StartGame = false;
            gameSetting.hasShowTutorial = true;
            GameController.Instance.BackFromMenu();
        }
        else
        {
            ShowTutorialPage();
        }
    }

    private void StartAnimateSlideStart()
    {
        objStart.SetActive(true);

        _tutorPageIndex = 0;
        
        startInfoPnl.localPosition += new Vector3(-1500f, 0f, 0f);
        
        highScoreText.text = GameController.Instance.gameSetting.curHighScore.ToString("N0");

        HideTutorialPages();
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(startInfoPnl.transform.DOLocalMoveX(0f, 0.5f).SetEase(Ease.OutBounce));
        mySequence.onComplete = () => ShowTutorialPage();
    }

    private void HideTutorialPages()
    {
        foreach (var page in tutorPages)
        {
            page.SetActive(false);
        }
    }
    private void ShowTutorialPage()
    {
        HideTutorialPages();
        tutorPages[_tutorPageIndex].SetActive(true);
    }

    public void ShowEnterPlayerName()
    {
        objEnterName.gameObject.SetActive(true);
        inputEnterName.text = "";
        btnEnterName.interactable = false;
    }

    private void InputEnterNameListener(string inputName)
    {
        string playerName = inputName.Trim();
        if (playerName.Length > 3) btnEnterName.interactable = true;
    }

    private void EnterNameButtonListener()
    {
        GameController.Instance.gameSetting.curPlayerName = inputEnterName.text.Trim();
        SoundController.Instance.PlayButtonClip();
        objEnterName.gameObject.SetActive(true);
        UnityServiceController.Instance.RenamePlayer(inputEnterName.text.Trim());
        GameController.Instance.CheckSwitchScene();
    }
}