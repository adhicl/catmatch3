using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScene : MonoBehaviour
{
    public GameSetting gameSetting;
    [SerializeField] private GameObject objTitle;
    
    [SerializeField] private GameObject objHighscore;
    [SerializeField] private TextMeshProUGUI txtHighscore;
    
    [SerializeField] private Button btnPlay;
    [SerializeField] private Button btnShop;
    [SerializeField] private Button btnLeaderboard;
    
    [SerializeField] CustomWindow customWindow;

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
        
        //listener
        btnPlay.onClick.AddListener(GoToPlay);
        btnShop.onClick.AddListener(GoToShop);
        btnLeaderboard.onClick.AddListener(GoToLeaderboard);
        
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
    }
}