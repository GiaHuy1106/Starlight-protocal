using UnityEngine;
using UnityEngine.InputSystem;

public class ReinputManager : MonoBehaviour
{
    InputSystem_Actions inputSystem;
    InputActionMap PlayerMap;
    InputAction curAction;
    private void Start()
    {
        //AddCallbacks();
        inputSystem = new InputSystem_Actions();
        PlayerMap = inputSystem.Player;
        
        InputActionMap playerMap = inputSystem.Player;    
        foreach (var action in playerMap.actions)
        {
            for (int i = 0; i < action.bindings.Count; i ++ )
            {
                var binding = action.bindings[i];
                print(action.name + "\t" + binding.path + "\t + Index: " + i);
            }
        }
        // Duyệt qua tất cả binding
       
    }
    public void StartRebinding(InputAction action, string contronExcluding)
    {
        curAction = action;
        action.Disable();

            action
            .PerformInteractiveRebinding()            
            .WithControlsExcluding(contronExcluding)
            .OnMatchWaitForAnother(0.5f)
            .OnComplete(
             callback =>
             {
                 callback.Dispose();
                 action.Enable();
                 RemoveDuplicateBinding();
             }
            )
            .OnCancel(callback => {
                callback.Dispose();
                curAction.Enable();
            })
            .Start();
      
    }

    void RemoveDuplicateBinding()
    {
        foreach (var action in PlayerMap.actions)
        {
            for (int i = 0; i < action.bindings.Count; i++)
            {

            }
        }
    }



    void AddCallbacks()
    {
        inputSystem.Player.Enable();
        inputSystem.Player.Move.performed += Move_performed;
        inputSystem.Player.Attack.performed += Attack_performed;
        inputSystem.Player.OpenBag.performed += OpenBag_performed;
        inputSystem.Player.Jump.performed += Jump_performed;
        inputSystem.Player.Skill1.performed += Skill1_performed;
        inputSystem.Player.Skill2.performed += Skill2_performed;
        
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
        //RemoveCallbacks();
    }
    private void RemoveCallbacks()
    {
        inputSystem.Player.Move.performed -= Move_performed;
        inputSystem.Player.Attack.performed -= Attack_performed;
        inputSystem.Player.OpenBag.performed -= OpenBag_performed;
        inputSystem.Player.Jump.performed -= Jump_performed;
        inputSystem.Player.Skill1.performed -= Skill1_performed;
        inputSystem.Player.Skill2.performed -= Skill2_performed;
    }
}
