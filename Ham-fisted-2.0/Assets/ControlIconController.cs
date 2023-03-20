using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[SerializeField] public class SetControllerSchemeEvent : UnityEvent<ControlIconScheme>{}

public class ControlIconController : MonoBehaviour
{
    public static ControlIconController instance;
    public SetControllerSchemeEvent SetControlScheme;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (SetControlScheme == null)
            SetControlScheme = new SetControllerSchemeEvent();
    }

    public void SetScheme (ControlIconScheme controlIconScheme)
    {
        SetControlScheme.Invoke(controlIconScheme);
    }
}
