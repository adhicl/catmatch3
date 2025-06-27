using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    public GameSetting gameSetting;

    public AssetReference[] locations;
    
    [Header("Error Message")]
    [SerializeField] private GameObject objError;
    [SerializeField] private TextMeshProUGUI txtMessageError;
    [SerializeField] private TextMeshProUGUI txtButtonMessage;
    [SerializeField] private Button btnNextError;
        
    [Header("loading")] 
    [SerializeField] private GameObject objLoading;
    [SerializeField] private Image m_LoadingSlider;
    [SerializeField] private TextMeshProUGUI tLoadingText;

    private AsyncOperationHandle m_SceneHandle;

    void Start()
    {
        objLoading.SetActive(true);
        objError.SetActive(false);
        
        CommonVars.StartGame = true;
        StoneFactory.Instance.Ready.AddListener(StoneFactory_Ready);
        StoneFactory.Instance.StoneErrorDownload.AddListener(CannotDownload);
        
        gameSetting.LoadData();
        UnityServiceController.Instance.dUserSignedIn += OnUserSignedIn;
        UnityServiceController.Instance.dUserCannotSignIn += FailSignIn;

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

    private void CannotDownload()
    {
        ShowErrorMessage(CommonVars.ErrorMessage[0], "Ok");
        btnNextError.onClick.RemoveAllListeners();
        btnNextError.onClick.AddListener(CloseApp);
    }

    private void FailSignIn()
    {
        ShowErrorMessage(CommonVars.ErrorMessage[0], "Retry");
        btnNextError.onClick.RemoveAllListeners();
        btnNextError.onClick.AddListener(RetrySignIn);
    }

    private void ShowErrorMessage(string errorMessage, string buttonMessage)
    {
        objError.SetActive(true);
        objLoading.SetActive(false);
        
        txtMessageError.text = errorMessage;
        txtButtonMessage.text = buttonMessage;
    }

    private void CloseApp()
    {
        Application.Quit();
    }

    private void RetrySignIn()
    {
        UnityServiceController.Instance.SignUpAnonymouslyAsync();
    }
}