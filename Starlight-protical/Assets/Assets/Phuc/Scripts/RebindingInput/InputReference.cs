using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReference : MonoBehaviour
{
    [SerializeField] InputActionReference referenceAction;
    [SerializeField] int bindingIndex;
    public InputAction GetInputAction() => referenceAction.action;
    public void Start()
    {

    }

   


}
