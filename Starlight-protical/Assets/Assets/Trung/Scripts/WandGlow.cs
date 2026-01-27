using UnityEngine;

public class WandGlow : MonoBehaviour
{
    [Header("Renderer tip phát sáng")]
    public Renderer glowRenderer;

    [Header("Glow Settings")]
    public Color glowColor = Color.white;
    public float intensity = 10f;

    Material mat;

    void Awake()
    {
        // lấy material instance riêng (tránh đổi toàn bộ prefab)
        mat = glowRenderer.material;

        SetGlow(false); // tắt khi start
    }

    public void SetGlow(bool on)
    {
        if (on)
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", glowColor * intensity);
        }
        else
        {
            mat.SetColor("_EmissionColor", Color.black);
        }
    }
}
