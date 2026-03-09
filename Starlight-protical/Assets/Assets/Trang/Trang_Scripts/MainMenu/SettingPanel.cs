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
        settingPanel.SetActive(false);

        float savedVolume = PlayerPrefs.GetFloat("volume", 1f);
        float savedSFX = PlayerPrefs.GetFloat("sfxVolume", 1f);

        sound.value = savedVolume;
        sfx.value = savedSFX;

        UpdateVolumeUI(savedVolume);
        UpdateSFXUI(savedSFX);

        sound.onValueChanged.AddListener(UpdateVolumeUI);
        sfx.onValueChanged.AddListener(UpdateSFXUI);
    }
    void UpdateVolumeUI(float value)
    {
        volume = value;
        slider.fillAmount = volume;
        mute[0].gameObject.SetActive(volume == 0f);
        mute[1].gameObject.SetActive(volume != 0f);
        MusicManager.Instance.SetVolume(volume);
        PlayerPrefs.SetFloat("volume", volume);
        PlayerPrefs.Save();
    }
    void UpdateSFXUI(float value)
    {
        sfxVolume = value;
        slider2.fillAmount = sfxVolume;
        mute[2].gameObject.SetActive(sfxVolume == 0f);
        mute[3].gameObject.SetActive(sfxVolume != 0f);
        MusicManager.Instance.SetSFXVolume(sfxVolume);
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
        PlayerPrefs.Save();
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
            if(settingPanel.activeSelf)
            {
                MainMenuManager.instance.PauseGame();
            }
            else
            {
                MainMenuManager.instance.ResumeGame();
            }
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
