using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SkipBTM : MonoBehaviour
{
    public GameObject skipBTN;

    void Start()
    {
        skipBTN.SetActive(false); // ẩn nút lúc đầu
        StartCoroutine(DelayBTN());
    }

    public void Skip()
    {
        SceneManager.LoadScene("GameScene");
    }

    IEnumerator DelayBTN()
    {
        yield return new WaitForSeconds(5f); // đợi 5 giây
        skipBTN.SetActive(true); // hiện nút
    }
}