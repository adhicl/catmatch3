using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    private AsyncOperationHandle m_SceneHandle;

    public AssetReference[] locations;
        
    [SerializeField] private Slider m_LoadingSlider;

    void Start()
    {
        CommonVars.StartGame = true;
        StoneFactory.Instance.Ready.AddListener(GoToNextLevel);
        //m_SceneHandle = Addressables.DownloadDependenciesAsync("GameScene", true);
        //m_SceneHandle.Completed += OnSceneLoaded;
    }

    private void OnDisable()
    {
        //m_SceneHandle.Completed -= OnSceneLoaded;
    }

    private void OnSceneLoaded(AsyncOperationHandle obj)
    {
        if(obj.Status == AsyncOperationStatus.Succeeded)
        {
            //GoToNextLevel();
        }
    }

    private void GoToNextLevel()
    {
        SceneManager.LoadSceneAsync("GameScene");
        //Addressables.LoadSceneAsync("GameScene", UnityEngine.SceneManagement.LoadSceneMode.Single, true);
    }

    private void Update()
    {
        m_LoadingSlider.value = StoneFactory.Instance.GetLoadPercentage();
        // We don't need to check for this value every single frame, and certainly not after the scene has been loaded
        //m_LoadingSlider.value = m_SceneHandle.PercentComplete;
    }
}