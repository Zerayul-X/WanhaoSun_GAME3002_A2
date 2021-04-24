using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammar : MonoBehaviour
{
    [SerializeField]
    private Vector3 Force = Vector3.zero;
    [SerializeField]
    private Vector3 HammarPoint = Vector3.zero;

    private Rigidbody hammarRb = null;
    private Vector3 Torque = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        hammarRb = GetComponent<Rigidbody>();
        hammarRb.centerOfMass = Vector3.zero;
        hammarRb.maxAngularVelocity = 100f;
    }

    private void FixedUpdate()
    {
        Torque = Vector3.Cross(Force, HammarPoint - hammarRb.centerOfMass);
        hammarRb.AddTorque(Torque);
    }
}
