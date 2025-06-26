using System.Collections;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    public GameSetting gameSetting;
    
    private AsyncOperationHandle m_SceneHandle;

    public AssetReference[] locations;
        
    [SerializeField] private Image m_LoadingSlider;
    [SerializeField] private TextMeshProUGUI tLoadingText;

    void Start()
    {
        CommonVars.StartGame = true;
        StoneFactory.Instance.Ready.AddListener(StoneFactory_Ready);
        
        gameSetting.LoadData();
        UnityServiceController.Instance.dUserSignedIn += OnUserSignedIn;

        StartCoroutine(AnimateLoadingText());
    }

    private void OnSceneLoaded(AsyncOperationHandle obj)
    {
        if(obj.Status == AsyncOperationStatus.Succeeded)
        {
            //GoToNextLevel();
        }
    }

    private void StoneFactory_Ready()
    {
        //Debug.Log("Sign up anonymously");
        UnityServiceController.Instance.SignUpAnonymouslyAsync();
    }

    private void OnUserSignedIn()
    {
        GoToNextLevel();
    }

    private void GoToNextLevel()
    {
        AdMediationController.Instance.StartInitGoogleAd();
        if (gameSetting.hasShowTutorial)
        {
            SceneManager.LoadSceneAsync("TitleScene");
        }
        else
        {
            SceneManager.LoadSceneAsync("GameScene");
        }
    }

    private void Update()
    {
        m_LoadingSlider.fillAmount = StoneFactory.Instance.GetLoadPercentage();
    }
    
    private IEnumerator AnimateLoadingText()
    {
        int dotCount = 0;
        string baseText = "Loading"; 
        while (true)
        {
            // Update the text with the current number of dots
            tLoadingText.text = baseText + new string('.', dotCount);

            // Increment the dot count and reset if it exceeds 3
            dotCount = (dotCount + 1) % 4;

            // Wait for the specified animation speed
            yield return new WaitForSeconds(0.5f);
        }
    }
}