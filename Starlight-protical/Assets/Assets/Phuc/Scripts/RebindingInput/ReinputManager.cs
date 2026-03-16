using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;

public class ReinputManager : MonoBehaviour
{
    InputSystem_Actions inputSystem;   
    [SerializeField] GameObject RebindingkeyUI;
    [SerializeField] List<InputReference> inputReferences = new List<InputReference>();
    private void Start()
    {
        inputSystem = new InputSystem_Actions();
        LoadBinding();
        AddCallbacks();
        
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


    public void SaveBinding()
    {
        string jsonInput = inputSystem.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("INPUTDATA", jsonInput);
        PlayerPrefs.Save();
    }
    public void LoadBinding()
    {
        string jsonInput = PlayerPrefs.GetString("INPUTDATA", null);
        if (!string.IsNullOrEmpty(jsonInput))
        {
            inputSystem.LoadBindingOverridesFromJson(jsonInput);
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
        action.Disable();
        action
        .PerformInteractiveRebinding(index)
        .WithControlsExcluding(controlExcluding)
        .OnMatchWaitForAnother(0.5f)
        .OnComplete(
         callback =>
         {
             callback.Dispose();
             action.Enable();
             var newBinding = action.bindings[index];
             RemoveDuplicateBinding(action, newBinding);
             InputReference invoker = (InputReference)sender;
             invoker.SetInputText(newBinding.name.ToUpper());
         }
        )
        .OnCancel(callback => {
            callback.Dispose();
            action.Enable();
        })
        .Start();
    }

    private void Skill2_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        print("Skill2" + obj.control.name);
    }

    private void Skill1_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        print("Skill1" + obj.control.name);
    }

    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        print("Jump" + obj.control.name);
    }

    private void OpenBag_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        print("OpenBag" + obj.control.name);
    }

    private void Attack_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        print("Attack" + obj.control.name);
    }

    private void Move_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        print("Move: " + obj.control.name);
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
        SaveBinding();
    }
}
