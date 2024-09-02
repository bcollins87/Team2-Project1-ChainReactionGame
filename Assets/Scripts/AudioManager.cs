using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioClip laserShotClip;
    public AudioClip laserBounceClip;
    public AudioClip enemyHitClip;
    public AudioClip enemyDeathClip;
    public AudioClip mirrorPlaceClip;
    public AudioClip invalidPlacementClip;
    public AudioClip mirrorPickupClip;
    public AudioClip winClip;
    public AudioClip loseClip;

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this AudioManager across scenes
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // There should only be one instance of AudioManager
        }
        
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("Attempted to play a null AudioClip.");
        }
    }
}
