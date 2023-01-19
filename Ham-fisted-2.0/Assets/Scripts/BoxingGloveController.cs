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
    private float punchStartTime;
    private Vector2 movementInput;

    void Update()
    {
        Vector3 controllerInput = new Vector3(0, FindAngle(movementInput), 0);

        if (movementInput.x != 0 || movementInput.y != 0)
            RotateTo(FlattenInput(controllerInput));

        transform.position = ballTransform.position;
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
        }
    }

    public void TryAttack ()
    {
        if (!isAttacking)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        bGL.canHit = true;
        punchStartTime = Time.time;
        float time = Time.time - punchStartTime;
        yield return null;
        while (time <= punchTime * punchRatio)
        {
            time = Time.time - punchStartTime;
            springTransform.localScale = new Vector3(1, 1, springCompressScale + swingLength * (time / (punchTime * punchRatio)));
            yield return null;
        }
        while (time <= punchTime)
        {
            time = Time.time - punchStartTime;
            springTransform.localScale = new Vector3(1, 1, springCompressScale + swingLength * (1 - ((time - (punchTime * punchRatio)) / (punchTime * (1 - punchRatio)))));
            yield return null;
        }
        springTransform.localScale = new Vector3(1, 1, springCompressScale);
        isAttacking = false;
        bGL.canHit = false;
        yield return null;
    }
}
