using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    public GameObject settingPanel;
    public Image slider;
    public Image slider2;
    public Slider sound;
    public Slider sfx;
    float volume;
    float sfxVolume;
    public Image[] mute;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sound.value = 1f;
        settingPanel.SetActive(false);
        sound.onValueChanged.AddListener(UpdateVolumeUI);
        UpdateVolumeUI(sound.value);
        float savedVolume = PlayerPrefs.GetFloat("volume", 1f);
        sound.value = savedVolume;
        UpdateVolumeUI(savedVolume);

        sfx.value = 1f;
        sfx.onValueChanged.AddListener(UpdateSFXUI);
        UpdateSFXUI(sfx.value);
        float savedSFX = PlayerPrefs.GetFloat("sfxVolume", 1f);
        sfx.value = savedSFX;
        UpdateSFXUI(savedSFX);
    }
    void UpdateVolumeUI(float value)
    {
        volume = value;
        slider.fillAmount = volume;
        mute[0].gameObject.SetActive(volume == 0f);
        mute[1].gameObject.SetActive(volume != 0f);
        MusicManager.Instance.SetVolume(volume);
        PlayerPrefs.SetFloat("volume", volume);
    }
    void UpdateSFXUI(float value)
    {
        sfxVolume = value;
        slider2.fillAmount = sfxVolume;
        mute[2].gameObject.SetActive(sfxVolume == 0f);
        mute[3].gameObject.SetActive(sfxVolume != 0f);
        MusicManager.Instance.SetSFXVolume(sfxVolume);
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void ToggelActive()
    {
        if (settingPanel != null)
        {
            settingPanel.SetActive(!settingPanel.activeSelf);
        }
    }
    public void MuteSound()
    {
        if (volume == 0f)
        {
            sound.value = 1f;

            slider.fillAmount = 1f;

        }
        else
        {
            sound.value = 0f;

            slider.fillAmount = 0f;
        }
        UpdateVolumeUI(sound.value);
    }

    public void MuteSfx()
    {
        if (sfxVolume == 0f)
        {
            sfx.value = 1f;

            slider2.fillAmount = 1f;

        }
        else
        {
            sfx.value = 0f;

            slider2.fillAmount = 0f;
        }
        UpdateSFXUI(sfx.value);
    }
}
