using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class BoxingGloveController : MonoBehaviourPun
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

    private Transform cameraRig;
    public bool isAttacking = false;
    private float punchStartTime;

    private void Awake()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        cameraRig = Camera.main.transform.parent;
    }

    void Update()
    {
        if (!photonView.IsMine)
            return;

        var gamepad = Gamepad.current;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 controllerInput = new Vector3(0, FindAngle(x, z), 0);

        if (x != 0 || z != 0)
            RotateTo(FlattenInput(controllerInput));

        transform.position = ballTransform.position;

        Attack(gamepad);
    }

    private void LateUpdate()
    {
        if (photonView.IsMine)
            gloveOrigin.position = springEnd.position;
    }

    Quaternion FlattenInput(Vector3 input)
    {
        Quaternion flatten = Quaternion.LookRotation(cameraRig.forward, Vector3.up) * Quaternion.Euler(input);
        return flatten;
    }

    float FindAngle (float x, float y)
    {
        float value = (float)(Mathf.Atan2(x, y) / Math.PI) * 180f;
        if (value < 0)
            value += 360;
        return value;
    }

    void RotateTo (Quaternion target)
    {
        Quaternion total = transform.rotation;
        float cr = transform.rotation.eulerAngles.y;
        float tr = target.eulerAngles.y;
        float angleDifference = Modulo(tr - cr, 360f);
        if (cr > tr || cr < tr)
        {
            total = Quaternion.Euler(0, Mathf.LerpAngle(cr, tr, Time.deltaTime * speed), 0);
            transform.rotation = total;
        }
    }

    float Modulo (float x, float m)
    {
        return (x % m + m) % m;
    }

    void Attack (Gamepad gamepad)
    {
        bGL.canHit = isAttacking;
        if (isAttacking)
        {
            float time = Time.time - punchStartTime;
            if (time <= punchTime)
            {
                if (time <= punchTime * punchRatio)
                {
                    springTransform.localScale = new Vector3(1, 1, springCompressScale + swingLength * (time / (punchTime * punchRatio)));
                }
                else
                {
                    springTransform.localScale = new Vector3(1, 1, springCompressScale + swingLength * (1 - ((time - (punchTime * punchRatio)) / (punchTime * (1 - punchRatio)))));
                }
            }
            else
            {
                springTransform.localScale = new Vector3(1, 1, springCompressScale);
                isAttacking = false;
                //Debug.Log("Ready to attack");
            }
            return;
        }
        if (Input.GetMouseButtonDown(0) || (gamepad != null && gamepad.rightTrigger.IsPressed()))
        {
                punchStartTime = Time.time;
                //Debug.Log("Attacking");
                isAttacking = true;
                bGL.force = punchForce;
        }
    }
}
