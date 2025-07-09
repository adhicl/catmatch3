using System.Collections;
using DG.Tweening;
using Game.Ads;
using Newtonsoft.Json;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultScene : MonoBehaviour
{
    public GameSetting gameSetting;
    
    [SerializeField] private ScrollRect scrollRect;

    [SerializeField] private GameObject objCurtainClose;
    
    [SerializeField] private Button btnLeaderboard;
    [SerializeField] private Button btnRetry;
    [SerializeField] private Button btnHome;

    [Header("Score")]
    [SerializeField] private GameObject lblScore;
    [SerializeField] private TextMeshProUGUI txtScore;
    
    [SerializeField] private GameObject lblHighScore;
    [SerializeField] private GameObject newRibbonTransform;
    [SerializeField] private TextMeshProUGUI txtHighScore;

    [Header("Reward")] 
    [SerializeField] private GameObject obRewardHolder;
    [SerializeField] private GameObject[] objRewards;
    [SerializeField] private TextMeshProUGUI[] lblRewards;
    [SerializeField] private TextMeshProUGUI[] txtRewards;
    [SerializeField] private Image[] goalRewards;
    [SerializeField] private Slider[] slideRewards;

    [Header("Rank")] 
    [SerializeField] private GameObject objRank;
    [SerializeField] private TextMeshProUGUI txtRank;
    [SerializeField] private Button btnRank;
    [SerializeField] private Button btnShareRank;

    [Header("Leaderboard")]
    [SerializeField] private GameObject objLeaderboard;
    [SerializeField] private LeaderboardPanel[] panelsLeaderboard;
    [SerializeField] private Button btnNextLeaderboard;
    [SerializeField] private LeaderboardPanel playerRank;

    [Header("Unlock")]
    [SerializeField] private GameObject objUnlock;
    [SerializeField] private Image imgUnlockCat;
    [SerializeField] private GameObject lblUnlock;
    [SerializeField] private Button btnNextUnlock;
    
    [Header("Error Message")]
    [SerializeField] private GameObject objError;
    [SerializeField] private TextMeshProUGUI txtMessageError;
    [SerializeField] private TextMeshProUGUI txtButtonMessage;
    [SerializeField] private Button btnNextError;

    [Header("VFX")]
    [SerializeField] private ParticleSystem confettiL;
    [SerializeField] private ParticleSystem confettiR;
    [SerializeField] private AudioSource bgmMusic;

    [Header("Loading")] [SerializeField] private GameObject objLoading;

    private void Start()
    {
        InitUI();
        ChangeBGM();
        StartCoroutine(AnimateScore());
    }

    private void ChangeBGM()
    {
        bgmMusic.volume = gameSetting.isMusicOn?0.4f:0f;
    }

    private void InitUI()
    {
        lblScore.gameObject.SetActive(false);
        lblHighScore.gameObject.SetActive(false);
        newRibbonTransform.gameObject.SetActive(false);

        obRewardHolder.SetActive(false);
        foreach (var t in objRewards)
        {
            t.SetActive(false);
        }
        
        btnHome.gameObject.SetActive(false);
        btnRetry.gameObject.SetActive(false);
        btnLeaderboard.gameObject.SetActive(false);
        
        //unlock 
        objUnlock.SetActive(false);
        lblUnlock.gameObject.SetActive(false);
        btnNextUnlock.gameObject.SetActive(false);
        imgUnlockCat.gameObject.SetActive(false);

        //rank
        objRank.gameObject.SetActive(false);
        
        //leaderboard
        objLeaderboard.SetActive(false);
        
        //loading
        objLoading.gameObject.SetActive(false);
        
        //error
        objError.SetActive(false);
        
        //on click listener
        btnHome.onClick.AddListener(GoToTitle);
        btnLeaderboard.onClick.AddListener(GoToLeaderboard);
        btnRetry.onClick.AddListener(RetryGame);
        btnNextUnlock.onClick.AddListener(GoToNextUnlock);
        btnRank.onClick.AddListener(ShowLeaderboardNext);
        btnNextLeaderboard.onClick.AddListener(ContinueToReward);
        btnNextError.onClick.AddListener(CloseErrorMessage);
        btnShareRank.onClick.AddListener(ShareRank);

        UnityServiceController.Instance.dLeaderboardResult += ShowLeaderboard;
        UnityServiceController.Instance.dLeaderboardRankResult += ShowLeaderboardRank;
        UnityServiceController.Instance.dLeaderboardSentResult += ShowRank;
        UnityServiceController.Instance.dLeaderboardError += ShowErrorMessage;
    }

    private void OnDestroy()
    {
        UnityServiceController.Instance.dLeaderboardResult -= ShowLeaderboard;
        UnityServiceController.Instance.dLeaderboardRankResult -= ShowLeaderboardRank;
        UnityServiceController.Instance.dLeaderboardSentResult -= ShowRank;
        UnityServiceController.Instance.dLeaderboardError -= ShowErrorMessage;
    }

    private void Update()
    {
        if (isDisplayScore)
        {
            _timeScore += Time.deltaTime;
            txtScore.text = Mathf.Lerp(_displayScore, (float) gameSetting.curScore, _timeScore).ToString("N0");
            if (_timeScore >= 1f) isDisplayScore = false;
        }
        
        if (isDisplayReward)
        {
            _timeReward += Time.deltaTime;
            _displayReward = Mathf.Lerp(_startReward, (float)_targetRewards[_posReward], _timeReward);
            
            txtRewards[_posReward].text =
                _displayReward.ToString("N0") + " / " + _maxRewards[_posReward].ToString("N0");
            slideRewards[_posReward].value = (float) _displayReward / _maxRewards[_posReward];
            if (_timeReward >= 1f)
            {
                _timeReward = 0f;

                if (_displayReward >= _maxRewards[_posReward])
                {
                    Sequence mySequence = DOTween.Sequence();
                    mySequence.Append(goalRewards[_posReward].transform.DOScale(2f, 0.2f));
                    mySequence.Append(goalRewards[_posReward].transform.DOScale(1f, 0.2f));
                    hasRewardUnlock = true;
                
                    int unlockType = _posReward + 1;
                    if (unlockType == (int)CommonVars.UnlockType.totalScore)
                    {
                        gameSetting.countAccumulatedClear++;
                    }
                    else if (unlockType == (int)CommonVars.UnlockType.highScore)
                    {
                        gameSetting.countHighscore++;
                    }
                    else if (unlockType == (int)CommonVars.UnlockType.dailyBonus)
                    {
                        gameSetting.countFinishDaily++;
                    }
                    
                    SoundController.Instance.PlayChestClip();
                }
                
                isDisplayReward = false;
            }
        }
    }

    private bool isDisplayScore = false;
    private float _displayScore = 0f;
    private float _timeScore = 0f;
    
    private bool isDisplayReward = false;
    private float _startReward = 0f;
    private float _displayReward = 0f;
    private float _timeReward = 0f;
    private int _posReward = 0;

    private string[] _rewards = {"Accumulated Points", "High Score", "Daily Play"};
    private float[] _targetRewards = {265000, 200000, 5};
    private float[] _maxRewards = {500000, 200000, 7};

    private bool hasRewardUnlock = false;
    private IEnumerator AnimateScore()
    {
        objCurtainClose.SetActive(true);
        Sequence curtainSequence = DOTween.Sequence();
        curtainSequence.Append(objCurtainClose.transform.DOLocalMoveY(1900f, 1f));
        curtainSequence.onComplete = () => { objCurtainClose.SetActive(false); };
        yield return new WaitForSeconds(1f);
        
        bool hasNewHighScore = false;
        hasRewardUnlock = false;
        
        PlayConfettiVFX();
        SoundController.Instance.PlayFillClip();
        
        lblScore.gameObject.SetActive(true);
        isDisplayScore = true;
        
        yield return new WaitForSeconds(1f);

        lblHighScore.gameObject.SetActive(true);
        txtHighScore.text = gameSetting.curHighScore.ToString("N0");
        
        float prevHighscore = gameSetting.curHighScore;
        
        SoundController.Instance.PlayFillClip();
        if (gameSetting.curScore > gameSetting.curHighScore)
        {
            newRibbonTransform.SetActive(true);
            gameSetting.curHighScore = gameSetting.curScore;
            txtHighScore.text = gameSetting.curHighScore.ToString("N0");
        
            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(txtHighScore.transform.DOScale(1.3f, 0.2f));
            mySequence.Append(txtHighScore.transform.DOScale(1f, 0.2f));

            hasNewHighScore = true;
            
            yield return new WaitForSeconds(0.5f);
        }
            
        yield return new WaitForSeconds(0.5f);
        
        /*
        obRewardHolder.SetActive(true);
        int totalRewardAdd = 3;
        for (int i = 0; i < totalRewardAdd; i++)
        {
            _posReward = i;

            objRewards[i].SetActive(true);
            int unlockType = (i + 1);
            if (unlockType == (int) CommonVars.UnlockType.totalScore)
            {
                _startReward = _displayReward = gameSetting.curAccumulatedScore;
                gameSetting.curAccumulatedScore += (float) gameSetting.curScore;
                _targetRewards[i] = gameSetting.curAccumulatedScore;

                int count = gameSetting.countAccumulatedClear;
                if (count >= CommonVars.AccumulatedScore.Length) count = CommonVars.AccumulatedScore.Length - 1;
                _maxRewards[i] = CommonVars.AccumulatedScore[count];
            }
            else if (unlockType == (int) CommonVars.UnlockType.highScore)
            {
                _startReward = _displayReward = prevHighscore;
                _targetRewards[i] = gameSetting.curHighScore;
                
                int count = gameSetting.countHighscore;
                if (count >= CommonVars.HighScoreTarget.Length) count = CommonVars.HighScoreTarget.Length - 1;
                _maxRewards[i] = CommonVars.HighScoreTarget[count];
            }
            else if (unlockType == (int) CommonVars.UnlockType.dailyBonus)
            {
                _startReward = _displayReward = gameSetting.dailyLoginBonus;
                //will check if day change

                _targetRewards[i] = 1;
                _maxRewards[i] = 7;
            }
            
            lblRewards[i].text = _rewards[i];
            goalRewards[i].sprite = StoneFactory.Instance.GetCatSprite(i + 3);
            txtRewards[i].text = _displayReward.ToString("N0")+" / "+_maxRewards[i].ToString("N0");
            slideRewards[i].value = (float) _displayReward / _maxRewards[i];
            
            isDisplayReward = true;
            _timeReward = 0f;
            
            Canvas.ForceUpdateCanvases();
            scrollRect.content.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical() ;
            scrollRect.content.GetComponent<ContentSizeFitter>().SetLayoutVertical() ;
            scrollRect.verticalNormalizedPosition = 0;
            
            SoundController.Instance.PlayFillClip();
            
            yield return new WaitForSeconds(1.5f);
        }
        
        //*/
        
        gameSetting.SaveData();

        //hasNewHighScore = true;
        if (hasNewHighScore)
        {
            //update high score to leaderboard
            UnityServiceController.Instance.AddScore(gameSetting.curHighScore);
            ShowLoading(true);
        }
        else
        {
            StartCoroutine(AnimateReward());
        }
    }

    private IEnumerator AnimateReward()
    {
        if (hasRewardUnlock)
        {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(AnimateUnlock());
        }
        else
        {
            StartCoroutine(AnimateButton());
        }
    }

    private IEnumerator AnimateButton()
    {
        btnLeaderboard.gameObject.SetActive(true);
        btnRetry.gameObject.SetActive(true);
        btnHome.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(0f);
    }

    private IEnumerator AnimateUnlock()
    {
        objUnlock.SetActive(true);
        SoundController.Instance.PlayUnlockClip();

        lblUnlock.gameObject.SetActive(false);
        btnNextUnlock.gameObject.SetActive(false);
        imgUnlockCat.gameObject.SetActive(false);

        Vector3 firstPosition = imgUnlockCat.transform.position;

        imgUnlockCat.sprite = StoneFactory.Instance.GetCatSprite(4);
        
        imgUnlockCat.gameObject.SetActive(true);
        imgUnlockCat.transform.position = firstPosition + new Vector3(0f, 100f, 0f);
        imgUnlockCat.transform.DOMoveY(firstPosition.y, 0.5f).SetEase(Ease.OutBounce);

        yield return new WaitForSeconds(0.5f);

        PlayConfettiVFX();
        lblUnlock.gameObject.SetActive(true);
        
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(lblUnlock.transform.DOScale(1.4f, 0.2f));
        mySequence.Append(lblUnlock.transform.DOScale(1f, 0.2f));
        
        btnNextUnlock.gameObject.SetActive(true);
        
        gameSetting.SaveData();
    }

    private void ShowLoading(bool show)
    {
        objLoading.gameObject.SetActive(show);
    }

    private void PlayConfettiVFX()
    {
        confettiL.Play();
        confettiR.Play();
    }

    private void GoToTitle()
    {
        SoundController.Instance.PlayButtonClip();
        
        gameSetting.curScore = 0f;
        
        gameSetting.SaveData();

#if (UNITY_ANDROID || UNITY_IOS)
        if (CommonVars.resultWithoutAd == 0)
        {
            ShowLoading(true);
            InterstitialAdController.Instance.dShowInterstitialAdFinish += ContinueLoadToTitle;
            InterstitialAdController.Instance.ShowInterstitialAd();
        }
        
        AddResultWithoutAd();
#endif
    }

    private void GoToLeaderboard()
    {
        SoundController.Instance.PlayButtonClip();
        
        //zShowLeaderboard();
        ShowLoading(true);
        UnityServiceController.Instance.GetPaginatedScores();
    }

    private void ShowLeaderboardNext()
    {
        SoundController.Instance.PlayButtonClip();
        
        objRank.gameObject.SetActive(false);
        //zShowLeaderboard();
        ShowLoading(true);
        UnityServiceController.Instance.GetPaginatedScores();
        
    }

    private void GoToNextUnlock()
    {
        SoundController.Instance.PlayButtonClip();
        
        objUnlock.SetActive(false);
        hasRewardUnlock = false;
        StartCoroutine(AnimateButton());
    }

    private void ShowLeaderboardRank(string rank)
    {
        var jsonResult = JsonConvert.DeserializeObject<CommonVars.NewLeaderboardResult>(rank);
        playerRank.SetPanel(jsonResult.rank, jsonResult.playerName, jsonResult.score);
    }

    private void ShowLeaderboard(string result)
    {
        ShowLoading(false);
        objLeaderboard.SetActive(true);
        foreach (var leaderboard in panelsLeaderboard)
        {
            leaderboard.gameObject.SetActive(false);
        }

        int num = 0;
        var jsonResult = JsonConvert.DeserializeObject<CommonVars.LeaderboardResult>(result);
        //Debug.Log(jsonResult.results.Length);
        foreach (var fill in jsonResult.results)
        {
            panelsLeaderboard[num].SetPanel(fill.rank, fill.playerName, fill.score, num);
            panelsLeaderboard[num].gameObject.SetActive(true);
            num++;
        }
    }

    private void ShowRank(string result)
    {
        SoundController.Instance.PlayUnlockClip();
        PlayConfettiVFX();
        ShowLoading(false);

        var jsonResult = JsonConvert.DeserializeObject<CommonVars.NewLeaderboardResult>(result);
        //Debug.Log(jsonResult.playerName);
        //Debug.Log(jsonResult.rank);
        objRank.SetActive(true);
        txtRank.text = (jsonResult.rank + 1).ToString();
    }

    private void ContinueToReward()
    {
        SoundController.Instance.PlayButtonClip();
        objLeaderboard.gameObject.SetActive(false);
        StartCoroutine(AnimateReward());
    }

    private void RetryGame()
    {
        SoundController.Instance.PlayButtonClip();
        gameSetting.curScore = 0f;
        
        gameSetting.SaveData();
        
#if (UNITY_ANDROID || UNITY_IOS)
        if (CommonVars.resultWithoutAd == 0)
        {
            ShowLoading(true);
            InterstitialAdController.Instance.dShowInterstitialAdFinish += ContinueLoadToGame;
            InterstitialAdController.Instance.ShowInterstitialAd();
        }

        AddResultWithoutAd();
#endif
    }

#if (UNITY_ANDROID || UNITY_IOS)
    private void AddResultWithoutAd()
    {
        CommonVars.resultWithoutAd++;
        if (CommonVars.resultWithoutAd == 2) CommonVars.resultWithoutAd = 0;
    }

    private void ContinueLoadToTitle()
    {
        Debug.Log("Load to title");
        InterstitialAdController.Instance.dShowInterstitialAdFinish -= ContinueLoadToTitle;
        SceneManager.LoadSceneAsync("TitleScene");
    }

    private void ContinueLoadToGame()
    {
        Debug.Log("Load to game");
        InterstitialAdController.Instance.dShowInterstitialAdFinish -= ContinueLoadToGame;
        SceneManager.LoadSceneAsync("GameScene");
    }
#endif
    
    private void ShowErrorMessage()
    {
        ShowLoading(false);
        objError.SetActive(true);

        txtMessageError.text = CommonVars.ErrorMessage[0];
        txtButtonMessage.text = "Ok";
    }

    private void CloseErrorMessage()
    {
        objError.SetActive(false);
    }

    private void ShareRank()
    {
        
    }
}