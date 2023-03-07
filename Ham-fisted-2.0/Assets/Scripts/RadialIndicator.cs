using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class RadialIndicator : MonoBehaviour
{
    private float currentFillAmount = 0f;
    public float FillAmount { get{ return currentFillAmount; }
        set        
        {
            float clampedValue = Mathf.Clamp(value, 0f, 1f);
            currentFillAmount = clampedValue;
            wheelImage.fillAmount = clampedValue;
        }
    }
    public UnityEvent sliderFilled;
    public bool canFill = true;
    private bool isLeaving;
    [SerializeField] private float fillSpeed = 0.1f;
    [SerializeField] private float drainSpeed = -0.05f;
    [SerializeField] private Image wheelImage;
    [SerializeField] private Transform ballTransform;
    [SerializeField] private Transform vCam;

    public void LeavePressed(InputAction.CallbackContext context)
    {
        if (!canFill)
            return;
        if (context.performed)
        {
            transform.parent.gameObject.SetActive(true);
            isLeaving = true;
        }
        else
        {
            isLeaving = false;
            if (currentFillAmount <= 0)
                transform.parent.gameObject.SetActive(false);
        }
    }

    public void ResetValues()
    {
        canFill = true;
        FillAmount = 0f;
    }

    private void Update()
    {
        transform.parent.parent.position = ballTransform.position;
        transform.parent.LookAt(vCam.position);
        transform.parent.Rotate(0, 180, 0);
        if (isLeaving)
        {
            UpdateIndicatorValue(fillSpeed * Time.deltaTime);
        }
        else
        {
            UpdateIndicatorValue(drainSpeed * Time.deltaTime);
        }
    }

    private void Awake()
    {
        wheelImage.fillAmount = currentFillAmount;
        if (sliderFilled == null)
            sliderFilled = new UnityEvent();
        transform.parent.gameObject.SetActive(false);

    }

    void UpdateIndicatorValue (float value)
    {
        if (!canFill)
            return;
        FillAmount += value;

        if (currentFillAmount == 1)
        {
            sliderFilled.Invoke();
            canFill = false;
            transform.parent.gameObject.SetActive(false);
        }
        else if (currentFillAmount <= 0)
            transform.parent.gameObject.SetActive(false);
    }
}
