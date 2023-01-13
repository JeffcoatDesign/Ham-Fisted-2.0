using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public float camSpeed;
    public static CameraController instance;

    private PlayerController targetPlayer;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        Vector2 rightStick;
        if (Gamepad.current != null)
            rightStick = Gamepad.current.rightStick.ReadValue();
        else
        {
            float x = Input.GetAxis("Mouse X");
            float y = Input.GetAxis("Mouse Y");

            rightStick = new Vector2(x, y);
        }
        transform.Rotate(transform.up, rightStick.x * camSpeed);
    }
    private void LateUpdate()
    {
        if (targetPlayer != null)
            transform.position = targetPlayer.transform.position;
    }

    public void SetRigParent (GameObject tr)
    {
        targetPlayer = tr.GetComponent<PlayerController>();
        transform.rotation = Quaternion.Euler(0, targetPlayer.transform.eulerAngles.y, 0);
    }
}
