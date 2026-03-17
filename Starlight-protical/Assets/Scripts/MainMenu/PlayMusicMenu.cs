using UnityEngine;

public class PlayMusicMenu : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioClip musicClip;
    private void Start()
    {
        musicSource.clip = musicClip;
        musicSource.Play();
    }

}
