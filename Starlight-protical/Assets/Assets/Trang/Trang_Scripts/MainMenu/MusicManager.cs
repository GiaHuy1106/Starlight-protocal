using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource musicSource, sfxSource;
    public AudioClip[] musicClips, sfxClips;
    public static MusicManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("volume", 1f);
        SetVolume(savedVolume);
        float savedSFX = PlayerPrefs.GetFloat("sfxVolume", 1f);
        SetSFXVolume(savedSFX);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SetVolume(float value)
    {
        musicSource.volume = value;
        PlayerPrefs.SetFloat("volume", value);
    }
    public void SetSFXVolume(float value)
    {
        sfxSource.volume = value;
        PlayerPrefs.SetFloat("sfxVolume", value);
    }
    public void PlayMusic(string name)
    {
        AudioClip clip = System.Array.Find(musicClips, music => music.name == name);
        if (clip != null)
        {
            musicSource.clip = clip;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("Music clip not found: " + name);
        }
    }
    public void playSFX(string name)
    {
        AudioClip clip = System.Array.Find(sfxClips, sfx => sfx.name == name);
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("SFX clip not found: " + name);
        }
    }
}
