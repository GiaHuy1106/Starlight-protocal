using UnityEngine;

public class WeatherManager: MonoBehaviour
{
    public Material mor_noon;
    public Material noon_afternoon;
    public Material aftnoon_evening;
    public Material evening_night;
    public Material night_mor;

    Material curMaterial;


    [Header("WeatherControll")]
    public ParticleSystem rainSystem;
    public ParticleSystem lightningSystem;
    private float nextToggleTimeRain;
    private float nextToggleTimeLightning;
    [SerializeField] bool onlyDayWeather = true;

    public Material CurMaterial { get => curMaterial; set {
            if(curMaterial != value)
            {
                curMaterial = value;
                RenderSettings.skybox = curMaterial;
            }
        } }

    private void Start()
    {
        CurMaterial = mor_noon;
        nextToggleTimeRain = Random.Range(1f, 5f);
        nextToggleTimeLightning = Random.Range(2f, 8f);

    }
    private void Update()
    {
        float timeOfDay = TimeManager.Instance.GetTimeOfDay();
        if(timeOfDay < 1/3f)
        {
            CurMaterial = night_mor;
            SetBlendMaterial(0, 1/3f, timeOfDay);
        }
        else if(timeOfDay < 0.5f)
        {
            CurMaterial = mor_noon;
                SetBlendMaterial(1/3f, 0.5f, timeOfDay);
        }
        else if(timeOfDay < 2/3f)
        {
            CurMaterial = noon_afternoon;
                SetBlendMaterial(0.5f, 2/3f, timeOfDay);
        }
        else if(timeOfDay < 5/6f)
        {
            CurMaterial = aftnoon_evening;
            SetBlendMaterial(2/3f, 5/6f, timeOfDay);
        }
        else
        {
            CurMaterial = evening_night;
            SetBlendMaterial(5/6f, 1f, timeOfDay);
        }

        WeatherControll();
    }




    void SetBlendMaterial(float a, float b, float value)
    {
        float t = Mathf.InverseLerp(a, b, value);
        curMaterial.SetFloat("_Blend", t);
    }


    void WeatherControll()
    {
        float timeOfDay = TimeManager.Instance.GetTimeOfDay();

        // Bật/tắt mưa
        if (Time.time >= nextToggleTimeRain)
        {
            ToggleParticle(rainSystem);
            nextToggleTimeRain = Time.time + Random.Range(5f, 15f);
        }

        // Bật/tắt sét
        if (Time.time >= nextToggleTimeLightning)
        {
            ToggleParticle(lightningSystem);
            nextToggleTimeLightning = Time.time + Random.Range(10f, 30f);
        }
        if (onlyDayWeather)
        {
            // Ví dụ: chỉ cho phép hiệu ứng xảy ra ban ngày (0.25 -> 0.75 ~ 6h–18h)
            if (timeOfDay < 0.25f || timeOfDay > 0.75f)
            {
                if (rainSystem.isPlaying) rainSystem.Stop();
                if (lightningSystem.isPlaying) lightningSystem.Stop();
            }
        }

    }

    void ToggleParticle(ParticleSystem ps)
    {
        if (ps.isPlaying)
            ps.Stop();
        else
            ps.Play();
    }


}
