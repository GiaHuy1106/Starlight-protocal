using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Ins { get; private set; }
    public InputSystem_Actions inputSystem;

    private void Awake()
    {
        if(Ins != null && Ins != this)
        {
            Destroy(gameObject);
            return;
        }
        Ins = this;
        inputSystem = new InputSystem_Actions();
        inputSystem.Player.Enable();       
    }
    private void Start()
    {
        SubEvent();
    }

    void SubEvent()
    {
        inputSystem.Player.Move.performed += Move_performed;
        inputSystem.Player.Attack.performed += Attack_performed;
        inputSystem.Player.Jump.performed += Jump_performed;
        inputSystem.Player.OpenBag.performed += OpenBag_performed;
        inputSystem.Player.CancelSkill.performed += CancelSkill_performed;
        inputSystem.Player.Skill1.performed += Skill1_performed;
        inputSystem.Player.Skill2.performed += Skill2_performed;

    }

    
    private void OnDestroy()
    {
        DeSubEvent();
    }

    void DeSubEvent()
    {

        inputSystem.Player.Move.performed -= Move_performed;
        inputSystem.Player.Attack.performed -= Attack_performed;
        inputSystem.Player.Jump.performed -= Jump_performed;
        inputSystem.Player.OpenBag.performed -= OpenBag_performed;
        inputSystem.Player.CancelSkill.performed -= CancelSkill_performed;
        inputSystem.Player.Skill1.performed -= Skill1_performed;
        inputSystem.Player.Skill2.performed -= Skill2_performed;
    }

    public string GetJsonData()
    {
        return inputSystem.SaveBindingOverridesAsJson();
    }
    public void LoadDataJson(string json)
    {
        inputSystem.LoadBindingOverridesFromJson(json);
    }
    private void Skill2_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        print("Skill2" + obj.control.name);
    }

    private void CancelSkill_performed(InputAction.CallbackContext obj)
    {
        print("Cancel" + obj.control.name);
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
}
