using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHamsterRotate : MonoBehaviour
{
    private float rotateSpeed = 0;
    public float addSpeed = 0.1f;
    void FixedUpdate()
    {
        rotateSpeed += addSpeed;
        transform.Rotate(rotateSpeed * Time.deltaTime, 0, rotateSpeed * Time.deltaTime);
    }
}
