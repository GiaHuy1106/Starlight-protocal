using Ultrabolt.SkyEngine;
using UnityEngine;

public class BossWeatherTrigger : MonoBehaviour
{
    public SkyCore sky;   // kéo Sky Engine vào đây

    public float transitionSpeed = 0.5f;

    private bool bossWeather = false;

    void Update()
    {
        if (bossWeather)
        {
            // tăng mưa từ từ
            sky.weatherSpeed = Mathf.Lerp(sky.weatherSpeed, 1.5f, Time.deltaTime * transitionSpeed);
        }
        else
        {
            // giảm mưa nếu cần
            sky.weatherSpeed = Mathf.Lerp(sky.weatherSpeed, 0f, Time.deltaTime * transitionSpeed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bossWeather = true;
            sky.weather = SkyCore.Weather.Rain;
            //sky.weather = SkyCore.Weather.HighCloud;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            bossWeather = false;
            sky.weather = SkyCore.Weather.Clear;
        }    
        
    }
}
