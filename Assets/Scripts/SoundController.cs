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
        }
    }

    #endregion

    [SerializeField] AudioSource _audioSource;
    
    public AudioClip[] kittenClips;
    public AudioClip[] popClips;
    public AudioClip[] bonusClips;

    private int clipNum = 0;
    public void PlayRandomKittenSFX()
    {
        int rnd = Random.Range(0, kittenClips.Length);
        _audioSource.PlayOneShot(kittenClips[rnd]);
    }
    public void PlayRandomPopSFX()
    {
        int rnd = Random.Range(0, popClips.Length);
        _audioSource.PlayOneShot(popClips[rnd]);
    }

    public void PlayBonusClip()
    {
        _audioSource.PlayOneShot(bonusClips[clipNum]);
        clipNum++;
        if (clipNum > 2) clipNum = 2;
    }
}