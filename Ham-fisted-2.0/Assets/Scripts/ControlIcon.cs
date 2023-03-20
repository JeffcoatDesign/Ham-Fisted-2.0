using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ControlIconScheme 
{
    keyboard,
    controller
}

public class ControlIcon : MonoBehaviour
{
    public ControlIconScheme iconScheme = ControlIconScheme.keyboard;
    [SerializeField] private Image iconImage;
    [SerializeField] private Sprite mkSprite;
    [SerializeField] private Sprite controllerSprite;

    private void Start()
    {
        iconImage.sprite = mkSprite;
        ControlIconController.instance.SetControlScheme.AddListener(SwitchScheme);
    }

    public void SwitchScheme(ControlIconScheme controlIconScheme)
    {
        iconScheme = controlIconScheme;
        if (iconScheme == ControlIconScheme.controller)
        {
            iconImage.sprite = controllerSprite;
        }
        if (iconScheme == ControlIconScheme.keyboard) 
        {
            iconImage.sprite = mkSprite;
        }
    }
}
