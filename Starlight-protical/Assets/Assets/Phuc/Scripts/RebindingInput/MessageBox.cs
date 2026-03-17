using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
    Action<bool> callback;
    [SerializeField] TextMeshProUGUI text;

    public void Show(string message, Action<bool> result, bool isSetText = false)
    {
        if(isSetText && text != null)
        {
            text.text = message;
        }
        callback = result;
        gameObject.SetActive(true);
    }
    public void Exit()
    {
        callback?.Invoke(false);
        gameObject.SetActive(false);
    }
    public void Save()
    {
        callback?.Invoke(true);
        gameObject.SetActive(false);
    }


}
