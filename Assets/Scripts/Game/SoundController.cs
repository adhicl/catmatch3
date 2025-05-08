using UnityEngine;

public class SoundController : MonoBehaviour
{

    #region singleton

    public static SoundController Instance { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    #endregion

    [SerializeField] AudioSource _audioSource;
    
    public AudioClip[] kittenClips;
    public AudioClip[] popClips;
    public AudioClip bonusClip;
    public AudioClip windClip;
    public AudioClip bombClip;

    public AudioClip countdownClip;
    public AudioClip startClip;
    public AudioClip upgradeClip;
    public AudioClip finishClip;
    
    public AudioClip buttonClip;

    public AudioClip fillClip;
    public AudioClip chestClip;
    public AudioClip unlockClip;

    public GameSetting gameSetting;
    
    public void PlayRandomKittenSFX()
    {
        if (!gameSetting.isSoundOn) return;
        int rnd = Random.Range(0, kittenClips.Length);
        _audioSource.PlayOneShot(kittenClips[rnd]);
    }
    
    public void PlayRandomPopSFX()
    {
        if (!gameSetting.isSoundOn) return;
        int rnd = Random.Range(0, popClips.Length);
        _audioSource.PlayOneShot(popClips[rnd]);
    }
    
    public void PlayWindClip()
    {
        if (!gameSetting.isSoundOn) return;
        _audioSource.PlayOneShot(windClip);
    }
    
    public void PlayBombClip()
    {
        if (!gameSetting.isSoundOn) return;
        _audioSource.PlayOneShot(bombClip);
    }
    
    public void PlayStartClip()
    {
        if (!gameSetting.isSoundOn) return;
        _audioSource.PlayOneShot(startClip);
    }
    
    public void PlayUpgradeClip()
    {
        if (!gameSetting.isSoundOn) return;
        _audioSource.PlayOneShot(upgradeClip);
    }
    
    public void PlayCountdownClip()
    {
        if (!gameSetting.isSoundOn) return;
        _audioSource.PlayOneShot(countdownClip);
    }
    
    public void PlayFinishClip()
    {
        if (!gameSetting.isSoundOn) return;
        _audioSource.PlayOneShot(finishClip);
    }

    public void PlayBonusClip()
    {
        if (!gameSetting.isSoundOn) return;
        _audioSource.PlayOneShot(bonusClip);
    }

    public void PlayFillClip()
    {
        if (!gameSetting.isSoundOn) return;
        _audioSource.PlayOneShot(fillClip);
    }

    public void PlayChestClip()
    {
        if (!gameSetting.isSoundOn) return;
        _audioSource.PlayOneShot(chestClip);
    }

    public void PlayUnlockClip()
    {
        if (!gameSetting.isSoundOn) return;
        _audioSource.PlayOneShot(unlockClip);
    }
    
    public void PlayButtonClip()
    {
        if (!gameSetting.isSoundOn) return;
        _audioSource.PlayOneShot(buttonClip);
    }
}