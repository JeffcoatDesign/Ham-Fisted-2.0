using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelocateOnStart : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    private void Start()
    {
        transform.position = targetTransform.position;
    }
}
