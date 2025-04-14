using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PauseWindow : MonoBehaviour
{
    [SerializeField] private Button musicButton;
    [SerializeField] private Button soundButton;
    [SerializeField] private Button homeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button continueButton;

    private void Start()
    {
        musicButton.onClick.AddListener(MusicButtonListener);
        soundButton.onClick.AddListener(SoundButtonListener);
        homeButton.onClick.AddListener(HomeButtonListener);
        restartButton.onClick.AddListener(RestartButtonListener);
        continueButton.onClick.AddListener(ContinueButtonListener);
    }

    public void MusicButtonListener()
    {
        
    }

    public void SoundButtonListener()
    {
        
    }

    public void HomeButtonListener()
    {
        
    }

    public void RestartButtonListener()
    {
        
    }

    public void ContinueButtonListener()
    {
        this.gameObject.SetActive(false);
        GameController.Instance.BackFromMenu();
    }
}