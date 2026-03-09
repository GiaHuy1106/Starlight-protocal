using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    public AudioSource musicSource, sfxSource;
    public AudioClip[] musicClips, sfxClips;
    public static MusicManager Instance;

    public AudioMixer myMixer;
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
        if (value <= 0.001f)
        {
            myMixer.SetFloat("MusicVol", -80f); 
        }
        else
        {
            myMixer.SetFloat("MusicVol", Mathf.Log10(value) * 20f);
        }

        PlayerPrefs.SetFloat("volume", value);
    }

    public void SetSFXVolume(float value)
    {
        if (value <= 0.001f)
        {
            myMixer.SetFloat("SFXVol", -80f);
        }
        else
        {
            myMixer.SetFloat("SFXVol", Mathf.Log10(value) * 20f);
        }

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
