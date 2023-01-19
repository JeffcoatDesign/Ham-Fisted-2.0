using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerModel : MonoBehaviour
{
    public PlayerController playerController;
    public float rotateSpeed;
    public bool canRotate;
    private Vector2 movementInput;

    void Update()
    {
        Vector3 controllerInput = new Vector3(0, FindAngle(movementInput), 0);

        if (movementInput.x != 0 || movementInput.y != 0)
            RotateTo(FlattenInput(controllerInput));

        transform.position = playerController.transform.position;
    }
    public void OnMove(InputAction.CallbackContext ctx) => movementInput = ctx.ReadValue<Vector2>();

    Quaternion FlattenInput(Vector3 input)
    {
        Quaternion flatten = Quaternion.LookRotation(playerController.vCam.transform.forward, Vector3.up) * Quaternion.Euler(input);
        return flatten;
    }

    float FindAngle(Vector2 vector2)
    {
        float value = (float)(Mathf.Atan2(vector2.x, vector2.y) / Mathf.PI) * 180f;
        if (value < 0)
            value += 360;
        return value;
    }

    void RotateTo(Quaternion target)
    {
        Quaternion total = transform.rotation;
        float cr = transform.rotation.eulerAngles.y;
        float tr = target.eulerAngles.y;
        if (cr > tr || cr < tr)
        {
            total = Quaternion.Euler(0, Mathf.LerpAngle(cr, tr, Time.deltaTime * rotateSpeed), 0);
            transform.rotation = total;
        }
    }
}
