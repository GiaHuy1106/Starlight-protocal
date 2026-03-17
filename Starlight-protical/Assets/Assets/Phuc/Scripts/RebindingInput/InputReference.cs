using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputReference : MonoBehaviour
{
    [SerializeField] InputActionReference referenceAction;
    [SerializeField] int bindingIndex;
    [SerializeField] Button button;
    [SerializeField] string controlExcluding;
    [SerializeField] TextMeshProUGUI inputText;
    public event EventHandler<DataRebindingInputEventArgs> onClickRebind;
    DataRebindingInputEventArgs data;

    public InputAction GetInputAction() => referenceAction.action;
    public int GetBindingIndex() => bindingIndex;
    private void Awake()
    {
        data = new DataRebindingInputEventArgs { action = GetInputAction(), bindingIndex = this.bindingIndex, controlExcluding = this.controlExcluding};
    }
    public void Start()
    {
        button.onClick.AddListener(OnClickRebind);
        inputText.text = GetInputAction().bindings[bindingIndex].name.ToUpper();
       
    }

    public void OnClickRebind()
    {
        onClickRebind?.Invoke(this, data);
    }

    public void SetInputText(string value)
    {
       if(inputText != null)
        {
            if(!string.IsNullOrEmpty(value))
            inputText.text = value.ToUpper();
            else
                inputText.text = value;
        }
        
    }

    public string GetInputText()
    {
        return inputText.text;
    }

    public void UpdateUI()
    {
        string path = GetInputAction().bindings[bindingIndex].path;
        var parts = path.Split('/');
        string text = null;
       if(parts.Length > 1)
        {
            text = parts[parts.Length - 1];
        }
        SetInputText(text);
    }
}

public class DataRebindingInputEventArgs: EventArgs
{
    public InputAction action;
    public int bindingIndex;
    public string controlExcluding;
}
