using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    // Existing Audio Clips
    public AudioClip laserShotClip;
    public AudioClip laserBounceClip;
    public AudioClip enemyHitClip;
    public AudioClip enemyDeathClip;
    public AudioClip mirrorPlaceClip;
    public AudioClip invalidPlacementClip;
    public AudioClip mirrorPickupClip;
    public AudioClip winClip;
    public AudioClip loseClip;

    // New Audio Clip for Mirror Hover
    public AudioClip mirrorHoverClip; // Add this for the hover sound

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
