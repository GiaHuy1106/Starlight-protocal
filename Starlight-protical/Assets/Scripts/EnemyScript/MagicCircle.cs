using UnityEngine;
using System.Collections;

public class MagicCircle : MonoBehaviour
{
    [Header("Settings")]
    public float damage = 20f;       // Sát thương
    public float delayTime = 1f;   // Thời gian cảnh báo trước khi nổ
    public float radius = 3f;        // Bán kính nổ


    void Start()
    {
        // Bắt đầu đếm ngược ngay khi được sinh ra
        StartCoroutine(ExplodeSequence());
    }

    IEnumerator ExplodeSequence()
    {
        // Giai đoạn Chờ (Cảnh báo)
        Debug.Log("Magic Circle: Warning...");
        yield return new WaitForSeconds(delayTime);

        // Giai đoạn Nổ
        Explode();
    }

    void Explode()
    {
        Debug.Log("BOOM!");

        // Physics.OverlapSphere sẽ lấy tất cả Collider trong hình cầu bán kính 'radius'
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);

        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                // Trừ máu Player
                PlayerHealth playerHP = hit.GetComponent<PlayerHealth>();
                if (playerHP != null)
                {
                    playerHP.TakeDamage(damage);
                }
            }
        }

        // Xóa vòng tròn sau khi nổ
        Destroy(gameObject);
    }

    // Vẽ vòng tròn đỏ trong Scene để dễ căn chỉnh bán kính
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}