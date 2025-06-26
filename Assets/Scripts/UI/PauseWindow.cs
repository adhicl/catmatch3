using System;
using Game.Ads;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseWindow : MonoBehaviour
{
    [SerializeField] private Button musicButton;
    [SerializeField] private Button soundButton;
    [SerializeField] private Button homeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button continueButton;
    
    public Sprite musicOn;
    public Sprite musicOff;
    public Sprite soundOn;
    public Sprite soundOff;

    [SerializeField] private Image musicButtonImage;
    [SerializeField] private Image soundButtonImage;

    private void Start()
    {
        musicButton.onClick.AddListener(MusicButtonListener);
        soundButton.onClick.AddListener(SoundButtonListener);
        homeButton.onClick.AddListener(HomeButtonListener);
        restartButton.onClick.AddListener(RestartButtonListener);
        continueButton.onClick.AddListener(ContinueButtonListener);

        UpdateSoundMusicSprite();
    }

    private void UpdateSoundMusicSprite()
    {
        musicButtonImage.sprite = GameController.Instance.gameSetting.isMusicOn ? musicOn : musicOff;
        soundButtonImage.sprite = GameController.Instance.gameSetting.isSoundOn ? soundOn : soundOff;
    }

    private void MusicButtonListener()
    {
        GameController.Instance.gameSetting.isMusicOn = !GameController.Instance.gameSetting.isMusicOn;
        SoundController.Instance.PlayButtonClip();
        UpdateSoundMusicSprite();
        GameController.Instance.ChangeBGM();
        GameController.Instance.UpdateSaveData();
    }

    private void SoundButtonListener()
    {
        GameController.Instance.gameSetting.isSoundOn = !GameController.Instance.gameSetting.isSoundOn;
        SoundController.Instance.PlayButtonClip();
        UpdateSoundMusicSprite();
        GameController.Instance.UpdateSaveData();
    }

    private void HomeButtonListener()
    {
        SoundController.Instance.PlayButtonClip();
#if (UNITY_ANDROID || UNITY_IOS)
        NativeAdController.Instance.HideAd();
#endif
        SceneManager.LoadSceneAsync("TitleScene");
    }

    private void RestartButtonListener()
    {
        SoundController.Instance.PlayButtonClip();
#if (UNITY_ANDROID || UNITY_IOS)
        NativeAdController.Instance.HideAd();
#endif
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    private void ContinueButtonListener()
    {
#if (UNITY_ANDROID || UNITY_IOS)
        NativeAdController.Instance.HideAd();
#endif
        this.gameObject.SetActive(false);
        SoundController.Instance.PlayButtonClip();
        GameController.Instance.BackFromMenu();
    }
}