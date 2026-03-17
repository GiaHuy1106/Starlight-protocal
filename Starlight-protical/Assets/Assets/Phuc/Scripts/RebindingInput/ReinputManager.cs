using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using System.Windows;

public class ReinputManager : MonoBehaviour
{    
    [SerializeField] GameObject RebindingkeyUI;
    [SerializeField] List<InputReference> inputReferences = new List<InputReference>();
    [SerializeField] MessageBox messageBox;
    const string INPUTDEFAULT = "INPUTDEFAULT";
    const string INPUTUSER = "INPUTUSER";

    [SerializeField] Button apply;
   

    bool ismodify = false;
    public bool IsModify { get => ismodify;
        set { 
            if(ismodify != value)
            {

                ismodify = value;
                if (ismodify)
                {
                    apply.GetComponentInChildren<TextMeshProUGUI>().color = Color.green;
                    apply.interactable = true;
                }
                else
                {
                    apply.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
                    apply.interactable = false;
                }
            }
        }
    }


    private void Start()
    {      
        AddCallbacks();
        UpdateUI();
        if (PlayerPrefs.HasKey(INPUTUSER))
        {
            string dataUSER = PlayerPrefs.GetString(INPUTUSER);
            if(string.IsNullOrEmpty(dataUSER)){
                LoadBinding(INPUTUSER, dataUSER);
                UpdateUI();
            }
        }

        if (!PlayerPrefs.HasKey(INPUTDEFAULT))
        {
            string dataDEFAULT = GameInput.Ins.GetJsonData();
            PlayerPrefs.SetString(INPUTDEFAULT, dataDEFAULT);
        }
        // Duyệt qua tất cả binding   
    }

    public void SetActiveRebindingUI(bool active)
    {
        if(RebindingkeyUI != null)
        {
            RebindingkeyUI.SetActive(active);          
        }
    }   

    void RemoveDuplicateBinding(InputAction action, InputBinding newbinding)
    {

        foreach (var i in inputReferences)
        {
            var act = i.GetInputAction();
            if (act == action) continue;
            for (int j = 0; j < act.bindings.Count; j ++)
            {
                var bind = act.bindings[j];
                if(bind == newbinding)
                {
                    act.ApplyBindingOverride("");
                    i.SetInputText("");
                }
            }
        }       
    }


    public void SaveBinding(string KEY, string value)
    {
        PlayerPrefs.SetString(KEY, value);
        PlayerPrefs.Save();
    }
    public void LoadBinding(string KEY, string value)
    {
       
        if (!string.IsNullOrEmpty(value))
        {
            GameInput.Ins.LoadDataJson(value);
           
        }
    }

    void AddCallbacks()
    {
        if (inputReferences != null && inputReferences.Count > 0)
        {
            foreach (var i in inputReferences)
            {
                i.onClickRebind += StartRebinding;
                
            }
        }
        
    }

   void StartRebinding(object sender, DataRebindingInputEventArgs data)
    {
        InputAction action = data.action;
        int index = data.bindingIndex;
        string controlExcluding = data.controlExcluding;
        string text = ((InputReference)sender).GetInputText();
        ((InputReference)sender).SetInputText("|");
        action.Disable();
        action
        .PerformInteractiveRebinding(index)
        .WithControlsExcluding(controlExcluding)
        .OnMatchWaitForAnother(0.5f)
        .OnComplete(
         callback =>
         {
             IsModify = true;
             callback.Dispose();
             action.Enable();
             var newBinding = action.bindings[index];     
             RemoveDuplicateBinding(action, newBinding);
             string controlName = newBinding.path.Split('/')[1];
             ((InputReference)sender).SetInputText(controlName);
         }
        )
        .OnCancel(callback => {
            callback.Dispose();
            ((InputReference)sender).SetInputText(text);
            action.Enable();
        })
        .Start();
    }

  
    private void OnDestroy()
    {
      RemoveCallbacks();
    }
    private void RemoveCallbacks()
    {
       if(inputReferences!= null && inputReferences.Count > 0)
        {
            foreach (var i in inputReferences)
            {
                i.onClickRebind -= StartRebinding;

            }
        }
    }
    public void Apply()
    {
        if (ismodify)
        {
            string value = GameInput.Ins.GetJsonData();
            SaveBinding(INPUTUSER, value);
            IsModify = false;
        }
    }
    public void Back()
    {
        if (ismodify)
        {
            messageBox.Show("", (result) =>
            {
                if (result)
                {
                    string data = GameInput.Ins.GetJsonData();
                    SaveBinding(INPUTUSER, data);                  
                    UpdateUI();
                }
                else
                {
                    string data = PlayerPrefs.GetString(INPUTUSER);
                    GameInput.Ins.LoadDataJson(data);
                    UpdateUI();
                }
            }

                );
        }       
        SetActiveRebindingUI(false);        
    }
    public void Reset()
    {
        IsModify = true;
        GameInput.Ins.inputSystem.RemoveAllBindingOverrides();
        UpdateUI();

    }
    void UpdateUI()
    {
        foreach (var i in inputReferences)
        {
            i.UpdateUI();
        }
    }
}
