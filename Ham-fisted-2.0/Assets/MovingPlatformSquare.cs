using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MovingPlatformSquare : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private Transform[] travelPoints;
    private Transform targetPoint;
    private Rigidbody rb;
    private int pointIndex;
    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        transform.position = travelPoints[0].position;
        targetPoint = travelPoints[1];
        pointIndex = 1;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(Vector3.MoveTowards(rb.position, targetPoint.position, moveSpeed));;

        if (targetPoint.position == transform.position)
        {
            if (pointIndex + 1 != travelPoints.Length)
                pointIndex += 1;
            else
                pointIndex = 0;
            targetPoint = travelPoints[pointIndex];
        }
    }
}
