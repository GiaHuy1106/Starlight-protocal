using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header("Thời gian một ngày trong game (giây)")]
    public float secondsPerDay = 1440f; // chỉnh trong Inspector
    public static TimeManager Instance { get; private set; }
    public int Seconds { get => seconds;
        set { 
            if(seconds != value)
            {
                seconds = value;
                timeText.text = $"{hours:00}:{minutes:00}:{seconds:00}";
            }
        }
    }

    private float timeOfDay = 0f; // giá trị từ 0 → 1 (tỉ lệ ngày)
    private int hours;
    private int minutes;
    private int seconds;
    [SerializeField] private TextMeshProUGUI timeText;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
       Instance = this;
        timeOfDay = 1 / 3f;
    }
    void Update()
    {
        // Tăng thời gian theo tốc độ game
        timeOfDay += Time.deltaTime / secondsPerDay;

        // Reset về 0 khi hết một ngày
        if (timeOfDay > 1f) timeOfDay -= 1f;

        // Quy đổi sang giờ, phút, giây
        float totalSeconds = timeOfDay * 24f * 3600f;

        hours = Mathf.FloorToInt(totalSeconds / 3600f);
        minutes = Mathf.FloorToInt((totalSeconds % 3600f) / 60f);
        Seconds = Mathf.FloorToInt(totalSeconds % 60f);
    }

    // Hàm public để lấy giờ hiện tại
    public int GetHours() => hours;
    public int GetMinutes() => minutes;
    public int GetSeconds() => seconds;

    // Nếu muốn lấy giá trị tỉ lệ ngày (0 → 1)
    public float GetTimeOfDay() => timeOfDay;
}