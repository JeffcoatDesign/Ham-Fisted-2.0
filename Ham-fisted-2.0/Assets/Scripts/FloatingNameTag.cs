using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingNameTag : MonoBehaviour
{
    public TextMeshPro tmp;
    public Transform ball;

    void Update ()
    {
        if (ball == null)
            return;

        transform.parent.position = ball.position;
        transform.LookAt(Camera.main.transform.position);
        transform.Rotate(0, 180, 0);
    }
    
    public void SetName(string name)
    {
        tmp.text = name;
    }

    public void Hide ()
    {
        gameObject.SetActive(false);
    }
}
