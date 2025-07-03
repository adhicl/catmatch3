using System.Collections;
using DG.Tweening;
using Game;
using Google.Play.Review;
using Newtonsoft.Json;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScene : MonoBehaviour
{
    public GameSetting gameSetting;

    [Header("Title Logo")]
    [SerializeField] private GameObject objTitle;
    [SerializeField] private Transform tNeko;
    [SerializeField] private Transform tMatch;
    [SerializeField] private Transform tBlast;
    [SerializeField] private GameObject objNekoCat1;
    [SerializeField] private GameObject objNekoCat2;
    
    [Header("Main Menu")]
    [SerializeField] private GameObject objHighscorePnl;
    [SerializeField] private GameObject objHighscore;
    [SerializeField] private TextMeshProUGUI txtHighscore;
    
    [SerializeField] private Button btnPlay;
    [SerializeField] private Button btnShop;
    [SerializeField] private Button btnLeaderboard;
    [SerializeField] private Button btnNoAd;
    [SerializeField] private Button btnRateMe;

    [SerializeField] private Button btnReset;

    [Header("Leaderboard")]
    [SerializeField] private GameObject objLeaderboard;
    [SerializeField] private LeaderboardPanel[] panelsLeaderboard;
    [SerializeField] private Button btnNextLeaderboard;
    [SerializeField] private LeaderboardPanel rankPanel;
    
    [Header("Custom board")]
    [SerializeField] CustomWindow customWindow;
    
    [Header("Loading")] [SerializeField] private GameObject objLoading;
    
    [Header("Error Message")]
    [SerializeField] private GameObject objError;
    [SerializeField] private TextMeshProUGUI txtMessageError;
    [SerializeField] private TextMeshProUGUI txtButtonMessage;
    [SerializeField] private Button btnNextError;

    [Header("Data")]
    [SerializeField] private AudioSource bgmMusic;

    public bool animateStart = true;
    
    private void Start()
    {
        objTitle.SetActive(false);
        objNekoCat1.SetActive(false);
        objNekoCat2.SetActive(false);
        
        objHighscorePnl.SetActive(false);
        objHighscore.SetActive(false);
        btnPlay.gameObject.SetActive(false);
        btnShop.gameObject.SetActive(false);
        btnLeaderboard.gameObject.SetActive(false);
        btnNoAd.gameObject.SetActive(false);
        btnRateMe.gameObject.SetActive(false);
        customWindow.gameObject.SetActive(false);
        
        objLeaderboard.gameObject.SetActive(false);
        ShowLoading(false);
        
        objError.SetActive(false);
        
        //listener
        btnPlay.onClick.AddListener(GoToPlay);
        btnShop.onClick.AddListener(GoToShop);
        btnLeaderboard.onClick.AddListener(GoToLeaderboard);
        btnNextLeaderboard.onClick.AddListener(CloseLeaderboard);
        btnNoAd.onClick.AddListener(OnNoAdButtonClicked);
        btnRateMe.onClick.AddListener(OnRateMeButtonClicked);
        
        if (CommonVars.AnimateStart)
        {
            CommonVars.AnimateStart = false;
            StartCoroutine(AnimateTitle());
        }
        else
        {
            StartCoroutine(InitTitle());
        }
        
        ChangeBGM();

        UnityServiceController.Instance.dLeaderboardResult += ShowLeaderboard;
        UnityServiceController.Instance.dLeaderboardRankResult += ShowLeaderboardRank;
        UnityServiceController.Instance.dLeaderboardError += ShowErrorMessage;
        
        btnReset.onClick.AddListener(ResetSaveData);
        btnNextError.onClick.AddListener(CloseErrorMessage);
        
        #if UNITY_ANDROID
        _reviewManager = new ReviewManager();
        #endif
    }

    private void OnDestroy()
    {
        UnityServiceController.Instance.dLeaderboardResult -= ShowLeaderboard;
        UnityServiceController.Instance.dLeaderboardRankResult -= ShowLeaderboardRank;
        UnityServiceController.Instance.dLeaderboardError -= ShowErrorMessage;
    }

    private void ChangeBGM()
    {
        bgmMusic.volume = gameSetting.isMusicOn?0.4f:0f;
    }

    private IEnumerator AnimateTitle()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        objTitle.SetActive(true);
        objTitle.transform.position = objTitle.transform.position + new Vector3(0f, 2f, 0f);
        tNeko.localScale = Vector3.zero;
        tMatch.localScale = Vector3.zero;
        tBlast.localScale = Vector3.zero;

        objNekoCat1.transform.position = objNekoCat1.transform.position - new Vector3(0f, 3f, 0f);
        objNekoCat2.transform.position = objNekoCat2.transform.position - new Vector3(0f, 3f, 0f);
        objNekoCat1.SetActive(true);
        objNekoCat2.SetActive(true);
        
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(objTitle.transform.DOMoveY(3.15f, 0.5f).SetEase(Ease.OutBounce));
        //mySequence.Append(objTitle.transform.DOScale(Vector3.one * 1.5f, 0.2f));
        //mySequence.Append(objTitle.transform.DOScale(Vector3.one, 0.2f));
        mySequence.Append(tNeko.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce));
        mySequence.Append(tMatch.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce));
        mySequence.Append(tBlast.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce));
        mySequence.Append(objNekoCat1.transform.DOMoveY(-4.875f, 1f).SetEase(Ease.OutElastic));
        mySequence.Append(objNekoCat2.transform.DOMoveY(-4.57f, 1f).SetEase(Ease.OutElastic));
        
        yield return new WaitForSeconds(4.5f);

        btnPlay.gameObject.SetActive(true);
#if (UNITY_ANDROID || UNITY_IOS)
        //btnNoAd.gameObject.SetActive(true);
        btnRateMe.gameObject.SetActive(true);
#else
        btnShop.gameObject.SetActive(true);
#endif
        btnLeaderboard.gameObject.SetActive(true);
        objHighscorePnl.SetActive(true);
        
        yield return new WaitForSeconds(.5f);

        objHighscore.SetActive(true);
        txtHighscore.text = gameSetting.curHighScore.ToString("N0");
    }

    private IEnumerator InitTitle()
    {
        objTitle.SetActive(true);
        
        btnPlay.gameObject.SetActive(true);
        #if (UNITY_ANDROID || UNITY_IOS)
        //btnNoAd.gameObject.SetActive(true);
        btnRateMe.gameObject.SetActive(true);
        #else
        btnShop.gameObject.SetActive(true);
        #endif
        btnLeaderboard.gameObject.SetActive(true);
        objHighscorePnl.SetActive(true);

        objNekoCat1.SetActive(true);
        objNekoCat2.SetActive(true);
        
        yield return new WaitForSeconds(1f);

        objHighscore.SetActive(true);
        txtHighscore.text = gameSetting.curHighScore.ToString("N0");
    }

    private void GoToPlay()
    {
        SoundController.Instance.PlayButtonClip();
        gameSetting.curScore = 0;
        SceneManager.LoadSceneAsync("GameScene");
    }

    private void GoToShop()
    {
        SoundController.Instance.PlayButtonClip();
        customWindow.gameObject.SetActive(true);
        customWindow.RedrawUI();
    }

    private void GoToLeaderboard()
    {
        SoundController.Instance.PlayButtonClip();
        
        //zShowLeaderboard();
        ShowLoading(true);
        UnityServiceController.Instance.GetPaginatedScores();
    }

    private void ShowLeaderboardRank(string rank)
    {
        var jsonResult = JsonConvert.DeserializeObject<CommonVars.NewLeaderboardResult>(rank);
        rankPanel.SetPanel(jsonResult.rank, jsonResult.playerName, jsonResult.score);
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

    private void CloseLeaderboard()
    {
        SoundController.Instance.PlayButtonClip();
        objLeaderboard.gameObject.SetActive(false);
    }

    private void ShowLoading(bool show)
    {
        objLoading.gameObject.SetActive(show);
    }

    private void ResetSaveData()
    {
        gameSetting.Reset();
        gameSetting.SaveData();
    }

    private void OnNoAdButtonClicked()
    {
        SoundController.Instance.PlayButtonClip();
    }

    private void ShowErrorMessage()
    {
        objError.SetActive(true);
        objLoading.SetActive(false);

        txtMessageError.text = CommonVars.ErrorMessage[0];
        txtButtonMessage.text = "Ok";
        
    }

    private void CloseErrorMessage()
    {
        objError.SetActive(false);
    }

    private ReviewManager _reviewManager;
    private PlayReviewInfo _playReviewInfo;
    
    private void OnRateMeButtonClicked()
    {
        SoundController.Instance.PlayButtonClip();
        StartCoroutine(RequestReview());
    }

    public IEnumerator RequestReview()
    {
        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            yield break;
        }
        _playReviewInfo = requestFlowOperation.GetResult();
            
        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;
        _playReviewInfo = null; // Reset the object
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using launchFlowOperation.Error.ToString().
            yield break;
        }
        // The flow has finished. The API does not indicate whether the user
        // reviewed or not, or even whether the review dialog was shown. Thus, no
        // matter the result, we continue our app flow.
    }
}