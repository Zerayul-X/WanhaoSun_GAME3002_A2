using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField]
    private Transform camTarget;
    [SerializeField]
    private Vector3 offset;
    
    void FixedUpdate()
    {
        Vector3 oldPosition = camTarget.position + offset;
        Vector3 newPosition = Vector3.Lerp(transform.position, oldPosition, 0.2f);
        transform.position = newPosition;
    }
}
