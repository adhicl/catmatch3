using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    public GameSetting gameSetting;
    
    private AsyncOperationHandle m_SceneHandle;

    public AssetReference[] locations;
        
    [SerializeField] private Slider m_LoadingSlider;

    void Start()
    {
        CommonVars.StartGame = true;
        StoneFactory.Instance.Ready.AddListener(StoneFactory_Ready);
        
        gameSetting.LoadData();
        UnityServiceController.Instance.dUserSignedIn += OnUserSignedIn;
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
        Debug.Log("Sign up anonymously");
        UnityServiceController.Instance.SignUpAnonymouslyAsync();
    }

    private void OnUserSignedIn()
    {
        GoToNextLevel();
    }

    private void GoToNextLevel()
    {
        SceneManager.LoadSceneAsync("GameScene");
    }

    private void Update()
    {
        m_LoadingSlider.value = StoneFactory.Instance.GetLoadPercentage();
    }
}