using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Audio Clips
    public AudioClip laserShotClip;
    public AudioClip laserBounceClip;
    public AudioClip enemyHitClip;
    public AudioClip enemyDeathClip;
    public AudioClip mirrorPlaceClip;
    public AudioClip invalidPlacementClip;
    public AudioClip mirrorPickupClip;
    public AudioClip winClip;
    public AudioClip loseClip;
    public AudioClip enemyAlert;
    public AudioClip enemyDeath;
    public AudioClip elevatorBeep;

    // New Audio Clip for Mirror Hover
    public AudioClip mirrorHoverClip;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("No AudioSource found on the AudioManager GameObject.");
        }
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
