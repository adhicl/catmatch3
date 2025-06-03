using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Newtonsoft.Json;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScene : MonoBehaviour
{
    public GameSetting gameSetting;
    [SerializeField] private GameObject objTitle;
    
    [Header("Main Menu")]
    [SerializeField] private GameObject objHighscore;
    [SerializeField] private TextMeshProUGUI txtHighscore;
    
    [SerializeField] private Button btnPlay;
    [SerializeField] private Button btnShop;
    [SerializeField] private Button btnLeaderboard;

    [Header("Leaderboard")]
    [SerializeField] private GameObject objLeaderboard;
    [SerializeField] private LeaderboardPanel[] panelsLeaderboard;
    [SerializeField] private Button btnNextLeaderboard;
    
    [Header("Custom board")]
    [SerializeField] CustomWindow customWindow;
    
    [Header("Loading")] [SerializeField] private GameObject objLoading;

    [Header("Data")]
    [SerializeField] private AudioSource bgmMusic;

    public bool animateStart = true;
    
    private void Start()
    {
        objTitle.SetActive(false);
        objHighscore.SetActive(false);
        btnPlay.gameObject.SetActive(false);
        btnShop.gameObject.SetActive(false);
        btnLeaderboard.gameObject.SetActive(false);
        customWindow.gameObject.SetActive(false);
        
        objLeaderboard.gameObject.SetActive(false);
        ShowLoading(false);
        
        //listener
        btnPlay.onClick.AddListener(GoToPlay);
        btnShop.onClick.AddListener(GoToShop);
        btnLeaderboard.onClick.AddListener(GoToLeaderboard);
        btnNextLeaderboard.onClick.AddListener(CloseLeaderboard);
        
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
    }

    private void OnDestroy()
    {
        UnityServiceController.Instance.dLeaderboardResult -= ShowLeaderboard;
    }

    private void ChangeBGM()
    {
        bgmMusic.volume = gameSetting.isMusicOn?0.4f:0f;
    }

    private IEnumerator AnimateTitle()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        objTitle.SetActive(true);
        
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(objTitle.transform.DOScale(Vector3.one * 1.5f, 0.2f));
        mySequence.Append(objTitle.transform.DOScale(Vector3.one, 0.2f));

        yield return new WaitForSeconds(1f);

        btnPlay.gameObject.SetActive(true);
        btnShop.gameObject.SetActive(true);
        btnLeaderboard.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(1f);

        objHighscore.SetActive(true);
        txtHighscore.text = gameSetting.curHighScore.ToString("N0");
    }

    private IEnumerator InitTitle()
    {
        objTitle.SetActive(true);
        
        btnPlay.gameObject.SetActive(true);
        btnShop.gameObject.SetActive(true);
        btnLeaderboard.gameObject.SetActive(true);
        
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
            panelsLeaderboard[num].SetPanel(fill.rank, fill.playerName, fill.score);
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
}