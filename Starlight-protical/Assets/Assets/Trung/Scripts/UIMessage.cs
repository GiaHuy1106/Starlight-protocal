using UnityEngine;
using TMPro;
using System.Collections;

public class UIMessage : MonoBehaviour
{
    public static UIMessage Instance;

    public TextMeshProUGUI text;

    void Awake()
    {
        Instance = this;
        text.gameObject.SetActive(false);
    }

    public void Show(string msg)
    {
        StopAllCoroutines();
        StartCoroutine(ShowRoutine(msg));
    }

    IEnumerator ShowRoutine(string msg)
    {
        text.text = msg;
        text.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        text.gameObject.SetActive(false);
    }
}
