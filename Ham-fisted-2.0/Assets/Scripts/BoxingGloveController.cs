using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoxingGloveController : MonoBehaviour
{
    public PlayerController playerController;
    public Transform ballTransform;
    public Transform gloveOrigin;
    public Transform springTransform;
    public Transform springEnd;
    public BoxingGloveLauncher bGL;
    public float punchForce;
    public float speed;
    public float punchTime = 1f;
    public float punchRatio = 0.5f;
    public float swingLength = 0.5f;
    public float springCompressScale = 0.5f;

    public bool isAttacking = false;
    public bool isBlocking = false;
    public bool wasBlocked = false;
    [SerializeField] private float chargeAmount = 0.01f;

    [Header("Shield")]
    [SerializeField] private GameObject shieldObj;
    [SerializeField] private float shieldStunTime;
    [SerializeField] private float shieldDrainAmount = 0.5f;
    public float ShieldStunTime { get { return shieldStunTime; } }
    private float charge = 0;
    private float punchStartTime;
    private Vector2 movementInput;

    public void ResetVariables ()
    {
        charge = 0;
    }

    void Update()
    {
        transform.position = ballTransform.position;

        shieldObj.SetActive(isBlocking);
    }

    private void FixedUpdate()
    {
        Vector3 controllerInput = new Vector3(0, FindAngle(movementInput), 0);

        if (movementInput.x != 0 || movementInput.y != 0)
            RotateTo(FlattenInput(controllerInput));

        if (isBlocking && !playerController.IsStunned)
        {
            charge -= Time.deltaTime * shieldDrainAmount;
            charge = Mathf.Clamp(charge, 0, 1);
            if (CameraManager.instance != null)
                CameraManager.instance.playerGameUIs[playerController.id].UpdateSliderValue(Mathf.Abs(charge));
        }
        if (charge <= 0)
            isBlocking = false;
    }

    public void OnMove(InputAction.CallbackContext ctx) => movementInput = ctx.ReadValue<Vector2>();

    private void LateUpdate()
    {
        gloveOrigin.position = springEnd.position;
    }

    Quaternion FlattenInput(Vector3 input)
    {
        Quaternion flatten = Quaternion.LookRotation(playerController.vCam.transform.forward, Vector3.up) * Quaternion.Euler(input);
        return flatten;
    }

    float FindAngle (Vector2 vector2)
    {
        float value = (float)(Mathf.Atan2(vector2.x, vector2.y) / Mathf.PI) * 180f;
        if (value < 0)
            value += 360;
        return value;
    }

    void RotateTo (Quaternion target)
    {
        Quaternion total = transform.rotation;
        float cr = transform.rotation.eulerAngles.y;
        float tr = target.eulerAngles.y;
        if (cr > tr || cr < tr)
        {
            total = Quaternion.Euler(0, Mathf.LerpAngle(cr, tr, Time.deltaTime * speed), 0);
            transform.rotation = total;
            if (tr > cr + 22 || tr < cr - 22) {
                charge = Mathf.Clamp(charge + chargeAmount, 0, 1);
            }
            if (CameraManager.instance != null)
                CameraManager.instance.playerGameUIs[playerController.id].UpdateSliderValue(Mathf.Abs(charge));
        }
    }

    public void TryAttack ()
    {
        if (!isAttacking && !playerController.IsStunned)
        {
            if (isBlocking)
                isBlocking = false;
            StartCoroutine(Attack());
        }
    }

    public void ShieldPressed (InputAction.CallbackContext context)
    {
        if (playerController.IsStunned)
            return;
        if (context.performed && charge > 0 && !isBlocking)
            isBlocking = true;
        else if (context.ReadValueAsButton())
            isBlocking = false;
        else
            isBlocking = false;
    }

    public void ClearCharge ()
    {
        bGL.ToggleCollider(false);
        charge = 0;
        bGL.charge = 0;
        if (CameraManager.instance != null)
            CameraManager.instance.playerGameUIs[playerController.id].UpdateSliderValue(Mathf.Abs(charge));
    }

    IEnumerator Attack()
    {
        float attackCharge = Mathf.Abs(charge);
        isAttacking = true;
        bGL.wasBlockChecked = false;
        bGL.ToggleCollider(!wasBlocked);
        bGL.charge = attackCharge;
        punchStartTime = Time.time;
        float time = Time.time - punchStartTime;
        yield return new WaitForEndOfFrame();
        bGL.wasBlockChecked = true;
        while (time <= punchTime * punchRatio)
        {
            time = Time.time - punchStartTime;
            if (!wasBlocked)
                springTransform.localScale = new Vector3(1, 1, springCompressScale + swingLength * (time / (punchTime * punchRatio)));
            yield return new WaitForEndOfFrame();
        }
        while (time <= punchTime)
        {
            bGL.ToggleCollider(!wasBlocked);
            time = Time.time - punchStartTime;
            springTransform.localScale = new Vector3(1, 1, springCompressScale + swingLength * (1 - ((time - (punchTime * punchRatio)) / (punchTime * (1 - punchRatio)))));
            yield return null;
        }
        wasBlocked = false;
        springTransform.localScale = new Vector3(1, 1, springCompressScale);
        isAttacking = false;
        bGL.ToggleCollider(false);
        bGL.charge = 0;
        charge = 0;
        if(CameraManager.instance != null)
            CameraManager.instance.playerGameUIs[playerController.id].UpdateSliderValue(Mathf.Abs(charge));
        yield return null;
    }

    public void SetBoxingGloveSize (float size)
    {
        Debug.Log(size);
        Vector3 scale = new(.66f + size, .66f + size, .66f + size);
        bGL.transform.parent.parent.localScale = scale;
    }
}
