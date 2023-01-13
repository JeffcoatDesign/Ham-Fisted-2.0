using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class PlayerModel : MonoBehaviourPun
{
    public Transform playerBall;
    public Transform cameraRig;
    public float rotateSpeed;
    public bool canRotate;

    private void Awake()
    {
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

        transform.position = playerBall.position;
    }
    Quaternion FlattenInput(Vector3 input)
    {
        Quaternion flatten = Quaternion.LookRotation(cameraRig.forward, Vector3.up) * Quaternion.Euler(input);
        return flatten;
    }

    float FindAngle(float x, float y)
    {
        float value = (float)(Mathf.Atan2(x, y) / Mathf.PI) * 180f;
        if (value < 0)
            value += 360;
        return value;
    }

    void RotateTo(Quaternion target)
    {
        Quaternion total = transform.rotation;
        float cr = transform.rotation.eulerAngles.y;
        float tr = target.eulerAngles.y;
        float angleDifference = Modulo(tr - cr, 360f);
        if (cr > tr || cr < tr)
        {
            total = Quaternion.Euler(0, Mathf.LerpAngle(cr, tr, Time.deltaTime * rotateSpeed), 0);
            transform.rotation = total;
        }
    }

    float Modulo(float x, float m)
    {
        return (x % m + m) % m;
    }
}
