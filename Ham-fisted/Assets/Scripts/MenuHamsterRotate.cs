using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHamsterRotate : MonoBehaviour
{
    private float rotateSpeed = 0;
    public float addSpeed = 0.1f;
    void Update()
    {
        rotateSpeed += addSpeed;
        transform.Rotate(rotateSpeed, 0, rotateSpeed);
    }
}
