using UnityEngine;

public class WeatherManager: MonoBehaviour
{
    public Material mor_noon;
    public Material noon_afternoon;
    public Material aftnoon_evening;
    public Material evening_night;
    public Material night_mor;

    Material curMaterial;

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
         
    }




    void SetBlendMaterial(float a, float b, float value)
    {
        float t = Mathf.InverseLerp(a, b, value);
        curMaterial.SetFloat("_Blend", t);
    }


}
